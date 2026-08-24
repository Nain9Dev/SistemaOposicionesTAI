DO $$
DECLARE
    v_BlockI INT;
    v_BlockII INT;
    v_BlockIII INT;
    v_BlockIV INT;
BEGIN
    INSERT INTO SyllabusBlocks(Code, Name)
    SELECT Code, Name FROM (VALUES
     ('I','Derecho y Administración electrónica'),
     ('II','Tecnología básica'),
     ('III','Desarrollo de sistemas'),
     ('IV','Sistemas y comunicaciones')
    ) AS v(Code, Name)
    WHERE NOT EXISTS (SELECT 1 FROM SyllabusBlocks b WHERE b.Code = v.Code);

    SELECT Id INTO v_BlockI FROM SyllabusBlocks WHERE Code='I';
    SELECT Id INTO v_BlockII FROM SyllabusBlocks WHERE Code='II';
    SELECT Id INTO v_BlockIII FROM SyllabusBlocks WHERE Code='III';
    SELECT Id INTO v_BlockIV FROM SyllabusBlocks WHERE Code='IV';

    IF NOT EXISTS (SELECT 1 FROM SyllabusTopics WHERE BlockId=v_BlockI AND TopicNumber=1) THEN
        INSERT INTO SyllabusTopics(BlockId, TopicNumber, Title) VALUES (v_BlockI, 1, 'La Constitución Española de 1978. Derechos y deberes fundamentales');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM SyllabusTopics WHERE BlockId=v_BlockII AND TopicNumber=1) THEN
        INSERT INTO SyllabusTopics(BlockId, TopicNumber, Title) VALUES (v_BlockII, 1, 'Informática básica. Arquitectura de ordenadores');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM SyllabusTopics WHERE BlockId=v_BlockIII AND TopicNumber=1) THEN
        INSERT INTO SyllabusTopics(BlockId, TopicNumber, Title) VALUES (v_BlockIII, 1, 'Modelado de datos. Diseño lógico/físico. Normalización');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM SyllabusTopics WHERE BlockId=v_BlockIV AND TopicNumber=1) THEN
        INSERT INTO SyllabusTopics(BlockId, TopicNumber, Title) VALUES (v_BlockIV, 1, 'Administración del sistema operativo y software de base');
    END IF;
END $$;