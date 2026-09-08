-- 007_seed_questions.sql
-- Carga del banco de preguntas TAI usado por el motor de estudio.
-- Idempotente: se puede reejecutar sin duplicar temas, preguntas ni opciones.

CREATE OR REPLACE FUNCTION SeedEnsureTopic(p_BlockCode VARCHAR, p_Title VARCHAR)
RETURNS INT
LANGUAGE plpgsql
AS $fn$
DECLARE
    v_BlockId INT;
    v_TopicId INT;
    v_Next    INT;
BEGIN
    SELECT Id INTO v_BlockId FROM SyllabusBlocks WHERE Code = p_BlockCode;
    IF v_BlockId IS NULL THEN
        RAISE EXCEPTION 'El bloque % no existe. Ejecute antes 002_seed_minimal.sql', p_BlockCode;
    END IF;

    SELECT Id INTO v_TopicId FROM SyllabusTopics WHERE BlockId = v_BlockId AND Title = p_Title;
    IF v_TopicId IS NOT NULL THEN
        RETURN v_TopicId;
    END IF;

    SELECT COALESCE(MAX(TopicNumber), 0) + 1 INTO v_Next FROM SyllabusTopics WHERE BlockId = v_BlockId;
    INSERT INTO SyllabusTopics (BlockId, TopicNumber, Title)
    VALUES (v_BlockId, v_Next, p_Title)
    RETURNING Id INTO v_TopicId;

    RETURN v_TopicId;
END;
$fn$;

CREATE OR REPLACE FUNCTION SeedQuestion(
    p_TopicId      INT,
    p_Difficulty   SMALLINT,
    p_Statement    VARCHAR,
    p_Options      TEXT[],
    p_CorrectIndex INT
)
RETURNS BIGINT
LANGUAGE plpgsql
AS $fn$
DECLARE
    v_QuestionId BIGINT;
    v_Pos        INT;
BEGIN
    SELECT Id INTO v_QuestionId
      FROM Questions
     WHERE SyllabusTopicId = p_TopicId AND Statement = p_Statement;

    IF v_QuestionId IS NOT NULL THEN
        RETURN v_QuestionId;
    END IF;

    INSERT INTO Questions (SyllabusTopicId, Difficulty, Statement)
    VALUES (p_TopicId, p_Difficulty, p_Statement)
    RETURNING Id INTO v_QuestionId;

    FOR v_Pos IN 1..COALESCE(array_length(p_Options, 1), 0) LOOP
        INSERT INTO AnswerOptions (QuestionId, SortOrder, OptionText, IsCorrect)
        VALUES (v_QuestionId, v_Pos::SMALLINT, p_Options[v_Pos], (v_Pos - 1) = p_CorrectIndex);
    END LOOP;

    RETURN v_QuestionId;
END;
$fn$;

DO $seed$
DECLARE
    v_Topic1 INT;
    v_Topic2 INT;
    v_Topic3 INT;
