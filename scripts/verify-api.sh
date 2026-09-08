#!/usr/bin/env bash
# End-to-end verification of the running API.
#
# Exercises the full public contract against a live instance: syllabus, question bank,
# registration, login, CSRF enforcement, progress tracking and the attempt lifecycle.
# Every assertion is a behaviour the client depends on, so a red run means a broken deploy.
#
# Usage:
#   scripts/verify-api.sh [base-url]        # defaults to http://localhost:5298/api
#
# Requires: curl, node (for JSON assertions).

set -uo pipefail

BASE_URL="${1:-http://localhost:5298/api}"
COOKIE_JAR="$(mktemp)"
COOKIE_JAR_B="$(mktemp)"
PASSED=0
FAILED=0

trap 'rm -f "$COOKIE_JAR" "$COOKIE_JAR_B"' EXIT

log_pass() { PASSED=$((PASSED + 1)); printf '  \033[32mPASS\033[0m %s\n' "$1"; }
log_fail() { FAILED=$((FAILED + 1)); printf '  \033[31mFAIL\033[0m %s\n' "$1"; }

assert_eq() {
  local label="$1" expected="$2" actual="$3"
  if [ "$expected" = "$actual" ]; then
    log_pass "$label"
  else
    log_fail "$label (expected '$expected', got '$actual')"
  fi
}

# Reads a value out of a JSON document on stdin using a JS expression over `j`.
json() { node -e "let d='';process.stdin.on('data',c=>d+=c).on('end',()=>{try{const j=JSON.parse(d);console.log($1)}catch(e){console.log('PARSE_ERROR')}})"; }

status_of() { curl -s -m 15 -o /dev/null -w '%{http_code}' "$@"; }

echo "Verifying $BASE_URL"

echo
echo "Health"
assert_eq "liveness probe responds" "ok" "$(curl -s -m 15 "$BASE_URL/health" | json 'j.status')"
assert_eq "database is reachable" "ok" "$(curl -s -m 15 "$BASE_URL/health/db" | json 'j.status')"

echo
echo "Syllabus"
assert_eq "blocks come back in curriculum order" "I,II,III,IV" \
  "$(curl -s -m 15 "$BASE_URL/syllabus/blocks" | json 'j.map(b=>b.code).join(",")')"
assert_eq "topics are returned without a block filter" "true" \
  "$(curl -s -m 15 "$BASE_URL/syllabus/topics" | json 'j.length>0')"

echo
echo "Question bank"
assert_eq "the bank serves questions" "true" \
  "$(curl -s -m 15 "$BASE_URL/preguntas?cantidad=5" | json 'j.length>0')"
assert_eq "a numeric selector maps to the curriculum block" "I" \
  "$(curl -s -m 15 "$BASE_URL/preguntas/bloque/1?cantidad=5" | json '[...new Set(j.map(q=>q.bloque))].join(",")')"
assert_eq "a roman selector resolves to the same block" "III" \
  "$(curl -s -m 15 "$BASE_URL/preguntas/bloque/III?cantidad=4" | json '[...new Set(j.map(q=>q.bloque))].join(",")')"
assert_eq "every question exposes a valid answer index" "true" \
  "$(curl -s -m 15 "$BASE_URL/preguntas?cantidad=20" | json 'j.every(q=>q.respuestaCorrecta>=0 && q.respuestaCorrecta<q.opciones.length)')"
assert_eq "the requested size is honoured" "3" \
  "$(curl -s -m 15 "$BASE_URL/preguntas?cantidad=3" | json 'j.length')"
# Asking for more questions than the server allows is a client error, not a silent
# truncation: a shorter exam than requested, with no explanation, is worse than a refusal.
assert_eq "an oversized request is refused" "400" \
  "$(status_of "$BASE_URL/preguntas?cantidad=100000")"
assert_eq "availability reports the normalised block" "II" \
  "$(curl -s -m 15 "$BASE_URL/preguntas/disponibilidad?bloque=2" | json 'j.bloque')"

echo
echo "Authentication"
EMAIL="verify-$(date +%s)-$RANDOM@example.test"
REGISTER=$(curl -s -m 20 -c "$COOKIE_JAR" -X POST "$BASE_URL/auth/register" \
  -H 'Content-Type: application/json' \
  -d "{\"nombre\":\"Verification Run\",\"email\":\"$EMAIL\",\"password\":\"Password123\"}")

