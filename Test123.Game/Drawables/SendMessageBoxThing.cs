using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace Test123.Game.Drawables
{
    public partial class SendMessageBoxThing : BasicTextBox
    {
        private Action sendAction;

        public SendMessageBoxThing(Action sendAction)
        {
            this.sendAction = sendAction;

            Height = 50;
            RelativeSizeAxes = Axes.X;

            ReleaseFocusOnCommit = false;
        }

        protected override void Commit()
        {
            sendAction.Invoke();
            Text = "";

            base.Commit();
        }
    }
}
