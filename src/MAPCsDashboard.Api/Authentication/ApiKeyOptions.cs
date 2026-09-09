namespace MAPCsDashboard.Api.Authentication;

public sealed class ApiKeyOptions
{
    public const string SectionName = "ApiKey";
    public string Key { get; init; } = string.Empty;
}

