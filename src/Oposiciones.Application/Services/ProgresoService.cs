using Oposiciones.Application.Common;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Exceptions;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Application.Services;

public sealed class ProgresoService : IProgresoService
{
    /// <summary>Numero de simulacros que se dibujan en la linea de tendencia.</summary>
    private const int TrendSize = 10;

    /// <summary>Un bloque necesita un minimo de preguntas antes de senalarlo como punto debil.</summary>
    private const int MinQuestionsForWeakestBlock = 10;

    private readonly IProgresoRepository _progresoRepository;
    private readonly IScoringService _scoringService;

    public ProgresoService(IProgresoRepository progresoRepository, IScoringService scoringService)
    {
        _progresoRepository = progresoRepository;
        _scoringService = scoringService;
    }

    public async Task<EstadisticasDto> GetEstadisticasAsync(int userId, CancellationToken cancellationToken = default)
    {
        var resumen = await _progresoRepository.GetEstadisticasResumidasAsync(userId, cancellationToken);
        var ultimos = await _progresoRepository.GetUltimosIntentosAsync(userId, TrendSize, cancellationToken);

        var rendimiento = resumen.RendimientoPorBloque
            .Where(bloque => !string.Equals(bloque.Bloque, BlockSelector.All, StringComparison.OrdinalIgnoreCase))
            .Select(bloque => new BloqueRendimientoDto
            {
                Bloque = bloque.Bloque,
                Intentos = bloque.Intentos,
                Aciertos = bloque.Aciertos,
                Fallos = bloque.Fallos,
                Total = bloque.Total,
                Precision = Math.Round(bloque.Precision, 2),
                NotaMedia = Math.Round(bloque.NotaMedia, 2)
            })
            .OrderBy(bloque => bloque.Precision)
            .ToArray();

        return new EstadisticasDto
        {
            TotalPreguntas = resumen.TotalPreguntas,
            Aciertos = resumen.Aciertos,
            Fallos = resumen.Fallos,
            Blancos = resumen.Blancos,
            NotaMedia = Math.Round(resumen.NotaMedia, 2),
            TotalIntentos = resumen.TotalIntentos,
            TasaAcierto = Math.Round(resumen.TasaAcierto, 2),
            MejorNota = Math.Round(resumen.MejorNota, 2),
            UltimaNota = Math.Round(resumen.UltimaNota, 2),
            UltimaActividad = resumen.UltimaActividad,
            ProgresoPorBloque = resumen.ProgresoPorBloque,
            RendimientoPorBloque = rendimiento,
            BloqueMasDebil = rendimiento
                .FirstOrDefault(bloque => bloque.Total >= MinQuestionsForWeakestBlock)?.Bloque,
            Tendencia = ultimos
                .Select(intento => new TendenciaPuntoDto { Fecha = intento.Fecha, Nota = Math.Round(intento.Nota, 2) })
                .ToArray()
        };
    }

    public async Task<IntentoResultadoDto> GuardarIntentoAsync(
        int userId, IntentoDto intento, CancellationToken cancellationToken = default)
    {
        if (intento.Total <= 0)
        {
            throw new BusinessRuleException("Un simulacro debe contener al menos una pregunta.");
        }

        if (intento.Aciertos + intento.Fallos > intento.Total)
        {
            throw new BusinessRuleException("Aciertos y fallos no pueden superar el total de preguntas.");
        }

        var score = _scoringService.Evaluate(intento.Total, intento.Aciertos, intento.Fallos);

        var entidad = new IntentoUsuario
        {
            UsuarioId = userId,
            Aciertos = score.Correct,
            Fallos = score.Wrong,
            Blancos = score.Blank,
            Total = score.Total,
            Nota = score.Grade,
            Bloque = BlockSelector.Normalize(intento.Bloque),
            Fecha = NormalizeToUtc(intento.Fecha)
        };

        entidad.Id = await _progresoRepository.AddIntentoAsync(entidad, cancellationToken);

        return Map(entidad);
    }

    public async Task<PagedResult<IntentoResultadoDto>> GetHistorialAsync(
        int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (safePage, safePageSize) = PagingDefaults.Sanitize(page, pageSize);
        var (items, totalCount) = await _progresoRepository.GetHistorialAsync(userId, safePage, safePageSize, cancellationToken);

        return new PagedResult<IntentoResultadoDto>
        {
            Items = items.Select(Map).ToArray(),
            TotalCount = totalCount,
            Page = safePage,
            PageSize = safePageSize
        };
    }

    public Task<int> BorrarHistorialAsync(int userId, CancellationToken cancellationToken = default) =>
        _progresoRepository.DeleteHistorialAsync(userId, cancellationToken);

    /// <summary>
    /// PostgreSQL rechaza un DateTime con Kind=Unspecified en columnas timestamptz. La fecha
    /// llega del cliente y puede venir sin zona, por lo que se interpreta siempre como UTC.
    /// </summary>
    private static DateTime NormalizeToUtc(DateTime? value)
    {
        if (value is not { } fecha)
        {
            return DateTime.UtcNow;
        }

        return fecha.Kind switch
        {
            DateTimeKind.Utc => fecha,
            DateTimeKind.Local => fecha.ToUniversalTime(),
            _ => DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
        };
    }

    private static IntentoResultadoDto Map(IntentoUsuario intento) => new()
    {
        Id = intento.Id,
        Aciertos = intento.Aciertos,
        Fallos = intento.Fallos,
        Blancos = intento.Blancos,
        Total = intento.Total,
        Nota = Math.Round(intento.Nota, 2),
        Bloque = intento.Bloque,
        Fecha = intento.Fecha
    };
}
