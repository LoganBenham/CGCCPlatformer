using CGCCPlatformer.Helpers;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CGCCPlatformer.UI.Common.Buttons
{
    public class TextButton : Button
    {
        public override Rectangle DrawRect
        {
            get { return drawRect; }
            set
            {
                drawRect = value;
                SetDrawPoint();
            }
        }

        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                SetDrawPoint();
            }
        }

        public IDrawableText Text { get; set; }
        public Rectangle FilledBound => new Rectangle(textPoint, Text.Size(Font).ToPoint());

        private Rectangle drawRect;
        private SpriteFont font;
        private Point textPoint;

        public TextButton(string text) : base(text)
        {
            Text = new ColorText(text, Color.White);
        }

        public TextButton(IDrawableText text) : base(text.Text)
        {
            Text = text;
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            Text.Draw(Font, textPoint.ToVector2(), 1,
                HitBox.Contains(mousePos));

            base.Draw(gameTime, mousePos);
        }

        private void SetDrawPoint()
        {
            if (DrawRect == Rectangle.Empty)
                return;
            textPoint = DrawRect.Center - (Text.Size(Font) / 2).ToPoint();
            if (textPoint.X < DrawRect.X || textPoint.Y < DrawRect.Y)
                Logging.WriteLine(Logging.Level.Warning, Text + " does not fit in TextButton with font: " + Font);
        }
    }
}
