using System.Numerics;
using Microsoft.AspNetCore.SignalR;

namespace Test123Server.Hubs
{
    public class TestHub : Hub
    {
        private static int numberOfClients = 0; // bro how does this work why have to be static

        public override Task OnConnectedAsync()
        {
            Interlocked.Increment(ref numberOfClients);

            Console.WriteLine(numberOfClients + " clients now");
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveName", "dude #" + numberOfClients.ToString());
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveChatMessage", "server", "welcome");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Interlocked.Decrement(ref numberOfClients);
            Console.WriteLine(numberOfClients + " clients now");

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendChatMessage(string user, string message)
        {
            Console.WriteLine("received chat message");
            await Clients.All.SendAsync("ReceiveChatMessage", user, message);
        }

        public async Task SendBoxPosition(float x, float y)
        {
            await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveBoxPosition", x, y);
        }
    }
}
