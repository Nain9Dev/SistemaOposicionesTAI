using Oposiciones.Application.Common;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// El cliente numera los bloques por su posicion en el temario y la base de datos los
/// codifica en numeracion romana con identificadores IDENTITY que no tienen por que
/// coincidir. Interpretar mal el selector devolvia listas vacias sin error visible.
/// </summary>
public class BlockSelectorTests
{
    [Theory]
    [InlineData("1", "I")]
    [InlineData("2", "II")]
    [InlineData("3", "III")]
    [InlineData("4", "IV")]
    public void El_indice_del_temario_se_traduce_a_codigo_romano(string selector, string expectedCode)
    {
        var (blockId, blockCode) = BlockSelector.Parse(selector);

        Assert.Null(blockId);
        Assert.Equal(expectedCode, blockCode);
    }

    [Theory]
    [InlineData("I")]
    [InlineData("iii")]
    [InlineData("  IV  ")]
    public void El_codigo_romano_se_normaliza_a_mayusculas(string selector)
    {
        var (blockId, blockCode) = BlockSelector.Parse(selector);

        Assert.Null(blockId);
        Assert.Equal(selector.Trim().ToUpperInvariant(), blockCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("all")]
    [InlineData("ALL")]
    [InlineData("todos")]
    public void El_comodin_no_aplica_ningun_filtro(string? selector)
    {
        var (blockId, blockCode) = BlockSelector.Parse(selector);

        Assert.Null(blockId);
        Assert.Null(blockCode);
        Assert.True(BlockSelector.IsAll(selector));
    }

    [Fact]
    public void Un_numero_fuera_del_temario_se_trata_como_identificador()
    {
        var (blockId, blockCode) = BlockSelector.Parse("17");

        Assert.Equal(17, blockId);
        Assert.Null(blockCode);
    }

    [Theory]
    [InlineData("1", "I")]
    [InlineData("i", "I")]
    [InlineData("I", "I")]
    public void Normalize_unifica_las_variantes_del_mismo_bloque(string selector, string expected)
    {
        // Sin esta unificacion las estadisticas por bloque se fragmentaban en "1", "I" e "i".
        Assert.Equal(expected, BlockSelector.Normalize(selector));
    }

    [Fact]
    public void Normalize_conserva_el_comodin()
    {
        Assert.Equal(BlockSelector.All, BlockSelector.Normalize(null));
        Assert.Equal(BlockSelector.All, BlockSelector.Normalize("all"));
    }

    [Theory]
    [InlineData("I", 1)]
    [InlineData("IV", 4)]
    [InlineData("XX", null)]
    [InlineData(null, null)]
    public void OrdinalOf_devuelve_la_posicion_oficial(string? code, int? expected)
    {
        Assert.Equal(expected, BlockSelector.OrdinalOf(code));
    }
}
