using System.Text;

namespace CodeOwnersParser;

internal class StringLexer
{
    private readonly string _content;
    private int _currentIndex;

    public StringLexer(string content)
    {
        _content = content;
    }

    public bool EndOfContent => _currentIndex >= _content.Length;

    public bool EndOfLine => EndOfContent || _content[_currentIndex] == '\n' ||
                             (_content[_currentIndex] == '\r' && _content[_currentIndex + 1] == '\n');

    public char? Current => EndOfContent ? null : _content[_currentIndex];

    public char? Peek()
    {
        if (_currentIndex + 1 >= _content.Length) return null;

        return _content[_currentIndex + 1];
    }

    public char Consume()
    {
        if (EndOfContent) throw new InvalidOperationException();

        return _content[_currentIndex++];
    }

    public void ConsumeUntilEndOfLine()
    {
        while (!EndOfLine)
        {
            if (EndOfContent) return;
            Consume();
        }
    }

    public void ConsumeAll(char character)
    {
        if (EndOfContent || _content[_currentIndex] != character) return;

        while (Peek() == character)
            Consume();

        Consume();
    }
}

public class CodeOwnersParser
{
    public IEnumerable<(string Pattern, IEnumerable<string> Owners)> Parse(string content)
    {
        var lexer = new StringLexer(content);
        while (!lexer.EndOfContent)
        {
            var lineResult = ParseLine(lexer);
            if (lineResult != null) yield return lineResult.Value;
        }
    }

    static (string Pattern, IEnumerable<string> Owners)? ParseLine(StringLexer lexer)
    {
        while (!lexer.EndOfLine)
        {
            var character = lexer.Current;

            if (character == null)
            {
                throw new InvalidOperationException();
            }

            if (character == '#')
            {
                lexer.ConsumeUntilEndOfLine();
                continue;
            }

            var pattern = ParsePathPattern(lexer);
            lexer.ConsumeAll(' ');
            var owners = ParseOwners(lexer);

            return (pattern, owners);
        }

        if (!lexer.EndOfContent)
            lexer.Consume();

        return null;
    }

    private static string ParsePathPattern(StringLexer lexer)
    {
        var stringBuilder = new StringBuilder();

        while (!lexer.EndOfLine)
        {
            var character = lexer.Consume();
            switch (character)
            {
                case ' ':
                case '\t':
                    return stringBuilder.ToString();
                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        return stringBuilder.ToString();
    }

    private static IEnumerable<string> ParseOwners(StringLexer lexer)
    {
        var owners = new List<string>();
        var stringBuilder = new StringBuilder();

        while (!lexer.EndOfLine)
        {
            var character = lexer.Consume();
            switch (character)
            {
                case ' ':
                    owners.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    break;
                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        if (stringBuilder.Length > 0)
            owners.Add(stringBuilder.ToString());

        return owners;
    }
}
