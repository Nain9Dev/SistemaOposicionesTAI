-- 005_progress_schema.sql
-- Tabla de historial de intentos del usuario. Faltaba en el esquema inicial pese a que
-- ProgresoRepository ya la consultaba: sin ella, /api/progreso fallaba en ejecucion.

CREATE TABLE IF NOT EXISTS IntentosUsuario
(
    Id        INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Aciertos  INT NOT NULL DEFAULT 0,
    Fallos    INT NOT NULL DEFAULT 0,
    Blancos   INT NOT NULL DEFAULT 0,
    Total     INT NOT NULL DEFAULT 0,
    Nota      NUMERIC(5,2) NOT NULL DEFAULT 0,
    Bloque    VARCHAR(20) NOT NULL DEFAULT 'all',
    Fecha     TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_IntentosUsuario_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    CONSTRAINT CK_IntentosUsuario_Totales CHECK (Aciertos >= 0 AND Fallos >= 0 AND Blancos >= 0 AND Total >= 0),
    CONSTRAINT CK_IntentosUsuario_Suma CHECK (Aciertos + Fallos + Blancos <= Total),
    CONSTRAINT CK_IntentosUsuario_Nota CHECK (Nota >= 0 AND Nota <= 10)
);

-- Columna anadida despues de la primera version de la tabla.
ALTER TABLE IntentosUsuario ADD COLUMN IF NOT EXISTS Blancos INT NOT NULL DEFAULT 0;

-- El listado paginado siempre filtra por usuario y ordena por fecha descendente.
CREATE INDEX IF NOT EXISTS IX_IntentosUsuario_Usuario_Fecha
    ON IntentosUsuario (UsuarioId, Fecha DESC);

-- La agregacion de estadisticas agrupa por bloque dentro de un usuario.
CREATE INDEX IF NOT EXISTS IX_IntentosUsuario_Usuario_Bloque
    ON IntentosUsuario (UsuarioId, Bloque);

-- Busqueda de refresh tokens por valor (login/refresh) y revocacion masiva por usuario.
CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UsuarioId
    ON RefreshTokens (UsuarioId);

-- Deteccion de reutilizacion: se conserva la cadena de rotacion.
ALTER TABLE RefreshTokens ADD COLUMN IF NOT EXISTS ReplacedByToken VARCHAR(255) NULL;

-- ---------------------------------------------------------------------------
-- Normalizacion temporal a TIMESTAMPTZ.
--
-- Desde Npgsql 6 escribir un DateTime con Kind=Utc en una columna
-- "timestamp without time zone" lanza InvalidCastException. Todo el codigo usa
-- DateTime.UtcNow, por lo que el alta de usuarios y de refresh tokens fallaba en
-- ejecucion. Se convierten las columnas interpretando los valores previos como UTC.
-- ---------------------------------------------------------------------------

ALTER TABLE Usuarios
    ALTER COLUMN FechaRegistro TYPE TIMESTAMPTZ USING FechaRegistro AT TIME ZONE 'UTC';

ALTER TABLE RefreshTokens
    ALTER COLUMN ExpiresAt TYPE TIMESTAMPTZ USING ExpiresAt AT TIME ZONE 'UTC',
    ALTER COLUMN CreatedAt TYPE TIMESTAMPTZ USING CreatedAt AT TIME ZONE 'UTC',
    ALTER COLUMN RevokedAt TYPE TIMESTAMPTZ USING RevokedAt AT TIME ZONE 'UTC';

ALTER TABLE Questions
    ALTER COLUMN CreatedAt TYPE TIMESTAMPTZ USING CreatedAt AT TIME ZONE 'UTC';

ALTER TABLE Tests
    ALTER COLUMN CreatedAt TYPE TIMESTAMPTZ USING CreatedAt AT TIME ZONE 'UTC';

ALTER TABLE Attempts
    ALTER COLUMN StartedAt TYPE TIMESTAMPTZ USING StartedAt AT TIME ZONE 'UTC',
    ALTER COLUMN FinishedAt TYPE TIMESTAMPTZ USING FinishedAt AT TIME ZONE 'UTC';

ALTER TABLE AttemptAnswers
    ALTER COLUMN AnsweredAt TYPE TIMESTAMPTZ USING AnsweredAt AT TIME ZONE 'UTC';

-- El correo se normaliza a minusculas en la capa de aplicacion; el indice funcional impide
-- que queden dos cuentas equivalentes si alguna fila antigua conserva mayusculas.
CREATE UNIQUE INDEX IF NOT EXISTS UX_Usuarios_Email_Lower ON Usuarios (LOWER(Email));
