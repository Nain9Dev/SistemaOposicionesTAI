namespace Oposiciones.Infrastructure.Configuration;

/// <summary>
/// Localizacion del contenido versionado (temarios y banco de preguntas). Los ficheros JSON son
/// la fuente de verdad: los lee el proveedor en memoria y de ellos se genera el guion de carga
/// para SQL Server, de forma que ambos caminos parten exactamente del mismo material.
/// </summary>
public sealed class ContentOptions
{
    public const string SectionName = "Content";

    /// <summary>Carpeta raiz del contenido, relativa al directorio de la aplicacion o absoluta.</summary>
    public string RootPath { get; set; } = "content";

    /// <summary>Subcarpeta con los perfiles de convocatoria y su temario.</summary>
    public string ExamsFolder { get; set; } = "exams";

    /// <summary>Subcarpeta con los ficheros del banco de preguntas.</summary>
    public string QuestionsFolder { get; set; } = "questions";

    /// <summary>
    /// Aborta el arranque si algun fichero de contenido es invalido. Conviene dejarlo activo:
    /// es preferible no arrancar a servir un banco con preguntas sin respuesta correcta.
    /// </summary>
    public bool FailOnInvalidContent { get; set; } = true;
}