CSRF=$(printf '%s' "$REGISTER" | json 'j.csrfToken')
assert_eq "registration issues a CSRF token" "true" "$([ -n "$CSRF" ] && [ "$CSRF" != "PARSE_ERROR" ] && echo true || echo false)"
assert_eq "the JWT is never returned in the body" "false" "$(printf '%s' "$REGISTER" | grep -q '"token"' && echo true || echo false)"
assert_eq "both session cookies are set" "2" "$(grep -c 'token' "$COOKIE_JAR")"
assert_eq "cookies are HttpOnly" "2" "$(grep -c '#HttpOnly_' "$COOKIE_JAR")"
assert_eq "the profile is readable from the cookie" "$EMAIL" \
  "$(curl -s -m 15 -b "$COOKIE_JAR" "$BASE_URL/auth/me" | json 'j.email')"
assert_eq "a duplicate email is rejected" "409" \
  "$(status_of -X POST "$BASE_URL/auth/register" -H 'Content-Type: application/json' \
      -d "{\"nombre\":\"Duplicate\",\"email\":\"$EMAIL\",\"password\":\"Password123\"}")"
assert_eq "a weak password is rejected" "400" \
  "$(status_of -X POST "$BASE_URL/auth/register" -H 'Content-Type: application/json' \
      -d '{"nombre":"Weak Password","email":"weak-'"$RANDOM"'@example.test","password":"123"}')"
assert_eq "a wrong password is rejected" "401" \
  "$(status_of -X POST "$BASE_URL/auth/login" -H 'Content-Type: application/json' \
      -d "{\"email\":\"$EMAIL\",\"password\":\"WrongPassword1\"}")"

echo
echo "Authorisation and CSRF"
assert_eq "progress requires a session" "401" "$(status_of "$BASE_URL/progreso/estadisticas")"
assert_eq "a write without the CSRF header is refused" "403" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" -H 'Content-Type: application/json' \
      -d '{"aciertos":8,"fallos":2,"total":10}')"
assert_eq "a write with a forged CSRF token is refused" "403" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" -H 'Content-Type: application/json' \
      -H 'X-CSRF-Token: forged-token' -d '{"aciertos":8,"fallos":2,"total":10}')"

