using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK.Graphics;
using Test123.Game.Drawables;

namespace Test123.Game
{
    public partial class MainScreen : Screen
    {
        private HubConnection hubConnection;
        private ChatBoxThing chatBox;
        private SendMessageBoxThing sendMessageBox;

        public MainScreen()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7287/testHub")
                .Build();

            hubConnection.Closed += async (error) =>
            {
                Logger.Log("connection closed");
                await hubConnection.StartAsync();
            };

            hubConnection.On<string, string>("ReceiveChatMessage", (user, message) =>
            {
                Schedule(() =>
                {
                    string newMessage = $"{user}: {message}";
                    chatBox.AddMessage(newMessage);
                });
            });

            hubConnection.On<string>("ReceiveName", (username) =>
            {
                Schedule(() =>
                {
                    chatBox.AddMessage("YOUR NAME IS " + username);
                    sendMessageBox.NameOfUserSendingTheMessages = username;
                });
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Violet,
                    RelativeSizeAxes = Axes.Both,
                },
                new MovingSpinningBox
                {
                    Anchor = Anchor.Centre,
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        chatBox = new ChatBoxThing(),
                        sendMessageBox = new SendMessageBoxThing(hubConnection)
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Task.Run(async () =>
            {
                await connectToServer(hubConnection);
            });

            chatBox.AddMessage("welcom");
        }

        private async Task connectToServer(HubConnection connection)
        {
            while (connection.State != HubConnectionState.Connected)
            {
                try
                {
                    Logger.Log("trying to connect");
                    await connection.StartAsync();
                }
                catch
                {
                    Logger.Log("failed to connect");
                    await Task.Delay(1000);
                }
            }

            Logger.Log("connected");
        }
    }
}
