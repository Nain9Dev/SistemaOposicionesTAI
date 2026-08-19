/*
    Carga de contenido del Sistema de Oposiciones.

    GENERADO AUTOMATICAMENTE a partir de la carpeta content/ mediante:
        dotnet run --project src/Oposiciones.Cli -- sql --out db/seed/content.sql

    No editar a mano: cualquier cambio debe hacerse en los ficheros JSON de contenido
    y regenerarse este guion. Requiere haber ejecutado antes db/migrations/.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

DECLARE @Options dbo.AnswerOptionList;
DECLARE @Tags dbo.TagList;

-- ---------- Convocatoria TAI ----------
EXEC dbo.ExamUpsert
    @Code = N'TAI',
    @Name = N'Cuerpo de Tecnicos Auxiliares de Informatica de la Administracion del Estado',
    @Authority = N'INAP - Instituto Nacional de Administracion Publica',
    @Description = N'Programa oficial de 33 temas repartidos en cuatro bloques. El primer ejercicio consta de 80 preguntas mas 5 de reserva, con 120 minutos de duracion, penalizacion de un tercio por respuesta erronea y calificacion de 0 a 50 puntos, siendo necesarios 25 para superarlo.',
    @SourceReference = N'Resolucion de 9 de julio de 2024, de la Secretaria de Estado de Funcion Publica, por la que se convoca proceso selectivo para ingreso en el Cuerpo de Tecnicos Auxiliares de Informatica de la Administracion del Estado',
    @SourcePublication = N'BOE num. 166, de 10 de julio de 2024',
    @SourceUrl = N'https://www.boe.es/diario_boe/txt.php?id=BOE-A-2024-14139',
    @CorrectPoints = 1,
    @IncorrectPoints = -0.3333,
    @BlankPoints = 0,
    @MaxScore = 50,
    @PassMark = 25,
    @QuestionCount = 80,
    @ReserveQuestions = 5,
    @DurationMinutes = 120,
    @OptionsPerQuestion = 4;

EXEC dbo.SyllabusBlockUpsert @ExamCode = N'TAI', @Code = N'I', @Name = N'Organizacion del Estado y Administracion electronica', @DisplayOrder = 1, @ExamWeightPercent = 27.27;
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 1, @Title = N'La Constitucion Espanola de 1978: estructura y contenido. Derechos y deberes fundamentales, su garantia y suspension. El Tribunal Constitucional. El Defensor del Pueblo.', @Slug = N'constitucion-espanola-1978', @Keywords = N'Constitucion Espanola 1978,Tribunal Constitucional,Defensor del Pueblo';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 2, @Title = N'La Corona. Las Cortes Generales: composicion y atribuciones del Congreso de los Diputados y del Senado. El Poder Judicial.', @Slug = N'corona-cortes-generales-poder-judicial', @Keywords = N'Cortes Generales,Congreso,Senado,Poder Judicial';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 3, @Title = N'El Gobierno y la Administracion. Composicion, nombramiento y cese del Gobierno. La Administracion General del Estado: organizacion central y periferica. Los organismos publicos.', @Slug = N'gobierno-y-administracion-general-del-estado', @Keywords = N'Gobierno,Ley 50/1997,Ley 40/2015,Administracion General del Estado';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 4, @Title = N'La organizacion territorial del Estado: las Comunidades Autonomas y la Administracion local. La Union Europea: instituciones y ordenamiento juridico comunitario.', @Slug = N'organizacion-territorial-y-union-europea', @Keywords = N'Comunidades Autonomas,Administracion local,Union Europea';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 5, @Title = N'El personal al servicio de las Administraciones publicas: el Estatuto Basico del Empleado Publico. Derechos y deberes, regimen disciplinario e incompatibilidades. Politicas de igualdad y contra la violencia de genero.', @Slug = N'empleo-publico-igualdad', @Keywords = N'TREBEP,Real Decreto Legislativo 5/2015,Ley Organica 3/2007,Ley Organica 1/2004';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 6, @Title = N'El procedimiento administrativo comun de las Administraciones publicas: interesados, actos administrativos, terminos y plazos, fases del procedimiento y revision de actos. Los recursos administrativos.', @Slug = N'procedimiento-administrativo-comun', @Keywords = N'Ley 39/2015,acto administrativo,silencio administrativo,recursos administrativos';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 7, @Title = N'El regimen juridico del sector publico: principios de actuacion, organos colegiados, convenios y funcionamiento electronico del sector publico. Transparencia y reutilizacion de la informacion del sector publico.', @Slug = N'regimen-juridico-sector-publico-transparencia', @Keywords = N'Ley 40/2015,Ley 19/2013,Ley 37/2007,datos abiertos';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 8, @Title = N'La proteccion de datos personales y su regimen juridico: principios, derechos de las personas, obligaciones del responsable y del encargado, el delegado de proteccion de datos y las autoridades de control.', @Slug = N'proteccion-de-datos-personales', @Keywords = N'Reglamento (UE) 2016/679,RGPD,Ley Organica 3/2018,AEPD';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'I', @TopicNumber = 9, @Title = N'Administracion electronica: sede electronica, identificacion y firma electronica, registro y notificaciones electronicas, archivo electronico. El Esquema Nacional de Interoperabilidad y el Esquema Nacional de Seguridad.', @Slug = N'administracion-electronica-eni-ens', @Keywords = N'sede electronica,Real Decreto 4/2010,Real Decreto 311/2022,Cl@ve';

EXEC dbo.SyllabusBlockUpsert @ExamCode = N'TAI', @Code = N'II', @Name = N'Tecnologia basica', @DisplayOrder = 2, @ExamWeightPercent = 15.15;
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'II', @TopicNumber = 10, @Title = N'Conceptos de informatica basica. Representacion de la informacion. Arquitectura de computadores: unidad central de proceso, memoria, buses y unidades de entrada y salida.', @Slug = N'informatica-basica-arquitectura-de-computadores', @Keywords = N'representacion de la informacion,arquitectura von Neumann,CPU,memoria';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'II', @TopicNumber = 11, @Title = N'Perifericos y dispositivos de almacenamiento. Interfaces y buses de conexion. Elementos de un puesto de usuario.', @Slug = N'perifericos-y-almacenamiento', @Keywords = N'perifericos,discos,SSD,USB,RAID';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'II', @TopicNumber = 12, @Title = N'Estructuras de datos. Organizacion de ficheros y registros. Algoritmos elementales de busqueda y ordenacion.', @Slug = N'estructuras-de-datos-y-ficheros', @Keywords = N'pila,cola,arbol,tabla hash,complejidad';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'II', @TopicNumber = 13, @Title = N'Sistemas operativos: conceptos y componentes. Gestion de procesos, de memoria, de entrada y salida y de sistemas de ficheros.', @Slug = N'sistemas-operativos-conceptos', @Keywords = N'procesos,planificacion,memoria virtual,sistemas de ficheros';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'II', @TopicNumber = 14, @Title = N'Sistemas de gestion de bases de datos. El modelo relacional. Nociones de SQL. Bases de datos no relacionales.', @Slug = N'sistemas-gestores-de-bases-de-datos', @Keywords = N'SGBD,modelo relacional,SQL,NoSQL,ACID';

EXEC dbo.SyllabusBlockUpsert @ExamCode = N'TAI', @Code = N'III', @Name = N'Desarrollo de sistemas', @DisplayOrder = 3, @ExamWeightPercent = 27.27;
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 15, @Title = N'Modelado de datos: metodologias y reglas. Entidades, atributos y relaciones. Diseno logico y fisico. Normalizacion.', @Slug = N'modelado-de-datos-y-normalizacion', @Keywords = N'entidad-relacion,clave primaria,normalizacion,formas normales';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 16, @Title = N'Lenguajes de programacion. Tipos de datos y operadores. Instrucciones condicionales, bucles y recursividad. Procedimientos, funciones y parametros. Vectores y registros. Estructura de un programa.', @Slug = N'lenguajes-de-programacion', @Keywords = N'tipos de datos,recursividad,paso de parametros,compilacion';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 17, @Title = N'Lenguajes de interrogacion de bases de datos. El estandar ANSI SQL. Procedimientos almacenados. Eventos y disparadores.', @Slug = N'sql-procedimientos-y-disparadores', @Keywords = N'ANSI SQL,DDL,DML,trigger,procedimiento almacenado';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 18, @Title = N'Diseno y programacion orientada a objetos. Objetos, clases, herencia, metodos, sobrecarga y polimorfismo. Ventajas e inconvenientes.', @Slug = N'programacion-orientada-a-objetos', @Keywords = N'encapsulamiento,herencia,polimorfismo,interfaz';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 19, @Title = N'Patrones de diseno y lenguaje unificado de modelado (UML). Arquitectura Java EE / Jakarta EE y plataforma .NET: componentes, persistencia y seguridad.', @Slug = N'patrones-uml-java-dotnet', @Keywords = N'UML,patrones de diseno,Jakarta EE,.NET';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 20, @Title = N'Aplicaciones web. Desarrollo en cliente y en servidor, multiplataforma y multidispositivo. Lenguajes HTML y XML y sus derivaciones. Navegadores y lenguajes de programacion web. Servicios web y API REST.', @Slug = N'aplicaciones-web-y-servicios', @Keywords = N'HTML,XML,JSON,REST,JavaScript';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 21, @Title = N'Accesibilidad, diseno universal y usabilidad. Acceso y usabilidad de las tecnologias, productos y servicios relacionados con la sociedad de la informacion. Seguridad en el desarrollo de sistemas.', @Slug = N'accesibilidad-usabilidad-y-desarrollo-seguro', @Keywords = N'WCAG,Real Decreto 1112/2018,UNE-EN 301549,OWASP';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 22, @Title = N'Ciclo de vida del software. Metodologias de desarrollo predictivas y agiles. Pruebas y calidad del software.', @Slug = N'ciclo-de-vida-y-metodologias', @Keywords = N'ciclo de vida,Metrica v3,Scrum,pruebas';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'III', @TopicNumber = 23, @Title = N'Repositorios de codigo y control de versiones: estructura y actualizacion. Integracion y despliegue continuos.', @Slug = N'control-de-versiones-e-integracion-continua', @Keywords = N'Git,control de versiones,CI/CD,rama';

EXEC dbo.SyllabusBlockUpsert @ExamCode = N'TAI', @Code = N'IV', @Name = N'Sistemas y comunicaciones', @DisplayOrder = 4, @ExamWeightPercent = 30.31;
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 24, @Title = N'Administracion del sistema operativo y del software de base. Sistemas Windows y sistemas Unix y Linux. Gestion de usuarios y permisos.', @Slug = N'administracion-de-sistemas-operativos', @Keywords = N'Windows,Linux,permisos,Active Directory';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 25, @Title = N'Administracion de bases de datos: instalacion, usuarios y privilegios, monitorizacion y optimizacion del rendimiento.', @Slug = N'administracion-de-bases-de-datos', @Keywords = N'indices,plan de ejecucion,privilegios,transacciones';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 26, @Title = N'Sistemas de almacenamiento y su virtualizacion. Politicas, sistemas y procedimientos de copia de seguridad y recuperacion.', @Slug = N'almacenamiento-y-copias-de-seguridad', @Keywords = N'SAN,NAS,RAID,backup,RTO,RPO';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 27, @Title = N'Virtualizacion de sistemas y de puestos de usuario. Contenedores y orquestacion. Computacion en la nube y modelos de servicio.', @Slug = N'virtualizacion-contenedores-y-nube', @Keywords = N'hipervisor,contenedores,Kubernetes,IaaS,PaaS,SaaS';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 28, @Title = N'Conceptos de seguridad de los sistemas de informacion: seguridad fisica y logica, amenazas y vulnerabilidades. Tecnicas criptograficas y protocolos seguros.', @Slug = N'seguridad-de-los-sistemas-de-informacion', @Keywords = N'cifrado simetrico,clave publica,hash,firma electronica,PKI';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 29, @Title = N'Comunicaciones. Medios de transmision, modos de comunicacion, equipos terminales y equipos de interconexion y conmutacion.', @Slug = N'comunicaciones-y-medios-de-transmision', @Keywords = N'par trenzado,fibra optica,conmutador,encaminador';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 30, @Title = N'Redes de comunicaciones. Redes de area local, metropolitana y extensa. Redes inalambricas. El modelo de referencia OSI.', @Slug = N'redes-de-comunicaciones-y-modelo-osi', @Keywords = N'modelo OSI,LAN,WAN,Ethernet,IEEE 802.11,VLAN';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 31, @Title = N'El modelo TCP/IP y sus protocolos. Direccionamiento IPv4 e IPv6. Encaminamiento. Servicios DNS y DHCP.', @Slug = N'modelo-tcp-ip-y-direccionamiento', @Keywords = N'TCP,UDP,IPv4,IPv6,DNS,DHCP';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 32, @Title = N'Internet: arquitectura de red, origen, evolucion y estado actual. Principales servicios. Protocolos HTTP, HTTPS y TLS.', @Slug = N'internet-y-protocolos-web', @Keywords = N'HTTP,HTTPS,TLS,URI,correo electronico';
EXEC dbo.SyllabusTopicUpsert @ExamCode = N'TAI', @BlockCode = N'IV', @TopicNumber = 33, @Title = N'Seguridad y proteccion en redes de comunicaciones. Cortafuegos, sistemas de deteccion de intrusiones, redes privadas virtuales y seguridad perimetral. Gestion de incidentes.', @Slug = N'seguridad-en-redes-y-gestion-de-incidentes', @Keywords = N'cortafuegos,IDS,IPS,VPN,CCN-CERT,SIEM';


-- ---------- Banco de preguntas de TAI ----------

-- TAI-B1-T01-001 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La libertad, la justicia, la igualdad y el pluralismo político', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La libertad, la seguridad, la solidaridad y la democracia', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La dignidad, la libertad, la igualdad y la justicia social', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La justicia, la igualdad, la solidaridad y la unidad nacional', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'titulo-preliminar');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 1,
    @Statement = N'Según el artículo 1.1 de la Constitución Española, ¿cuáles son los valores superiores del ordenamiento jurídico?',
    @Explanation = N'El artículo 1.1 CE proclama que España se constituye en un Estado social y democrático de Derecho, que propugna como valores superiores de su ordenamiento jurídico la libertad, la justicia, la igualdad y el pluralismo político.',
    @SourceReference = N'Constitución Española, artículo 1.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-002 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La República parlamentaria', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La Monarquía parlamentaria', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La Monarquía constitucional', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Estado autonómico', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'titulo-preliminar');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 1,
    @Statement = N'De acuerdo con el artículo 1.3 de la Constitución Española, la forma política del Estado español es:',
    @Explanation = N'El artículo 1.3 CE establece literalmente que la forma política del Estado español es la Monarquía parlamentaria.',
    @SourceReference = N'Constitución Española, artículo 1.3',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-003 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'En las Cortes Generales, que representan al pueblo español', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'En el Rey, como Jefe del Estado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'En el pueblo español, del que emanan los poderes del Estado', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'En el conjunto de las instituciones del Estado y de las Comunidades Autónomas', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'titulo-preliminar');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 2,
    @Statement = N'¿En quién reside la soberanía nacional según el artículo 1.2 de la Constitución Española?',
    @Explanation = N'El artículo 1.2 CE dispone que la soberanía nacional reside en el pueblo español, del que emanan los poderes del Estado.',
    @SourceReference = N'Constitución Española, artículo 1.2',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-004 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'155', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'169', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'175', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'182', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'estructura');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-004',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 2,
    @Statement = N'¿Cuántos artículos tiene la Constitución Española de 1978?',
    @Explanation = N'La Constitución Española consta de un preámbulo, 169 artículos distribuidos en un Título Preliminar y diez Títulos, además de disposiciones adicionales, transitorias, derogatoria y final.',
    @SourceReference = N'Constitución Española, texto consolidado',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-005 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Todo el Título I de la Constitución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Los artículos 14 a 29 y la objeción de conciencia del artículo 30.2', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Los artículos 15 a 38 de la Constitución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Únicamente la Sección 1.ª del Capítulo II del Título I', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'derechos-fundamentales');
INSERT INTO @Tags (Name) VALUES (N'amparo');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-005',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 3,
    @Statement = N'El recurso de amparo ante el Tribunal Constitucional protege, conforme al artículo 53.2 de la Constitución, los derechos reconocidos en:',
    @Explanation = N'El artículo 53.2 CE reserva la tutela mediante recurso de amparo ante el Tribunal Constitucional a las libertades y derechos reconocidos en el artículo 14 y en la Sección 1.ª del Capítulo II (artículos 15 a 29), así como a la objeción de conciencia del artículo 30.2.',
    @SourceReference = N'Constitución Española, artículo 53.2',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-006 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Que se garantiza el acceso universal a Internet como derecho fundamental', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Que la ley limitará el uso de la informática para garantizar el honor y la intimidad personal y familiar de los ciudadanos y el pleno ejercicio de sus derechos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Que los ficheros públicos deben inscribirse en un registro dependiente del Ministerio del Interior', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que toda persona tiene derecho a la portabilidad de sus datos personales', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
INSERT INTO @Tags (Name) VALUES (N'derechos-fundamentales');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-006',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 3,
    @Statement = N'El artículo 18.4 de la Constitución Española contiene una previsión de especial relevancia informática. ¿Cuál es?',
    @Explanation = N'El artículo 18.4 CE establece que la ley limitará el uso de la informática para garantizar el honor y la intimidad personal y familiar de los ciudadanos y el pleno ejercicio de sus derechos. Es el fundamento constitucional de la protección de datos en España.',
    @SourceReference = N'Constitución Española, artículo 18.4',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-007 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Por 10 miembros: 4 propuestos por el Congreso, 4 por el Senado y 2 por el Gobierno', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Por 12 miembros: 4 propuestos por el Congreso, 4 por el Senado, 2 por el Gobierno y 2 por el Consejo General del Poder Judicial', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Por 15 miembros elegidos íntegramente por las Cortes Generales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Por 12 miembros elegidos por el Consejo General del Poder Judicial', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'tribunal-constitucional');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-007',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 3,
    @Statement = N'¿Cómo se compone el Tribunal Constitucional según el artículo 159 de la Constitución?',
    @Explanation = N'El artículo 159.1 CE dispone que el Tribunal Constitucional se compone de 12 miembros nombrados por el Rey: 4 a propuesta del Congreso por mayoría de tres quintos, 4 a propuesta del Senado con idéntica mayoría, 2 a propuesta del Gobierno y 2 a propuesta del Consejo General del Poder Judicial.',
    @SourceReference = N'Constitución Española, artículo 159.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-008 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un órgano del Poder Judicial encargado de la tutela de los derechos fundamentales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un alto comisionado de las Cortes Generales designado por éstas para la defensa de los derechos del Título I', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Un órgano consultivo del Gobierno en materia de derechos humanos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un miembro nato del Tribunal Constitucional', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'defensor-del-pueblo');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-008',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 2,
    @Statement = N'Conforme al artículo 54 de la Constitución Española, el Defensor del Pueblo es:',
    @Explanation = N'El artículo 54 CE configura al Defensor del Pueblo como alto comisionado de las Cortes Generales, designado por éstas para la defensa de los derechos comprendidos en el Título I, a cuyo efecto podrá supervisar la actividad de la Administración, dando cuenta a las Cortes Generales.',
    @SourceReference = N'Constitución Española, artículo 54',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-009 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Título VIII, relativo a la organización territorial del Estado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Título Preliminar, la Sección 1.ª del Capítulo II del Título I y el Título II', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Cualquier precepto del Título I de la Constitución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Título VII, relativo a economía y hacienda', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'reforma-constitucional');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-009',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 4,
    @Statement = N'La reforma agravada del artículo 168 de la Constitución se aplica, entre otros supuestos, a la revisión total del texto constitucional y a la revisión parcial que afecte a:',
    @Explanation = N'El artículo 168.1 CE reserva el procedimiento agravado a la revisión total de la Constitución o a una parcial que afecte al Título Preliminar, a la Sección 1.ª del Capítulo II del Título I (derechos fundamentales y libertades públicas) o al Título II (De la Corona).',
    @SourceReference = N'Constitución Española, artículo 168.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T01-010 | Bloque I | Tema 1
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La soberanía nacional y la unidad de la Nación española', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La dignidad de la persona, los derechos inviolables que le son inherentes, el libre desarrollo de la personalidad, el respeto a la ley y a los derechos de los demás', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La igualdad de todos los españoles ante la ley', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El sometimiento de los poderes públicos a la Constitución y al resto del ordenamiento jurídico', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'derechos-fundamentales');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T01-010',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 1,
    @Difficulty = 2,
    @Statement = N'El artículo 10.1 de la Constitución Española señala como fundamento del orden político y de la paz social:',
    @Explanation = N'El artículo 10.1 CE enumera la dignidad de la persona, los derechos inviolables que le son inherentes, el libre desarrollo de la personalidad, el respeto a la ley y a los derechos de los demás como fundamento del orden político y de la paz social.',
    @SourceReference = N'Constitución Española, artículo 10.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T02-001 | Bloque I | Tema 2
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un mínimo de 250 y un máximo de 400 diputados', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un mínimo de 300 y un máximo de 400 diputados', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Exactamente 350 diputados', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un mínimo de 350 y un máximo de 450 diputados', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'cortes-generales');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T02-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 2,
    @Difficulty = 2,
    @Statement = N'Según el artículo 68.1 de la Constitución Española, el Congreso de los Diputados se compone de:',
    @Explanation = N'El artículo 68.1 CE establece que el Congreso se compone de un mínimo de 300 y un máximo de 400 diputados, elegidos por sufragio universal, libre, igual, directo y secreto. La cifra concreta de 350 la fija la legislación electoral, no la Constitución.',
    @SourceReference = N'Constitución Española, artículo 68.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T02-002 | Bloque I | Tema 2
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cámara de representación territorial', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Cámara de segunda lectura legislativa', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Cámara de representación de las Comunidades Autónomas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cámara alta de designación autonómica', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'cortes-generales');
INSERT INTO @Tags (Name) VALUES (N'senado');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T02-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 2,
    @Difficulty = 2,
    @Statement = N'¿Cómo define la Constitución Española al Senado en su artículo 69.1?',
    @Explanation = N'El artículo 69.1 CE dispone que el Senado es la Cámara de representación territorial.',
    @SourceReference = N'Constitución Española, artículo 69.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T02-003 | Bloque I | Tema 2
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Presidente del Tribunal Supremo, que lo preside, y 20 miembros nombrados por el Rey por un período de cinco años', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Ministro de Justicia y 20 vocales elegidos por las Cortes Generales por seis años', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Presidente del Tribunal Constitucional y 12 vocales por un período de nueve años', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Únicamente 20 vocales de procedencia judicial elegidos por sus pares', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'poder-judicial');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T02-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 2,
    @Difficulty = 3,
    @Statement = N'El Consejo General del Poder Judicial, según el artículo 122.3 de la Constitución, está integrado por:',
    @Explanation = N'El artículo 122.3 CE establece que el Consejo General del Poder Judicial estará integrado por el Presidente del Tribunal Supremo, que lo presidirá, y por 20 miembros nombrados por el Rey por un período de cinco años.',
    @SourceReference = N'Constitución Española, artículo 122.3',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T02-004 | Bloque I | Tema 2
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Sin excepción alguna', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Salvo lo dispuesto en materia de garantías constitucionales', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Salvo en el orden contencioso-administrativo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Salvo en los territorios con derecho foral propio', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'poder-judicial');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T02-004',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 2,
    @Difficulty = 3,
    @Statement = N'De acuerdo con el artículo 123.1 de la Constitución, el Tribunal Supremo es el órgano jurisdiccional superior en todos los órdenes:',
    @Explanation = N'El artículo 123.1 CE señala que el Tribunal Supremo, con jurisdicción en toda España, es el órgano jurisdiccional superior en todos los órdenes, salvo lo dispuesto en materia de garantías constitucionales, competencia que corresponde al Tribunal Constitucional.',
    @SourceReference = N'Constitución Española, artículo 123.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T02-005 | Bloque I | Tema 2
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Del Rey, en cuyo nombre se administra', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'De las Cortes Generales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Del pueblo y se administra en nombre del Rey por Jueces y Magistrados', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Del Consejo General del Poder Judicial', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'poder-judicial');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T02-005',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 2,
    @Difficulty = 2,
    @Statement = N'Conforme al artículo 117.1 de la Constitución Española, la justicia emana:',
    @Explanation = N'El artículo 117.1 CE dispone que la justicia emana del pueblo y se administra en nombre del Rey por Jueces y Magistrados integrantes del poder judicial, independientes, inamovibles, responsables y sometidos únicamente al imperio de la ley.',
    @SourceReference = N'Constitución Española, artículo 117.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T03-001 | Bloque I | Tema 3
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Presidente, los Vicepresidentes en su caso, los Ministros y los demás miembros que establezca la ley', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Presidente, los Ministros y los Secretarios de Estado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Presidente y los Ministros exclusivamente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Presidente, los Vicepresidentes, los Ministros y los Subsecretarios', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'gobierno');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T03-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 3,
    @Difficulty = 2,
    @Statement = N'Según el artículo 98.1 de la Constitución Española, el Gobierno se compone de:',
    @Explanation = N'El artículo 98.1 CE establece que el Gobierno se compone del Presidente, de los Vicepresidentes, en su caso, de los Ministros y de los demás miembros que establezca la ley. Los Secretarios de Estado son órganos superiores de la Administración, pero no miembros del Gobierno.',
    @SourceReference = N'Constitución Española, artículo 98.1',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T03-002 | Bloque I | Tema 3
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Los Ministros y los Secretarios de Estado', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Los Subsecretarios y los Secretarios generales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Los Directores generales y los Subdirectores generales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Los Secretarios generales técnicos y los Directores generales', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-40-2015');
INSERT INTO @Tags (Name) VALUES (N'age');
INSERT INTO @Tags (Name) VALUES (N'organizacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T03-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 3,
    @Difficulty = 3,
    @Statement = N'En la organización central de la Administración General del Estado regulada en la Ley 40/2015, ¿cuáles son órganos superiores?',
    @Explanation = N'El artículo 55.3 de la Ley 40/2015 distingue entre órganos superiores (Ministros y Secretarios de Estado) y órganos directivos (Subsecretarios, Secretarios generales, Secretarios generales técnicos, Directores generales y Subdirectores generales).',
    @SourceReference = N'Ley 40/2015, de 1 de octubre, de Régimen Jurídico del Sector Público, artículo 55',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10566',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T03-003 | Bloque I | Tema 3
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Los organismos autónomos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Las entidades públicas empresariales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Las autoridades administrativas independientes', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Las delegaciones del Gobierno en las Comunidades Autónomas', 1);
INSERT INTO @Tags (Name) VALUES (N'ley-40-2015');
INSERT INTO @Tags (Name) VALUES (N'sector-publico-institucional');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T03-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 3,
    @Difficulty = 3,
    @Statement = N'¿Cuál de las siguientes NO es una entidad integrante del sector público institucional estatal según la Ley 40/2015?',
    @Explanation = N'Las Delegaciones del Gobierno en las Comunidades Autónomas forman parte de la organización periférica de la Administración General del Estado, no del sector público institucional. El artículo 84 de la Ley 40/2015 enumera entre este último los organismos autónomos, las entidades públicas empresariales, las autoridades administrativas independientes, las sociedades mercantiles estatales, los consorcios, las fundaciones del sector público y los fondos carentes de personalidad jurídica.',
    @SourceReference = N'Ley 40/2015, de 1 de octubre, artículo 84',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10566',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T04-001 | Bloque I | Tema 4
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Municipios, comarcas y Comunidades Autónomas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Municipios, provincias y las Comunidades Autónomas que se constituyan', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Provincias y Comunidades Autónomas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Municipios, provincias, regiones y Comunidades Autónomas', 0);
INSERT INTO @Tags (Name) VALUES (N'constitucion');
INSERT INTO @Tags (Name) VALUES (N'organizacion-territorial');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T04-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 4,
    @Difficulty = 2,
    @Statement = N'Según el artículo 137 de la Constitución Española, el Estado se organiza territorialmente en:',
    @Explanation = N'El artículo 137 CE establece que el Estado se organiza territorialmente en municipios, en provincias y en las Comunidades Autónomas que se constituyan, entidades que gozan de autonomía para la gestión de sus respectivos intereses.',
    @SourceReference = N'Constitución Española, artículo 137',
    @SourcePublication = N'BOE núm. 311, de 29 de diciembre de 1978',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-1978-31229',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T04-002 | Bloque I | Tema 4
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Reglamento obliga solo en cuanto al resultado y deja libertad de forma y medios a los Estados', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Reglamento tiene alcance general, es obligatorio en todos sus elementos y directamente aplicable en cada Estado miembro', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Reglamento requiere siempre una norma nacional de transposición', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Reglamento solo vincula a los destinatarios expresamente designados', 0);
INSERT INTO @Tags (Name) VALUES (N'union-europea');
INSERT INTO @Tags (Name) VALUES (N'derecho-comunitario');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T04-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 4,
    @Difficulty = 3,
    @Statement = N'En el Derecho de la Unión Europea, ¿cuál es la característica esencial de un Reglamento frente a una Directiva?',
    @Explanation = N'El artículo 288 del Tratado de Funcionamiento de la Unión Europea define el Reglamento como norma de alcance general, obligatoria en todos sus elementos y directamente aplicable en cada Estado miembro. La Directiva, en cambio, obliga al Estado destinatario en cuanto al resultado, dejando a las autoridades nacionales la elección de la forma y de los medios.',
    @SourceReference = N'Tratado de Funcionamiento de la Unión Europea, artículo 288',
    @SourcePublication = N'Versión consolidada, DOUE C 202, de 7 de junio de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/legal-content/ES/TXT/?uri=celex%3A12016E%2FTXT',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T04-003 | Bloque I | Tema 4
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Consejo Europeo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Tribunal de Cuentas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Banco Central Europeo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Comité Económico y Social Europeo', 1);
INSERT INTO @Tags (Name) VALUES (N'union-europea');
INSERT INTO @Tags (Name) VALUES (N'instituciones');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T04-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 4,
    @Difficulty = 3,
    @Statement = N'¿Cuál de las siguientes NO es una institución de la Unión Europea según el artículo 13 del Tratado de la Unión Europea?',
    @Explanation = N'El artículo 13 TUE enumera siete instituciones: el Parlamento Europeo, el Consejo Europeo, el Consejo, la Comisión Europea, el Tribunal de Justicia de la Unión Europea, el Banco Central Europeo y el Tribunal de Cuentas. El Comité Económico y Social Europeo es un órgano consultivo, no una institución.',
    @SourceReference = N'Tratado de la Unión Europea, artículo 13',
    @SourcePublication = N'Versión consolidada, DOUE C 202, de 7 de junio de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/legal-content/ES/TXT/?uri=celex%3A12016M%2FTXT',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T05-001 | Bloque I | Tema 5
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Funcionarios de carrera, funcionarios interinos, personal laboral y personal eventual', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Funcionarios de carrera, personal laboral y personal directivo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Funcionarios de carrera, funcionarios interinos y personal estatutario', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Funcionarios, contratados administrativos y personal eventual', 0);
INSERT INTO @Tags (Name) VALUES (N'trebep');
INSERT INTO @Tags (Name) VALUES (N'empleo-publico');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T05-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 5,
    @Difficulty = 2,
    @Statement = N'¿Qué clases de personal al servicio de las Administraciones Públicas reconoce el artículo 8 del texto refundido de la Ley del Estatuto Básico del Empleado Público?',
    @Explanation = N'El artículo 8.2 del Real Decreto Legislativo 5/2015 (TREBEP) clasifica a los empleados públicos en funcionarios de carrera, funcionarios interinos, personal laboral (fijo, por tiempo indefinido o temporal) y personal eventual.',
    @SourceReference = N'Real Decreto Legislativo 5/2015, texto refundido del Estatuto Básico del Empleado Público, artículo 8',
    @SourcePublication = N'BOE núm. 261, de 31 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-11719',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T05-002 | Bloque I | Tema 5
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Seis meses', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un año', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Dos años', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Tres años', 1);
INSERT INTO @Tags (Name) VALUES (N'trebep');
INSERT INTO @Tags (Name) VALUES (N'regimen-disciplinario');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T05-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 5,
    @Difficulty = 3,
    @Statement = N'Según el artículo 97 del TREBEP, ¿cuál es el plazo de prescripción de las faltas muy graves?',
    @Explanation = N'El artículo 97 del TREBEP fija la prescripción de las infracciones muy graves a los 3 años, las graves a los 2 años y las leves a los 6 meses.',
    @SourceReference = N'Real Decreto Legislativo 5/2015, artículo 97',
    @SourcePublication = N'BOE núm. 261, de 31 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-11719',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T05-003 | Bloque I | Tema 5
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un código de conducta voluntario sin efectos jurídicos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un conjunto ordenado de medidas adoptadas tras realizar un diagnóstico de situación, tendentes a alcanzar la igualdad de trato y de oportunidades y a eliminar la discriminación por razón de sexo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Un informe anual que las empresas remiten a la Inspección de Trabajo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Una autorización administrativa previa para la contratación de personal', 0);
INSERT INTO @Tags (Name) VALUES (N'igualdad');
INSERT INTO @Tags (Name) VALUES (N'ley-organica-3-2007');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T05-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 5,
    @Difficulty = 3,
    @Statement = N'De acuerdo con la Ley Orgánica 3/2007, para la igualdad efectiva de mujeres y hombres, un plan de igualdad es:',
    @Explanation = N'El artículo 46.1 de la Ley Orgánica 3/2007 define los planes de igualdad como un conjunto ordenado de medidas, adoptadas después de realizar un diagnóstico de situación, tendentes a alcanzar en la empresa la igualdad de trato y de oportunidades entre mujeres y hombres y a eliminar la discriminación por razón de sexo.',
    @SourceReference = N'Ley Orgánica 3/2007, de 22 de marzo, artículo 46',
    @SourcePublication = N'BOE núm. 71, de 23 de marzo de 2007',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2007-6115',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-001 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un mes', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Tres meses', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Seis meses', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un año', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'plazos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 2,
    @Statement = N'Según el artículo 21.3 de la Ley 39/2015, cuando las normas reguladoras del procedimiento no fijen plazo máximo para resolver y notificar, éste será de:',
    @Explanation = N'El artículo 21.3 de la Ley 39/2015 establece que cuando las normas reguladoras de los procedimientos no fijen el plazo máximo, éste será de tres meses, contados desde la fecha del acuerdo de iniciación en los procedimientos de oficio o desde la entrada de la solicitud en el registro electrónico de la Administración competente en los iniciados a solicitud del interesado.',
    @SourceReference = N'Ley 39/2015, de 1 de octubre, del Procedimiento Administrativo Común, artículo 21.3',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-002 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La desestimación de la solicitud por silencio administrativo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La estimación de la solicitud por silencio administrativo, salvo las excepciones legalmente previstas', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La caducidad del procedimiento', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La suspensión del procedimiento hasta que se resuelva', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'silencio-administrativo');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 3,
    @Statement = N'En los procedimientos iniciados a solicitud del interesado, el vencimiento del plazo máximo sin haberse notificado resolución expresa produce, con carácter general:',
    @Explanation = N'El artículo 24.1 de la Ley 39/2015 establece la regla general del silencio positivo en procedimientos iniciados a solicitud del interesado, salvo que una norma con rango de ley o de Derecho de la Unión Europea o de Derecho internacional aplicable en España establezca lo contrario, y salvo los supuestos expresamente exceptuados en el propio precepto.',
    @SourceReference = N'Ley 39/2015, artículo 24',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-003 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Son siempre días naturales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Son hábiles, excluyéndose del cómputo los sábados, los domingos y los declarados festivos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Son hábiles, excluyéndose únicamente los domingos y festivos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Son naturales salvo que la norma diga expresamente lo contrario', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'plazos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 3,
    @Statement = N'En el cómputo de plazos señalados por días en la Ley 39/2015, se entiende que:',
    @Explanation = N'El artículo 30.2 de la Ley 39/2015 dispone que, siempre que por ley o Derecho de la Unión Europea no se exprese otro cómputo, cuando los plazos se señalen por días se entiende que éstos son hábiles, excluyéndose del cómputo los sábados, los domingos y los declarados festivos.',
    @SourceReference = N'Ley 39/2015, artículo 30.2',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-004 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cinco días hábiles desde su puesta a disposición', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Diez días naturales desde su puesta a disposición', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Quince días hábiles desde su puesta a disposición', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un mes desde su puesta a disposición', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'notificaciones-electronicas');
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-004',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 3,
    @Statement = N'Una notificación practicada por medios electrónicos se entenderá rechazada cuando hayan transcurrido, sin acceder a su contenido:',
    @Explanation = N'El artículo 43.2 de la Ley 39/2015 establece que, cuando la notificación por medios electrónicos sea de carácter obligatorio o haya sido expresamente elegida por el interesado, se entenderá rechazada cuando hayan transcurrido diez días naturales desde la puesta a disposición de la notificación sin que se acceda a su contenido.',
    @SourceReference = N'Ley 39/2015, artículo 43.2',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-005 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Las personas jurídicas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Las entidades sin personalidad jurídica', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Las personas físicas que no ejerzan actividad profesional con colegiación obligatoria', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Quienes representen a un interesado que esté obligado a relacionarse electrónicamente', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-005',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 3,
    @Statement = N'¿Cuál de los siguientes sujetos NO está obligado a relacionarse electrónicamente con las Administraciones Públicas según el artículo 14.2 de la Ley 39/2015?',
    @Explanation = N'El artículo 14.2 de la Ley 39/2015 obliga a relacionarse electrónicamente a las personas jurídicas, las entidades sin personalidad jurídica, quienes ejerzan una actividad profesional para la que se requiera colegiación obligatoria, quienes representen a un interesado obligado y los empleados públicos en los trámites que realicen por razón de su condición. Las personas físicas no incluidas en esos supuestos pueden elegir el medio.',
    @SourceReference = N'Ley 39/2015, artículo 14',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-006 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Diez días hábiles', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Quince días hábiles', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Un mes', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Tres meses', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'recursos-administrativos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-006',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 3,
    @Statement = N'El plazo para interponer recurso de alzada contra un acto expreso es de:',
    @Explanation = N'El artículo 122.1 de la Ley 39/2015 establece que el plazo para la interposición del recurso de alzada será de un mes si el acto fuera expreso. Si no lo fuera, el recurso podrá interponerse en cualquier momento a partir del día siguiente a aquel en que se produzcan los efectos del silencio administrativo.',
    @SourceReference = N'Ley 39/2015, artículo 122.1',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T06-007 | Bloque I | Tema 6
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cualquier infracción del ordenamiento jurídico, incluida la desviación de poder', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Los actos dictados por órgano manifiestamente incompetente por razón de la materia o del territorio', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Los defectos de forma que no impidan al acto alcanzar su fin', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La actuación administrativa realizada fuera del tiempo establecido, con carácter general', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-39-2015');
INSERT INTO @Tags (Name) VALUES (N'acto-administrativo');
INSERT INTO @Tags (Name) VALUES (N'nulidad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T06-007',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 6,
    @Difficulty = 4,
    @Statement = N'¿Cuál de los siguientes supuestos determina la nulidad de pleno derecho de un acto administrativo conforme al artículo 47 de la Ley 39/2015?',
    @Explanation = N'El artículo 47.1.b) de la Ley 39/2015 declara nulos de pleno derecho los actos dictados por órgano manifiestamente incompetente por razón de la materia o del territorio. Los demás supuestos citados corresponden a la anulabilidad del artículo 48 o carecen de efecto invalidante.',
    @SourceReference = N'Ley 39/2015, artículos 47 y 48',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T07-001 | Bloque I | Tema 7
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Dos años, prorrogables por otros dos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Cuatro años, salvo que normativamente se prevea un plazo superior, pudiendo acordarse una prórroga de hasta cuatro años adicionales', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Seis años improrrogables', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Diez años, prorrogables indefinidamente', 0);
INSERT INTO @Tags (Name) VALUES (N'ley-40-2015');
INSERT INTO @Tags (Name) VALUES (N'convenios');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T07-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 7,
    @Difficulty = 3,
    @Statement = N'Según el artículo 49 de la Ley 40/2015, los convenios administrativos tendrán una duración determinada que no podrá ser superior a:',
    @Explanation = N'El artículo 49.h) de la Ley 40/2015 fija en cuatro años la duración máxima de los convenios, salvo que normativamente se prevea un plazo superior, y permite acordar unánimemente una prórroga o su extinción, con un máximo de cuatro años adicionales.',
    @SourceReference = N'Ley 40/2015, artículo 49',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10566',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T07-002 | Bloque I | Tema 7
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Diez días hábiles', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un mes desde la recepción de la solicitud por el órgano competente para resolver', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Tres meses', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Seis meses', 0);
INSERT INTO @Tags (Name) VALUES (N'transparencia');
INSERT INTO @Tags (Name) VALUES (N'ley-19-2013');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T07-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 7,
    @Difficulty = 3,
    @Statement = N'En la Ley 19/2013, de transparencia, acceso a la información pública y buen gobierno, el plazo máximo para resolver una solicitud de acceso a la información es de:',
    @Explanation = N'El artículo 20.1 de la Ley 19/2013 establece que la resolución se notificará al solicitante y a los terceros afectados en el plazo máximo de un mes desde la recepción de la solicitud por el órgano competente para resolver, ampliable por otro mes en caso de volumen o complejidad de la información.',
    @SourceReference = N'Ley 19/2013, de 9 de diciembre, artículo 20',
    @SourcePublication = N'BOE núm. 295, de 10 de diciembre de 2013',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2013-12887',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T07-003 | Bloque I | Tema 7
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La Ley 37/2007, de 16 de noviembre, sobre reutilización de la información del sector público', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La Ley 11/2007, de acceso electrónico de los ciudadanos a los servicios públicos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Real Decreto 4/2010, por el que se regula el Esquema Nacional de Interoperabilidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La Ley 9/2017, de Contratos del Sector Público', 0);
INSERT INTO @Tags (Name) VALUES (N'reutilizacion');
INSERT INTO @Tags (Name) VALUES (N'datos-abiertos');
INSERT INTO @Tags (Name) VALUES (N'ley-37-2007');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T07-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 7,
    @Difficulty = 3,
    @Statement = N'La reutilización de la información del sector público en España se regula principalmente en:',
    @Explanation = N'La Ley 37/2007, de 16 de noviembre, sobre reutilización de la información del sector público, traspone la normativa europea en materia de datos abiertos y regula la reutilización de los documentos elaborados o custodiados por las Administraciones y organismos del sector público.',
    @SourceReference = N'Ley 37/2007, de 16 de noviembre, sobre reutilización de la información del sector público',
    @SourcePublication = N'BOE núm. 276, de 17 de noviembre de 2007',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2007-19814',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-001 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Minimización de datos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Limitación de la finalidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Gratuidad del tratamiento', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Integridad y confidencialidad', 0);
INSERT INTO @Tags (Name) VALUES (N'rgpd');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
INSERT INTO @Tags (Name) VALUES (N'principios');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 2,
    @Statement = N'¿Cuál de los siguientes NO es un principio relativo al tratamiento de datos personales del artículo 5 del Reglamento General de Protección de Datos?',
    @Explanation = N'El artículo 5 del RGPD enumera los principios de licitud, lealtad y transparencia; limitación de la finalidad; minimización de datos; exactitud; limitación del plazo de conservación; integridad y confidencialidad; y responsabilidad proactiva. La gratuidad del tratamiento no es uno de ellos.',
    @SourceReference = N'Reglamento (UE) 2016/679, artículo 5',
    @SourcePublication = N'DOUE L 119, de 4 de mayo de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/eli/reg/2016/679/oj',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-002 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'24 horas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'48 horas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'72 horas', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'5 días hábiles', 0);
INSERT INTO @Tags (Name) VALUES (N'rgpd');
INSERT INTO @Tags (Name) VALUES (N'brechas-de-seguridad');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 3,
    @Statement = N'El responsable del tratamiento debe notificar una violación de la seguridad de los datos personales a la autoridad de control, salvo que sea improbable que constituya un riesgo, sin dilación indebida y a más tardar en:',
    @Explanation = N'El artículo 33.1 del RGPD exige notificar la violación de seguridad a la autoridad de control competente sin dilación indebida y, de ser posible, a más tardar 72 horas después de que haya tenido constancia de ella, salvo que sea improbable que la violación suponga un riesgo para los derechos y libertades de las personas físicas.',
    @SourceReference = N'Reglamento (UE) 2016/679, artículo 33',
    @SourcePublication = N'DOUE L 119, de 4 de mayo de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/eli/reg/2016/679/oj',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-003 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El tratamiento lo lleve a cabo una autoridad u organismo público, excepto los tribunales que actúen en ejercicio de su función judicial', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La entidad tenga más de 250 empleados, en todo caso', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Se traten datos de más de 100.000 personas al año', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El responsable esté establecido fuera de la Unión Europea', 0);
INSERT INTO @Tags (Name) VALUES (N'rgpd');
INSERT INTO @Tags (Name) VALUES (N'dpd');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 3,
    @Statement = N'Respecto al delegado de protección de datos, el RGPD establece que su designación es obligatoria cuando:',
    @Explanation = N'El artículo 37.1.a) del RGPD impone la designación de delegado de protección de datos cuando el tratamiento lo lleve a cabo una autoridad u organismo público, excepto los tribunales que actúen en ejercicio de su función judicial, además de los supuestos de observación habitual y sistemática a gran escala y de tratamiento a gran escala de categorías especiales de datos.',
    @SourceReference = N'Reglamento (UE) 2016/679, artículo 37',
    @SourcePublication = N'DOUE L 119, de 4 de mayo de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/eli/reg/2016/679/oj',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-004 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'10.000.000 de euros o el 2 % del volumen de negocio total anual global del ejercicio financiero anterior', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'20.000.000 de euros o el 4 % del volumen de negocio total anual global del ejercicio financiero anterior', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'600.000 euros como límite absoluto', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El 10 % del volumen de negocio, sin límite absoluto', 0);
INSERT INTO @Tags (Name) VALUES (N'rgpd');
INSERT INTO @Tags (Name) VALUES (N'sanciones');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-004',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 4,
    @Statement = N'Las multas administrativas más graves previstas en el artículo 83.5 del RGPD pueden alcanzar:',
    @Explanation = N'El artículo 83.5 del RGPD prevé, para las infracciones más graves, multas de hasta 20.000.000 de euros o, tratándose de una empresa, de una cuantía equivalente al 4 % como máximo del volumen de negocio total anual global del ejercicio financiero anterior, optándose por la de mayor cuantía.',
    @SourceReference = N'Reglamento (UE) 2016/679, artículo 83.5',
    @SourcePublication = N'DOUE L 119, de 4 de mayo de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/eli/reg/2016/679/oj',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-005 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La Ley Orgánica 15/1999, de Protección de Datos de Carácter Personal', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La Ley Orgánica 3/2018, de Protección de Datos Personales y garantía de los derechos digitales', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La Ley 34/2002, de servicios de la sociedad de la información', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Real Decreto 1720/2007', 0);
INSERT INTO @Tags (Name) VALUES (N'lopdgdd');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
INSERT INTO @Tags (Name) VALUES (N'derechos-digitales');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-005',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 3,
    @Statement = N'¿Qué norma española adapta el ordenamiento interno al Reglamento General de Protección de Datos y regula los derechos digitales?',
    @Explanation = N'La Ley Orgánica 3/2018, de 5 de diciembre, de Protección de Datos Personales y garantía de los derechos digitales (LOPDGDD), adapta el ordenamiento español al RGPD y dedica su Título X a la garantía de los derechos digitales.',
    @SourceReference = N'Ley Orgánica 3/2018, de 5 de diciembre',
    @SourcePublication = N'BOE núm. 294, de 6 de diciembre de 2018',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2018-16673',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T08-006 | Bloque I | Tema 8
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Derecho a la portabilidad de los datos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Derecho de supresión o derecho al olvido', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Derecho a la limitación del tratamiento', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Derecho a la titularidad exclusiva del algoritmo de tratamiento', 1);
INSERT INTO @Tags (Name) VALUES (N'rgpd');
INSERT INTO @Tags (Name) VALUES (N'derechos');
INSERT INTO @Tags (Name) VALUES (N'proteccion-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T08-006',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 8,
    @Difficulty = 3,
    @Statement = N'¿Cuál de los siguientes derechos NO está expresamente reconocido en el Capítulo III del RGPD?',
    @Explanation = N'El Capítulo III del RGPD reconoce los derechos de información, acceso, rectificación, supresión, limitación del tratamiento, portabilidad, oposición y a no ser objeto de decisiones individuales automatizadas. No existe un derecho a la titularidad del algoritmo.',
    @SourceReference = N'Reglamento (UE) 2016/679, artículos 12 a 22',
    @SourcePublication = N'DOUE L 119, de 4 de mayo de 2016',
    @SourceUrl = N'https://eur-lex.europa.eu/eli/reg/2016/679/oj',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-001 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El Real Decreto 3/2010, de 8 de enero', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El Real Decreto 311/2022, de 3 de mayo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Real Decreto 4/2010, de 8 de enero', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El Real Decreto 1112/2018, de 7 de septiembre', 0);
INSERT INTO @Tags (Name) VALUES (N'ens');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-001',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 3,
    @Statement = N'El Esquema Nacional de Seguridad vigente se aprueba por:',
    @Explanation = N'El Real Decreto 311/2022, de 3 de mayo, regula el Esquema Nacional de Seguridad y deroga el anterior Real Decreto 3/2010. El Real Decreto 4/2010 corresponde al Esquema Nacional de Interoperabilidad y el 1112/2018 a la accesibilidad de sitios web y aplicaciones móviles.',
    @SourceReference = N'Real Decreto 311/2022, de 3 de mayo, por el que se regula el Esquema Nacional de Seguridad',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-002 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Confidencialidad, integridad y disponibilidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Disponibilidad, autenticidad, integridad, confidencialidad y trazabilidad', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Autenticidad, no repudio, integridad y disponibilidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Confidencialidad, integridad, disponibilidad y resiliencia', 0);
INSERT INTO @Tags (Name) VALUES (N'ens');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'dimensiones');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-002',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 3,
    @Statement = N'¿Cuáles son las dimensiones de seguridad consideradas en el Esquema Nacional de Seguridad?',
    @Explanation = N'El Esquema Nacional de Seguridad considera cinco dimensiones de seguridad: disponibilidad, autenticidad, integridad, confidencialidad y trazabilidad, sobre las que se determina la categoría del sistema.',
    @SourceReference = N'Real Decreto 311/2022, Anexo I',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-003 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Baja, media y alta', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Básica, media y alta', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Nivel 1, nivel 2 y nivel 3', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ordinaria, reforzada y crítica', 0);
INSERT INTO @Tags (Name) VALUES (N'ens');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'categorias');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-003',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 3,
    @Statement = N'Las categorías de seguridad de un sistema en el Esquema Nacional de Seguridad son:',
    @Explanation = N'El Esquema Nacional de Seguridad clasifica los sistemas en tres categorías: BÁSICA, MEDIA y ALTA, determinadas en función del nivel más alto alcanzado por sus dimensiones de seguridad.',
    @SourceReference = N'Real Decreto 311/2022, artículo 40 y Anexo I',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-004 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Normas Técnicas de Interoperabilidad', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Instrucciones Técnicas de Seguridad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Guías CCN-STIC de obligado cumplimiento', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Órdenes ministeriales de cada departamento', 0);
INSERT INTO @Tags (Name) VALUES (N'eni');
INSERT INTO @Tags (Name) VALUES (N'interoperabilidad');
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-004',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 3,
    @Statement = N'El Esquema Nacional de Interoperabilidad se aprueba por el Real Decreto 4/2010 y se desarrolla mediante:',
    @Explanation = N'El Real Decreto 4/2010, por el que se regula el Esquema Nacional de Interoperabilidad, se desarrolla mediante Normas Técnicas de Interoperabilidad (NTI), que abordan aspectos como el documento electrónico, el expediente electrónico, la política de firma o el catálogo de estándares. Las Instrucciones Técnicas de Seguridad corresponden al Esquema Nacional de Seguridad.',
    @SourceReference = N'Real Decreto 4/2010, de 8 de enero, Esquema Nacional de Interoperabilidad',
    @SourcePublication = N'BOE núm. 25, de 29 de enero de 2010',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2010-1331',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-005 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Una empresa proveedora de servicios acreditada', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Una Administración Pública, o bien a una o varios organismos públicos o entidades de Derecho Público en el ejercicio de sus competencias', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El Ministerio competente en materia de transformación digital, en exclusiva', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cualquier entidad que preste servicios públicos, sea pública o privada', 0);
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
INSERT INTO @Tags (Name) VALUES (N'sede-electronica');
INSERT INTO @Tags (Name) VALUES (N'ley-40-2015');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-005',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 3,
    @Statement = N'Según el artículo 38 de la Ley 40/2015, la sede electrónica es aquella dirección electrónica disponible para los ciudadanos a través de redes de telecomunicaciones cuya titularidad corresponde a:',
    @Explanation = N'El artículo 38.1 de la Ley 40/2015 define la sede electrónica como aquella dirección electrónica, disponible para los ciudadanos a través de redes de telecomunicaciones, cuya titularidad corresponde a una Administración Pública, o bien a una o varios organismos públicos o entidades de Derecho Público en el ejercicio de sus competencias.',
    @SourceReference = N'Ley 40/2015, artículo 38',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10566',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-006 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cada seis meses', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Cada año', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Cada dos años', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cada cinco años', 0);
INSERT INTO @Tags (Name) VALUES (N'ens');
INSERT INTO @Tags (Name) VALUES (N'auditoria');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-006',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 4,
    @Statement = N'Conforme al Esquema Nacional de Seguridad, los sistemas de categoría MEDIA y ALTA deben ser objeto de una auditoría ordinaria de seguridad, al menos:',
    @Explanation = N'El artículo 31 del Real Decreto 311/2022 exige que los sistemas de información de categoría MEDIA o ALTA sean objeto de una auditoría ordinaria al menos cada dos años, además de auditorías extraordinarias cuando se produzcan modificaciones sustanciales. Los de categoría BÁSICA pueden sustituirla por una autoevaluación.',
    @SourceReference = N'Real Decreto 311/2022, artículo 31',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B1-T09-007 | Bloque I | Tema 9
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un sistema de identificación, autenticación y firma electrónica común para el conjunto del sector público administrativo estatal', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un registro centralizado de apoderamientos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Una plataforma de intermediación de datos entre Administraciones', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El punto de acceso general electrónico de la Administración', 0);
INSERT INTO @Tags (Name) VALUES (N'administracion-electronica');
INSERT INTO @Tags (Name) VALUES (N'identificacion');
INSERT INTO @Tags (Name) VALUES (N'clave');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B1-T09-007',
    @ExamCode = N'TAI',
    @BlockCode = N'I',
    @TopicNumber = 9,
    @Difficulty = 2,
    @Statement = N'Cl@ve es, en el ámbito de la Administración electrónica española:',
    @Explanation = N'Cl@ve es el sistema de identificación, autenticación y firma electrónica común para el sector público administrativo estatal, que admite tanto claves concertadas como certificados electrónicos y el DNI electrónico. La intermediación de datos corresponde a la plataforma SVD y el punto de acceso general es un servicio distinto.',
    @SourceReference = N'Ley 39/2015, artículos 9 y 10, sobre sistemas de identificación y firma de los interesados',
    @SourcePublication = N'BOE núm. 236, de 2 de octubre de 2015',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2015-10565',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-001 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'128', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'255', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'256', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'512', 0);
INSERT INTO @Tags (Name) VALUES (N'representacion-de-la-informacion');
INSERT INTO @Tags (Name) VALUES (N'binario');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-001',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 1,
    @Statement = N'¿Cuántos valores distintos pueden representarse con un byte de 8 bits?',
    @Explanation = N'Con n bits se representan 2^n combinaciones distintas. Para 8 bits son 2^8 = 256 valores, que en binario sin signo corresponden al rango de 0 a 255.',
    @SourceReference = N'Representación binaria de la información. Fundamentos de arquitectura de computadores',
    @SourcePublication = N'Temario TAI, Bloque II, tema 10',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-002 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Utiliza memorias y buses separados para instrucciones y datos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Almacena instrucciones y datos en la misma memoria, accesible por un mismo bus', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'No dispone de unidad aritmético-lógica', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ejecuta siempre las instrucciones en paralelo', 0);
INSERT INTO @Tags (Name) VALUES (N'arquitectura-de-computadores');
INSERT INTO @Tags (Name) VALUES (N'von-neumann');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-002',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 2,
    @Statement = N'En la arquitectura de von Neumann, la característica diferencial respecto a la arquitectura Harvard es que:',
    @Explanation = N'La arquitectura de von Neumann se caracteriza por el concepto de programa almacenado: instrucciones y datos comparten la misma memoria y el mismo bus de acceso. La arquitectura Harvard separa físicamente ambas memorias y sus buses.',
    @SourceReference = N'Arquitectura de von Neumann. Fundamentos de arquitectura de computadores',
    @SourcePublication = N'Temario TAI, Bloque II, tema 10',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-003 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La unidad aritmético-lógica (ALU)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La unidad de control', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El banco de registros', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La memoria caché de nivel 1', 0);
INSERT INTO @Tags (Name) VALUES (N'arquitectura-de-computadores');
INSERT INTO @Tags (Name) VALUES (N'cpu');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-003',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 2,
    @Statement = N'¿Qué unidad de la CPU se encarga de interpretar las instrucciones y generar las señales que gobiernan el resto de componentes?',
    @Explanation = N'La unidad de control decodifica cada instrucción y emite las señales de control que coordinan la ALU, los registros y el acceso a memoria. La ALU realiza las operaciones aritméticas y lógicas propiamente dichas.',
    @SourceReference = N'Unidad central de proceso: unidad de control y unidad aritmético-lógica',
    @SourcePublication = N'Temario TAI, Bloque II, tema 10',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-004 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Números enteros con signo en complemento a dos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Números en coma flotante de precisión simple y doble', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Caracteres alfanuméricos en 8 bits', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Direcciones de memoria virtual', 0);
INSERT INTO @Tags (Name) VALUES (N'representacion-de-la-informacion');
INSERT INTO @Tags (Name) VALUES (N'coma-flotante');
INSERT INTO @Tags (Name) VALUES (N'ieee-754');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-004',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 3,
    @Statement = N'El estándar IEEE 754 define la representación de:',
    @Explanation = N'El estándar IEEE 754 especifica los formatos de representación de números en coma flotante binaria, incluyendo precisión simple (32 bits) y doble (64 bits), con sus campos de signo, exponente y mantisa, así como valores especiales como NaN o infinito.',
    @SourceReference = N'IEEE 754, Standard for Floating-Point Arithmetic',
    @SourcePublication = N'IEEE Standards Association',
    @SourceUrl = N'https://standards.ieee.org/ieee/754/6210/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-005 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'A6', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'B6', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'C6', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'D6', 0);
INSERT INTO @Tags (Name) VALUES (N'representacion-de-la-informacion');
INSERT INTO @Tags (Name) VALUES (N'hexadecimal');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-005',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 2,
    @Statement = N'El número binario 1011 0110 equivale, en hexadecimal, a:',
    @Explanation = N'Cada grupo de cuatro bits corresponde a un dígito hexadecimal: 1011 es B y 0110 es 6, de modo que 10110110 en binario equivale a B6 en hexadecimal (182 en decimal).',
    @SourceReference = N'Sistemas de numeración y conversión entre bases',
    @SourcePublication = N'Temario TAI, Bloque II, tema 10',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T10-006 | Bloque II | Tema 10
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Usar siempre 2 bytes por carácter', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Ser de longitud variable, de 1 a 4 bytes, y compatible con ASCII en su primer rango', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Usar siempre 4 bytes por carácter', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Codificar únicamente los caracteres del alfabeto latino', 0);
INSERT INTO @Tags (Name) VALUES (N'representacion-de-la-informacion');
INSERT INTO @Tags (Name) VALUES (N'unicode');
INSERT INTO @Tags (Name) VALUES (N'utf-8');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T10-006',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 10,
    @Difficulty = 3,
    @Statement = N'UTF-8 es una codificación de caracteres que se caracteriza por:',
    @Explanation = N'UTF-8 codifica cada punto de código Unicode con una secuencia de 1 a 4 bytes. Los 128 primeros puntos de código coinciden byte a byte con ASCII, lo que le proporciona compatibilidad hacia atrás.',
    @SourceReference = N'RFC 3629, UTF-8, a transformation format of ISO 10646',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc3629',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T11-001 | Bloque II | Tema 11
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Distribución de datos en bandas sin redundancia', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Duplicación en espejo de los datos en dos o más discos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Distribución en bandas con paridad distribuida', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Distribución en bandas con doble paridad', 0);
INSERT INTO @Tags (Name) VALUES (N'almacenamiento');
INSERT INTO @Tags (Name) VALUES (N'raid');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T11-001',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 11,
    @Difficulty = 2,
    @Statement = N'En un sistema RAID 1, la técnica empleada es:',
    @Explanation = N'RAID 1 aplica mirroring o duplicación en espejo: cada dato se escribe íntegramente en dos o más discos. RAID 0 es distribución en bandas sin redundancia, RAID 5 usa paridad distribuida y RAID 6 doble paridad.',
    @SourceReference = N'Niveles RAID. Sistemas de almacenamiento redundante',
    @SourcePublication = N'Temario TAI, Bloque II, tema 11',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T11-002 | Bloque II | Tema 11
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Dos discos, con la capacidad de uno de ellos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Tres discos, con la capacidad equivalente a n-1 discos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Cuatro discos, con la capacidad equivalente a n-2 discos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Tres discos, con la capacidad de los n discos', 0);
INSERT INTO @Tags (Name) VALUES (N'almacenamiento');
INSERT INTO @Tags (Name) VALUES (N'raid');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T11-002',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 11,
    @Difficulty = 3,
    @Statement = N'¿Cuántos discos como mínimo necesita un conjunto RAID 5 y cuál es su capacidad útil?',
    @Explanation = N'RAID 5 requiere un mínimo de tres discos y reserva el equivalente a la capacidad de un disco para la información de paridad, distribuida entre todos ellos. La capacidad útil es por tanto la de n-1 discos y tolera el fallo de uno.',
    @SourceReference = N'Niveles RAID. Sistemas de almacenamiento redundante',
    @SourcePublication = N'Temario TAI, Bloque II, tema 11',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T11-003 | Bloque II | Tema 11
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Su mayor capacidad máxima por unidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La ausencia de partes mecánicas móviles, con menor latencia de acceso', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Su capacidad ilimitada de ciclos de escritura', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que no requiere sistema de ficheros', 0);
INSERT INTO @Tags (Name) VALUES (N'almacenamiento');
INSERT INTO @Tags (Name) VALUES (N'ssd');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T11-003',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 11,
    @Difficulty = 2,
    @Statement = N'La principal ventaja de una unidad SSD frente a un disco duro magnético tradicional es:',
    @Explanation = N'Las unidades de estado sólido almacenan la información en memoria flash, sin cabezales ni platos giratorios. Esto elimina el tiempo de búsqueda y la latencia rotacional, reduciendo drásticamente el tiempo de acceso. Su número de ciclos de escritura es, en cambio, finito.',
    @SourceReference = N'Dispositivos de almacenamiento. Memoria flash y unidades de estado sólido',
    @SourcePublication = N'Temario TAI, Bloque II, tema 11',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T12-001 | Bloque II | Tema 12
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'FIFO, el primero en entrar es el primero en salir', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'LIFO, el último en entrar es el primero en salir', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'De acceso aleatorio por índice', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'De acceso ordenado por prioridad', 0);
INSERT INTO @Tags (Name) VALUES (N'estructuras-de-datos');
INSERT INTO @Tags (Name) VALUES (N'pila');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T12-001',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 12,
    @Difficulty = 1,
    @Statement = N'Una estructura de datos de tipo pila (stack) se caracteriza por seguir una política:',
    @Explanation = N'La pila es una estructura LIFO (Last In, First Out): las operaciones de inserción (push) y extracción (pop) actúan sobre el mismo extremo. La cola, en cambio, es FIFO.',
    @SourceReference = N'Estructuras de datos lineales: pilas y colas',
    @SourcePublication = N'Temario TAI, Bloque II, tema 12',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T12-002 | Bloque II | Tema 12
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'O(1)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'O(log n)', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'O(n)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'O(n log n)', 0);
INSERT INTO @Tags (Name) VALUES (N'algoritmos');
INSERT INTO @Tags (Name) VALUES (N'complejidad');
INSERT INTO @Tags (Name) VALUES (N'busqueda');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T12-002',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 12,
    @Difficulty = 3,
    @Statement = N'¿Cuál es la complejidad temporal media de una búsqueda binaria sobre un vector ordenado de n elementos?',
    @Explanation = N'La búsqueda binaria descarta la mitad del espacio de búsqueda en cada comparación, por lo que su número de operaciones crece de forma logarítmica con el tamaño del vector: O(log n). Exige que el vector esté previamente ordenado.',
    @SourceReference = N'Algoritmos de búsqueda y análisis de complejidad',
    @SourcePublication = N'Temario TAI, Bloque II, tema 12',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T12-003 | Bloque II | Tema 12
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La tabla se ha llenado por completo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Dos claves distintas producen el mismo valor de la función hash', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La función hash devuelve un valor negativo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Se intenta insertar una clave nula', 0);
INSERT INTO @Tags (Name) VALUES (N'estructuras-de-datos');
INSERT INTO @Tags (Name) VALUES (N'hash');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T12-003',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 12,
    @Difficulty = 3,
    @Statement = N'En una tabla hash, una colisión se produce cuando:',
    @Explanation = N'Una colisión ocurre cuando dos claves diferentes se transforman en la misma posición de la tabla. Se resuelve mediante técnicas como el encadenamiento (listas enlazadas por cubeta) o el direccionamiento abierto (sondeo lineal, cuadrático o doble hash).',
    @SourceReference = N'Tablas de dispersión y tratamiento de colisiones',
    @SourcePublication = N'Temario TAI, Bloque II, tema 12',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T12-004 | Bloque II | Tema 12
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'O(n)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'O(n log n)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'O(n²)', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'O(2^n)', 0);
INSERT INTO @Tags (Name) VALUES (N'algoritmos');
INSERT INTO @Tags (Name) VALUES (N'ordenacion');
INSERT INTO @Tags (Name) VALUES (N'complejidad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T12-004',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 12,
    @Difficulty = 3,
    @Statement = N'El algoritmo de ordenación por burbuja (bubble sort) presenta, en el caso peor, una complejidad de:',
    @Explanation = N'La ordenación por burbuja compara y permuta elementos adyacentes recorriendo repetidamente el vector, lo que da lugar a un número de comparaciones del orden de n² en el caso peor y en el caso medio.',
    @SourceReference = N'Algoritmos de ordenación y análisis de complejidad',
    @SourcePublication = N'Temario TAI, Bloque II, tema 12',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T12-005 | Bloque II | Tema 12
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Requerir la lectura secuencial de todos los registros anteriores al buscado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Permitir acceder a un registro concreto sin recorrer los precedentes, calculando su posición', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Almacenar los registros siempre ordenados por clave primaria', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No admitir operaciones de escritura una vez creado', 0);
INSERT INTO @Tags (Name) VALUES (N'ficheros');
INSERT INTO @Tags (Name) VALUES (N'organizacion-de-ficheros');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T12-005',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 12,
    @Difficulty = 2,
    @Statement = N'En la organización de ficheros, un fichero de acceso directo o aleatorio se caracteriza por:',
    @Explanation = N'En la organización directa la posición del registro se obtiene a partir de su clave, normalmente mediante una función de transformación, lo que permite acceder a él sin recorrer los anteriores. En la organización secuencial sí es necesario ese recorrido.',
    @SourceReference = N'Organización de ficheros: secuencial, directa e indexada',
    @SourcePublication = N'Temario TAI, Bloque II, tema 12',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T13-001 | Bloque II | Tema 13
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Exclusión mutua', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Retención y espera', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Expropiación de los recursos asignados', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Espera circular', 0);
INSERT INTO @Tags (Name) VALUES (N'sistemas-operativos');
INSERT INTO @Tags (Name) VALUES (N'procesos');
INSERT INTO @Tags (Name) VALUES (N'interbloqueo');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T13-001',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 13,
    @Difficulty = 2,
    @Statement = N'En la gestión de procesos de un sistema operativo, un interbloqueo (deadlock) requiere que se cumplan simultáneamente cuatro condiciones. ¿Cuál de las siguientes NO es una de ellas?',
    @Explanation = N'Las condiciones de Coffman para el interbloqueo son exclusión mutua, retención y espera, NO expropiación y espera circular. Precisamente la ausencia de expropiación es una de las condiciones necesarias; si los recursos pudieran expropiarse, el interbloqueo no se produciría.',
    @SourceReference = N'Condiciones de Coffman para el interbloqueo. Gestión de procesos',
    @SourcePublication = N'Temario TAI, Bloque II, tema 13',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T13-002 | Bloque II | Tema 13
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Ejecutar procesos cuyo espacio de direcciones sea mayor que la memoria física disponible', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Eliminar por completo el acceso a disco durante la ejecución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Garantizar que ningún proceso genere fallos de página', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Asignar a cada proceso exactamente la misma cantidad de memoria física', 0);
INSERT INTO @Tags (Name) VALUES (N'sistemas-operativos');
INSERT INTO @Tags (Name) VALUES (N'memoria-virtual');
INSERT INTO @Tags (Name) VALUES (N'paginacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T13-002',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 13,
    @Difficulty = 3,
    @Statement = N'La memoria virtual con paginación permite:',
    @Explanation = N'La memoria virtual divide el espacio de direcciones en páginas y mantiene en memoria física solo las necesarias, trasladando el resto a un dispositivo de intercambio. Eso permite ejecutar procesos mayores que la memoria física, a costa de los fallos de página.',
    @SourceReference = N'Gestión de memoria: paginación y memoria virtual',
    @SourcePublication = N'Temario TAI, Bloque II, tema 13',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T13-003 | Bloque II | Tema 13
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Ejecutar siempre primero el proceso con menor tiempo estimado de ejecución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Asignar a cada proceso un quantum de tiempo de CPU, expropiándolo al agotarse', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'No permitir la expropiación una vez asignada la CPU', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ordenar los procesos exclusivamente por su prioridad estática', 0);
INSERT INTO @Tags (Name) VALUES (N'sistemas-operativos');
INSERT INTO @Tags (Name) VALUES (N'planificacion');
INSERT INTO @Tags (Name) VALUES (N'round-robin');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T13-003',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 13,
    @Difficulty = 3,
    @Statement = N'El algoritmo de planificación de procesos Round Robin se caracteriza por:',
    @Explanation = N'Round Robin es un algoritmo expropiativo que reparte la CPU en rodajas de tiempo (quantum). Cuando un proceso agota su quantum se le retira la CPU y pasa al final de la cola de preparados, lo que garantiza un reparto equitativo.',
    @SourceReference = N'Algoritmos de planificación de procesos',
    @SourcePublication = N'Temario TAI, Bloque II, tema 13',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T13-004 | Bloque II | Tema 13
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La tabla de páginas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El bloque de control de proceso (PCB)', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El descriptor de fichero', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La tabla de interrupciones', 0);
INSERT INTO @Tags (Name) VALUES (N'sistemas-operativos');
INSERT INTO @Tags (Name) VALUES (N'procesos');
INSERT INTO @Tags (Name) VALUES (N'pcb');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T13-004',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 13,
    @Difficulty = 2,
    @Statement = N'¿Qué estructura de datos del núcleo mantiene la información de control de un proceso, como su identificador, su estado y el contenido de sus registros?',
    @Explanation = N'El bloque de control de proceso o PCB (Process Control Block) almacena toda la información que el sistema operativo necesita para gestionar un proceso: identificador, estado, contador de programa, registros, información de planificación, memoria y ficheros abiertos.',
    @SourceReference = N'Gestión de procesos: el bloque de control de proceso',
    @SourcePublication = N'Temario TAI, Bloque II, tema 13',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T14-001 | Bloque II | Tema 14
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Que ningún atributo de la clave primaria puede tomar valor nulo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Que toda clave ajena debe apuntar a una tupla existente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Que no puede haber tablas sin filas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que todos los atributos deben ser atómicos', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'modelo-relacional');
INSERT INTO @Tags (Name) VALUES (N'integridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T14-001',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 14,
    @Difficulty = 2,
    @Statement = N'En el modelo relacional, ¿qué garantiza la regla de integridad de entidad?',
    @Explanation = N'La integridad de entidad exige que ningún atributo que forme parte de la clave primaria de una relación pueda tomar valor nulo, ya que la clave primaria debe identificar unívocamente cada tupla. La regla que obliga a que las claves ajenas apunten a tuplas existentes es la integridad referencial.',
    @SourceReference = N'Modelo relacional de Codd. Reglas de integridad',
    @SourcePublication = N'Temario TAI, Bloque II, tema 14',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T14-002 | Bloque II | Tema 14
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Atomicidad, Consistencia, Aislamiento y Durabilidad', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Acceso, Control, Integridad y Disponibilidad', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Atomicidad, Concurrencia, Indexación y Distribución', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Auditoría, Consistencia, Integridad y Dependencia', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'transacciones');
INSERT INTO @Tags (Name) VALUES (N'acid');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T14-002',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 14,
    @Difficulty = 2,
    @Statement = N'El acrónimo ACID, aplicado a las transacciones de un sistema gestor de bases de datos, corresponde a:',
    @Explanation = N'ACID son las propiedades que garantizan la fiabilidad de las transacciones: Atomicidad (todo o nada), Consistencia (la base pasa de un estado válido a otro), Aislamiento (las transacciones concurrentes no interfieren) y Durabilidad (los cambios confirmados persisten).',
    @SourceReference = N'Propiedades ACID de las transacciones',
    @SourcePublication = N'Temario TAI, Bloque II, tema 14',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T14-003 | Bloque II | Tema 14
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Clave-valor', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Documental', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Columnar', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Jerárquica relacional normalizada', 1);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'nosql');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T14-003',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 14,
    @Difficulty = 3,
    @Statement = N'¿Cuál de las siguientes NO es una categoría habitual de base de datos NoSQL?',
    @Explanation = N'Las familias habituales de bases de datos NoSQL son clave-valor, documentales, de familias de columnas y de grafos. El modelo jerárquico es anterior al relacional y no forma parte de la clasificación NoSQL.',
    @SourceReference = N'Bases de datos no relacionales. Clasificación de sistemas NoSQL',
    @SourcePublication = N'Temario TAI, Bloque II, tema 14',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T14-004 | Bloque II | Tema 14
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Grado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Cardinalidad', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Dominio', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Aridad', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'modelo-relacional');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T14-004',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 14,
    @Difficulty = 3,
    @Statement = N'En terminología del modelo relacional, el número de tuplas de una relación se denomina:',
    @Explanation = N'La cardinalidad de una relación es el número de tuplas (filas) que contiene, mientras que el grado o aridad es el número de atributos (columnas). El dominio es el conjunto de valores admisibles de un atributo.',
    @SourceReference = N'Modelo relacional: relaciones, tuplas, atributos y dominios',
    @SourcePublication = N'Temario TAI, Bloque II, tema 14',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B2-T14-005 | Bloque II | Tema 14
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El nivel externo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El nivel conceptual', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El nivel interno', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El nivel lógico de usuario', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'arquitectura');
INSERT INTO @Tags (Name) VALUES (N'ansi-sparc');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B2-T14-005',
    @ExamCode = N'TAI',
    @BlockCode = N'II',
    @TopicNumber = 14,
    @Difficulty = 2,
    @Statement = N'En la arquitectura ANSI/SPARC de tres niveles de un sistema gestor de bases de datos, el nivel que describe cómo se almacenan físicamente los datos es:',
    @Explanation = N'La arquitectura ANSI/SPARC distingue el nivel externo (las vistas de cada usuario), el nivel conceptual (la descripción global y lógica de los datos) y el nivel interno o físico (cómo se almacenan realmente).',
    @SourceReference = N'Arquitectura ANSI/SPARC de tres niveles',
    @SourcePublication = N'Temario TAI, Bloque II, tema 14',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T15-001 | Bloque III | Tema 15
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'No existen grupos repetitivos ni atributos multivaluados', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Todos los atributos no clave dependen funcionalmente de forma completa de la clave primaria', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'No existe ninguna dependencia funcional transitiva entre atributos no clave', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Todo determinante es una clave candidata', 0);
INSERT INTO @Tags (Name) VALUES (N'modelado-de-datos');
INSERT INTO @Tags (Name) VALUES (N'normalizacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T15-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 15,
    @Difficulty = 2,
    @Statement = N'Una relación se encuentra en Segunda Forma Normal (2FN) cuando está en 1FN y además:',
    @Explanation = N'La 2FN exige que la relación esté en 1FN y que todo atributo no perteneciente a la clave dependa funcionalmente de forma completa de la clave primaria, es decir, que no existan dependencias parciales sobre parte de una clave compuesta. La eliminación de dependencias transitivas corresponde a la 3FN y la condición sobre determinantes a la forma normal de Boyce-Codd.',
    @SourceReference = N'Normalización de bases de datos relacionales. Formas normales',
    @SourcePublication = N'Temario TAI, Bloque III, tema 15',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T15-002 | Bloque III | Tema 15
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Añadiendo una clave ajena en cualquiera de las dos tablas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Creando una nueva tabla cuya clave primaria está formada por las claves ajenas de ambas entidades', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Fusionando ambas entidades en una única tabla', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Duplicando los atributos de una entidad dentro de la otra', 0);
INSERT INTO @Tags (Name) VALUES (N'modelado-de-datos');
INSERT INTO @Tags (Name) VALUES (N'entidad-relacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T15-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 15,
    @Difficulty = 3,
    @Statement = N'En el modelo entidad-relación, ¿cómo se transforma habitualmente al modelo relacional una relación N:M entre dos entidades?',
    @Explanation = N'Una interrelación de cardinalidad N:M no puede representarse mediante una simple clave ajena. Se transforma en una tabla intermedia cuya clave primaria es la combinación de las claves ajenas que referencian a ambas entidades, y donde se ubican además los atributos propios de la interrelación.',
    @SourceReference = N'Transformación del modelo entidad-relación al modelo relacional',
    @SourcePublication = N'Temario TAI, Bloque III, tema 15',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T15-003 | Bloque III | Tema 15
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Ningún atributo no clave depende transitivamente de la clave primaria', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Todos sus atributos son atómicos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'No admite valores nulos en ninguna columna', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Tiene una única clave candidata', 0);
INSERT INTO @Tags (Name) VALUES (N'modelado-de-datos');
INSERT INTO @Tags (Name) VALUES (N'normalizacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T15-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 15,
    @Difficulty = 3,
    @Statement = N'Una relación está en Tercera Forma Normal (3FN) si está en 2FN y:',
    @Explanation = N'La 3FN elimina las dependencias funcionales transitivas: ningún atributo no perteneciente a la clave puede depender de otro atributo que tampoco forme parte de ella.',
    @SourceReference = N'Normalización de bases de datos relacionales. Tercera forma normal',
    @SourcePublication = N'Temario TAI, Bloque III, tema 15',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T16-001 | Bloque III | Tema 16
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La función recibe una copia del argumento y las modificaciones no afectan al original', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La función recibe la dirección de memoria del argumento y puede modificarlo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El argumento solo se evalúa si se utiliza dentro de la función', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El argumento se convierte automáticamente en una constante global', 0);
INSERT INTO @Tags (Name) VALUES (N'programacion');
INSERT INTO @Tags (Name) VALUES (N'parametros');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T16-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 16,
    @Difficulty = 2,
    @Statement = N'En el paso de parámetros por valor:',
    @Explanation = N'En el paso por valor se copia el valor del argumento en el parámetro formal, de modo que cualquier modificación dentro del subprograma se realiza sobre la copia y no trasciende al ámbito llamador. El paso por referencia sí permite modificar el original.',
    @SourceReference = N'Procedimientos, funciones y paso de parámetros',
    @SourcePublication = N'Temario TAI, Bloque III, tema 16',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T16-002 | Bloque III | Tema 16
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Al menos dos llamadas recursivas por rama', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un caso base que detenga la recursión y una reducción del problema en cada llamada', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Una variable global que cuente las llamadas realizadas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un bucle que sustituya a la última llamada', 0);
INSERT INTO @Tags (Name) VALUES (N'programacion');
INSERT INTO @Tags (Name) VALUES (N'recursividad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T16-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 16,
    @Difficulty = 3,
    @Statement = N'Toda función recursiva correctamente construida debe disponer necesariamente de:',
    @Explanation = N'Sin caso base la recursión no termina y agota la pila de llamadas. Además, cada llamada recursiva debe aproximarse al caso base reduciendo el tamaño del problema; en otro caso tampoco se alcanzaría la condición de parada.',
    @SourceReference = N'Recursividad. Diseño de algoritmos recursivos',
    @SourcePublication = N'Temario TAI, Bloque III, tema 16',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T16-003 | Bloque III | Tema 16
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El compilado traduce el programa completo a código objeto antes de su ejecución, mientras que el interpretado traduce y ejecuta instrucción a instrucción', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El interpretado siempre genera un fichero ejecutable independiente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El compilado no admite comprobación de tipos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El interpretado no puede acceder al sistema de ficheros', 0);
INSERT INTO @Tags (Name) VALUES (N'programacion');
INSERT INTO @Tags (Name) VALUES (N'compilacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T16-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 16,
    @Difficulty = 2,
    @Statement = N'La diferencia esencial entre un lenguaje compilado y uno interpretado es que:',
    @Explanation = N'El compilador realiza una traducción previa y completa del código fuente a código objeto o máquina, que se ejecuta después. El intérprete analiza y ejecuta el programa sentencia a sentencia en tiempo de ejecución, sin generar necesariamente un ejecutable.',
    @SourceReference = N'Traductores: compiladores e intérpretes',
    @SourcePublication = N'Temario TAI, Bloque III, tema 16',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T17-001 | Bloque III | Tema 17
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'DDL (lenguaje de definición de datos)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'DML (lenguaje de manipulación de datos)', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'DCL (lenguaje de control de datos)', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'TCL (lenguaje de control de transacciones)', 0);
INSERT INTO @Tags (Name) VALUES (N'sql');
INSERT INTO @Tags (Name) VALUES (N'dcl');
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T17-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 17,
    @Difficulty = 2,
    @Statement = N'En SQL, ¿a qué sublenguaje pertenece la sentencia GRANT?',
    @Explanation = N'GRANT y REVOKE forman parte del DCL, que gobierna los permisos y privilegios sobre los objetos de la base de datos. CREATE, ALTER y DROP son DDL; SELECT, INSERT, UPDATE y DELETE son DML; COMMIT y ROLLBACK son control de transacciones.',
    @SourceReference = N'ISO/IEC 9075, Information technology - Database languages - SQL',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T17-002 | Bloque III | Tema 17
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Solo las filas de A que tienen correspondencia en B', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Todas las filas de A, con valores nulos en las columnas de B cuando no hay correspondencia', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Todas las filas de B, con nulos en las columnas de A cuando no hay correspondencia', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El producto cartesiano de A y B', 0);
INSERT INTO @Tags (Name) VALUES (N'sql');
INSERT INTO @Tags (Name) VALUES (N'joins');
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T17-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 17,
    @Difficulty = 3,
    @Statement = N'Un LEFT OUTER JOIN entre las tablas A y B devuelve:',
    @Explanation = N'El LEFT OUTER JOIN conserva todas las filas de la tabla situada a la izquierda de la cláusula, completando con valores nulos las columnas de la tabla derecha cuando no existe fila coincidente. El INNER JOIN devolvería solo las coincidencias.',
    @SourceReference = N'ISO/IEC 9075, operaciones de combinación en SQL',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T17-003 | Bloque III | Tema 17
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'HAVING filtra filas antes de agrupar y WHERE después', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'HAVING filtra los grupos resultantes tras aplicar GROUP BY, permitiendo usar funciones de agregación', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'HAVING solo puede emplearse con la función COUNT', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No existe diferencia funcional entre ambas', 0);
INSERT INTO @Tags (Name) VALUES (N'sql');
INSERT INTO @Tags (Name) VALUES (N'agregacion');
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T17-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 17,
    @Difficulty = 3,
    @Statement = N'En una consulta SQL con agregación, la cláusula HAVING se diferencia de WHERE en que:',
    @Explanation = N'WHERE se aplica a las filas antes de la agrupación, mientras que HAVING actúa sobre los grupos ya formados por GROUP BY y admite condiciones sobre funciones de agregación como SUM, AVG o COUNT.',
    @SourceReference = N'ISO/IEC 9075, cláusulas GROUP BY y HAVING',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T17-004 | Bloque III | Tema 17
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un procedimiento almacenado que el usuario invoca explícitamente por su nombre', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un bloque de código que el gestor ejecuta automáticamente ante un evento sobre una tabla, como INSERT, UPDATE o DELETE', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Una restricción declarativa equivalente a una clave ajena', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Un índice que se reconstruye periódicamente', 0);
INSERT INTO @Tags (Name) VALUES (N'sql');
INSERT INTO @Tags (Name) VALUES (N'triggers');
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T17-004',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 17,
    @Difficulty = 3,
    @Statement = N'Un disparador o trigger de base de datos es:',
    @Explanation = N'El disparador se asocia a una tabla y a uno o varios eventos de manipulación de datos, y el propio gestor lo ejecuta automáticamente cuando se producen, antes o después de la operación. A diferencia de un procedimiento almacenado, no se invoca explícitamente.',
    @SourceReference = N'ISO/IEC 9075, disparadores y procedimientos almacenados',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T17-005 | Bloque III | Tema 17
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'DELETE elimina la estructura de la tabla y TRUNCATE solo sus filas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'DELETE es una sentencia DML que admite cláusula WHERE y registro individual de filas; TRUNCATE es DDL y elimina todas las filas sin condición', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'TRUNCATE permite filtrar filas mediante WHERE y DELETE no', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ambas son idénticas y solo cambia el nombre según el gestor', 0);
INSERT INTO @Tags (Name) VALUES (N'sql');
INSERT INTO @Tags (Name) VALUES (N'dml');
INSERT INTO @Tags (Name) VALUES (N'ddl');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T17-005',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 17,
    @Difficulty = 2,
    @Statement = N'¿Qué diferencia fundamental existe entre las sentencias DELETE y TRUNCATE?',
    @Explanation = N'DELETE pertenece al DML, permite filtrar con WHERE y registra el borrado fila a fila. TRUNCATE se considera DDL, vacía la tabla completa sin condición y suele ser mucho más rápido por no registrar cada fila individualmente.',
    @SourceReference = N'ISO/IEC 9075, sentencias de manipulación y definición de datos',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T18-001 | Bloque III | Tema 18
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Permitir que una clase herede el comportamiento de otra', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Ocultar el estado interno de un objeto y exponerlo únicamente a través de una interfaz controlada', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Definir varios métodos con el mismo nombre y distinta signatura', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Sustituir en tiempo de ejecución la implementación de un método heredado', 0);
INSERT INTO @Tags (Name) VALUES (N'poo');
INSERT INTO @Tags (Name) VALUES (N'encapsulamiento');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T18-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 18,
    @Difficulty = 2,
    @Statement = N'En programación orientada a objetos, el encapsulamiento consiste en:',
    @Explanation = N'El encapsulamiento agrupa datos y comportamiento y restringe el acceso directo al estado interno, que solo se manipula mediante los métodos públicos que la clase decide exponer. La herencia, la sobrecarga y el polimorfismo son conceptos distintos.',
    @SourceReference = N'Principios de la programación orientada a objetos',
    @SourcePublication = N'Temario TAI, Bloque III, tema 18',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T18-002 | Bloque III | Tema 18
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La sobrecarga define varios métodos con el mismo nombre y distintos parámetros en la misma clase; la sobrescritura redefine en una subclase un método heredado con la misma signatura', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La sobrecarga solo es posible entre clases relacionadas por herencia', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La sobrescritura exige cambiar el nombre del método en la subclase', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ambos términos designan exactamente el mismo mecanismo', 0);
INSERT INTO @Tags (Name) VALUES (N'poo');
INSERT INTO @Tags (Name) VALUES (N'polimorfismo');
INSERT INTO @Tags (Name) VALUES (N'herencia');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T18-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 18,
    @Difficulty = 3,
    @Statement = N'¿Cuál es la diferencia entre sobrecarga (overloading) y sobrescritura (overriding) de métodos?',
    @Explanation = N'La sobrecarga se resuelve normalmente en tiempo de compilación en función de la lista de parámetros. La sobrescritura sustituye la implementación de un método heredado manteniendo su signatura y se resuelve en tiempo de ejecución, siendo la base del polimorfismo dinámico.',
    @SourceReference = N'Herencia, sobrecarga y polimorfismo en la orientación a objetos',
    @SourcePublication = N'Temario TAI, Bloque III, tema 18',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T18-003 | Bloque III | Tema 18
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La clase abstracta no puede contener ningún método implementado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La clase abstracta puede aportar estado y una implementación parcial, mientras que la interfaz define fundamentalmente un contrato de comportamiento', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La interfaz puede instanciarse directamente y la clase abstracta no', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Una clase puede heredar de varias clases abstractas en todos los lenguajes', 0);
INSERT INTO @Tags (Name) VALUES (N'poo');
INSERT INTO @Tags (Name) VALUES (N'abstraccion');
INSERT INTO @Tags (Name) VALUES (N'interfaces');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T18-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 18,
    @Difficulty = 3,
    @Statement = N'Una clase abstracta se diferencia de una interfaz en que:',
    @Explanation = N'La clase abstracta puede declarar campos y métodos ya implementados, sirviendo de base parcial para sus subclases, mientras que la interfaz establece un contrato que las clases se comprometen a cumplir. Ninguna de las dos es instanciable directamente.',
    @SourceReference = N'Clases abstractas e interfaces en la orientación a objetos',
    @SourcePublication = N'Temario TAI, Bloque III, tema 18',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T19-001 | Bloque III | Tema 19
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Definir una interfaz para crear objetos delegando en las subclases la clase concreta a instanciar', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Garantizar que una clase tenga una única instancia y proporcionar un punto de acceso global a ella', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Convertir la interfaz de una clase en otra que los clientes esperan', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Notificar automáticamente a varios objetos los cambios de estado de otro', 0);
INSERT INTO @Tags (Name) VALUES (N'patrones-de-diseno');
INSERT INTO @Tags (Name) VALUES (N'singleton');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T19-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 19,
    @Difficulty = 3,
    @Statement = N'El patrón de diseño Singleton tiene como propósito:',
    @Explanation = N'Singleton es un patrón creacional que restringe la instanciación de una clase a un único objeto y ofrece un punto de acceso global. Las otras descripciones corresponden a Factory Method, Adapter y Observer respectivamente.',
    @SourceReference = N'Patrones de diseño. Catálogo de patrones creacionales, estructurales y de comportamiento',
    @SourcePublication = N'Temario TAI, Bloque III, tema 19',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T19-002 | Bloque III | Tema 19
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El diagrama de clases', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El diagrama de secuencia', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El diagrama de despliegue', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El diagrama de casos de uso', 0);
INSERT INTO @Tags (Name) VALUES (N'uml');
INSERT INTO @Tags (Name) VALUES (N'modelado');
INSERT INTO @Tags (Name) VALUES (N'diagramas');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T19-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 19,
    @Difficulty = 3,
    @Statement = N'En UML, ¿qué diagrama muestra la interacción entre objetos ordenada temporalmente a lo largo de líneas de vida?',
    @Explanation = N'El diagrama de secuencia es un diagrama de interacción que representa el intercambio de mensajes entre objetos a lo largo del tiempo, mediante líneas de vida verticales. El de clases es estructural, el de despliegue describe nodos físicos y el de casos de uso el comportamiento desde la perspectiva del actor.',
    @SourceReference = N'OMG Unified Modeling Language (UML), diagramas de interacción',
    @SourcePublication = N'Object Management Group',
    @SourceUrl = N'https://www.omg.org/spec/UML/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T19-003 | Bloque III | Tema 19
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Reducir el número de clases de una aplicación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Separar la lógica de negocio, la presentación y la gestión de la interacción del usuario', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Sustituir el uso de bases de datos relacionales', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Garantizar la ejecución concurrente de las peticiones', 0);
INSERT INTO @Tags (Name) VALUES (N'patrones-de-diseno');
INSERT INTO @Tags (Name) VALUES (N'mvc');
INSERT INTO @Tags (Name) VALUES (N'arquitectura');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T19-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 19,
    @Difficulty = 3,
    @Statement = N'El patrón Modelo-Vista-Controlador (MVC) persigue principalmente:',
    @Explanation = N'MVC divide la aplicación en tres responsabilidades: el modelo mantiene los datos y las reglas de negocio, la vista presenta la información y el controlador gestiona la entrada del usuario y coordina a ambos. El objetivo es la separación de responsabilidades y la mantenibilidad.',
    @SourceReference = N'Patrones arquitectónicos. Modelo-Vista-Controlador',
    @SourcePublication = N'Temario TAI, Bloque III, tema 19',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T19-004 | Bloque III | Tema 19
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'JPA', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'JSF', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'JMS', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'JAX-WS', 0);
INSERT INTO @Tags (Name) VALUES (N'jakarta-ee');
INSERT INTO @Tags (Name) VALUES (N'persistencia');
INSERT INTO @Tags (Name) VALUES (N'jpa');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T19-004',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 19,
    @Difficulty = 3,
    @Statement = N'En la plataforma Jakarta EE, la especificación encargada de la persistencia de objetos en bases de datos relacionales es:',
    @Explanation = N'JPA (Jakarta Persistence API) define el mapeo objeto-relacional y la gestión de entidades. JSF corresponde a la capa de presentación, JMS a la mensajería asíncrona y JAX-WS a los servicios web basados en SOAP.',
    @SourceReference = N'Especificaciones de la plataforma Jakarta EE',
    @SourcePublication = N'Eclipse Foundation',
    @SourceUrl = N'https://jakarta.ee/specifications/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T20-001 | Bloque III | Tema 20
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'POST', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'PUT', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'PATCH', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'CONNECT', 0);
INSERT INTO @Tags (Name) VALUES (N'rest');
INSERT INTO @Tags (Name) VALUES (N'http');
INSERT INTO @Tags (Name) VALUES (N'servicios-web');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T20-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 20,
    @Difficulty = 2,
    @Statement = N'En una arquitectura REST, ¿qué método HTTP se considera idempotente y se emplea para sustituir por completo un recurso?',
    @Explanation = N'PUT sustituye la representación completa del recurso identificado por la URI y es idempotente: repetir la misma petición produce el mismo estado final. POST no es idempotente y PATCH aplica modificaciones parciales sin garantía general de idempotencia.',
    @SourceReference = N'RFC 9110, HTTP Semantics, sección de métodos',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc9110',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T20-002 | Bloque III | Tema 20
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'XML define un conjunto cerrado de etiquetas predefinidas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'XML es un metalenguaje extensible que permite definir lenguajes de marcado propios, y exige documentos bien formados', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'XML no distingue entre mayúsculas y minúsculas en los nombres de etiqueta', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'XML solo puede utilizarse para describir páginas web', 0);
INSERT INTO @Tags (Name) VALUES (N'xml');
INSERT INTO @Tags (Name) VALUES (N'web');
INSERT INTO @Tags (Name) VALUES (N'marcado');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T20-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 20,
    @Difficulty = 2,
    @Statement = N'¿Cuál de las siguientes afirmaciones sobre XML es correcta?',
    @Explanation = N'XML es un metalenguaje que permite definir vocabularios de marcado propios. Todo documento debe estar bien formado (etiquetas correctamente anidadas y cerradas, un único elemento raíz) y es sensible a mayúsculas y minúsculas.',
    @SourceReference = N'Extensible Markup Language (XML) 1.0',
    @SourcePublication = N'World Wide Web Consortium (W3C)',
    @SourceUrl = N'https://www.w3.org/TR/xml/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T20-003 | Bloque III | Tema 20
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cifrar las comunicaciones entre navegador y servidor', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Permitir de forma controlada que una página solicite recursos a un origen distinto del suyo, relajando la política del mismo origen', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Comprimir las respuestas HTTP para reducir el ancho de banda', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Autenticar al usuario mediante certificados de cliente', 0);
INSERT INTO @Tags (Name) VALUES (N'web');
INSERT INTO @Tags (Name) VALUES (N'cors');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T20-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 20,
    @Difficulty = 3,
    @Statement = N'El mecanismo CORS (Cross-Origin Resource Sharing) tiene por objeto:',
    @Explanation = N'La política del mismo origen impide por defecto que un documento acceda a recursos de otro origen. CORS define un conjunto de cabeceras HTTP mediante las cuales el servidor autoriza explícitamente esos accesos cruzados.',
    @SourceReference = N'Fetch Standard, sección CORS protocol',
    @SourcePublication = N'WHATWG',
    @SourceUrl = N'https://fetch.spec.whatwg.org/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T20-004 | Bloque III | Tema 20
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'400', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'401', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'404', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'500', 0);
INSERT INTO @Tags (Name) VALUES (N'http');
INSERT INTO @Tags (Name) VALUES (N'web');
INSERT INTO @Tags (Name) VALUES (N'codigos-de-estado');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T20-004',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 20,
    @Difficulty = 2,
    @Statement = N'¿Qué código de estado HTTP indica que el recurso solicitado no existe en el servidor?',
    @Explanation = N'El código 404 Not Found indica que el servidor no ha encontrado una representación actual para el recurso solicitado. El 400 señala una petición mal formada, el 401 falta de autenticación y el 500 un error interno del servidor.',
    @SourceReference = N'RFC 9110, HTTP Semantics, códigos de estado',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc9110',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T20-005 | Bloque III | Tema 20
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La detección del sistema operativo del servidor', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Rejillas flexibles, imágenes fluidas y consultas de medios (media queries) de CSS', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El uso exclusivo de aplicaciones nativas por dispositivo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La compresión de las hojas de estilo', 0);
INSERT INTO @Tags (Name) VALUES (N'web');
INSERT INTO @Tags (Name) VALUES (N'css');
INSERT INTO @Tags (Name) VALUES (N'responsive');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T20-005',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 20,
    @Difficulty = 3,
    @Statement = N'El diseño web adaptativo o responsive se apoya principalmente en:',
    @Explanation = N'El diseño responsive combina maquetación flexible, imágenes que se adaptan al contenedor y media queries de CSS, que aplican reglas distintas según características del dispositivo como la anchura del viewport.',
    @SourceReference = N'CSS Media Queries',
    @SourcePublication = N'World Wide Web Consortium (W3C)',
    @SourceUrl = N'https://www.w3.org/TR/mediaqueries-4/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T21-001 | Bloque III | Tema 21
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Nivel A', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Nivel AA', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Nivel AAA', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No fija ningún nivel concreto', 0);
INSERT INTO @Tags (Name) VALUES (N'accesibilidad');
INSERT INTO @Tags (Name) VALUES (N'wcag');
INSERT INTO @Tags (Name) VALUES (N'rd-1112-2018');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T21-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 21,
    @Difficulty = 3,
    @Statement = N'El Real Decreto 1112/2018 exige que los sitios web y aplicaciones móviles del sector público cumplan un nivel de conformidad con las pautas de accesibilidad de:',
    @Explanation = N'El Real Decreto 1112/2018, que traspone la Directiva (UE) 2016/2102, remite al estándar europeo EN 301 549 y exige el cumplimiento del nivel AA de las Pautas de Accesibilidad para el Contenido Web (WCAG).',
    @SourceReference = N'Real Decreto 1112/2018, de 7 de septiembre, sobre accesibilidad de los sitios web y aplicaciones para dispositivos móviles del sector público',
    @SourcePublication = N'BOE núm. 227, de 19 de septiembre de 2018',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2018-12699',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T21-002 | Bloque III | Tema 21
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Perceptible, operable, comprensible y robusto', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Accesible, usable, adaptable y seguro', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Visible, navegable, legible y compatible', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Universal, flexible, tolerante y equitativo', 0);
INSERT INTO @Tags (Name) VALUES (N'accesibilidad');
INSERT INTO @Tags (Name) VALUES (N'wcag');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T21-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 21,
    @Difficulty = 3,
    @Statement = N'¿Cuáles son los cuatro principios en los que se organizan las Pautas de Accesibilidad para el Contenido Web (WCAG)?',
    @Explanation = N'Las WCAG se estructuran en cuatro principios: perceptible, operable, comprensible y robusto, de los que cuelgan las pautas y los criterios de conformidad de niveles A, AA y AAA.',
    @SourceReference = N'Web Content Accessibility Guidelines (WCAG)',
    @SourcePublication = N'World Wide Web Consortium (W3C)',
    @SourceUrl = N'https://www.w3.org/TR/WCAG22/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T21-003 | Bloque III | Tema 21
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El cifrado del canal de comunicaciones con TLS', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El uso de consultas parametrizadas junto con la validación de la entrada', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La ofuscación del código fuente de la aplicación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La restricción del tamaño máximo del formulario', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'owasp');
INSERT INTO @Tags (Name) VALUES (N'inyeccion-sql');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T21-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 21,
    @Difficulty = 3,
    @Statement = N'En materia de desarrollo seguro, la inyección SQL se previene fundamentalmente mediante:',
    @Explanation = N'La inyección SQL se produce al concatenar entrada no confiable dentro de una sentencia. La defensa principal son las consultas parametrizadas o sentencias preparadas, que separan el código de los datos, complementadas con validación de entrada y el principio de mínimo privilegio.',
    @SourceReference = N'OWASP Top 10, categoría de inyección',
    @SourcePublication = N'Open Worldwide Application Security Project',
    @SourceUrl = N'https://owasp.org/www-project-top-ten/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T21-004 | Bloque III | Tema 21
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Inyectar código script malicioso que se ejecuta en el navegador de otros usuarios', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Interceptar el tráfico entre cliente y servidor para modificarlo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Agotar los recursos del servidor mediante peticiones masivas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Suplantar la dirección IP de origen de las peticiones', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'owasp');
INSERT INTO @Tags (Name) VALUES (N'xss');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T21-004',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 21,
    @Difficulty = 3,
    @Statement = N'Un ataque de Cross-Site Scripting (XSS) consiste en:',
    @Explanation = N'En un XSS el atacante consigue que la aplicación devuelva contenido no saneado que el navegador de la víctima interpreta como script. La defensa pasa por codificar la salida según el contexto y validar la entrada, además de aplicar políticas de seguridad de contenido.',
    @SourceReference = N'OWASP Top 10, Cross-Site Scripting',
    @SourcePublication = N'Open Worldwide Application Security Project',
    @SourceUrl = N'https://owasp.org/www-project-top-ten/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T22-001 | Bloque III | Tema 22
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Sprint Backlog', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Product Backlog', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Incremento', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Definición de Terminado', 0);
INSERT INTO @Tags (Name) VALUES (N'metodologias-agiles');
INSERT INTO @Tags (Name) VALUES (N'scrum');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T22-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 22,
    @Difficulty = 2,
    @Statement = N'En el marco Scrum, el artefacto que contiene la lista ordenada de todo lo que se conoce que es necesario para el producto se denomina:',
    @Explanation = N'El Product Backlog es la lista emergente y ordenada de todo lo necesario para mejorar el producto, y es la única fuente de trabajo del equipo Scrum. El Sprint Backlog es el subconjunto seleccionado para un Sprint junto con el plan para entregarlo.',
    @SourceReference = N'La Guía de Scrum. Artefactos de Scrum',
    @SourcePublication = N'Scrum.org',
    @SourceUrl = N'https://scrumguides.org/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T22-002 | Bloque III | Tema 22
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Iterar sobre incrementos funcionales entregables cada pocas semanas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Ejecutar las fases de forma secuencial, iniciando cada una al finalizar la anterior', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Prescindir de la fase de documentación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Basarse en la construcción de prototipos desechables', 0);
INSERT INTO @Tags (Name) VALUES (N'ciclo-de-vida');
INSERT INTO @Tags (Name) VALUES (N'metodologias');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T22-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 22,
    @Difficulty = 3,
    @Statement = N'El modelo en cascada del ciclo de vida del software se caracteriza por:',
    @Explanation = N'El modelo en cascada organiza el desarrollo en fases secuenciales (análisis, diseño, codificación, pruebas y mantenimiento), donde cada fase comienza cuando la anterior ha concluido. Su rigidez frente al cambio de requisitos es su principal limitación.',
    @SourceReference = N'Modelos de ciclo de vida del software',
    @SourcePublication = N'Temario TAI, Bloque III, tema 22',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T22-003 | Bloque III | Tema 22
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Diseñarse a partir de la estructura interna del código, buscando cobertura de sentencias y caminos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Basarse exclusivamente en la especificación funcional, sin conocer la implementación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Ejecutarse siempre en el entorno de producción', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ser realizadas únicamente por el usuario final', 0);
INSERT INTO @Tags (Name) VALUES (N'pruebas');
INSERT INTO @Tags (Name) VALUES (N'calidad');
INSERT INTO @Tags (Name) VALUES (N'ciclo-de-vida');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T22-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 22,
    @Difficulty = 3,
    @Statement = N'Las pruebas de caja blanca se caracterizan por:',
    @Explanation = N'Las pruebas de caja blanca o estructurales se diseñan conociendo el código y persiguen criterios de cobertura como sentencias, decisiones o caminos. Las de caja negra parten únicamente de la especificación de entradas y salidas esperadas.',
    @SourceReference = N'Pruebas del software: técnicas estructurales y funcionales',
    @SourcePublication = N'Temario TAI, Bloque III, tema 22',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T23-001 | Bloque III | Tema 23
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Centralizado, donde el histórico reside únicamente en el servidor', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Distribuido, donde cada copia de trabajo contiene el repositorio completo con su histórico', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'De bloqueo obligatorio de ficheros antes de editarlos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que solo admite un único usuario por repositorio', 0);
INSERT INTO @Tags (Name) VALUES (N'control-de-versiones');
INSERT INTO @Tags (Name) VALUES (N'git');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T23-001',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 23,
    @Difficulty = 2,
    @Statement = N'Git es un sistema de control de versiones:',
    @Explanation = N'Git es distribuido: al clonar un repositorio se obtiene todo el histórico localmente, lo que permite trabajar y consultar el historial sin conexión y facilita los flujos con ramas. Subversion o CVS responden al modelo centralizado.',
    @SourceReference = N'Git, sistema de control de versiones distribuido',
    @SourcePublication = N'Documentación oficial de Git',
    @SourceUrl = N'https://git-scm.com/doc',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T23-002 | Bloque III | Tema 23
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'No hay diferencia, son sinónimos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'''git fetch'' descarga los cambios remotos sin integrarlos en la rama local, mientras que ''git pull'' los descarga e integra', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'''git fetch'' envía los cambios locales al remoto', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'''git pull'' solo actualiza las etiquetas del repositorio', 0);
INSERT INTO @Tags (Name) VALUES (N'control-de-versiones');
INSERT INTO @Tags (Name) VALUES (N'git');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T23-002',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 23,
    @Difficulty = 3,
    @Statement = N'En Git, ¿qué diferencia existe entre las órdenes ''git fetch'' y ''git pull''?',
    @Explanation = N'''git fetch'' actualiza las referencias remotas descargando los objetos nuevos, pero deja intacta la rama de trabajo. ''git pull'' equivale a un fetch seguido de una integración (merge o rebase) sobre la rama actual.',
    @SourceReference = N'Documentación de Git: git-fetch y git-pull',
    @SourcePublication = N'Documentación oficial de Git',
    @SourceUrl = N'https://git-scm.com/docs/git-pull',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B3-T23-003 | Bloque III | Tema 23
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Desplegar automáticamente cada cambio en el entorno de producción', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Integrar con frecuencia el trabajo de los desarrolladores en una rama común, verificándolo mediante compilación y pruebas automatizadas', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Mantener una única rama sin permitir el trabajo en paralelo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Sustituir las pruebas manuales por revisiones de código', 0);
INSERT INTO @Tags (Name) VALUES (N'ci-cd');
INSERT INTO @Tags (Name) VALUES (N'integracion-continua');
INSERT INTO @Tags (Name) VALUES (N'devops');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B3-T23-003',
    @ExamCode = N'TAI',
    @BlockCode = N'III',
    @TopicNumber = 23,
    @Difficulty = 3,
    @Statement = N'La integración continua (CI) consiste fundamentalmente en:',
    @Explanation = N'La integración continua busca reducir el coste de integrar trabajo divergente mediante fusiones frecuentes en una rama común, cada una validada automáticamente con compilación y batería de pruebas. El despliegue automático en producción corresponde al despliegue continuo.',
    @SourceReference = N'Integración y entrega continuas en el ciclo de vida del software',
    @SourcePublication = N'Temario TAI, Bloque III, tema 23',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T24-001 | Bloque IV | Tema 24
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Lectura, escritura y ejecución para el propietario; lectura y ejecución para el grupo; ningún permiso para el resto', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Lectura y escritura para todos los usuarios', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Solo lectura para el propietario y el grupo', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Lectura, escritura y ejecución para todos los usuarios', 0);
INSERT INTO @Tags (Name) VALUES (N'linux');
INSERT INTO @Tags (Name) VALUES (N'administracion-de-sistemas');
INSERT INTO @Tags (Name) VALUES (N'permisos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T24-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 24,
    @Difficulty = 2,
    @Statement = N'En un sistema de ficheros Unix o Linux, los permisos representados por el valor octal 750 sobre un directorio significan:',
    @Explanation = N'Cada dígito octal codifica lectura (4), escritura (2) y ejecución (1). El 7 concede rwx al propietario, el 5 concede r-x al grupo y el 0 no concede ningún permiso al resto de usuarios.',
    @SourceReference = N'Permisos de ficheros en sistemas POSIX',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 24',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T24-002 | Bloque IV | Tema 24
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La lista de grupos del sistema y sus miembros', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Los resúmenes criptográficos de las contraseñas de los usuarios y su información de caducidad', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Los puntos de montaje de los sistemas de ficheros', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Las rutas de los ejecutables del sistema', 0);
INSERT INTO @Tags (Name) VALUES (N'linux');
INSERT INTO @Tags (Name) VALUES (N'administracion-de-sistemas');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T24-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 24,
    @Difficulty = 3,
    @Statement = N'En un sistema Linux, ¿qué contiene el fichero /etc/shadow?',
    @Explanation = N'/etc/shadow almacena el hash de la contraseña de cada usuario y los datos de envejecimiento de la misma, con permisos restringidos. La información pública de las cuentas está en /etc/passwd, los grupos en /etc/group y los puntos de montaje en /etc/fstab.',
    @SourceReference = N'Gestión de usuarios y contraseñas en sistemas Linux',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 24',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T24-003 | Bloque IV | Tema 24
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Delimitar un límite de replicación independiente entre dominios', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Agrupar objetos con el fin de delegar la administración y aplicar directivas de grupo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Sustituir a los grupos de seguridad en la asignación de permisos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Definir el esquema de la base de datos del directorio', 0);
INSERT INTO @Tags (Name) VALUES (N'windows');
INSERT INTO @Tags (Name) VALUES (N'active-directory');
INSERT INTO @Tags (Name) VALUES (N'administracion-de-sistemas');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T24-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 24,
    @Difficulty = 3,
    @Statement = N'En un dominio de Active Directory, una Unidad Organizativa (OU) sirve principalmente para:',
    @Explanation = N'Las unidades organizativas son contenedores dentro de un dominio que permiten estructurar los objetos, delegar su administración y vincular objetos de directiva de grupo (GPO). No son un límite de replicación ni sustituyen a los grupos de seguridad.',
    @SourceReference = N'Servicios de directorio. Estructura lógica de Active Directory',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 24',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T24-004 | Bloque IV | Tema 24
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'init de System V', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'systemd', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'upstart', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'cron', 0);
INSERT INTO @Tags (Name) VALUES (N'linux');
INSERT INTO @Tags (Name) VALUES (N'systemd');
INSERT INTO @Tags (Name) VALUES (N'administracion-de-sistemas');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T24-004',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 24,
    @Difficulty = 2,
    @Statement = N'El gestor de servicios y de inicio predominante en las distribuciones Linux modernas es:',
    @Explanation = N'systemd es el sistema de inicio y gestor de servicios adoptado por la mayoría de distribuciones actuales, con unidades de servicio, arranque paralelo y la orden systemctl como interfaz. cron es un planificador de tareas, no un gestor de servicios.',
    @SourceReference = N'Administración de servicios en sistemas Linux',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 24',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T25-001 | Bloque IV | Tema 25
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Acelera las consultas de búsqueda por esa columna y penaliza ligeramente las operaciones de inserción y actualización', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Acelera por igual todas las operaciones sobre la tabla', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Reduce el espacio total ocupado por la base de datos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Impide el uso de la columna en cláusulas de ordenación', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'indices');
INSERT INTO @Tags (Name) VALUES (N'rendimiento');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T25-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 25,
    @Difficulty = 3,
    @Statement = N'¿Cuál es el efecto habitual de crear un índice sobre una columna muy consultada de una tabla grande?',
    @Explanation = N'El índice proporciona una vía de acceso alternativa que evita recorrer la tabla completa, mejorando las búsquedas y ordenaciones por esa columna. A cambio ocupa espacio adicional y debe mantenerse en cada inserción, actualización o borrado.',
    @SourceReference = N'Optimización de bases de datos. Índices y planes de ejecución',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 25',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T25-002 | Bloque IV | Tema 25
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Únicamente las lecturas fantasma', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Las lecturas sucias, pero no necesariamente las lecturas no repetibles ni las fantasma', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Todos los fenómenos de concurrencia', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Los interbloqueos entre transacciones', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'transacciones');
INSERT INTO @Tags (Name) VALUES (N'aislamiento');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T25-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 25,
    @Difficulty = 3,
    @Statement = N'El nivel de aislamiento de transacciones READ COMMITTED evita:',
    @Explanation = N'READ COMMITTED garantiza que solo se leen datos confirmados, evitando las lecturas sucias, pero permite lecturas no repetibles y fantasma. Evitarlas todas exige el nivel SERIALIZABLE.',
    @SourceReference = N'ISO/IEC 9075, niveles de aislamiento de transacciones',
    @SourcePublication = N'Estándar ANSI/ISO SQL',
    @SourceUrl = N'https://www.iso.org/standard/76583.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T25-003 | Bloque IV | Tema 25
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Todas las aplicaciones deben conectarse con la cuenta de administrador para simplificar la gestión', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Cada usuario o aplicación debe recibir únicamente los privilegios estrictamente necesarios para su función', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Los privilegios deben concederse siempre a nivel de base de datos completa', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Los permisos deben revisarse solo cuando se produce un incidente', 0);
INSERT INTO @Tags (Name) VALUES (N'bases-de-datos');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'privilegios');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T25-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 25,
    @Difficulty = 2,
    @Statement = N'El principio de mínimo privilegio aplicado a la administración de una base de datos implica que:',
    @Explanation = N'El mínimo privilegio limita el alcance de un compromiso: si una aplicación solo necesita leer determinadas tablas, no debe disponer de permisos de escritura ni de administración. Es además una exigencia expresa de los marcos de seguridad como el Esquema Nacional de Seguridad.',
    @SourceReference = N'Real Decreto 311/2022, principio de mínimo privilegio',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T26-001 | Bloque IV | Tema 26
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El tiempo máximo admisible para restaurar el servicio', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La cantidad máxima de datos que la organización está dispuesta a perder, expresada como intervalo de tiempo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El número de copias que deben conservarse fuera de las instalaciones', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La capacidad total del sistema de respaldo', 0);
INSERT INTO @Tags (Name) VALUES (N'copias-de-seguridad');
INSERT INTO @Tags (Name) VALUES (N'continuidad');
INSERT INTO @Tags (Name) VALUES (N'rpo-rto');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T26-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 26,
    @Difficulty = 3,
    @Statement = N'En una política de copias de seguridad, el RPO (Recovery Point Objective) mide:',
    @Explanation = N'El RPO indica hasta qué punto del pasado es aceptable retroceder, es decir, cuántos datos pueden perderse como máximo, y condiciona la frecuencia de las copias. El tiempo máximo de restauración del servicio es el RTO (Recovery Time Objective).',
    @SourceReference = N'Continuidad de negocio. Objetivos de recuperación RPO y RTO',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 26',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T26-002 | Bloque IV | Tema 26
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La diferencial copia los cambios desde la última copia completa; la incremental copia los cambios desde la última copia de cualquier tipo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La diferencial copia siempre todos los datos y la incremental solo los ficheros nuevos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La incremental exige detener el servicio y la diferencial no', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No existe diferencia técnica entre ambas', 0);
INSERT INTO @Tags (Name) VALUES (N'copias-de-seguridad');
INSERT INTO @Tags (Name) VALUES (N'backup');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T26-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 26,
    @Difficulty = 3,
    @Statement = N'¿Qué diferencia una copia de seguridad diferencial de una incremental?',
    @Explanation = N'La copia diferencial acumula todos los cambios producidos desde la última copia completa, por lo que su restauración requiere la completa más la última diferencial. La incremental guarda solo lo modificado desde la copia anterior, sea completa o incremental, y su restauración exige la completa más toda la cadena.',
    @SourceReference = N'Tipos de copia de seguridad: completa, diferencial e incremental',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 26',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T26-003 | Bloque IV | Tema 26
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La SAN ofrece acceso a nivel de bloque a través de una red dedicada, mientras que la NAS ofrece acceso a nivel de fichero sobre la red de datos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La NAS solo admite discos de estado sólido', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La SAN no permite la redundancia de caminos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La NAS requiere obligatoriamente fibra óptica', 0);
INSERT INTO @Tags (Name) VALUES (N'almacenamiento');
INSERT INTO @Tags (Name) VALUES (N'san');
INSERT INTO @Tags (Name) VALUES (N'nas');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T26-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 26,
    @Difficulty = 2,
    @Statement = N'¿Cuál es la diferencia fundamental entre una arquitectura de almacenamiento SAN y una NAS?',
    @Explanation = N'Una SAN presenta volúmenes al servidor como si fueran discos locales, operando a nivel de bloque sobre protocolos como Fibre Channel o iSCSI. Una NAS expone recursos compartidos a nivel de fichero mediante protocolos como NFS o SMB sobre la red IP.',
    @SourceReference = N'Arquitecturas de almacenamiento en red: SAN y NAS',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 26',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T27-001 | Bloque IV | Tema 27
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Ejecutarse como una aplicación sobre un sistema operativo anfitrión', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Ejecutarse directamente sobre el hardware, sin sistema operativo anfitrión subyacente', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Requerir un contenedor por cada máquina virtual', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No permitir la asignación dinámica de memoria', 0);
INSERT INTO @Tags (Name) VALUES (N'virtualizacion');
INSERT INTO @Tags (Name) VALUES (N'hipervisor');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T27-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 27,
    @Difficulty = 3,
    @Statement = N'Un hipervisor de tipo 1 se caracteriza por:',
    @Explanation = N'El hipervisor de tipo 1 o bare-metal se instala directamente sobre el hardware y gestiona por sí mismo los recursos físicos. El de tipo 2 se ejecuta como una aplicación sobre un sistema operativo anfitrión, con la sobrecarga que ello conlleva.',
    @SourceReference = N'Virtualización de sistemas. Tipos de hipervisor',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 27',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T27-002 | Bloque IV | Tema 27
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El contenedor comparte el núcleo del sistema operativo anfitrión, mientras que la máquina virtual incluye un sistema operativo invitado completo', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El contenedor requiere más recursos que una máquina virtual equivalente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La máquina virtual no permite aislar procesos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El contenedor no puede ejecutarse en entornos de producción', 0);
INSERT INTO @Tags (Name) VALUES (N'virtualizacion');
INSERT INTO @Tags (Name) VALUES (N'contenedores');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T27-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 27,
    @Difficulty = 3,
    @Statement = N'¿Cuál es la diferencia esencial entre un contenedor y una máquina virtual?',
    @Explanation = N'Los contenedores virtualizan a nivel de sistema operativo: comparten el núcleo del anfitrión y aíslan procesos y sistema de ficheros mediante mecanismos como espacios de nombres y grupos de control. Cada máquina virtual, en cambio, ejecuta su propio sistema operativo invitado sobre hardware virtualizado.',
    @SourceReference = N'Virtualización ligera. Contenedores frente a máquinas virtuales',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 27',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T27-003 | Bloque IV | Tema 27
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Únicamente de los datos que introduce en la aplicación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Del sistema operativo invitado, el middleware, las aplicaciones y los datos, mientras el proveedor gestiona la infraestructura física y de virtualización', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'De la infraestructura física del centro de datos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'De nada, ya que el proveedor asume la responsabilidad completa', 0);
INSERT INTO @Tags (Name) VALUES (N'cloud');
INSERT INTO @Tags (Name) VALUES (N'iaas');
INSERT INTO @Tags (Name) VALUES (N'modelos-de-servicio');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T27-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 27,
    @Difficulty = 2,
    @Statement = N'En el modelo de servicio IaaS de computación en la nube, el cliente es responsable de:',
    @Explanation = N'En Infraestructura como Servicio el proveedor aporta cómputo, almacenamiento y red virtualizados, y el cliente gestiona desde el sistema operativo invitado hacia arriba. En PaaS el proveedor asume además la plataforma de ejecución y en SaaS también la aplicación.',
    @SourceReference = N'NIST SP 800-145, The NIST Definition of Cloud Computing',
    @SourcePublication = N'National Institute of Standards and Technology',
    @SourceUrl = N'https://csrc.nist.gov/pubs/sp/800/145/final',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T27-004 | Bloque IV | Tema 27
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Autoservicio bajo demanda', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Elasticidad rápida', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Servicio medido', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Ubicación física conocida y fija de los recursos', 1);
INSERT INTO @Tags (Name) VALUES (N'cloud');
INSERT INTO @Tags (Name) VALUES (N'nist');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T27-004',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 27,
    @Difficulty = 3,
    @Statement = N'Según la definición del NIST, ¿cuál de las siguientes NO es una característica esencial de la computación en la nube?',
    @Explanation = N'El NIST define cinco características esenciales: autoservicio bajo demanda, amplio acceso a la red, agrupación de recursos, elasticidad rápida y servicio medido. Precisamente la agrupación de recursos implica independencia de la ubicación, no una ubicación fija conocida.',
    @SourceReference = N'NIST SP 800-145, The NIST Definition of Cloud Computing',
    @SourcePublication = N'National Institute of Standards and Technology',
    @SourceUrl = N'https://csrc.nist.gov/pubs/sp/800/145/final',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T28-001 | Bloque IV | Tema 28
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La clave privada del emisor', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'La clave pública del destinatario', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La clave privada del destinatario', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Una clave simétrica conocida solo por el emisor', 0);
INSERT INTO @Tags (Name) VALUES (N'criptografia');
INSERT INTO @Tags (Name) VALUES (N'clave-publica');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T28-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 28,
    @Difficulty = 2,
    @Statement = N'En criptografía de clave pública, para enviar un mensaje confidencial a un destinatario se cifra con:',
    @Explanation = N'La confidencialidad se obtiene cifrando con la clave pública del destinatario, ya que solo él posee la clave privada correspondiente para descifrarlo. Cifrar con la clave privada del emisor proporciona autenticidad e integridad, que es el fundamento de la firma electrónica.',
    @SourceReference = N'Criptografía asimétrica. Confidencialidad y firma electrónica',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 28',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T28-002 | Bloque IV | Tema 28
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Que es computacionalmente inviable encontrar dos entradas distintas con el mismo resumen', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Que el resumen puede invertirse para recuperar el mensaje original', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Que la longitud del resumen varía con la del mensaje', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que dos mensajes parecidos producen resúmenes parecidos', 0);
INSERT INTO @Tags (Name) VALUES (N'criptografia');
INSERT INTO @Tags (Name) VALUES (N'hash');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T28-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 28,
    @Difficulty = 3,
    @Statement = N'Una función hash criptográfica debe cumplir, entre otras, la propiedad de resistencia a colisiones, que significa:',
    @Explanation = N'Las propiedades exigibles a una función hash criptográfica son la resistencia a preimagen, a segunda preimagen y a colisiones. Esta última implica que resulta computacionalmente inviable hallar dos mensajes distintos con idéntico resumen. Además, el resumen tiene longitud fija y un cambio mínimo en la entrada altera radicalmente la salida.',
    @SourceReference = N'Funciones resumen criptográficas',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 28',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T28-003 | Bloque IV | Tema 28
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Asimétrico, con longitudes de clave de 1024 y 2048 bits', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Simétrico de bloque, con tamaño de bloque de 128 bits y claves de 128, 192 o 256 bits', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'De flujo, empleado exclusivamente en redes inalámbricas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'De resumen, que produce una salida de 256 bits', 0);
INSERT INTO @Tags (Name) VALUES (N'criptografia');
INSERT INTO @Tags (Name) VALUES (N'aes');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T28-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 28,
    @Difficulty = 3,
    @Statement = N'AES es un algoritmo de cifrado:',
    @Explanation = N'AES (Advanced Encryption Standard) es un cifrador simétrico de bloque que opera sobre bloques de 128 bits y admite claves de 128, 192 o 256 bits. RSA es el algoritmo asimétrico con las longitudes de clave citadas en la primera opción.',
    @SourceReference = N'FIPS 197, Advanced Encryption Standard (AES)',
    @SourcePublication = N'National Institute of Standards and Technology',
    @SourceUrl = N'https://csrc.nist.gov/pubs/fips/197/final',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T28-004 | Bloque IV | Tema 28
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Verificar presencialmente la identidad de los solicitantes', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Emitir y firmar los certificados electrónicos, así como gestionar su revocación', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Custodiar las claves privadas de todos los usuarios', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Sellar temporalmente todos los documentos electrónicos', 0);
INSERT INTO @Tags (Name) VALUES (N'pki');
INSERT INTO @Tags (Name) VALUES (N'certificados');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T28-004',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 28,
    @Difficulty = 3,
    @Statement = N'En una infraestructura de clave pública (PKI), la Autoridad de Certificación (CA) tiene como función principal:',
    @Explanation = N'La Autoridad de Certificación emite y firma los certificados, vinculando una clave pública a una identidad, y publica la información de revocación (CRL u OCSP). La comprobación de la identidad del solicitante corresponde típicamente a la Autoridad de Registro, y el sellado de tiempo a una autoridad específica.',
    @SourceReference = N'RFC 5280, Internet X.509 Public Key Infrastructure Certificate and CRL Profile',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc5280',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T29-001 | Bloque IV | Tema 29
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Su menor coste de instalación en tiradas cortas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Su inmunidad a las interferencias electromagnéticas y su mayor alcance sin repetidores', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La posibilidad de alimentar dispositivos por el propio cable', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Su mayor facilidad de empalme en campo', 0);
INSERT INTO @Tags (Name) VALUES (N'comunicaciones');
INSERT INTO @Tags (Name) VALUES (N'medios-de-transmision');
INSERT INTO @Tags (Name) VALUES (N'fibra-optica');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T29-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 29,
    @Difficulty = 2,
    @Statement = N'Frente al par trenzado de cobre, la fibra óptica presenta como ventaja destacada:',
    @Explanation = N'Al transmitir mediante pulsos de luz, la fibra óptica es inmune a las interferencias electromagnéticas y permite distancias muy superiores con gran ancho de banda. La alimentación por el cable (PoE) es propia del par trenzado, cuyo empalme es además más sencillo y económico en distancias cortas.',
    @SourceReference = N'Medios de transmisión guiados',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 29',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T29-002 | Bloque IV | Tema 29
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Transmitir en un único sentido de forma permanente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Transmitir en ambos sentidos, pero no simultáneamente', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Transmitir y recibir simultáneamente en ambos sentidos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Transmitir únicamente en difusión a todos los nodos', 0);
INSERT INTO @Tags (Name) VALUES (N'comunicaciones');
INSERT INTO @Tags (Name) VALUES (N'modos-de-comunicacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T29-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 29,
    @Difficulty = 2,
    @Statement = N'Un modo de comunicación full-dúplex permite:',
    @Explanation = N'En full-dúplex ambos extremos pueden transmitir y recibir a la vez. El half-dúplex permite los dos sentidos pero alternándolos, y el símplex un único sentido fijo.',
    @SourceReference = N'Modos de comunicación: símplex, half-dúplex y full-dúplex',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 29',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T29-003 | Bloque IV | Tema 29
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Reenvía cada trama por todos los puertos, aumentando las colisiones', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Aprende las direcciones MAC y reenvía cada trama solo por el puerto correspondiente, creando un dominio de colisión por puerto', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Trabaja en el nivel de red decidiendo por dirección IP', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No permite la conexión de más de dos equipos', 0);
INSERT INTO @Tags (Name) VALUES (N'redes');
INSERT INTO @Tags (Name) VALUES (N'conmutacion');
INSERT INTO @Tags (Name) VALUES (N'ethernet');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T29-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 29,
    @Difficulty = 3,
    @Statement = N'Un conmutador (switch) Ethernet, a diferencia de un concentrador (hub):',
    @Explanation = N'El conmutador opera en el nivel de enlace, construye una tabla de direcciones MAC asociadas a puertos y conmuta cada trama solo hacia su destino, de modo que cada puerto constituye un dominio de colisión independiente. El concentrador repite la señal por todos los puertos.',
    @SourceReference = N'Equipos de interconexión: concentradores, conmutadores y encaminadores',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 29',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T30-001 | Bloque IV | Tema 30
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cinco: físico, enlace, red, transporte y aplicación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Siete: físico, enlace de datos, red, transporte, sesión, presentación y aplicación', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Siete: físico, red, enlace, transporte, sesión, aplicación y presentación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cuatro: acceso a red, internet, transporte y aplicación', 0);
INSERT INTO @Tags (Name) VALUES (N'redes');
INSERT INTO @Tags (Name) VALUES (N'modelo-osi');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T30-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 30,
    @Difficulty = 2,
    @Statement = N'¿Cuántos niveles tiene el modelo de referencia OSI y en qué orden ascendente se ordenan?',
    @Explanation = N'El modelo OSI de ISO define siete niveles: 1 físico, 2 enlace de datos, 3 red, 4 transporte, 5 sesión, 6 presentación y 7 aplicación. El modelo de cuatro capas de la última opción corresponde a TCP/IP.',
    @SourceReference = N'ISO/IEC 7498-1, Modelo de referencia básico de interconexión de sistemas abiertos',
    @SourcePublication = N'ISO',
    @SourceUrl = N'https://www.iso.org/standard/20269.html',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T30-002 | Bloque IV | Tema 30
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Aumentar la velocidad física de los enlaces', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Segmentar lógicamente la red en dominios de difusión independientes con independencia de la ubicación física de los equipos', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Sustituir la necesidad de encaminadores para comunicar subredes', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cifrar automáticamente el tráfico entre puertos', 0);
INSERT INTO @Tags (Name) VALUES (N'redes');
INSERT INTO @Tags (Name) VALUES (N'vlan');
INSERT INTO @Tags (Name) VALUES (N'ieee-802-1q');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T30-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 30,
    @Difficulty = 3,
    @Statement = N'El uso de VLAN en una red conmutada permite fundamentalmente:',
    @Explanation = N'Una VLAN agrupa lógicamente puertos o equipos en un mismo dominio de difusión sin depender de su ubicación física, lo que mejora la segmentación y la seguridad. La comunicación entre VLAN distintas sigue requiriendo encaminamiento.',
    @SourceReference = N'IEEE 802.1Q, Virtual Bridged Local Area Networks',
    @SourcePublication = N'IEEE Standards Association',
    @SourceUrl = N'https://standards.ieee.org/ieee/802.1Q/10323/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T30-003 | Bloque IV | Tema 30
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Redes de área local por cable Ethernet', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Redes de área local inalámbricas', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Redes de área personal por Bluetooth', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Redes de área extensa por conmutación de circuitos', 0);
INSERT INTO @Tags (Name) VALUES (N'redes');
INSERT INTO @Tags (Name) VALUES (N'wifi');
INSERT INTO @Tags (Name) VALUES (N'ieee-802-11');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T30-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 30,
    @Difficulty = 2,
    @Statement = N'La familia de estándares IEEE 802.11 corresponde a:',
    @Explanation = N'IEEE 802.11 define las redes de área local inalámbricas (WLAN), conocidas comercialmente como Wi-Fi. Ethernet cableada corresponde a IEEE 802.3 y Bluetooth se recogió inicialmente en IEEE 802.15.1.',
    @SourceReference = N'IEEE 802.11, Wireless LAN Medium Access Control and Physical Layer Specifications',
    @SourcePublication = N'IEEE Standards Association',
    @SourceUrl = N'https://standards.ieee.org/ieee/802.11/7028/',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-001 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'TCP es orientado a conexión y fiable, con control de flujo y congestión; UDP no es orientado a conexión ni garantiza entrega ni orden', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'UDP es orientado a conexión y TCP no', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'TCP opera en el nivel de red y UDP en el de transporte', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'UDP incorpora control de congestión y TCP no', 0);
INSERT INTO @Tags (Name) VALUES (N'tcp-ip');
INSERT INTO @Tags (Name) VALUES (N'transporte');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 2,
    @Statement = N'¿Cuál es la diferencia esencial entre los protocolos TCP y UDP?',
    @Explanation = N'TCP establece una conexión previa, numera los segmentos, confirma su recepción y aplica control de flujo y de congestión. UDP es un servicio de datagramas sin conexión, sin confirmación ni reordenación, con menor sobrecarga y latencia.',
    @SourceReference = N'RFC 9293 (TCP) y RFC 768 (UDP)',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc9293',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-002 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'64 direcciones totales y 62 hosts utilizables', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'32 direcciones totales y 30 hosts utilizables', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'128 direcciones totales y 126 hosts utilizables', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'256 direcciones totales y 254 hosts utilizables', 0);
INSERT INTO @Tags (Name) VALUES (N'tcp-ip');
INSERT INTO @Tags (Name) VALUES (N'direccionamiento');
INSERT INTO @Tags (Name) VALUES (N'subredes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 3,
    @Statement = N'La dirección IPv4 192.168.10.0 con máscara 255.255.255.192 (/26) permite direccionar:',
    @Explanation = N'Una máscara /26 deja 6 bits para host, es decir, 2^6 = 64 direcciones por subred. Descontando la dirección de red y la de difusión quedan 62 direcciones asignables a equipos.',
    @SourceReference = N'RFC 4632, Classless Inter-domain Routing (CIDR)',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc4632',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-003 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'32 bits', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'64 bits', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'128 bits', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'256 bits', 0);
INSERT INTO @Tags (Name) VALUES (N'tcp-ip');
INSERT INTO @Tags (Name) VALUES (N'ipv6');
INSERT INTO @Tags (Name) VALUES (N'direccionamiento');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 2,
    @Statement = N'¿Cuál es la longitud de una dirección IPv6?',
    @Explanation = N'IPv6 emplea direcciones de 128 bits, frente a los 32 bits de IPv4, lo que amplía enormemente el espacio de direccionamiento. Se representan en ocho grupos de cuatro dígitos hexadecimales separados por dos puntos.',
    @SourceReference = N'RFC 8200, Internet Protocol, Version 6 (IPv6) Specification',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc8200',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-004 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Resolver nombres de dominio en direcciones IP', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Asignar dinámicamente parámetros de configuración de red, como la dirección IP, la máscara, la puerta de enlace y los servidores DNS', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Encaminar paquetes entre sistemas autónomos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Cifrar el tráfico entre cliente y servidor', 0);
INSERT INTO @Tags (Name) VALUES (N'tcp-ip');
INSERT INTO @Tags (Name) VALUES (N'dhcp');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-004',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 2,
    @Statement = N'El protocolo DHCP tiene como finalidad:',
    @Explanation = N'DHCP automatiza la configuración de los equipos de una red asignando dirección IP y parámetros asociados mediante un intercambio de mensajes (Discover, Offer, Request y Acknowledge). La resolución de nombres corresponde al DNS.',
    @SourceReference = N'RFC 2131, Dynamic Host Configuration Protocol',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc2131',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-005 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La dirección IPv6 asociada a un nombre', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El servidor de correo responsable de aceptar mensajes para un dominio', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Un alias de un nombre canónico', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Los servidores autorizados del dominio', 0);
INSERT INTO @Tags (Name) VALUES (N'dns');
INSERT INTO @Tags (Name) VALUES (N'redes');
INSERT INTO @Tags (Name) VALUES (N'correo');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-005',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 3,
    @Statement = N'En el sistema de nombres de dominio, un registro de tipo MX indica:',
    @Explanation = N'El registro MX (Mail eXchanger) identifica los servidores de correo del dominio y su prioridad. El registro AAAA aporta la dirección IPv6, el CNAME define alias y el NS los servidores de nombres autorizados.',
    @SourceReference = N'RFC 1035, Domain Names - Implementation and Specification',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc1035',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T31-006 | Bloque IV | Tema 31
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'10.0.0.0 a 10.255.255.255', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'172.16.0.0 a 172.31.255.255', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'192.168.0.0 a 192.168.255.255', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'169.254.0.0 a 169.254.255.255', 0);
INSERT INTO @Tags (Name) VALUES (N'tcp-ip');
INSERT INTO @Tags (Name) VALUES (N'direccionamiento');
INSERT INTO @Tags (Name) VALUES (N'rfc-1918');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T31-006',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 31,
    @Difficulty = 3,
    @Statement = N'El rango de direcciones IPv4 privadas definido en la RFC 1918 para la clase B es:',
    @Explanation = N'La RFC 1918 reserva tres bloques privados: 10.0.0.0/8, 172.16.0.0/12 (de 172.16.0.0 a 172.31.255.255) y 192.168.0.0/16. El rango 169.254.0.0/16 corresponde a la autoconfiguración de enlace local.',
    @SourceReference = N'RFC 1918, Address Allocation for Private Internets',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc1918',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T32-001 | Bloque IV | Tema 32
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'80 y 443', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'8080 y 8443', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'21 y 22', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'25 y 465', 0);
INSERT INTO @Tags (Name) VALUES (N'http');
INSERT INTO @Tags (Name) VALUES (N'puertos');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T32-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 32,
    @Difficulty = 2,
    @Statement = N'¿Qué puertos utilizan por defecto los protocolos HTTP y HTTPS respectivamente?',
    @Explanation = N'HTTP emplea por defecto el puerto TCP 80 y HTTPS el 443. Los puertos 8080 y 8443 son alternativas habituales pero no las asignadas oficialmente por IANA.',
    @SourceReference = N'Registro de nombres de servicio y números de puerto de IANA',
    @SourcePublication = N'IANA',
    @SourceUrl = N'https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.xhtml',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T32-002 | Bloque IV | Tema 32
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Únicamente el cifrado del contenido', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Confidencialidad, integridad de los datos y autenticación del servidor, y opcionalmente del cliente', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Solo la autenticación mutua, sin cifrado', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Compresión obligatoria de la carga útil', 0);
INSERT INTO @Tags (Name) VALUES (N'tls');
INSERT INTO @Tags (Name) VALUES (N'https');
INSERT INTO @Tags (Name) VALUES (N'seguridad');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T32-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 32,
    @Difficulty = 3,
    @Statement = N'En una conexión HTTPS, el protocolo TLS proporciona:',
    @Explanation = N'TLS establece un canal seguro que aporta confidencialidad mediante cifrado simétrico, integridad mediante códigos de autenticación de mensaje y autenticación del servidor a través de su certificado, pudiendo exigirse también la del cliente.',
    @SourceReference = N'RFC 8446, The Transport Layer Security (TLS) Protocol Version 1.3',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc8446',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T32-003 | Bloque IV | Tema 32
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Que mantiene el estado de la sesión entre peticiones de forma nativa', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Que es un protocolo sin estado, en el que cada petición se procesa con independencia de las anteriores', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Que requiere obligatoriamente una conexión UDP', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Que solo admite transferencia de documentos HTML', 0);
INSERT INTO @Tags (Name) VALUES (N'http');
INSERT INTO @Tags (Name) VALUES (N'web');
INSERT INTO @Tags (Name) VALUES (N'protocolos');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T32-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 32,
    @Difficulty = 3,
    @Statement = N'¿Qué caracteriza al protocolo HTTP como protocolo de aplicación?',
    @Explanation = N'HTTP es un protocolo sin estado: el servidor no conserva por sí mismo información entre peticiones. El mantenimiento de sesión se construye por encima mediante cookies, tokens u otros mecanismos.',
    @SourceReference = N'RFC 9110, HTTP Semantics',
    @SourcePublication = N'IETF',
    @SourceUrl = N'https://www.rfc-editor.org/rfc/rfc9110',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-001 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'El IDS detecta y alerta, mientras que el IPS puede además bloquear activamente el tráfico malicioso', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El IPS solo funciona en redes inalámbricas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El IDS actúa en línea y el IPS fuera de línea', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No existe diferencia funcional entre ambos', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'ids');
INSERT INTO @Tags (Name) VALUES (N'ips');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-001',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 2,
    @Statement = N'La diferencia principal entre un sistema de detección de intrusiones (IDS) y uno de prevención (IPS) es que:',
    @Explanation = N'El IDS supervisa el tráfico o los sistemas y genera alertas ante actividad sospechosa. El IPS se sitúa en línea con el tráfico y, además de detectar, puede descartar paquetes o cortar la conexión en tiempo real.',
    @SourceReference = N'Seguridad perimetral. Sistemas de detección y prevención de intrusiones',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 33',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-002 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Alojar los servidores accesibles desde el exterior, aislándolos de la red interna', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Concentrar las copias de seguridad de la organización', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Sustituir al cortafuegos perimetral', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Conectar directamente la red interna a Internet sin filtrado', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'dmz');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-002',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 3,
    @Statement = N'Una zona desmilitarizada (DMZ) en una arquitectura de red se emplea para:',
    @Explanation = N'La DMZ es un segmento intermedio donde se ubican los servicios que deben ser accesibles desde redes no confiables. Si uno de ellos se ve comprometido, el atacante no obtiene acceso directo a la red interna, que permanece protegida por reglas de filtrado adicionales.',
    @SourceReference = N'Arquitecturas de seguridad perimetral',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 33',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-003 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Un aumento del ancho de banda disponible del usuario', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Un túnel cifrado sobre una red no confiable que permite acceder a los recursos internos de la organización', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'La eliminación de la necesidad de autenticar al usuario', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'El acceso anónimo a los servicios internos sin registro alguno', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'vpn');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-003',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 3,
    @Statement = N'Una red privada virtual (VPN) de acceso remoto proporciona:',
    @Explanation = N'La VPN encapsula y cifra el tráfico sobre una red pública, de modo que el usuario remoto opera como si estuviera conectado a la red corporativa, con confidencialidad e integridad de las comunicaciones. Sigue requiriendo autenticación del usuario y del dispositivo.',
    @SourceReference = N'Redes privadas virtuales y protocolos de tunelización',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 33',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-004 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'La capacidad de respuesta a incidentes de seguridad de la información del Centro Criptológico Nacional', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'El registro estatal de certificados electrónicos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'El organismo europeo de certificación de productos criptográficos', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'La autoridad de control en materia de protección de datos', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'ccn-cert');
INSERT INTO @Tags (Name) VALUES (N'incidentes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-004',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 3,
    @Statement = N'El CCN-CERT es:',
    @Explanation = N'El CCN-CERT es la capacidad de respuesta a incidentes de ciberseguridad del Centro Criptológico Nacional, competente en el ámbito del sector público y de las empresas de interés estratégico. La autoridad de control en protección de datos es la Agencia Española de Protección de Datos.',
    @SourceReference = N'Real Decreto 311/2022, referencias al Centro Criptológico Nacional y su capacidad de respuesta a incidentes',
    @SourcePublication = N'BOE núm. 106, de 4 de mayo de 2022',
    @SourceUrl = N'https://www.boe.es/buscar/act.php?id=BOE-A-2022-7191',
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-005 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Solo examina las direcciones IP de origen y destino', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Mantiene una tabla con el estado de las conexiones y evalúa cada paquete en el contexto de la conexión a la que pertenece', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Trabaja exclusivamente en el nivel de aplicación', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'No permite definir reglas de denegación', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'cortafuegos');
INSERT INTO @Tags (Name) VALUES (N'redes');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-005',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 3,
    @Statement = N'Un cortafuegos de inspección de estado (stateful) se diferencia de uno de filtrado de paquetes sin estado en que:',
    @Explanation = N'El cortafuegos de inspección de estado registra las conexiones establecidas y decide sobre cada paquete atendiendo a si pertenece a una conexión legítima ya iniciada, lo que evita tener que abrir reglas amplias para el tráfico de retorno.',
    @SourceReference = N'Cortafuegos: filtrado de paquetes e inspección de estado',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 33',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

-- TAI-B4-T33-006 | Bloque IV | Tema 33
DELETE @Options; DELETE @Tags;
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (1, N'Cifrar el tráfico entre las sedes de la organización', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (2, N'Recopilar, correlacionar y analizar eventos de seguridad procedentes de múltiples fuentes para detectar incidentes', 1);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (3, N'Sustituir a las copias de seguridad periódicas', 0);
INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES (4, N'Gestionar el ciclo de vida de los certificados electrónicos', 0);
INSERT INTO @Tags (Name) VALUES (N'seguridad');
INSERT INTO @Tags (Name) VALUES (N'siem');
INSERT INTO @Tags (Name) VALUES (N'monitorizacion');
EXEC dbo.QuestionUpsert
    @ExternalId = N'TAI-B4-T33-006',
    @ExamCode = N'TAI',
    @BlockCode = N'IV',
    @TopicNumber = 33,
    @Difficulty = 2,
    @Statement = N'Un sistema SIEM tiene como función principal:',
    @Explanation = N'Un SIEM (Security Information and Event Management) centraliza los registros y eventos de sistemas, aplicaciones y dispositivos de red, los normaliza y correlaciona para detectar patrones de ataque y dar soporte a la respuesta ante incidentes.',
    @SourceReference = N'Monitorización y gestión de incidentes de seguridad',
    @SourcePublication = N'Temario TAI, Bloque IV, tema 33',
    @SourceUrl = NULL,
    @IsActive = 1,
    @Options = @Options,
    @Tags = @Tags;

GO

PRINT 'Carga de contenido completada.';
GO
