namespace CVMS.Web.Models.Auth;

public class AccountMenuViewModel
{
    public bool IsAuthenticated { get; init; }

    public string DisplayName { get; init; } = string.Empty;
}