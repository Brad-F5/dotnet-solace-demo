using SolaceSystems.Solclient.Messaging;

namespace SolaceNEMS.Configuration;

public class NEMSConfig
{
    public required NEMSProperties Properties { get; set; }
    public OAuthCredentials? OAuthCredentials { get; set; }
    public BasicCredentials? BasicCredentials { get; set; }
    public SolLogLevel SolaceClientLogLevel { get; set; } = SolLogLevel.Warning;
}
