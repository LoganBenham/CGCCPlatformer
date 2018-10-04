using System;
using System.Diagnostics;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.UI.DrawableText
{
    public struct Icon : IDrawableText
    {
        public string Text => "";
        public Texture2D Texture { get; }
        public float Radius { get; }
        public Color Color { get; }
        public Color HoverColor { get; }
        public float Width { get; }
        private int hovered;

        public Icon(Texture2D texture, float radius, Color color, float width)
            : this(texture, radius, color, width, color) {}

        public Icon(Texture2D texture, float radius, Color color, float width, Color hoverColor)
        {
            Texture = texture;
            Radius = radius;
            Color = color;
            HoverColor = hoverColor;
            if (width < radius * 2)
                throw new ArgumentException("Width is too small. Must be at least 2x radius");
            Width = width;
            hovered = 0;
        }

        public Vector2 Size(SpriteFont font)
        {
            return new Vector2(Width + 2, Radius * 2);
        }

        public void ResetHover() => hovered = 0;

        public void Draw(SpriteFont font, Vector2 pos, float scale, bool hover = false)
        {
            var color = Color;
            if (hover)
            {
                hovered += 1;
                if (hovered > DrawableText.TotalFrames)
                    hovered = 0;
                var relative = (float)hovered / DrawableText.TotalFrames;
                var val = (float)(Math.Cos(relative * Math.PI * 2) + 1) / 2;
                color = HoverColor.Interpolate(Color, val);
            }

            float radius = Radius * scale; //maybe should be smarter than this

            float textHeight = font.MeasureString("ABC123").Y;
            if (radius * 2 > textHeight)
            {
                radius = textHeight / 2;
                Debug.WriteLine("Shrinking icon to fit");
            }
            Texture.Draw(radius, new Vector2(pos.X + Width / 2, pos.Y + textHeight / 2), color);
            //Draw(new Rectangle(pos.ToPoint(), new Point((int)Width, (int) textHeight)), Color.Red, new Rectangle(-1, -1, 9999, 9999));
        }

        public override string ToString() => "Icon";

        public static SectionedText operator +(Icon thisText, IDrawableText other)
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