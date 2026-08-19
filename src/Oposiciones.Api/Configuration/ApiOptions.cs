namespace Oposiciones.Api.Configuration;

/// <summary>
/// Ajustes del host HTTP. Todo lo que antes estaba escrito en el codigo de arranque (los origenes
/// CORS, los limites de peticiones) vive aqui para poder cambiarlo por entorno sin recompilar.
/// </summary>
public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public CorsOptions Cors { get; set; } = new();

    public RateLimitOptions RateLimit { get; set; } = new();

    /// <summary>Expone la interfaz de Swagger tambien fuera de desarrollo.</summary>
    public bool EnableSwaggerInProduction { get; set; }
}

/// <summary>Politica CORS.</summary>
public sealed class CorsOptions
{
    /// <summary>
    /// Origenes permitidos. Vacio equivale a permitir cualquiera, que es lo razonable para una
    /// demo publica de solo lectura, pero conviene concretarlo en un despliegue real.
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    public bool AllowCredentials { get; set; }
}

/// <summary>
/// Limitacion de peticiones. Protege el generador de tests, que es la operacion mas cara del
/// sistema, frente a un cliente que entre en bucle.
/// </summary>
public sealed class RateLimitOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>Peticiones permitidas por ventana y por cliente.</summary>
    public int PermitLimit { get; set; } = 120;

    public int WindowSeconds { get; set; } = 60;

    /// <summary>Peticiones que pueden quedar en cola antes de empezar a rechazar con 429.</summary>
    public int QueueLimit { get; set; } = 0;
}
