using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;

namespace CGCCPlatformer.UI.Common.Buttons
{
    public abstract class Button : InteractiveWindow
    {
        public override Rectangle Bounds
        {
            get { return DrawRect; }
            set
            {
                DrawRect = value;
                HitBox = value;
                //Logging.WriteLine("Button bound set to " + value, 2);
            }
        }

        public Option Option { get; set; }
        public string Name => Option.Name;
        public virtual Rectangle DrawRect { get; set; }
        public Rectangle HitBox { get; set; }
        public event Utils.EventHandler Click;
        public event Utils.EventHandler Hover;
        
        public Color BorderColor { get; set; }
        public Color BorderHoverColor { get; set; }
        public int BorderThickness { get; set; }

        protected Button(string name)
        {
            Option = new Option(name, ClickFunc);
            BorderColor = new Color(128, 128, 128);
            BorderHoverColor = Color.White;
            BorderThickness = 1;
        }

        private void ClickFunc()
        {
            Click?.Invoke();
        }

        public override void Update(GameTime gameTime, Input input)
        {
            if (!HitBox.Contains(input.MousePos) || input.Hovered)
                return;
            
            if (input.LeftPress)
            {
                input.CaptureClick();
                Option?.Execute();
            }
            else if (input.MouseMovement != Point.Zero)
            {
                input.CaptureHover();
                Hover?.Invoke();
            }
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            var hovered = HitBox.Contains(mousePos);
            for (var i = 0; i < BorderThickness; i++)
                DrawRect.PadIn(i).Draw(hovered
                    ? BorderHoverColor
                    : BorderColor, Bounds);
        }
    }
}
