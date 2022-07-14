using System.Text;

namespace CodeOwners;

/// <summary>
///
/// </summary>
/// <param name="Pattern"></param>
/// <param name="Owners"></param>
public record CodeOwnersEntry(string Pattern, IList<string> Owners);

/// <summary>
/// The parser for CODEOWNERS format
/// </summary>
public static class CodeOwnersParser
{
    /// <summary>
    ///     Parses CODEOWNERS content
    /// </summary>
    /// <param name="content">The content in CODEOWNERS format</param>
    /// <returns>The list of CODEOWNERS entries</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="content"/> is null</exception>
    public static IEnumerable<CodeOwnersEntry> Parse(string content)
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
