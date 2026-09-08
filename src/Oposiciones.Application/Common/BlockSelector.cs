using System.Globalization;

namespace Oposiciones.Application.Common;

/// <summary>
/// Traduce el selector de bloque que envia el cliente.
///
/// La interfaz numera los bloques por su posicion en el temario ("1".."4") mientras que el
/// temario esta codificado en numeracion romana ("I".."IV"), y ademas admite el comodin "all".
/// El indice NO es el identificador de la tabla: SyllabusBlocks se siembra con IDENTITY y el
/// orden de insercion no esta garantizado, asi que el bloque 1 del temario puede tener Id 2.
/// Interpretar el numero como Id devolvia listas vacias o preguntas del bloque equivocado.
/// </summary>
public static class BlockSelector
{
    public const string All = "all";

    /// <summary>Codigos del temario TAI en su orden oficial.</summary>
    private static readonly string[] RomanByOrdinal = ["I", "II", "III", "IV"];

    /// <summary>Descompone el selector en filtro por identificador o por codigo de bloque.</summary>
    public static (int? BlockId, string? BlockCode) Parse(string? selector)
    {
        if (IsAll(selector))
        {
            return (null, null);
        }

        var value = selector!.Trim();

        if (TryParseOrdinal(value, out var roman))
        {
            return (null, roman);
        }

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric) && numeric > 0)
        {
            // Fuera del rango del temario se interpreta como identificador real de la tabla.
            return (numeric, null);
        }

        return (null, value.ToUpperInvariant());
    }

    public static bool IsAll(string? selector) =>
        string.IsNullOrWhiteSpace(selector) ||
        selector.Trim().Equals(All, StringComparison.OrdinalIgnoreCase) ||
        selector.Trim().Equals("todos", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Etiqueta estable con la que se archiva el intento. Se normaliza para que las
    /// estadisticas por bloque no se fragmenten entre "1", "I" y "i".
    /// </summary>
    public static string Normalize(string? selector)
    {
        if (IsAll(selector))
        {
            return All;
        }

        var value = selector!.Trim();

        return TryParseOrdinal(value, out var roman) ? roman : value.ToUpperInvariant();
    }

    /// <summary>Posicion oficial del bloque en el temario (1..4), o null si no se reconoce.</summary>
    public static int? OrdinalOf(string? blockCode)
    {
        if (string.IsNullOrWhiteSpace(blockCode))
        {
            return null;
        }

        var index = Array.IndexOf(RomanByOrdinal, blockCode.Trim().ToUpperInvariant());
        return index >= 0 ? index + 1 : null;
    }

    private static bool TryParseOrdinal(string value, out string roman)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ordinal)
            && ordinal >= 1
            && ordinal <= RomanByOrdinal.Length)
        {
            roman = RomanByOrdinal[ordinal - 1];
            return true;
        }

        roman = string.Empty;
        return false;
    }
}
