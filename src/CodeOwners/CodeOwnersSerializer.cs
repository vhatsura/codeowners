using System.Text;

namespace CodeOwners;

/// <summary>
///
/// </summary>
/// <param name="Pattern"></param>
/// <param name="Owners"></param>
public record CodeOwnersEntry(string Pattern, IList<string> Owners)
{
    /// <inheritdoc />
    public virtual bool Equals(CodeOwnersEntry? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pattern == other.Pattern && Owners.SequenceEqual(other.Owners);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Pattern);
        foreach (var owner in Owners)
            hash.Add(owner);
        return hash.ToHashCode();
    }
}

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
        ArgumentNullException.ThrowIfNull(content);
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
        ArgumentNullException.ThrowIfNull(entries);

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

    private static List<string> ParseOwners(StringLexer lexer, StringBuilder stringBuilder)
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
