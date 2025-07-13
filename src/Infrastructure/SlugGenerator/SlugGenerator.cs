using System.Globalization;
using System.Text;
using Application.Abstractions.Authentication;

namespace Infrastructure.SlugGenerator;

public class SlugGenerator : ISlugGenerator
{
    /// <summary>
    /// Produces optional, URL-friendly version of a title, "like-this-one". 
    /// hand-tuned for speed, reflects performance refactoring contributed
    /// by John Gietzen (user otac0n) 
    /// </summary>
    public string URLFriendly(string title)
    {
        if (title == null) return "";

        const int maxlen = 80;
        int len = title.Length;
        bool prevdash = false;
        var sb = new StringBuilder(len);
        char c;

        for (int i = 0; i < len; i++)
        {
            c = title[i];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                sb.Append(c);
                prevdash = false;
            }
            else if (c >= 'A' && c <= 'Z')
            {
                // tricky way to convert to lowercase
                sb.Append((char)(c | 32));
                prevdash = false;
            }
            else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                c == '\\' || c == '-' || c == '_' || c == '=')
            {
                if (!prevdash && sb.Length > 0)
                {
                    sb.Append('-');
                    prevdash = true;
                }
            }
            else if ((int)c >= 128)
            {
                int prevlen = sb.Length;
                sb.Append(SlugHelper.RemapInternationalCharToAscii(c));
                if (prevlen != sb.Length)
                {
                    prevdash = false;
                }
            }
            if (i == maxlen) break;
        }

        if (prevdash)
            return sb.ToString().Substring(0, sb.Length - 1);
        else
            return sb.ToString();
    }

    /// <summary>
    /// Generates a URL-friendly slug from multiple parameters of any type
    /// </summary>
    /// <param name="separator">Character to separate different parameters (default: '-')</param>
    /// <param name="parameters">Variable number of parameters of any type</param>
    /// <returns>URL-friendly slug</returns>
    public string URLFriendly(params object[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
            return "";

        var parts = new List<string>();

        foreach (var param in parameters)
        {
            if (param == null) continue;

            string stringValue = ConvertToString(param);
            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                string slugPart = URLFriendly(stringValue);
                if (!string.IsNullOrEmpty(slugPart))
                {
                    parts.Add(slugPart);
                }
            }
        }

        return string.Join('-', parts);
    }

    /// <summary>
    /// Generates a URL-friendly slug from multiple string parameters
    /// </summary>
    /// <param name="separator">Character to separate different parameters (default: '-')</param>
    /// <param name="parameters">Variable number of string parameters</param>
    /// <returns>URL-friendly slug</returns>
    public string URLFriendly(params string[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
            return "";

        var parts = new List<string>();

        foreach (var param in parameters)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                string slugPart = URLFriendly(param);
                if (!string.IsNullOrEmpty(slugPart))
                {
                    parts.Add(slugPart);
                }
            }
        }

        return string.Join('-', parts);
    }

    /// <summary>
    /// Generates a unique slug with multiple parameters and database existence check
    /// </summary>
    /// <param name="existsInDatabase">Function to check if slug exists in database</param>
    /// <param name="separator">Character to separate different parameters (default: '-')</param>
    /// <param name="parameters">Variable number of parameters of any type</param>
    /// <returns>Unique URL-friendly slug</returns>
    public async Task<string> GenerateUniqueSlug(Func<string, Task<bool>> existsInDatabase, params object[] parameters)
    {
        string baseSlug = URLFriendly(parameters);

        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "item"; // fallback if no valid parameters

        string slug = baseSlug;
        int counter = 1;

        while (await existsInDatabase(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }


    /// <summary>
    /// Generates a unique slug with multiple string parameters and database existence check
    /// </summary>
    /// <param name="existsInDatabase">Function to check if slug exists in database</param>
    /// <param name="separator">Character to separate different parameters (default: '-')</param>
    /// <param name="parameters">Variable number of string parameters</param>
    /// <returns>Unique URL-friendly slug</returns>
    public async Task<string> GenerateUniqueSlug(Func<string, Task<bool>> existsInDatabase, char separator = '-', params string[] parameters)
    {
        string baseSlug = URLFriendly(separator, parameters);

        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "item"; // fallback if no valid parameters

        string slug = baseSlug;
        int counter = 1;

        while (await existsInDatabase(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    /// <summary>
    /// Converts various types to string representation suitable for slug generation
    /// </summary>
    private string ConvertToString(object value)
    {
        return value switch
        {
            string str => str,
            int i => i.ToString(),
            long l => l.ToString(),
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            double db => db.ToString(CultureInfo.InvariantCulture),
            float f => f.ToString(CultureInfo.InvariantCulture),
            DateTime dt => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly t => t.ToString("HH-mm", CultureInfo.InvariantCulture),
            bool b => b.ToString().ToLower(),
            Enum e => e.ToString(),
            Guid g => g.ToString(),
            _ => value.ToString()
        };
    }
}
