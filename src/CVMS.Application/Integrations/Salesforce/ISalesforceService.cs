namespace CVMS.Application.Integrations;

public interface ISalesforceService
{
    Task<bool> PushUserToSalesforceAsync(
        Guid userId,
        string additionalInfo,
        CancellationToken cancellationToken);
}
