using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;

namespace CGCCPlatformer.UI.Common.Bar
{
    public class FillableBar : GameWindow
    {
        private int filled;

        public int Filled
        {
            get { return filled; }
            set {
                filled = value;
                BoundFilled();
            }
        }

        private bool vertical;
        public bool Vertical
        {
            get { return vertical; }
            set {
                vertical = value;
                BoundFilled();
            }
        }

        public bool Reversed { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BoundaryColor { get; set; }
        public Color FillColor { get; set; }
        
        public FillableBar(Rectangle bounds) : base(bounds)
        {
            BackgroundColor = Color.Black;
            BoundaryColor = new Color(128, 128, 128);
            FillColor = Color.RoyalBlue;
            filled = bounds.Width / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            Bounds.DrawFilled(BackgroundColor);
            Rectangle fillRect;
            if (Vertical)
            {
                fillRect = new Rectangle(Bounds.X + 1,
                    Reversed ? Bounds.Y + 1 : Bounds.Bottom - 1 - Filled,
                    Bounds.Width - 2, Filled);
            }
            else
                fillRect = new Rectangle(Reversed ? Bounds.Right - 1 - Filled : Bounds.X + 1,
                    Bounds.Y + 1, Filled, Bounds.Height - 2);
            fillRect.DrawFilled(FillColor);

            Bounds.Draw(BoundaryColor, TheGame.Bounds);
        }

        public override void Update(GameTime gameTime)
        {
        }

        private void BoundFilled()
        {
            filled = filled.Clamp(0, Vertical ? Bounds.Height - 2 : Bounds.Width - 2);
        }
    }
}
