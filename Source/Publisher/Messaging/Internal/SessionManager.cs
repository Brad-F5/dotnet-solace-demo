using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaceNEMS.Auth;
using SolaceNEMS.Configuration;
using SolaceSystems.Solclient.Messaging;

namespace SolaceNEMS.Messaging.Internal;

internal class SessionManager : ISessionManager
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<SessionManager> _logger;
    private readonly IContext _context;
    private readonly NEMSConfig _config;
    private readonly TokenManager _tokenManager;

    public SessionManager(ILoggerFactory loggerFactory, ILogger<SessionManager> logger, IContext context, IOptions<NEMSConfig> config, TokenManager tokenManager)
    {
        _loggerFactory = loggerFactory;
        _logger = logger;
        _context = context;
        _config = config.Value;
        _tokenManager = tokenManager;
    }

    private async Task<SessionProperties> GenerateSessionProperties()
    {
        var sessionProperties = new SessionProperties
        {
            Host = _config.Properties.Host,
            VPNName = _config.Properties.VPNName,
            ReconnectRetries = _config.Properties.ReconnectRetries,
            SSLValidateCertificate = _config.Properties.SSLValidateCertificate,
        };

        if (_config.OAuthCredentials is not null)
        {
            var token = await _tokenManager.AcquireAsync();
            _logger.LogInformation("Acquired session token.");

            sessionProperties.OAuth2AccessToken = token;
            sessionProperties.AuthenticationScheme = AuthenticationSchemes.OAUTH2;
        }
        else if (_config.BasicCredentials is not null)
        {
            sessionProperties.UserName = _config.BasicCredentials.Username;
            sessionProperties.Password = _config.BasicCredentials.Password;
            sessionProperties.AuthenticationScheme = AuthenticationSchemes.BASIC;
        }
        else throw new Exception("Failed to find either OAuth or Basic Solace credentials.");

        return sessionProperties;
    }

    public async Task<ISession> ConnectAsync(EventHandler<MessageEventArgs>? messageEventHandler, EventHandler<SessionEventArgs>? sessionEventHandler)
    {
        var sessionProperties = await GenerateSessionProperties();
        _logger.LogInformation("Connecting as {} on {}...", sessionProperties.VPNName, sessionProperties.Host);

        var session = new SessionWrapper(
            _loggerFactory.CreateLogger<SessionWrapper>(),
            _tokenManager,
            _context,
            sessionProperties,
            messageEventHandler,
            sessionEventHandler);

        var result = session.Connect();
        if (result is not ReturnCode.SOLCLIENT_OK)
        {
            throw new Exception($"Failed to connect to NEMS. Reason: {result}");
        }

        _logger.LogInformation("Successfully connected to {}!", sessionProperties.VPNName);

        return session;
    }
}

