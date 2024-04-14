using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK.Graphics;
using Test123.Game.Drawables;

namespace Test123.Game
{ // this is the biggest abomination i have ever made
    public partial class MainScreen : Screen
    {
        private HubConnection hubConnection;
        private bool connected = false; // idc i just want this to work now
        private ChatBoxThing chatBox;
        private SendMessageBoxThing sendMessageBox;
        private MovingSpinningBox movingSpinningBox;
        private TextBox ipTextBox;

        private double timeOfLastTimeSentBoxPosition;

        private string nameOfUserSendingTheMessages = "???";
        private bool isSpinningBoxOwner = false;

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
                        sendMessageBox = new SendMessageBoxThing(() =>
                        {
                            if (hubConnection.State == HubConnectionState.Connected)
                            {
                                hubConnection.InvokeAsync("SendChatMessage", nameOfUserSendingTheMessages, sendMessageBox.Text);
                            }

                            if (sendMessageBox.Text == "SUPER")
                            {
                                isSpinningBoxOwner = true;
                            }
                        })
                    }
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        ipTextBox = new BasicTextBox
                        {
                            Height = 50,
                            Width = 500,
                            PlaceholderText = "ip"
                        },
                        new BasicButton
                        {
                            Height = 50,
                            Width = 200,
                            Text = "connect",
                            Action = async () =>
                            {
                                await connectToServer();
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            _ = connectToServer(); // im so confused

            timeOfLastTimeSentBoxPosition = Time.Current;
        }

        protected override void Update()
        {
            base.Update();

            // send the box position every few milliseconds lol
            // is signalr even supposed to be used like this??? https://github.com/dotnet/aspnetcore/issues/41343
            double current_time = Time.Current;

            if (isSpinningBoxOwner && current_time - timeOfLastTimeSentBoxPosition > 17)
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    sendBoxPosition(); // i guess we don't await
                }

                timeOfLastTimeSentBoxPosition = current_time;
            }
        }

        private async Task connectToServer()
        {
            if (hubConnection != null)
            {
                connected = false;
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                Logger.Log("Disconnected previous connection");
            }

            string url = ipTextBox.Text == "" ? "https://localhost:7287/testHub" : ipTextBox.Text;

            hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build(); // if this crashes because of wrong url then it crashes lol

            hubConnection.Closed += async (error) =>
            {
                Logger.Log("connection closed");
                await hubConnection.StartAsync();
            };

            hubConnection.On("ReceiveChatMessage", (string user, string message) =>
            {
                Schedule(() =>
                {
                    string newMessage = $"{user}: {message}";
                    chatBox.AddMessage(newMessage);
                });
            });

            hubConnection.On("ReceiveName", (string username) =>
            {
                Schedule(() =>
                {
                    chatBox.AddMessage("YOUR NAME IS " + username);
                    nameOfUserSendingTheMessages = username;
                });
            });

            hubConnection.On("ReceiveBoxPosition", (float x, float y) =>
            {
                Schedule(() =>
                {
                    movingSpinningBox.Position = new osuTK.Vector2(x, y);
                });
            });

            try
            {
                Logger.Log("trying to connect");
                await hubConnection.StartAsync();

                Logger.Log("connected");
                connected = true;
            }
            catch
            {
                Logger.Log("failed to connect");
            }
        }

        private async void sendBoxPosition()
        {
            // i tried sending System.Numerics.Vector2 but server kept getting 0,0 so i guess that doesn't work
            if (connected)
                await hubConnection.InvokeAsync("SendBoxPosition", movingSpinningBox.Position.X, movingSpinningBox.Position.Y);
        }
    }
}
