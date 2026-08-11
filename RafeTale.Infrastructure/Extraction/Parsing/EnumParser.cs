public sealed class EnumParser : IEnumParser
{
    public T Parse<T>(string input) where T : struct, Enum
        => Enum.TryParse<T>(input?.Trim(), true, out var result) ? result : default;

    public List<T> ParseList<T>(string input) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(input)) return new List<T>();
        return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => Enum.TryParse<T>(s, true, out _))
                    .Select(s => Enum.Parse<T>(s, true))
                    .ToList();
    }
}