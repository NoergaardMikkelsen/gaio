namespace Statistics.Shared.Extensions;

public static class StringExtensions
{
    public static string ToJsonPropertyCapitalisation(this string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
        {
            return input;
        }

        return char.ToLower(input[0]) + input[1..];
    }

    public static string ScreamingSnakeCaseToTitleCase(this string input)
    {
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(string.Join(' ', input.Split('_')).ToLower());
    }
}
