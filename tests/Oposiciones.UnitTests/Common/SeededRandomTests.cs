using Oposiciones.Domain.Common;

namespace Oposiciones.UnitTests.Common;

/// <summary>
/// La reproducibilidad por semilla es lo que permite compartir un examen o repetirlo identico.
/// Si el generador dejase de ser determinista, esa promesa se rompe en silencio.
/// </summary>
public class SeededRandomTests
{
    [Fact]
    public void MismaSemilla_ProduceElMismoOrden()
    {
        var origen = Enumerable.Range(1, 50).ToList();

        List<int> primera = new SeededRandom(2026).Shuffled(origen);
        List<int> segunda = new SeededRandom(2026).Shuffled(origen);

        Assert.Equal(primera, segunda);
    }

    [Fact]
    public void SemillasDistintas_ProducenOrdenesDistintos()
    {
        var origen = Enumerable.Range(1, 50).ToList();

        List<int> primera = new SeededRandom(1).Shuffled(origen);
        List<int> segunda = new SeededRandom(2).Shuffled(origen);

        Assert.NotEqual(primera, segunda);
    }

    [Fact]
    public void BarajarNoPierdeNiDuplicaElementos()
    {
        var origen = Enumerable.Range(1, 100).ToList();

        List<int> barajado = new SeededRandom(7).Shuffled(origen);

        Assert.Equal(origen.Count, barajado.Count);
        Assert.Equal(origen.OrderBy(value => value), barajado.OrderBy(value => value));
    }

    [Fact]
    public void ShuffledNoMutaLaColeccionOriginal()
    {
        var origen = Enumerable.Range(1, 20).ToList();
        var copia = origen.ToList();

        _ = new SeededRandom(3).Shuffled(origen);

        Assert.Equal(copia, origen);
    }

    [Fact]
    public void NextDevuelveValoresDentroDelRango()
    {
        var random = new SeededRandom(11);

        for (int i = 0; i < 1_000; i++)
        {
            int value = random.Next(10);
            Assert.InRange(value, 0, 9);
        }
    }

    [Fact]
    public void NextConRangoInvalido_SeRechaza()
    {
        var random = new SeededRandom(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => random.Next(0));
    }

    [Fact]
    public void NewSeed_DevuelveValoresPositivos()
    {
        for (int i = 0; i < 100; i++)
        {
            Assert.InRange(SeededRandom.NewSeed(), 1, int.MaxValue - 1);
        }
    }
}
