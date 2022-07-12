using System.Text;

namespace CodeOwnersParser;

internal static class StringBuilderExtensions
{
    public static string ToStringAndClear(this StringBuilder stringBuilder)
    {
        var result = stringBuilder.ToString();
        stringBuilder.Clear();
        return result;
    }
}
