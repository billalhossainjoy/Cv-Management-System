namespace CVMS.Infrastructure.Integrations;

public class SalesforceOptions
{
    public const string SectionName = "Salesforce";

    public string LoginUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}