using Oposiciones.Application.DTOs;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// Sin saturacion, page=0 producia un OFFSET negativo (error de SQL) y pageSize sin tope
/// permitia volcar la tabla completa en una sola peticion.
/// </summary>
public class PagingDefaultsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Una_pagina_no_positiva_se_satura_a_la_primera(int page)
    {
        var (safePage, _) = PagingDefaults.Sanitize(page, PagingDefaults.DefaultPageSize);

        Assert.Equal(PagingDefaults.DefaultPage, safePage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Un_tamano_no_positivo_usa_el_valor_por_defecto(int pageSize)
    {
        var (_, safePageSize) = PagingDefaults.Sanitize(1, pageSize);

        Assert.Equal(PagingDefaults.DefaultPageSize, safePageSize);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(10_000)]
    [InlineData(int.MaxValue)]
    public void El_tamano_nunca_supera_el_maximo(int pageSize)
    {
        var (_, safePageSize) = PagingDefaults.Sanitize(1, pageSize);

        Assert.Equal(PagingDefaults.MaxPageSize, safePageSize);
    }

    [Fact]
    public void Los_valores_validos_no_se_alteran()
    {
        var (page, pageSize) = PagingDefaults.Sanitize(3, 50);

        Assert.Equal(3, page);
        Assert.Equal(50, pageSize);
    }

    [Fact]
    public void El_envoltorio_calcula_la_navegacion()
    {
        var result = new PagedResult<int>
        {
            Items = [1, 2, 3],
            TotalCount = 25,
            Page = 2,
            PageSize = 10
        };

        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void Un_resultado_vacio_no_divide_entre_cero()
    {
        var result = new PagedResult<int> { TotalCount = 0, Page = 1, PageSize = 0 };

        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }
}
