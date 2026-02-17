namespace CodeOwners;

internal sealed class StringLexer(string content)
{
    private int _currentIndex;

    public bool EndOfContent => _currentIndex >= content.Length;

    public bool EndOfLine => EndOfContent || content[_currentIndex] == '\n' ||
                             (content[_currentIndex] == '\r' && content[_currentIndex + 1] == '\n');

    public char Current => !EndOfContent
        ? content[_currentIndex]
        : throw new InvalidOperationException("End of content was reached. It is impossible to get current character");

    public char? Peek()
    {
        if (_currentIndex + 1 >= content.Length)
            return null;

        return content[_currentIndex + 1];
    }

    public char Consume()
    {
        return EndOfContent
            ? throw new InvalidOperationException("End of content was reached; consume operation are not allowed")
            : content[_currentIndex++];
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
        if (EndOfContent || content[_currentIndex] != character)
            return;

        while (Peek() == character)
            Consume();

        Consume();
    }
}