BEGIN
    v_Topic1 := SeedEnsureTopic('I', 'Constitución Española de 1978: derechos y deberes fundamentales');
    v_Topic2 := SeedEnsureTopic('II', 'Arquitectura básica de ordenadores');
    v_Topic3 := SeedEnsureTopic('III', 'Modelado relacional y normalización');

    -- Bloque I :: Constitución Española de 1978: derechos y deberes fundamentales
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, '¿Qué norma es la norma suprema del ordenamiento jurídico español?',
        ARRAY['El Código Civil', 'La Constitución', 'Una ley orgánica', 'Un reglamento']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, 'Los derechos fundamentales se recogen principalmente en:',
        ARRAY['Título I', 'Título II', 'Título III', 'Título VIII']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, '¿Qué mayoría se requiere para reformar la Constitución por el procedimiento ordinario (art. 167 CE)?',
        ARRAY['Mayoría simple', 'Tres quintos de cada Cámara', 'Dos tercios de cada Cámara', 'Unanimidad']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, 'El derecho a la tutela judicial efectiva está en el artículo:',
        ARRAY['Art. 14', 'Art. 24', 'Art. 27', 'Art. 30']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, '¿Cuál es el órgano que interpreta la Constitución con carácter supremo?',
        ARRAY['Tribunal Supremo', 'Tribunal Constitucional', 'Consejo de Estado', 'Congreso de los Diputados']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, 'La igualdad ante la ley está en el artículo:',
        ARRAY['Art. 9', 'Art. 14', 'Art. 16', 'Art. 20']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, '¿Cuál de los siguientes es un derecho fundamental?',
        ARRAY['Derecho a la vivienda', 'Libertad ideológica', 'Protección de la salud', 'Protección a la familia']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, 'El procedimiento agravado de reforma constitucional (art. 168 CE) exige:',
        ARRAY['Mayoría simple y referéndum siempre', 'Dos tercios, disolución de Cortes y referéndum', 'Tres quintos sin referéndum', 'Unanimidad del Senado']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, 'La libertad de expresión está en el artículo:',
        ARRAY['Art. 18', 'Art. 20', 'Art. 22', 'Art. 28']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic1, 1::SMALLINT, '¿Qué artículo reconoce el derecho de reunión?',
        ARRAY['Art. 21', 'Art. 23', 'Art. 25', 'Art. 29']::TEXT[], 0);

    -- Bloque II :: Arquitectura básica de ordenadores
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, '¿Qué componente ejecuta las instrucciones de un programa?',
        ARRAY['GPU', 'CPU', 'SSD', 'BIOS']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'La memoria principal se refiere normalmente a:',
        ARRAY['RAM', 'ROM', 'Cache L3', 'Disco duro']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, '¿Qué almacén es más rápido en general?',
        ARRAY['SSD NVMe', 'HDD', 'Cinta magnética', 'DVD']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'La caché de CPU se usa para:',
        ARRAY['Almacenar backups', 'Reducir latencia de acceso a datos/instrucciones', 'Conectar periféricos', 'Sustituir la RAM']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'La BIOS/UEFI se ejecuta:',
        ARRAY['Después del sistema operativo', 'Al arrancar el equipo', 'Solo cuando hay internet', 'En el navegador']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, '¿Qué bus conecta CPU con memoria y periféricos?',
        ARRAY['PCIe', 'Bus del sistema', 'USB', 'HDMI']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'Un bit puede valer:',
        ARRAY['0 o 1', '0 a 9', 'A o B', 'Solo 1']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'La unidad aritmético-lógica (ALU) pertenece a:',
        ARRAY['CPU', 'RAM', 'SSD', 'Fuente de alimentación']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, 'El paralelismo a nivel de instrucción se asocia con:',
        ARRAY['Pipeline', 'RAID', 'DNS', 'DHCP']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic2, 1::SMALLINT, '¿Qué memoria es volátil?',
        ARRAY['ROM', 'RAM', 'Flash', 'SSD']::TEXT[], 1);

    -- Bloque III :: Modelado relacional y normalización
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, '¿Qué forma normal reduce dependencias parciales en claves compuestas?',
        ARRAY['1FN', '2FN', '3FN', 'BCNF']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'Una clave primaria sirve para:',
        ARRAY['Permitir nulos', 'Identificar unívocamente una fila', 'Duplicar datos', 'Evitar índices']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, '¿Cuál es el objetivo principal de la normalización?',
        ARRAY['Aumentar redundancia', 'Reducir anomalías de inserción/actualización/borrado', 'Evitar claves', 'Hacer más lenta la BD']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'Una relación N:M se implementa típicamente con:',
        ARRAY['Una sola tabla', 'Tabla intermedia con FKs', 'Trigger obligatorio', 'Índice clustered']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, '¿Qué constraint asegura unicidad en una columna?',
        ARRAY['CHECK', 'UNIQUE', 'DEFAULT', 'FOREIGN KEY']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'Un índice ayuda principalmente a:',
        ARRAY['Acelerar búsquedas y joins', 'Aumentar el tamaño del log siempre', 'Eliminar necesidad de claves', 'Evitar bloqueos']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'La integridad referencial se garantiza con:',
        ARRAY['FOREIGN KEY', 'DEFAULT', 'VIEW', 'IDENTITY']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, '¿Qué hace un CHECK constraint?',
        ARRAY['Obliga a que un valor cumpla una condición', 'Crea un índice', 'Borra filas automáticamente', 'Permite duplicados']::TEXT[], 0);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'Una transacción sirve para:',
        ARRAY['Ejecutar SELECT más rápido', 'Garantizar atomicidad de operaciones', 'Evitar claves foráneas', 'Cambiar la collation']::TEXT[], 1);
    PERFORM SeedQuestion(v_Topic3, 1::SMALLINT, 'Un índice compuesto se recomienda cuando:',
        ARRAY['Siempre, sin analizar queries', 'Filtras por varias columnas en conjunto', 'Nunca', 'Solo para columnas NVARCHAR']::TEXT[], 1);
END;
$seed$;

DROP FUNCTION IF EXISTS SeedQuestion(INT, SMALLINT, VARCHAR, TEXT[], INT);
DROP FUNCTION IF EXISTS SeedEnsureTopic(VARCHAR, VARCHAR);
