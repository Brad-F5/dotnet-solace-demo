using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolaceNEMS.Messaging;
using SolaceNEMS.Messaging.Internal;
using SolaceSystems.Solclient.Messaging;
using SolaceSystems.Solclient.Messaging.SDT;
using System.Text;

namespace Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublisherController : ControllerBase
{
    private readonly ILogger<PublisherController> _logger;
    private readonly ISessionManager _sessions;
    private readonly ISolaceContextFactory _contextFactory;

    public PublisherController(ILogger<PublisherController> logger, ISessionManager sessions, ISolaceContextFactory contextFactory)
    {
        _logger = logger;
        _sessions = sessions;
        _contextFactory = contextFactory;
    }

    [HttpPost]
    public async Task<IActionResult> PostMessage()
    {   
        using var session = await _sessions.ConnectAsync();

        using var message = _contextFactory.CreateMessage();
        message.Destination = _contextFactory.CreateTopic("test/dotnet/demo");

        var payload = """{ "payload": "message" }""";
        SDTUtils.SetText(message, payload);
        
        var properties = message.CreateUserPropertyMap();
        properties.AddString("source", "source");
        properties.AddString("id", "id");
        properties.AddString("type", "type");
        properties.AddString("datacontenttype", "datacontenttype");
        properties.AddString("subject", "subject");
        properties.AddString("time", "time");

        var response = session.Send(message);
        if (response == ReturnCode.SOLCLIENT_OK)
        {
            _logger.LogInformation("Success!");
            return Created();
        }
        else
        {
            _logger.LogError("Failed: {}", response);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}