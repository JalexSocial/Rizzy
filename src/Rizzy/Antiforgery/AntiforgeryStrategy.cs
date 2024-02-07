namespace Rizzy.Antiforgery;

public enum AntiforgeryStrategy
{
    None = 1,
    GenerateTokensPerPage = 2,
    GenerateTokensPerRequest = 3
}
