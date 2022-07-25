using System.Text;

namespace CodeOwners;

/// <summary>
///
/// </summary>
/// <param name="Pattern"></param>
/// <param name="Owners"></param>
public record CodeOwnersEntry(string Pattern, IList<string> Owners);

/// <summary>
/// The serializer for CODEOWNERS format
/// </summary>
public static class CodeOwnersSerializer
{
    /// <summary>
    ///     Deserializes CODEOWNERS content
    /// </summary>
    /// <param name="content">The content in CODEOWNERS format</param>
    /// <returns>The list of CODEOWNERS entries</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="content"/> is null</exception>
    public static IEnumerable<CodeOwnersEntry> Deserialize(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrWhiteSpace(content))
            yield break;

        var lexer = new StringLexer(content);
        var stringBuilder = new StringBuilder();

        while (!lexer.EndOfContent)
        {
            var lineResult = ParseLine(lexer, stringBuilder);
            if (lineResult != null)
                yield return lineResult;
        }
    }

    /// <summary>
    ///     Serializes codeowners entries to CODEOWNERS content
    /// </summary>
    /// <param name="entries">Codeowners entries</param>
    /// <returns>The content in CODEOWNERS format</returns>
    public static string Serialize(IEnumerable<CodeOwnersEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var stringBuilder = new StringBuilder();

        foreach (var entry in entries)
        {
#pragma warning disable CA1305
            stringBuilder.AppendLine($"{entry.Pattern} {string.Join(" ", entry.Owners)}");
#pragma warning restore CA1305
        }

        return stringBuilder.ToString();
    }

    private static CodeOwnersEntry? ParseLine(StringLexer lexer, StringBuilder stringBuilder)
    {
        while (!lexer.EndOfLine)
        {
            var character = lexer.Current;

            if (character == '#')
            {
                lexer.ConsumeUntilEndOfLine();
                continue;
            }

            var pattern = ParsePathPattern(lexer, stringBuilder);
            lexer.ConsumeAll(' ');
            var owners = ParseOwners(lexer, stringBuilder);

            return new CodeOwnersEntry(pattern, owners);
        }

        if (!lexer.EndOfContent)
            lexer.Consume();

        return null;
    }

    private static string ParsePathPattern(StringLexer lexer, StringBuilder stringBuilder)
    {
        while (!lexer.EndOfLine)
        {
            var character = lexer.Consume();
            switch (character)
            {
                case ' ':
                case '\t':
                    return stringBuilder.ToStringAndClear();
                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        return stringBuilder.ToStringAndClear();
    }

    private static IList<string> ParseOwners(StringLexer lexer, StringBuilder stringBuilder)
    {
        var owners = new List<string>();

        while (!lexer.EndOfLine)
        {
            var character = lexer.Consume();
            switch (character)
            {
                case ' ':
                    owners.Add(stringBuilder.ToStringAndClear());
                    break;
                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        if (stringBuilder.Length > 0)
            owners.Add(stringBuilder.ToStringAndClear());

        return owners;
    }
}
