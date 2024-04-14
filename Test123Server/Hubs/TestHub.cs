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

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            numberOfClients--;

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendChatMessage(string user, string message)
        {
            Console.WriteLine("received chat message");
            await Clients.All.SendAsync("ReceiveChatMessage", user, message);
        }
    }
}
