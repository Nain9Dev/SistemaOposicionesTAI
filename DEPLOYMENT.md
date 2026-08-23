# Guía de Despliegue 0€ - Sistema Oposiciones TAI

Arquitectura de Despliegue seleccionada:
- **Frontend**: Vercel (React 19 + Vite)
- **Backend**: Render (Web Service Docker, ASP.NET Core)
- **Base de Datos**: Neon (PostgreSQL)

Esta arquitectura garantiza un coste mensual de **0 €**, asegurando la disponibilidad mediante scripts Keep-Alive.

---

## 1. Fase 1: Preparación en Neon (Base de Datos)
1. Crea una cuenta gratuita en [Neon](https://neon.tech/).
2. **Crear Proyecto**:
   - Nombre: `OposicionesTAI`
   - Postgres version: `16` (o superior)
   - Región: Selecciona la más cercana (ej. `aws-eu-central-1`).
3. **Obtener Connection String (URI)**:
   - Copia la cadena en formato `postgres://usuario:contraseña@host.neon.tech/neondb?sslmode=require`.
4. **Ejecutar Migración de Esquema**:
   - Ve a la pestaña **SQL Editor** en Neon.
   - Pega y ejecuta el contenido del script: `src/Oposiciones.Infrastructure/Scripts/01_Create_Usuarios_Progresos.sql`.
   - Verifica en **Tables** que se han creado `Usuarios` e `IntentosUsuario`.

---

## 2. Fase 2: Despliegue del Backend en Render
1. Haz un commit y push de tus últimos cambios a GitHub en el repositorio `SistemaOposicionesTAI`.
   ```bash
   git add .
   git commit -m "20260823 - UPD - CORS and API deployment preparation"
   git push origin main
   ```
2. Crea una cuenta en [Render](https://render.com/).
3. Haz clic en **New > Web Service** y selecciona "Build and deploy from a Git repository".
4. Conecta el repositorio `SistemaOposicionesTAI`.
5. **Configuración del Servicio**:
   - Name: `tai-api`
   - Language: `Docker`
   - Branch: `main`
   - Instance Type: **Free**
6. **Añadir Variables de Entorno (Environment Variables)**:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `ConnectionStrings__DefaultConnection`: `<URI_DE_NEON>`
   - `Jwt__Key`: `<CLAVE_SUPER_SECRETA_MAYOR_A_32_CARACTERES>`
   - `Jwt__Issuer`: `tai-api`
   - `Jwt__Audience`: `tai-frontend`
   - `CORS__AllowedOrigins`: `https://tai-frontend.vercel.app,http://localhost:5173`
7. Haz clic en **Create Web Service**. El despliegue inicial tomará ~3-5 minutos debido a la compilación Docker.
8. **Estrategia Keep-Alive** (Evitar suspensión por inactividad):
   - Ve a [cron-job.org](https://cron-job.org/).
   - Crea un nuevo Cronjob:
     - URL: `https://<tu-url-de-render.onrender.com>/swagger/index.html` (o un endpoint GET de prueba que devuelva 200/404 rápido).
     - Execution schedule: **Cada 10 minutos**.
   - Render ofrece 750 horas gratis; este cron consumirá unas 720 al mes (100% de cobertura).

---

## 3. Fase 3: Despliegue del Frontend en Vercel
1. En tu local, asegúrate de haber pusheado el repo de `tai-study-system-js`.
   ```bash
   git add .
   git commit -m "20260823 - UPD - Setup Vercel deployment configs"
   git push origin main
   ```
2. Inicia sesión en [Vercel](https://vercel.com/) con GitHub.
3. Haz clic en **Add New... > Project** y selecciona `tai-study-system-js`.
4. **Configuración**:
   - Framework Preset: `Vite`
   - Root Directory: `./` (por defecto)
5. **Añadir Variable de Entorno**:
   - Despliega la sección *Environment Variables*.
   - Name: `VITE_API_BASE_URL`
   - Value: `https://<tu-url-de-render.onrender.com>/api`
6. Haz clic en **Deploy**.
7. Vercel te proporcionará una URL (ej. `https://tai-frontend.vercel.app`).

---

## 4. Fase 4: Pruebas y Verificación
Se ha incluido un script `test-api.sh` en la raíz del repositorio Backend.

Para ejecutar las pruebas en tu local (usando Bash, WSL o Git Bash):
```bash
./test-api.sh https://<tu-url-de-render.onrender.com>
```
Este script automáticamente:
1. Comprueba si el backend está online (Health Check).
2. Valida un endpoint público (Syllabus/Temario).
3. Simula la creación de un nuevo usuario (POST /api/auth/register).
4. Inicia sesión para recuperar un JWT (POST /api/auth/login).
5. Usa el JWT para probar un endpoint protegido (GET /api/progreso/estadisticas).

### Verificación Manual de Usuario
1. Abre tu URL de Vercel en el navegador.
2. Comprueba que no hay errores de CORS en la consola de herramientas de desarrollo (F12).
3. Prueba el registro de usuario desde el frontend.
4. Completa un test y comprueba que se guarda el progreso y las estadísticas (que requiere Neon DB funcionando).

## Estrategia de Costes y Escalabilidad
- **Base de Datos (Neon)**: 0.5 GB es más que suficiente para almacenar datos de progreso JSON y miles de registros de usuario. 100 CU-horas cubren la latencia y persistencia general del backend sin sobrepasar el Tier Gratuito.
- **API (Render)**: El contenedor Docker multi-stage optimizado hace que el runtime de .NET ocupe poca RAM (los 512 MB de Render Free son idóneos).
- **Frontend (Vercel)**: Los archivos estáticos quedan alojados globalmente (CDN), minimizando la carga.
