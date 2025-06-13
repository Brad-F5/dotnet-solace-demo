using SolaceSystems.Solclient.Messaging;

namespace SolaceNEMS.Messaging;

public interface ISessionManager
{
    public Task<ISession> ConnectAsync(EventHandler<MessageEventArgs>? messageEventHandler = null, EventHandler<SessionEventArgs>? sessionEventHandler = null);
}
