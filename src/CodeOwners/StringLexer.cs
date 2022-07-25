namespace CodeOwners;

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

    public char Current => !EndOfContent
        ? _content[_currentIndex]
        : throw new InvalidOperationException("End of content was reached. It is impossible to get current character");

    public char? Peek()
    {
        if (_currentIndex + 1 >= _content.Length)
            return null;

        return _content[_currentIndex + 1];
    }

    public char Consume()
    {
        if (EndOfContent)
            throw new InvalidOperationException("End of content was reached; consume operation are not allowed");

        return _content[_currentIndex++];
    }

    public void ConsumeUntilEndOfLine()
    {
        while (!EndOfLine)
        {
            if (EndOfContent)
                return;
            Consume();
        }
    }

    public void ConsumeAll(char character)
    {
        if (EndOfContent || _content[_currentIndex] != character)
            return;

        while (Peek() == character)
            Consume();

        Consume();
    }
}
