using System;
using Microsoft.AspNetCore.SignalR.Client;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;

namespace Test123.Game.Drawables
{
    public partial class SendMessageBoxThing : BasicTextBox
    {
        public string NameOfUserSendingTheMessages { get; set; }

        private HubConnection hubConnection;

        public SendMessageBoxThing(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;

            Height = 50;
            RelativeSizeAxes = Axes.X;

            ReleaseFocusOnCommit = false;
        }

        protected override void Commit()
        {
            try
            {
                sendChatMessage();
                Text = string.Empty;
            }
            catch (Exception)
            {
                Logger.Log("failed to send message");
            }

            base.Commit();
        }

        private async void sendChatMessage()
        {
            if (hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.InvokeAsync("SendChatMessage", NameOfUserSendingTheMessages, Text);
            }
        }
    }
}
