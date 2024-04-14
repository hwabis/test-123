using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
namespace Test123.Game.Drawables
{
    public partial class SpinningBox : CompositeDrawable
    {
        private Container box;

        private Random random;

        public SpinningBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;

            random = new Random();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = box = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Width = 100,
                        Height = 100,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = new Colour4(random.NextSingle(), random.NextSingle(), random.NextSingle(), 1)
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            box.Loop(b => b.RotateTo(0).RotateTo(360, 10000));
        }
    }
}
