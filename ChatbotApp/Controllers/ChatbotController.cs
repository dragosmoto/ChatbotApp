using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatbotApp.Hubs;  // Ensure you include the namespace where ChatHub is defined
using ChatbotApp.Services;
using System;

public class ChatbotController : Controller
{
    private readonly OpenAIService _openAIService;
    private readonly IHubContext<ChatHub> _hubContext;


    public ChatbotController(OpenAIService openAIService, IHubContext<ChatHub> hubContext)
    {
        _openAIService = openAIService;
        _hubContext = hubContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    public class ChatMessageInput
    {
        public string UserInput { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty; // Added connection ID property
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageInput input)
    {
        

        Console.WriteLine("SendMessage action hit with input: " + input.UserInput);

        //var connectionId = HttpContext.Connection.Id;
        var connectionId = ChatHub.GetConnectionId("SomeUserIdentifier");
        if (connectionId == null)
        {
            return BadRequest("No connection found for the specified user.");
        }

        // Use the _hubContext to send a message to the client
        //await _hubContext.Clients.All.SendAsync("ReceiveMessage", "user", input.UserInput);
        //await _hubContext.Clients.Client(connectionId).SendAsync("SendMessage", "user", input.UserInput);
        await _hubContext.Clients.Client(input.ConnectionId).SendAsync("ReceiveMessage", "user", input.UserInput);

        // Optionally, use your OpenAIService to process and respond to the message
        await _openAIService.GetChatResponseAsync( input.UserInput, connectionId);



        return Ok();
    }
}
