namespace SolaceNEMS.Configuration;

public class NEMSProperties
{
    public required string Host { get; set; }
    public required string VPNName { get; set; }
    public int ReconnectRetries { get; set; } = 5;
    public bool SSLValidateCertificate { get; set; } = false;
}

