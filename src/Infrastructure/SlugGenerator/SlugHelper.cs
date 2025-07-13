using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.SlugGenerator;

public static class SlugHelper
{
    private static readonly Dictionary<char, string> CharMap = new Dictionary<char, string>
    {
        // Lowercase mappings (will convert input to lowercase first)
        { 'à', "a" }, { 'á', "a" }, { 'â', "a" }, { 'ã', "a" }, { 'ä', "a" }, { 'å', "a" }, { 'ā', "a" }, { 'ă', "a" },
        { 'ą', "a" },
        { 'è', "e" }, { 'é', "e" }, { 'ê', "e" }, { 'ë', "e" }, { 'ē', "e" }, { 'ĕ', "e" }, { 'ė', "e" }, { 'ę', "e" },
        { 'ě', "e" },
        { 'ì', "i" }, { 'í', "i" }, { 'î', "i" }, { 'ï', "i" }, { 'ĩ', "i" }, { 'ī', "i" }, { 'ĭ', "i" }, { 'į', "i" },
        { 'ı', "i" },
        { 'ò', "o" }, { 'ó', "o" }, { 'ô', "o" }, { 'õ', "o" }, { 'ö', "o" }, { 'ō', "o" }, { 'ŏ', "o" }, { 'ő', "o" },
        { 'ø', "o" }, { 'ǒ', "o" }, { 'ǫ', "o" },
        { 'ù', "u" }, { 'ú', "u" }, { 'û', "u" }, { 'ü', "u" }, { 'ũ', "u" }, { 'ū', "u" }, { 'ŭ', "u" }, { 'ů', "u" },
        { 'ű', "u" }, { 'ų', "u" },
        { 'ý', "y" }, { 'ÿ', "y" }, { 'ŷ', "y" },
        { 'ñ', "n" }, { 'ń', "n" }, { 'ņ', "n" }, { 'ň', "n" }, { 'ŋ', "n" },
        { 'ç', "c" }, { 'ć', "c" }, { 'ĉ', "c" }, { 'ċ', "c" }, { 'č', "c" },
        { 'ğ', "g" }, { 'ĝ', "g" }, { 'ġ', "g" }, { 'ģ', "g" },
        { 'ĥ', "h" }, { 'ħ', "h" },
        { 'ĵ', "j" },
        { 'ł', "l" }, { 'ĺ', "l" }, { 'ļ', "l" }, { 'ľ', "l" }, { 'ŀ', "l" },
        { 'ř', "r" }, { 'ŕ', "r" }, { 'ŗ', "r" },
        { 'ś', "s" }, { 'ŝ', "s" }, { 'ş', "s" }, { 'š', "s" },
        { 'ţ', "t" }, { 'ť', "t" }, { 'ŧ', "t" },
        { 'ź', "z" }, { 'ż', "z" }, { 'ž', "z" },
        { 'ß', "ss" },
        { 'æ', "ae" },
        { 'œ', "oe" },
        { 'đ', "d" }, { 'ď', "d" },
        { 'ĳ', "ij" },
        { 'ĸ', "k" },
        { 'ŉ', "n" },
        { 'ſ', "s" },

        // Uppercase versions will be handled by Char.ToLower()

        // Symbols and punctuation
        { '¿', "" }, { '¡', "" },
        { '«', "" }, { '»', "" },
        { '©', "" }, { '®', "" }, { '™', "" },

        // Currency symbols
        { '€', "euro" }, { '£', "gbp" }, { '¥', "yen" }, { '¢', "cent" }, { '¤', "currency" }, { 'ƒ', "florin" },

        // Mathematical symbols
        { '±', "plusminus" }, { '×', "x" }, { '÷', "div" }, { '¼', "1_4" }, { '½', "1_2" }, { '¾', "3_4" },

        // Other special characters
        { '°', "deg" }, { 'µ', "mu" }, { '¶', "pilcrow" }, { '·', "middot" }, { 'º', "ordm" }, { 'ª', "ordf" }
    };

    public static string RemapInternationalCharToAscii(char c)
    {
        // First check if we have a direct mapping
        if (CharMap.TryGetValue(Char.ToLower(c), out string mapped))
        {
            return mapped;
        }

        // For characters not in our dictionary, use Unicode normalization
        string normalized = c.ToString().Normalize(NormalizationForm.FormKD);

        // Remove any remaining non-ASCII characters and diacritics
        string asciiOnly = Regex.Replace(normalized, @"[^\u0000-\u007F]", "");

        // If we got something back, return it
        if (!string.IsNullOrEmpty(asciiOnly))
        {
            return asciiOnly;
        }

        // For completely unmappable characters, return an empty string
        return "";
    }
}