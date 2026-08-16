public interface IEnumParser
{
    T Parse<T>(string input) where T : struct, Enum;
    List<T> ParseList<T>(string input) where T : struct, Enum;
}