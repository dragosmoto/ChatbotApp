using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatbotApp.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            _connections[Context.ConnectionId] = "SomeUserIdentifier"; // Optionally associate with a user
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public static string GetConnectionId(string userIdentifier)
        {
            return _connections.FirstOrDefault(x => x.Value == userIdentifier).Key;
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"Received message from {user}: {message}"); // Add this for debugging
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
