using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace Test123.Game.Drawables
{
    public partial class ChatBoxThing : Container
    {
        private FillFlowContainer textContainer;

        public ChatBoxThing()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new BasicScrollContainer
            {
                Height = 300,
                Width = 500,
                Child = textContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical
                }
            };
        }

        public void AddMessage(string message)
        {
            var text = new SpriteText
            {
                Text = message
            };

            textContainer.Add(text);
        }
    }
}
