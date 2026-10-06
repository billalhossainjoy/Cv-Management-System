using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CVMS.Application.Integrations;
using CVMS.Application.Services.Interfaces;
using CVMS.Infrastructure.Identity;
using CVMS.Infrastructure.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CVMS.Infrastructure.Services;

public class SalesforceService : ISalesforceService
{
    private readonly HttpClient _http;
    private readonly SalesforceOptions _options;
    private readonly IProfileService _profileService;
    private readonly UserManager<ApplicationUser> _userManager;

    

    public SalesforceService(IOptions<SalesforceOptions> options, HttpClient httpClient,
        IProfileService profileService,  UserManager<ApplicationUser> userManager)
    {
        _http = httpClient;
        _options = options.Value;
        _profileService = profileService;
        _userManager =  userManager;
    }
    public async Task<bool> PushUserToSalesforceAsync(Guid userId, string extraInfo, CancellationToken ct)
    {

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.LoginUrl}/services/oauth2/token");

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
        });
        
        var response = await _http.SendAsync(request, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Salesforce API Error ({(int)response.StatusCode}): {errorBody}");
        }

        var tokenData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            
        var accessToken = tokenData.GetProperty("access_token").GetString();
        var instanceUrl = tokenData.GetProperty("instance_url").GetString();
        
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        // _http.DefaultRequestHeaders.Add("Sforce-Duplicate-Rule-Header", "allowSave=true");

        var profile = await _profileService.GetProfileAsync(userId, ct);
        if (profile == null) return false;
        
        var firstName = profile.Values.FirstOrDefault(v => v.Attribute.Name == "First Name")?.Value ?? "Unknown";
        var lastName = profile.Values.FirstOrDefault(v => v.Attribute.Name == "Last Name")?.Value ?? "Unknown";
        var phone = profile.Values.FirstOrDefault(v => v.Attribute.Name == "Phone")?.Value ?? "Unknown";
        
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        var email = appUser?.Email;
        
        
        var query = $"SELECT Id, AccountId FROM Contact WHERE Email = '{email}' LIMIT 1";
        var queryUrl = $"{instanceUrl}/services/data/v60.0/query/?q={Uri.EscapeDataString(query)}";
        
        var queryResponse = await _http.GetAsync(queryUrl, ct);
        var queryData = await queryResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        var totalSize = queryData.GetProperty("totalSize").GetInt32();
        
        if (totalSize > 0)
        {
            var contactRecord = queryData.GetProperty("records")[0];
            var contactId = contactRecord.GetProperty("Id").GetString();
            
            var updateResponse = await _http.PatchAsJsonAsync($"{instanceUrl}/services/data/v60.0/sobjects/Contact/{contactId}", new 
            { 
                FirstName = firstName, 
                LastName = lastName, 
                Phone = phone,
                Description = extraInfo 
            }, ct);
            
            if (!updateResponse.IsSuccessStatusCode)
                throw new Exception($"Contact Update Error: {await updateResponse.Content.ReadAsStringAsync(ct)}");
        }
        else
        {
            var accountResponse = await _http.PostAsJsonAsync($"{instanceUrl}/services/data/v60.0/sobjects/Account/", new { Name = $"{firstName} {lastName} Household" }, ct);
            if (!accountResponse.IsSuccessStatusCode)
                throw new Exception($"Account Creation Error: {await accountResponse.Content.ReadAsStringAsync(ct)}");
                
            var accountId = (await accountResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct)).GetProperty("id").GetString();
            
            var createResponse = await _http.PostAsJsonAsync($"{instanceUrl}/services/data/v60.0/sobjects/Contact/", new 
            { 
                AccountId = accountId,
                FirstName = firstName, 
                LastName = lastName, 
                Email = email,
                Phone = phone,
                Description = extraInfo 
            }, ct);
            
            if (!createResponse.IsSuccessStatusCode)
                throw new Exception($"Contact Creation Error: {await createResponse.Content.ReadAsStringAsync(ct)}");
        }
        
        return true;
    }
}
