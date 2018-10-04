using System.Collections.ObjectModel;
using System.Linq;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.UI.Common.Buttons
{
    public class MultiLineTextButton : Button
    {
        public override Rectangle DrawRect
        {
            get { return drawRect; }
            set
            {
                drawRect = value;
                text.Bounds = value.PadInY(-2, -5);
            }
        }

        public ObservableCollection<IDrawableText> Lines => text.Lines;

        public SpriteFont Font
        {
            get { return text.Font; }
            set { text.Font = value; }
        }

        private readonly TextBox text;
        private Rectangle drawRect;

        public MultiLineTextButton(string name, params IDrawableText[] lines) : base(name)
        {
            text = new TextBox(Rectangle.Empty, TextBox.XAlignType.Center, TextBox.YAlignType.Center)
            {
                Lines = new ObservableCollection<IDrawableText>(lines)
            };
        }

        public MultiLineTextButton(string name, params string[] lines)
            : this(name, lines.Select(text => (IDrawableText) new PlainText(text)).ToArray())
        {}

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            text.Draw(gameTime, mousePos);

            base.Draw(gameTime, mousePos);
        }
    }
}
