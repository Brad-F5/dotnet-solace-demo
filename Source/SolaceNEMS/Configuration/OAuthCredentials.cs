namespace SolaceNEMS.Configuration;

public class OAuthCredentials
{
    public required string TokenServer { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string GrantType { get; set; }
    public required string Scope { get; set; }
}

