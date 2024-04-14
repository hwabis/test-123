using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Test123.Game.Drawables
{
    public partial class MovingSpinningBox : AwesomeKeyBindingContainer, IKeyBindingHandler<InputAction>
    {
        private int xDirection = 0;
        private int yDirection = 0;

        public MovingSpinningBox()
        {
            RelativeSizeAxes = Axes.None;
            AutoSizeAxes = Axes.Both; // uhh is there a reason why relative size was on before
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new SpinningBox
            {
                Anchor = Anchor.Centre
            };
        }

        public bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.Down:
                    yDirection = 1;
                    break;
                case InputAction.Up:
                    yDirection = -1;
                    break;
                case InputAction.Left:
                    xDirection = -1;
                    break;
                case InputAction.Right:
                    xDirection = 1;
                    break;
            }

            return true;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.Down:
                case InputAction.Up:
                    yDirection = 0;
                    break;
                case InputAction.Left:
                case InputAction.Right:
                    xDirection = 0;
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            const double speed = 1;
            Position += new Vector2(
                (float)(xDirection * speed * Time.Elapsed), (float)(yDirection * speed * Time.Elapsed));
        }
    }

    public partial class AwesomeKeyBindingContainer : KeyBindingContainer<InputAction>
    {
        public AwesomeKeyBindingContainer(
            KeyCombinationMatchingMode keyCombinationMatchingMode = KeyCombinationMatchingMode.Any,
            SimultaneousBindingMode simultaneousBindingMode = SimultaneousBindingMode.All)
            : base(simultaneousBindingMode, keyCombinationMatchingMode)
        {
        }

        public override IEnumerable<IKeyBinding> DefaultKeyBindings => new[]
        {
            new KeyBinding(new[] { InputKey.Left }, InputAction.Left),
            new KeyBinding(new[] { InputKey.Right }, InputAction.Right),
            new KeyBinding(new[] { InputKey.Up }, InputAction.Up),
            new KeyBinding(new[] { InputKey.Down }, InputAction.Down),
        };
    }

    public enum InputAction
    {
        Left,
        Right,
        Up,
        Down
    }
}
