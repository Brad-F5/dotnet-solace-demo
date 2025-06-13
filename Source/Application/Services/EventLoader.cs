using System.Text.Json;
using SolaceSystems.Solclient.Messaging;
using SolaceSystems.Solclient.Messaging.SDT;

namespace Application.Services;

public class EventLoader
{
    private readonly ILogger<EventLoader> _logger;

    public EventLoader(ILogger<EventLoader> logger)
    {
        _logger = logger;
    }

    public EventProcessResult ProcessEvent(IMessage message)
    {
        try
        {
            var userProperties = message.UserPropertyMap;
            if (userProperties is null)
            {
                return EventProcessResult.Rejected;
            }

            var source = userProperties.GetString("source") ?? throw new ArgumentNullException("source", "Invalid message, the property 'source' cannot be NULL.");
            var id = userProperties.GetString("id") ?? throw new ArgumentNullException("id", "Invalid message, the property 'id' cannot be NULL.");
            var type = userProperties.GetString("type") ?? throw new ArgumentNullException("type", "Invalid message, the property 'type' cannot be NULL.");
            var datacontenttype = userProperties.GetString("datacontenttype") ?? throw new ArgumentNullException("datacontenttype", "Invalid message, the property 'datacontenttype' cannot be NULL.");
            var subject = userProperties.GetString("subject") ?? throw new ArgumentNullException("subject", "Invalid message, the property 'subject' cannot be NULL.");
            var time = userProperties.GetString("time") ?? throw new ArgumentNullException("time", "Invalid message, the property 'time' cannot be NULL.");

            ArgumentNullException.ThrowIfNull(message.BinaryAttachment, "Message has no payload");
            var payload = SDTUtils.GetText(message);

            _ = JsonDocument.Parse(payload);

            _logger.LogInformation("Message topic: {}", message.Destination.Name);
            _logger.LogInformation("Message content: {}", payload);
            _logger.LogInformation("Header source: {}", source);

            return EventProcessResult.Success;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Message '{}' is invalid", message.ReplicationGroupMessageId);
            return EventProcessResult.Rejected;
        }
        catch (FieldNotFoundException ex)
        {
            _logger.LogError(ex, "Message '{}' is invalid, missing mandatory property", message.ReplicationGroupMessageId);
            return EventProcessResult.Rejected;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Message '{}' is invalid JSON format", message.ReplicationGroupMessageId);
            return EventProcessResult.Rejected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Message '{}' is invalid", message.ReplicationGroupMessageId);
            return EventProcessResult.Failed;
        }
    }
}
