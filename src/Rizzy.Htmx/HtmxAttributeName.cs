namespace Rizzy.Htmx;

public static class HtmxAttributeName
{
    public static string Resolve(string canonicalName, string? prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalName);

        if (!canonicalName.StartsWith("hx-", StringComparison.Ordinal))
            return canonicalName;

        return string.IsNullOrWhiteSpace(prefix)
            ? canonicalName
            : prefix + canonicalName["hx-".Length..];
    }
}
