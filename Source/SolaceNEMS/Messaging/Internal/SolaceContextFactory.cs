using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaceNEMS.Configuration;
using SolaceSystems.Solclient.Messaging;

namespace SolaceNEMS.Messaging.Internal;

internal class SolaceContextFactory : ISolaceContextFactory
{
    private readonly ILogger<SolaceContextFactory> _logger;

    public SolaceContextFactory(ILogger<SolaceContextFactory> logger, IOptions<NEMSConfig> config)
    {
        _logger = logger;

        var contextFactoryProperties = new ContextFactoryProperties
        {
            SolClientLogLevel = config.Value.SolaceClientLogLevel,
            LogDelegate = LogContextFactoryDelegate,
        };

        ContextFactory.Instance.Init(contextFactoryProperties);
    }

    public IContext CreateContext()
    {
        var properties = new ContextProperties();
        var context = ContextFactory.Instance.CreateContext(properties, null);
        return context;
    }

    public ITopic CreateTopic(string name)
    {
        var topic = ContextFactory.Instance.CreateTopic(name);
        return topic;
    }

    public IMessage CreateMessage()
    {
        var message = ContextFactory.Instance.CreateMessage();
        return message;
    }

    public IQueue CreateQueue(string name)
    {
        var queue = ContextFactory.Instance.CreateQueue(name);
        return queue;
    }

    public void Dispose()
    {
        ContextFactory.Instance.Cleanup();
    }

    private void LogContextFactoryDelegate(SolLogInfo logInfo)
    {
        var logLevel = logInfo.LogLevel switch
        {
            SolLogLevel.Emergency => LogLevel.Critical,
            SolLogLevel.Alert => LogLevel.Critical,
            SolLogLevel.Critical => LogLevel.Critical,
            SolLogLevel.Error => LogLevel.Error,
            SolLogLevel.Warning => LogLevel.Warning,
            SolLogLevel.Notice => LogLevel.Information,
            SolLogLevel.Info => LogLevel.Information,
            SolLogLevel.Debug => LogLevel.Debug,
            _ => LogLevel.Information,
        };

        if (logInfo.LogException is null)
        {
            _logger.Log(logLevel, "{}", logInfo.LogMessage);
        }
        else
        {
            _logger.Log(logLevel, logInfo.LogException, "{}", logInfo.LogMessage);
        }
    }
}
