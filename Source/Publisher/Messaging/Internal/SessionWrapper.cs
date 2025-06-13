using Microsoft.Extensions.Logging;
using SolaceNEMS.Auth;
using SolaceSystems.Solclient.Messaging;
using SolaceSystems.Solclient.Messaging.Cache;

namespace SolaceNEMS.Messaging.Internal;

internal class SessionWrapper
    : ISession
{
    private readonly ILogger<SessionWrapper> _logger;
    private readonly TokenManager _tokenManager;

    private readonly ISession _session;

    internal SessionWrapper(ILogger<SessionWrapper> logger, TokenManager tokenManager, IContext context, SessionProperties properties, EventHandler<MessageEventArgs>? messageEventHandler, EventHandler<SessionEventArgs>? sessionEventHandler)
    {
        _logger = logger;
        _tokenManager = tokenManager;

        _session = context.CreateSession(properties, messageEventHandler, OnSessionEventAsync);
    }

    public SessionProperties Properties => _session.Properties;

    public IList<IFlow> Flows => _session.Flows;

    public ReturnCode ClearStats()
    {
        return _session.ClearStats();
    }

    public ReturnCode Connect()
    {
        return _session.Connect();
    }

    public IBrowser CreateBrowser(IEndpoint endpointToBrowse, BrowserProperties browserProperties)
    {
        return _session.CreateBrowser(endpointToBrowse, browserProperties);
    }

    public ICacheSession CreateCacheSession(CacheSessionProperties cacheSessionProperties)
    {
        return _session.CreateCacheSession(cacheSessionProperties);
    }

    public IDispatchTarget CreateDispatchTarget(ISubscription subscription, EventHandler<MessageEventArgs> messageCallback)
    {
        return _session.CreateDispatchTarget(subscription, messageCallback);
    }

    public IFlow CreateFlow(FlowProperties flowProperties, IEndpoint endPoint, ISubscription subscription, EventHandler<MessageEventArgs> messageEventHandler, EventHandler<FlowEventArgs> flowEventHandler)
    {
        return _session.CreateFlow(flowProperties, endPoint, subscription, messageEventHandler, flowEventHandler);
    }

    public IFlow CreateFlow(FlowProperties flowProperties, IEndpoint endPoint, ISubscription subscription, EventHandler<MessageEventArgs> messageEventHandler, EventHandler<FlowEventArgs> flowEventHandler, EndpointProperties endPointProperties)
    {
        return _session.CreateFlow(flowProperties, endPoint, subscription, messageEventHandler, flowEventHandler, endPointProperties);
    }

    public IMessage CreateMessage()
    {
        return _session.CreateMessage();
    }

    public ITopicEndpoint CreateNonDurableTopicEndpoint()
    {
        return _session.CreateNonDurableTopicEndpoint();
    }

    public ITopicEndpoint CreateNonDurableTopicEndpoint(string name)
    {
        return _session.CreateNonDurableTopicEndpoint(name);
    }

    public IQueue CreateTemporaryQueue()
    {
        return _session.CreateTemporaryQueue();
    }

    public IQueue CreateTemporaryQueue(string name)
    {
        return _session.CreateTemporaryQueue(name);
    }

    public ITopic CreateTemporaryTopic()
    {
        return _session.CreateTemporaryTopic();
    }

    public ITransactedSession CreateTransactedSession(TransactedSessionProperties props)
    {
        return _session.CreateTransactedSession(props);
    }

    public ReturnCode Deprovision(IEndpoint endpoint, int flags, object correlationKey)
    {
        return _session.Deprovision(endpoint, flags, correlationKey);
    }

    public ReturnCode Disconnect()
    {
        return _session.Disconnect();
    }


    public ICapability GetCapability(CapabilityType capabilityType)
    {
        return _session.GetCapability(capabilityType);
    }

    public IContext GetContext()
    {
        return _session.GetContext();
    }

    public object GetProperty(SessionProperties.PROPERTY sessionProperty)
    {
        return _session.GetProperty(sessionProperty);
    }

    public IDictionary<Stats_Rx, long> GetRxStats()
    {
        return _session.GetRxStats();
    }

    public IDictionary<Stats_Tx, long> GetTxStats()
    {
        return _session.GetTxStats();
    }

    public bool IsCapable(CapabilityType capabilityType)
    {
        return _session.IsCapable(capabilityType);
    }

    public ReturnCode ModifyClientInfo(SessionProperties.PROPERTY sessionProperty, object value, int flags, object correlationKey)
    {
        return _session.ModifyClientInfo(sessionProperty, value, flags, correlationKey);
    }

    public ReturnCode ModifyProperty(SessionProperties.PROPERTY sessionProperty, object value)
    {
        return _session.ModifyProperty(sessionProperty, value);
    }

    public ReturnCode Provision(IEndpoint endpoint, EndpointProperties props, int flags, object correlationKey)
    {
        return _session.Provision(endpoint, props, flags, correlationKey);
    }

    public ReturnCode Send(IMessage message)
    {
        return _session.Send(message);
    }

    public ReturnCode Send(IMessage[] messages, int offset, int length, out int messagesSent)
    {
        return _session.Send(messages, offset, length, out messagesSent);
    }

    public ReturnCode SendReply(IMessage messageToReplyTo, IMessage replyMessage)
    {
        return _session.SendReply(messageToReplyTo, replyMessage);
    }

    public ReturnCode SendRequest(IMessage requestMessage, out IMessage replyMessage, int timeoutInMsecs)
    {
        return _session.SendRequest(requestMessage, out replyMessage, timeoutInMsecs);
    }

    [Obsolete]
    public ReturnCode SetClientDescription(string clientDescription)
    {
        return _session.SetClientDescription(clientDescription);
    }

    public ReturnCode Subscribe(ISubscription subscription, bool waitForConfirm)
    {
        return _session.Subscribe(subscription, waitForConfirm);
    }

    public ReturnCode Subscribe(IEndpoint endpoint, ISubscription subscription, int subscribeFlags, object correlationKey)
    {
        return _session.Subscribe(endpoint, subscription, subscribeFlags, correlationKey);
    }

    public ReturnCode Subscribe(IDispatchTarget dispatchTarget, int flags, object correlationKey)
    {
        return _session.Subscribe(dispatchTarget, flags, correlationKey);
    }

    public ReturnCode Unsubscribe(ISubscription subscription, bool waitForConfirm)
    {
        return _session.Unsubscribe(subscription, waitForConfirm);
    }

    public ReturnCode Unsubscribe(IEndpoint endpoint, ISubscription subscription, int subscribeFlags, object correlationKey)
    {
        return _session.Unsubscribe(endpoint, subscription, subscribeFlags, correlationKey);
    }

    [Obsolete]
    public ReturnCode Unsubscribe(ITopicEndpoint dte, int correlationId)
    {
        return _session.Unsubscribe(dte, correlationId);
    }

    public ReturnCode Unsubscribe(ITopicEndpoint dte, object correlationKey)
    {
        return _session.Unsubscribe(dte, correlationKey);
    }

    public ReturnCode Unsubscribe(IDispatchTarget dispatchTarget, int flags, object correlationKey)
    {
        return _session.Unsubscribe(dispatchTarget, flags, correlationKey);
    }

    public ReturnCode ValidateTopic(string topicName)
    {
        return _session.ValidateTopic(topicName);
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    // Async void is intentially used as this function is an event handler.
    private async void OnSessionEventAsync(object? sender, SessionEventArgs args)
    {
        if (args.Event != SessionEvent.Reconnecting)
        {
            return;
        }

        if (!_tokenManager.IsUsingOAuth)
        {
            _logger.LogInformation("Reconnection in progress not using oauth");
            return;
        }

        _logger.LogInformation("Reconnection in progress, renewing oauth token for session.");

        var token = await _tokenManager.AcquireAsync();
        ModifyProperty(SessionProperties.PROPERTY.OAuth2AccessToken, token);
    }
}

