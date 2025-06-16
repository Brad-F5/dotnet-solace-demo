using Microsoft.Extensions.Options;
using SolaceNEMS.Messaging;
using SolaceNEMS.Messaging.Internal;
using SolaceSystems.Solclient.Messaging;
using ISession = SolaceSystems.Solclient.Messaging.ISession;

namespace Application.Services;

public class SubscriberService
    : BackgroundService
{
    private readonly ILogger<SubscriberService> _logger;
    private readonly ISessionManager _sessions;
    private readonly ISolaceContextFactory _contextFactory;
    private readonly EventLoader _eventLoader;
    private readonly AppConfig _config;

    private ISession _session;
    private IQueue _queue;
    private IFlow _flow;

    public SubscriberService(ILogger<SubscriberService> logger, ISessionManager sessions, ISolaceContextFactory contextFactory, EventLoader eventLoader, IOptions<AppConfig> config)
    {
        _logger = logger;
        _sessions = sessions;
        _contextFactory = contextFactory;
        _eventLoader = eventLoader;
        _config = config.Value;

        _session = null;
        _queue = null;
        _flow = null;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = _config.Queue;

        _session = await _sessions.ConnectAsync();

        _logger.LogInformation("Attempting to connect to queue '{}'", queueName);
        _queue = _contextFactory.CreateQueue(queueName);

        _logger.LogInformation("Attempting to create flow '{}'", queueName);
        var flowProperties = new FlowProperties
        {
            AckMode = MessageAckMode.ClientAck,
            RequiredOutcomeFailed = true,
            RequiredOutcomeRejected = true
        };

        _flow = _session.CreateFlow(flowProperties, _queue, null, HandleMessageEvent, HandleFlowEvent);
        _flow.Start();
    }

    private void HandleFlowEvent(object? sender, FlowEventArgs e)
    {
        _logger.LogInformation("Received flow event: {}", e.Event);
    }

    private void HandleMessageEvent(object? sender, MessageEventArgs args)
    {
        using var message = args.Message;

        var result = _eventLoader.ProcessEvent(message);
        if (result == EventProcessResult.Success)
        {
            _flow.Settle(message.ADMessageId, MessageOutcome.Accepted);
        }
        else if (result == EventProcessResult.Rejected)
        {
            _logger.LogInformation("Message : {} has FAILED and placed back on the queue for reprocessing", message.ADMessageId);
            _flow.Settle(message.ADMessageId, MessageOutcome.Failed);
        } 
        else
        {
            _logger.LogInformation("Message : {} has been REJECTED and dropped form the queue", message.ADMessageId);
            _flow?.Settle(message.ADMessageId, MessageOutcome.Rejected);
        }
    }

    public override void Dispose()
    {
        _flow?.Dispose();
        _queue?.Dispose();
        _session?.Dispose();

        base.Dispose();
    }
}
