using System;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.UI.DrawableText
{
    public struct ColorText : IDrawableText
    {
        public string Text { get; }
        public Color Color { get; }
        public Color HoverColor { get; }
        private int hovered;

        public ColorText(string text, Color color, Color hoverColor)
        {
            Text = text;
            Color = color;
            HoverColor = hoverColor;
            hovered = 0;
        }

        public ColorText(string text, Color color)
            : this(text, color, color)
        {}

        public ColorText(string text)
            : this(text, Color.White)
        {}

        public void Draw(SpriteFont font, Vector2 pos, float scale = 1, bool hover = false)
        {
            Color color;

            if (hover)
            {
                //TODO consider moving this to the UiManager

                //new Rectangle(pos.ToPoint() - new Point(1, 0),
                //    Size(font).ToPoint() + new Point(2, 0))
                //    .Draw(Color, TotC.Game.Bounds);

                hovered += 1;
                //Logging.WriteLine("Hovered for " + hovered + " frames", 2);
                //if (hovered > DrawableText.HoverFrames)
                //    hover = false;
                if (hovered > DrawableText.TotalFrames)
                    hovered = 0;

                var relative = (float) hovered / DrawableText.TotalFrames;
                var val = (float)(Math.Cos(relative * Math.PI * 2) + 1) / 2;
                
                /*float val = 2 * relative;
                if (relative < 0.5f)
                {
                    val = 1 - val * val; //1 - (2x)^2
                }
                else
                {
                    val -= 2;
                    val = 1 - val * val; //1 - (2x - 2)^2
                }*/


                //Debug.WriteLine("relative=" + (Math.Round(100 * relative) / 100f) + "\tval=" + Math.Round(100 * val) / 100);
                color = Color.Interpolate(HoverColor, val);
            }
            else
                color = Color;

            Gfx.SpriteBatch.DrawString(font, Text, pos, color,
                0, Vector2.Zero, scale, SpriteEffects.None, 0);
            
        }

        public void ResetHover()
        {
            hovered = 0;
            //Logging.WriteLine("Reset hover on " + Text, 2);
        }

        public Vector2 Size(SpriteFont font)
        {
            return font.MeasureString(Text);
        }

        public override string ToString() => Text;

        public static SectionedText operator +(ColorText thisText, IDrawableText other)
        {
            if (other is SectionedText)
            {
                var otherSectioned = (SectionedText) other;
                var arr = new IDrawableText[otherSectioned.Sections.Length];
                arr[0] = thisText;
                for (var i = 0; i < otherSectioned.Sections.Length; i++)
                    arr[i + 1] = otherSectioned.Sections[i];
                return new SectionedText(arr);
            }

            /*if (other is ColorText)
            {
                return new MulticolorText(thisText, (ColorText) other);
            }*/

            return new SectionedText(thisText, other);
        }
    }
}