using SolaceSystems.Solclient.Messaging;

namespace SolaceNEMS.Messaging;

public interface ISolaceContextFactory
    : IDisposable
{
    public IContext CreateContext();
    public IMessage CreateMessage();
    public IQueue CreateQueue(string name);
    public ITopic CreateTopic(string name);
}