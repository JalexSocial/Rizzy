namespace Rizzy.Extensions;
internal static class StringExtensions
{
    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Convert the first character to uppercase and concatenate the rest of the string
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
