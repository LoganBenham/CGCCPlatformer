using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.UI.DrawableText
{
    public struct PlainText : IDrawableText
    {
        public string Text { get; }

        public PlainText(string text)
        {
            Text = text;
        }

        public void Draw(SpriteFont font, Vector2 pos, float scale = 1, bool hover = false)
        {
            Gfx.SpriteBatch.DrawString(font, Text, pos, Gfx.DefaultTextColor,
                0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public void ResetHover()
        {
        }

        public Vector2 Size(SpriteFont font)
        {
            return font.MeasureString(Text);
        }

        public override string ToString() => Text;

        public static SectionedText operator +(PlainText thisText, IDrawableText other)
        {
            if (other is SectionedText)
            {
                var otherSectioned = (SectionedText)other;
                var arr = new IDrawableText[otherSectioned.Sections.Length];
                arr[0] = thisText;
                for (var i = 0; i < otherSectioned.Sections.Length; i++)
                    arr[i + 1] = otherSectioned.Sections[i];
                return new SectionedText(arr);
            }

            return new SectionedText(thisText, other);
        }
    }
}