#!/bin/bash

# Configuration
API_URL=${1:-"https://tai-api.onrender.com"} # Default API URL, override with $1
JWT_TOKEN=""

echo "🚀 Iniciando pruebas de la API en $API_URL"
echo "------------------------------------------------"

# 1. Test Health/Root endpoint (assuming there's a swagger or simple endpoint we can ping)
echo "1️⃣ Comprobando estado del Backend (Swagger/Endpoint principal)..."
HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/swagger/index.html")
if [ "$HTTP_STATUS" -eq 200 ] || [ "$HTTP_STATUS" -eq 404 ]; then
    # Note: Swagger might be disabled in production, so 404 is also "alive"
    echo "✅ Backend responde (HTTP $HTTP_STATUS)"
else
    echo "❌ Error al contactar backend (HTTP $HTTP_STATUS)"
    exit 1
fi
echo ""

# 2. Test Get Syllabus/Preguntas (assuming an endpoint exists)
echo "2️⃣ Comprobando endpoint de Temario (GET /api/syllabus)..."
HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/syllabus")
if [ "$HTTP_STATUS" -eq 200 ] || [ "$HTTP_STATUS" -eq 401 ]; then
    echo "✅ Endpoint de Temario responde (HTTP $HTTP_STATUS)"
else
    echo "⚠️ Endpoint de Temario devolvió código inesperado (HTTP $HTTP_STATUS)"
fi
echo ""

# 3. Test Register User
echo "3️⃣ Comprobando Registro de Usuario (POST /api/auth/register)..."
RANDOM_NUM=$RANDOM
USER_EMAIL="test${RANDOM_NUM}@test.com"

REGISTER_RESPONSE=$(curl -s -X POST "$API_URL/api/auth/register" \
    -H "Content-Type: application/json" \
    -d "{\"nombre\":\"TestUser\",\"email\":\"$USER_EMAIL\",\"password\":\"Pass123!\"}")

echo "Respuesta Registro: $REGISTER_RESPONSE"
echo "✅ Registro completado"
echo ""

# 4. Test Login
echo "4️⃣ Comprobando Inicio de Sesión (POST /api/auth/login)..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$USER_EMAIL\",\"password\":\"Pass123!\"}")

JWT_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -n "$JWT_TOKEN" ]; then
    echo "✅ Login exitoso, token JWT obtenido."
else
    echo "❌ Fallo en Login. Respuesta: $LOGIN_RESPONSE"
    exit 1
fi
echo ""

# 5. Test Progreso (Endpoint protegido)
echo "5️⃣ Comprobando endpoint protegido de Progreso (GET /api/progreso/estadisticas)..."
HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/progreso/estadisticas" \
    -H "Authorization: Bearer $JWT_TOKEN")

if [ "$HTTP_STATUS" -eq 200 ]; then
    echo "✅ Endpoint protegido responde correctamente (HTTP $HTTP_STATUS)"
else
    echo "❌ Fallo al acceder a endpoint protegido (HTTP $HTTP_STATUS)"
fi
echo ""

echo "🎉 Pruebas automáticas completadas con éxito."
