using Oposiciones.Application.DTOs;
using Oposiciones.Application.Services;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Exceptions;
using Oposiciones.Domain.Interfaces;
using Xunit;

namespace Oposiciones.UnitTests;

public class ProgresoServiceTests
{
    private readonly FakeProgresoRepository _repository = new();
    private readonly ProgresoService _service;

    public ProgresoServiceTests()
    {
        _service = new ProgresoService(_repository, new ScoringService());
    }

    [Fact]
    public async Task La_nota_la_calcula_el_servidor_y_no_el_cliente()
    {
        // El DTO de entrada ni siquiera expone Nota: el cliente no puede declararla.
        await _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 8, Fallos = 2, Total = 10 });

        var guardado = Assert.Single(_repository.Guardados);
        Assert.Equal(7.34d, guardado.Nota);
    }

    [Fact]
    public async Task El_intento_se_asocia_al_usuario_autenticado()
    {
        await _service.GuardarIntentoAsync(42, new IntentoDto { Aciertos = 1, Fallos = 0, Total = 1 });

        Assert.Equal(42, Assert.Single(_repository.Guardados).UsuarioId);
    }

    [Fact]
    public async Task Los_blancos_se_derivan_del_total()
    {
        await _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 5, Fallos = 5, Total = 20 });

        Assert.Equal(10, Assert.Single(_repository.Guardados).Blancos);
    }

    [Fact]
    public async Task El_bloque_se_normaliza_antes_de_archivar()
    {
        await _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 1, Fallos = 0, Total = 1, Bloque = "2" });

        Assert.Equal("II", Assert.Single(_repository.Guardados).Bloque);
    }

    [Fact]
    public async Task Sin_bloque_el_intento_se_archiva_como_temario_completo()
    {
        await _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 1, Fallos = 0, Total = 1 });

        Assert.Equal(IntentoUsuario.BloqueTodos, Assert.Single(_repository.Guardados).Bloque);
    }

    [Fact]
    public async Task Una_fecha_sin_zona_se_interpreta_como_utc()
    {
        // PostgreSQL rechaza un DateTime con Kind=Unspecified en columnas timestamptz.
        var sinZona = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Unspecified);

        await _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 1, Fallos = 0, Total = 1, Fecha = sinZona });

        Assert.Equal(DateTimeKind.Utc, Assert.Single(_repository.Guardados).Fecha.Kind);
    }

    [Fact]
    public async Task Un_simulacro_sin_preguntas_se_rechaza()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 0, Fallos = 0, Total = 0 }));
    }

    [Fact]
    public async Task Aciertos_y_fallos_no_pueden_superar_el_total()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.GuardarIntentoAsync(1, new IntentoDto { Aciertos = 8, Fallos = 8, Total = 10 }));
    }

    [Fact]
    public async Task El_bloque_mas_debil_ignora_muestras_pequenas()
    {
        // Fallar 2 de 2 preguntas de un bloque no lo convierte en el punto debil del opositor.
        _repository.Resumen = new EstadisticasResumen
        {
            TotalPreguntas = 42,
            RendimientoPorBloque =
            [
                new BloqueRendimiento { Bloque = "III", Total = 2,  Precision = 0d },
                new BloqueRendimiento { Bloque = "II",  Total = 40, Precision = 45d }
            ]
        };

        var estadisticas = await _service.GetEstadisticasAsync(1);

        Assert.Equal("II", estadisticas.BloqueMasDebil);
        Assert.Equal("III", estadisticas.RendimientoPorBloque[0].Bloque);
    }

    [Fact]
    public async Task El_comodin_no_aparece_como_bloque_del_temario()
    {
        _repository.Resumen = new EstadisticasResumen
        {
            RendimientoPorBloque =
            [
                new BloqueRendimiento { Bloque = "all", Total = 100, Precision = 60d },
                new BloqueRendimiento { Bloque = "I",   Total = 20,  Precision = 70d }
            ]
        };

        var estadisticas = await _service.GetEstadisticasAsync(1);

        Assert.Equal("I", Assert.Single(estadisticas.RendimientoPorBloque).Bloque);
    }

    [Fact]
    public async Task El_historial_satura_la_paginacion()
    {
        var pagina = await _service.GetHistorialAsync(1, page: 0, pageSize: 9999);

        Assert.Equal(PagingDefaults.DefaultPage, pagina.Page);
        Assert.Equal(PagingDefaults.MaxPageSize, pagina.PageSize);
    }

    private sealed class FakeProgresoRepository : IProgresoRepository
    {
        public List<IntentoUsuario> Guardados { get; } = [];
        public EstadisticasResumen Resumen { get; set; } = new();

        public Task<int> AddIntentoAsync(IntentoUsuario intento, CancellationToken cancellationToken = default)
        {
            Guardados.Add(intento);
            return Task.FromResult(Guardados.Count);
        }

        public Task<(IReadOnlyList<IntentoUsuario> Items, int TotalCount)> GetHistorialAsync(
            int usuarioId, int page, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult<(IReadOnlyList<IntentoUsuario>, int)>(([], 0));

        public Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Resumen);

        public Task<IReadOnlyList<IntentoUsuario>> GetUltimosIntentosAsync(
            int usuarioId, int limit, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<IntentoUsuario>>([]);

        public Task<int> DeleteHistorialAsync(int usuarioId, CancellationToken cancellationToken = default) =>
            Task.FromResult(0);
    }
}
