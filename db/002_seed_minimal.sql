INSERT INTO dbo.SyllabusBlocks(Code, Name)
SELECT v.Code, v.Name
FROM (VALUES
 ('I','Derecho y Administración electrónica'),
 ('II','Tecnología básica'),
 ('III','Desarrollo de sistemas'),
 ('IV','Sistemas y comunicaciones')
) v(Code, Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.SyllabusBlocks b WHERE b.Code = v.Code);

DECLARE @BlockI INT = (SELECT Id FROM dbo.SyllabusBlocks WHERE Code='I');
DECLARE @BlockII INT = (SELECT Id FROM dbo.SyllabusBlocks WHERE Code='II');
DECLARE @BlockIII INT = (SELECT Id FROM dbo.SyllabusBlocks WHERE Code='III');
DECLARE @BlockIV INT = (SELECT Id FROM dbo.SyllabusBlocks WHERE Code='IV');


IF NOT EXISTS (SELECT 1 FROM dbo.SyllabusTopics WHERE BlockId=@BlockI AND TopicNumber=1)
INSERT INTO dbo.SyllabusTopics(BlockId, TopicNumber, Title) VALUES (@BlockI, 1, 'La Constitución Española de 1978. Derechos y deberes fundamentales');

IF NOT EXISTS (SELECT 1 FROM dbo.SyllabusTopics WHERE BlockId=@BlockII AND TopicNumber=1)
INSERT INTO dbo.SyllabusTopics(BlockId, TopicNumber, Title) VALUES (@BlockII, 1, 'Informática básica. Arquitectura de ordenadores');

IF NOT EXISTS (SELECT 1 FROM dbo.SyllabusTopics WHERE BlockId=@BlockIII AND TopicNumber=1)
INSERT INTO dbo.SyllabusTopics(BlockId, TopicNumber, Title) VALUES (@BlockIII, 1, 'Modelado de datos. Diseño lógico/físico. Normalización');

IF NOT EXISTS (SELECT 1 FROM dbo.SyllabusTopics WHERE BlockId=@BlockIV AND TopicNumber=1)
INSERT INTO dbo.SyllabusTopics(BlockId, TopicNumber, Title) VALUES (@BlockIV, 1, 'Administración del sistema operativo y software de base');