using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
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
        private MovingSpinningBox movingSpinningBox;

        private double timeOfLastTimeSentBoxPosition;

        private bool isSpinningBoxOwner => sendMessageBox.NameOfUserSendingTheMessages == "dude #1"; // i've made an absolute mess LOL

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

            hubConnection.On<float, float>("ReceiveBoxPosition", (x, y) =>
            {
                Schedule(() =>
                {
                    movingSpinningBox.Position = new osuTK.Vector2(x, y);
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
                movingSpinningBox = new MovingSpinningBox
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

            timeOfLastTimeSentBoxPosition = Time.Current;
        }

        protected override void Update()
        {
            base.Update();

            // send the box position every few milliseconds lol
            double current_time = Time.Current;

            if (isSpinningBoxOwner && current_time - timeOfLastTimeSentBoxPosition > 33)
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    sendBoxPosition(); // i guess we don't await
                }

                timeOfLastTimeSentBoxPosition = current_time;
            }
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

        private async void sendBoxPosition()
        {
            // i tried sending System.Numerics.Vector2 but server kept getting 0,0 so i guess that doesn't work
            await hubConnection.InvokeAsync("SendBoxPosition", movingSpinningBox.Position.X, movingSpinningBox.Position.Y);
        }
    }
}