echo
echo "Progress"
ATTEMPT=$(curl -s -m 15 -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" \
  -H 'Content-Type: application/json' -H "X-CSRF-Token: $CSRF" \
  -d '{"aciertos":8,"fallos":2,"total":10,"bloque":"1"}')
assert_eq "the server applies the INAP scale" "7.34" "$(printf '%s' "$ATTEMPT" | json 'j.nota')"
assert_eq "the block label is normalised" "I" "$(printf '%s' "$ATTEMPT" | json 'j.bloque')"

# The client must not be able to declare its own grade or row id.
FORGED=$(curl -s -m 15 -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" \
  -H 'Content-Type: application/json' -H "X-CSRF-Token: $CSRF" \
  -d '{"aciertos":5,"fallos":5,"total":20,"nota":10,"id":9999,"usuarioId":1,"bloque":"2"}')
assert_eq "a client-declared grade is ignored" "1.67" "$(printf '%s' "$FORGED" | json 'j.nota')"
assert_eq "a client-declared id is ignored" "false" "$(printf '%s' "$FORGED" | json 'j.id===9999')"
assert_eq "blanks are derived from the total" "10" "$(printf '%s' "$FORGED" | json 'j.blancos')"

assert_eq "an empty exam is rejected" "400" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" -H 'Content-Type: application/json' \
      -H "X-CSRF-Token: $CSRF" -d '{"aciertos":0,"fallos":0,"total":0}')"
assert_eq "incoherent counters are rejected" "400" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" -H 'Content-Type: application/json' \
      -H "X-CSRF-Token: $CSRF" -d '{"aciertos":9,"fallos":9,"total":10}')"

STATS=$(curl -s -m 15 -b "$COOKIE_JAR" "$BASE_URL/progreso/estadisticas")
assert_eq "statistics aggregate every attempt" "2" "$(printf '%s' "$STATS" | json 'j.totalIntentos')"
assert_eq "the weakest block is identified" "II" "$(printf '%s' "$STATS" | json 'j.bloqueMasDebil')"
assert_eq "the trend keeps chronological order" "true" \
  "$(printf '%s' "$STATS" | json 'j.tendencia.every((p,i,a)=>i===0||new Date(a[i-1].fecha)<=new Date(p.fecha))')"

PAGE=$(curl -s -m 15 -b "$COOKIE_JAR" "$BASE_URL/progreso/historial?page=0&pageSize=9999")
assert_eq "page zero is clamped to the first page" "1" "$(printf '%s' "$PAGE" | json 'j.page')"
assert_eq "the page size is clamped to the maximum" "100" "$(printf '%s' "$PAGE" | json 'j.pageSize')"

echo
echo "Attempt lifecycle"
TOPIC_ID=$(curl -s -m 15 "$BASE_URL/preguntas?cantidad=1" | json 'j[0].temaId')
TEST_ID=$(curl -s -m 15 -b "$COOKIE_JAR" -X POST "$BASE_URL/tests/generate" \
  -H 'Content-Type: application/json' -H "X-CSRF-Token: $CSRF" \
  -d "{\"title\":\"Verification run\",\"syllabusTopicId\":$TOPIC_ID,\"difficulty\":1,\"totalQuestions\":5}" | json 'j.testId')
assert_eq "a test can be generated" "true" "$([ -n "$TEST_ID" ] && [ "$TEST_ID" != "PARSE_ERROR" ] && echo true || echo false)"

TEST_BODY=$(curl -s -m 15 "$BASE_URL/tests/$TEST_ID")
assert_eq "the test carries its questions" "true" "$(printf '%s' "$TEST_BODY" | json 'j.questions.length>0')"
assert_eq "correct answers are never exposed" "false" \
  "$(printf '%s' "$TEST_BODY" | grep -qi 'iscorrect' && echo true || echo false)"

ATTEMPT_ID=$(curl -s -m 15 -b "$COOKIE_JAR" -X POST "$BASE_URL/attempts/start" \
  -H 'Content-Type: application/json' -H "X-CSRF-Token: $CSRF" \
  -d "{\"testId\":$TEST_ID}" | json 'j.attemptId')
assert_eq "an attempt can be started" "true" "$([ -n "$ATTEMPT_ID" ] && [ "$ATTEMPT_ID" != "PARSE_ERROR" ] && echo true || echo false)"

assert_eq "a question from another test is rejected" "400" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/attempts/$ATTEMPT_ID/answer" \
      -H 'Content-Type: application/json' -H "X-CSRF-Token: $CSRF" \
      -d '{"questionId":999999,"answerOptionId":999999}')"

FINISH=$(curl -s -m 15 -b "$COOKIE_JAR" -X POST "$BASE_URL/attempts/$ATTEMPT_ID/finish" -H "X-CSRF-Token: $CSRF")
assert_eq "an unanswered attempt scores zero" "0.00" "$(printf '%s' "$FINISH" | json 'j.score.toFixed?j.score.toFixed(2):j.score')"
assert_eq "every question counts as blank" "true" "$(printf '%s' "$FINISH" | json 'j.blank===j.total && j.total>0')"
assert_eq "an attempt cannot be closed twice" "409" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/attempts/$ATTEMPT_ID/finish" -H "X-CSRF-Token: $CSRF")"

# A second account must not be able to touch the first account's attempt.
INTRUDER=$(curl -s -m 20 -c "$COOKIE_JAR_B" -X POST "$BASE_URL/auth/register" \
  -H 'Content-Type: application/json' \
  -d "{\"nombre\":\"Intruder Account\",\"email\":\"intruder-$(date +%s)-$RANDOM@example.test\",\"password\":\"Password123\"}")
CSRF_B=$(printf '%s' "$INTRUDER" | json 'j.csrfToken')
assert_eq "another account cannot close the attempt" "403" \
  "$(status_of -b "$COOKIE_JAR_B" -X POST "$BASE_URL/attempts/$ATTEMPT_ID/finish" -H "X-CSRF-Token: $CSRF_B")"

echo
echo "Session lifecycle"
# Refreshing rotates the session, so it also issues a new CSRF token bound to the new
# session id. The client has to adopt it: the previous one no longer resolves.
REFRESHED=$(curl -s -m 15 -b "$COOKIE_JAR" -c "$COOKIE_JAR" -X POST "$BASE_URL/auth/refresh")
CSRF_REFRESHED=$(printf '%s' "$REFRESHED" | json 'j.csrfToken')
assert_eq "the session can be refreshed" "true" \
  "$([ -n "$CSRF_REFRESHED" ] && [ "$CSRF_REFRESHED" != "PARSE_ERROR" ] && echo true || echo false)"
assert_eq "rotation issues a different CSRF token" "false" \
  "$([ "$CSRF_REFRESHED" = "$CSRF" ] && echo true || echo false)"
assert_eq "the stale CSRF token stops working" "403" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/progreso" -H 'Content-Type: application/json' \
      -H "X-CSRF-Token: $CSRF" -d '{"aciertos":1,"fallos":0,"total":1}')"
assert_eq "logout succeeds with the current token" "200" \
  "$(status_of -b "$COOKIE_JAR" -X POST "$BASE_URL/auth/logout" -H "X-CSRF-Token: $CSRF_REFRESHED")"
assert_eq "the session stops working after logout" "401" \
  "$(status_of -b "$COOKIE_JAR" "$BASE_URL/auth/me")"

echo
echo "-----------------------------------------"
printf 'passed: %s   failed: %s\n' "$PASSED" "$FAILED"

[ "$FAILED" -eq 0 ] || exit 1
