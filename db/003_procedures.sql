CREATE OR REPLACE FUNCTION TestGenerate(
    p_Title VARCHAR(200),
    p_SyllabusTopicId INT,
    p_Difficulty SMALLINT,
    p_TotalQuestions INT
)
RETURNS BIGINT
LANGUAGE plpgsql
AS $$
DECLARE
    v_Seed INT := (random() * 2147483647)::INT;
    v_Pivot INT := (random() * 2147483647)::INT;
    v_TestId BIGINT;
BEGIN
    INSERT INTO Tests (Title, TotalQuestions, Seed)
    VALUES (p_Title, p_TotalQuestions, v_Seed)
    RETURNING Id INTO v_TestId;

    WITH PickA AS (
        SELECT Id
        FROM Questions
        WHERE SyllabusTopicId = p_SyllabusTopicId
          AND Difficulty = p_Difficulty
          AND IsActive = TRUE
          AND RandomKey >= v_Pivot
        ORDER BY RandomKey
        LIMIT p_TotalQuestions
    ),
    PickB AS (
        SELECT Id
        FROM Questions
        WHERE SyllabusTopicId = p_SyllabusTopicId
          AND Difficulty = p_Difficulty
          AND IsActive = TRUE
          AND RandomKey < v_Pivot
          AND Id NOT IN (SELECT Id FROM PickA)
        ORDER BY RandomKey
        LIMIT p_TotalQuestions
    ),
    FinalPick AS (
        SELECT Id FROM PickA
        UNION ALL
        SELECT Id FROM PickB
        LIMIT p_TotalQuestions
    )
    INSERT INTO TestQuestions (TestId, QuestionId, SortOrder)
    SELECT v_TestId, fp.Id, ROW_NUMBER() OVER ()
    FROM FinalPick fp;

    RETURN v_TestId;
END;
$$;