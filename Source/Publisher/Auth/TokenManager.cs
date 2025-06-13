using Microsoft.Extensions.Options;
using SolaceNEMS.Configuration;
using System.Net.Http.Json;

namespace SolaceNEMS.Auth;

public class TokenManager
{
    private readonly NEMSConfig _config;

    public TokenManager(IOptions<NEMSConfig> config)
    {
        _config = config.Value;
    }

    public bool IsUsingOAuth => _config.OAuthCredentials is not null;

    public async Task<string> AcquireAsync()
    {
        var credentials = _config.OAuthCredentials 
            ?? throw new Exception("Tried to acquire OAuth token while OAuth is not set.");
        
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, credentials.TokenServer)
        {
            Content = new FormUrlEncodedContent(
            [
                new ("client_id", credentials.ClientId),
                new ("client_secret", credentials.ClientSecret),
                new ("grant_type", credentials.GrantType),
                new ("scope", credentials.Scope),
            ])
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to acquire token. OAuth Token server returned {response.StatusCode}\n{response.Content}");
        }

        var deserialized = await response.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new Exception($"Failed to deserialize token. OAuth Token server returned {response.StatusCode}\n{response.Content}");
            
        var token = deserialized.access_token;
        return token;
    }
}
