namespace Statistics.Shared.Extensions;

public static class EnumTypeExtensions
{
    public static IEnumerable<string> EnumNamesToTitleCase(this Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Provided type must be an enum.", nameof(enumType));
        }

        return Enum.GetNames(enumType).Select(x => x.ScreamingSnakeCaseToTitleCase());
    }
}
