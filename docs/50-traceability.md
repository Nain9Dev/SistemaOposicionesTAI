# 50 — Traceability

Every requirement and the test that proves it. A requirement with no test is not done,
whatever the code says.

Two suites cover the system:

- **Unit** — `dotnet test tests/Oposiciones.UnitTests` (70 tests). Business rules in
  isolation, no database.
- **End to end** — `bash scripts/verify-api.sh` (48 assertions). The public contract against
  a live instance with a freshly migrated database.

## Syllabus

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-001 | `blocks come back in curriculum order` | E2E |
| REQ-002 | `topics are returned without a block filter` | E2E |
| REQ-003 | Covered by `SyllabusController.GetOrSetAsync` falling through on cache failure | — (manual) |

## Question bank

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-010 | `the bank serves questions`, `every question exposes a valid answer index` | E2E |
| REQ-011 | `BlockSelectorTests.El_indice_del_temario_se_traduce_a_codigo_romano`, `..._El_codigo_romano_se_normaliza_a_mayusculas`, `..._El_comodin_no_aplica_ningun_filtro` | Unit |
| REQ-011 | `a numeric selector maps to the curriculum block`, `a roman selector resolves to the same block` | E2E |
| REQ-012 | `StudyQuestionService` skips `!IsAnswerable`; asserted indirectly by `every question exposes a valid answer index` | E2E |
| REQ-013 | `an oversized request is refused` | E2E |
| REQ-014 | `availability reports the normalised block` | E2E |

## Authentication

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-020 | `a duplicate email is rejected` | E2E |
| REQ-021 | `the JWT is never returned in the body`, `both session cookies are set`, `cookies are HttpOnly` | E2E |
| REQ-022 | `a wrong password is rejected` | E2E |
| REQ-023 | `the session can be refreshed`, `rotation issues a different CSRF token` | E2E |
| REQ-024 | `AuthService.RefreshTokenAsync` revokes all sessions on replay | — (manual) |
| REQ-025 | `the session stops working after logout` | E2E |
| REQ-026 | `both session cookies are set` under the development cookie profile | E2E |

## Authorisation and protection

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-030 | `a write without the CSRF header is refused`, `a write with a forged CSRF token is refused` | E2E |
| REQ-031 | `registration issues a CSRF token` | E2E |
| REQ-032 | `the stale CSRF token stops working` | E2E |
| REQ-033 | `RateLimitPolicies.Configure` partitions by client key | — (manual) |
| REQ-034 | `ExceptionHandlingMiddleware` translation table | — (manual) |

## Attempts

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-040 | `AttemptServiceTests.Empezar_un_test_inexistente_devuelve_no_encontrado` | Unit |
| REQ-041 | `AttemptServiceTests.Responder_un_intento_ajeno_esta_prohibido`, `..._Cerrar_un_intento_ajeno_esta_prohibido` | Unit |
| REQ-041 | `another account cannot close the attempt` | E2E |
| REQ-042 | `AttemptServiceTests.Una_respuesta_de_otro_test_se_rechaza` | Unit |
| REQ-042 | `a question from another test is rejected` | E2E |
| REQ-043 | `AttemptServiceTests.Un_intento_cerrado_no_admite_mas_respuestas`, `..._Un_intento_no_se_puede_cerrar_dos_veces` | Unit |
| REQ-043 | `an attempt cannot be closed twice` | E2E |
| REQ-044 | `ScoringServiceTests` (7 tests), `an unanswered attempt scores zero` | Unit + E2E |
| REQ-045 | `every question counts as blank` | E2E |
| REQ-046 | `correct answers are never exposed` | E2E |

## Progress

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-050 | `ProgresoServiceTests.La_nota_la_calcula_el_servidor_y_no_el_cliente`, `..._Los_blancos_se_derivan_del_total` | Unit |
| REQ-050 | `the server applies the INAP scale`, `a client-declared grade is ignored`, `blanks are derived from the total` | E2E |
| REQ-051 | `ProgresoServiceTests.El_intento_se_asocia_al_usuario_autenticado`, `a client-declared id is ignored` | Unit + E2E |
| REQ-052 | `ProgresoServiceTests.Aciertos_y_fallos_no_pueden_superar_el_total`, `incoherent counters are rejected` | Unit + E2E |
| REQ-052 | `ProgresoServiceTests.Un_simulacro_sin_preguntas_se_rechaza`, `an empty exam is rejected` | Unit + E2E |
| REQ-053 | `ProgresoServiceTests.El_bloque_se_normaliza_antes_de_archivar`, `the block label is normalised` | Unit + E2E |
| REQ-054 | `PagingDefaultsTests` (6 tests), `ProgresoServiceTests.El_historial_satura_la_paginacion` | Unit |
| REQ-054 | `page zero is clamped to the first page`, `the page size is clamped to the maximum` | E2E |
| REQ-055 | `statistics aggregate every attempt`, weighted aggregation in `GetEstadisticasResumidasAsync` | E2E |
| REQ-056 | `ProgresoServiceTests.El_bloque_mas_debil_ignora_muestras_pequenas`, `the weakest block is identified` | Unit + E2E |
| REQ-057 | `ProgresoServiceTests.Una_fecha_sin_zona_se_interpreta_como_utc` | Unit |
| REQ-058 | `ProgresoRepository.DeleteHistorialAsync` filters by user | — (manual) |

## Operations

| Requirement | Test | Suite |
| :--- | :--- | :--- |
| REQ-060 | `PostgresConnectionStringTests.Una_cadena_vacia_falla_al_arrancar_y_no_en_la_primera_peticion` | Unit |
| REQ-061 | `PostgresConnectionStringTests` (7 tests) | Unit |
| REQ-062 | `liveness probe responds`, `database is reachable` | E2E |
| REQ-063 | Start-up warning in `Program.cs` | — (manual) |

## Coverage gaps

Requirements marked `— (manual)` are verified by reading the code, not by an automated
test. They are listed here so the gap is visible rather than assumed away. Closing them
needs an integration-test project using `WebApplicationFactory`, which is tracked as
`T-014` in [`40-tasks.md`](40-tasks.md).
