using System;
using System.Text;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.UI.DrawableText
{
    public struct MulticolorText : IDrawableText
    {
        public string[] Texts { get; }
        public string Text { get; }
        public Color[] Colors { get; }
        public Color[] HoverColors { get; }
        private int hovered;

        public MulticolorText(string[] texts, Color[] colors, Color[] hoverColors)
        {
            Texts = texts;
            var t = new StringBuilder();
            foreach (string text in texts)
                t.Append(text);
            Text = t.ToString();
            Colors = colors;
            HoverColors = hoverColors;
            if (texts.Length != colors.Length || texts.Length != hoverColors.Length)
                throw new ArgumentException("Must be one color and hover color for every string");
            hovered = 0;
        }

        public MulticolorText(string[] texts, Color[] colors, Color hoverColor)
        {
            Texts = texts;
            var t = new StringBuilder();
            foreach (string text in texts)
                t.Append(text);
            Text = t.ToString();
            Colors = colors;
            HoverColors = new Color[Colors.Length];
            for (var i = 0; i < Colors.Length; i++)
                HoverColors[i] = hoverColor;
            if (texts.Length != colors.Length)
                throw new ArgumentException("Must be one color and hover color for every string");
            hovered = 0;
        }

        public MulticolorText(string[] texts, Color[] colors)
        {
            Texts = texts;
            var t = new StringBuilder();
            foreach (string text in texts)
                t.Append(text);
            Text = t.ToString();
            Colors = colors;
            HoverColors = colors;
            if (texts.Length != colors.Length)
                throw new ArgumentException("Must be one color for every string");
            hovered = 0;
        }

        public MulticolorText(params ColorText[] texts)
        {
            int len = texts.Length;
            Texts = new string[len];
            Colors = new Color[len];
            HoverColors = new Color[len];

            var t = new StringBuilder();
            for (var i = 0; i < len; i++)
            {
                Texts[i] = texts[i].Text;
                t.Append(texts[i].Text);
                Colors[i] = texts[i].Color;
                HoverColors[i] = texts[i].HoverColor;
            }
            Text = t.ToString();
            hovered = 0;
        }

        public void Draw(SpriteFont font, Vector2 pos, float scale = 1, bool hover = false)
        {
            float val = 1;
            if (hover)
            {
                hovered += 1;
                if (hovered > DrawableText.HoverFrames)
                    hover = false;
                if (hovered > DrawableText.TotalFrames)
                    hovered = 0;

                
                if (hover)
                {
                    hovered += 1;
                    if (hovered > DrawableText.TotalFrames)
                        hovered = 0;
                    var relative = (float)hovered / DrawableText.TotalFrames;
                    val = (float)(Math.Cos(relative * Math.PI * 2) + 1) / 2;
                }
            }

            for (var i = 0; i < Texts.Length; i++)
            {
                var color = Colors[i];
                color = HoverColors[i].Interpolate(color, val);
                string text = Texts[i];
                Gfx.SpriteBatch.DrawString(font, text, pos, color,
                    0, Vector2.Zero, scale, SpriteEffects.None, 0);
                pos = new Vector2(pos.X + font.MeasureString(text).X * scale, pos.Y);
            }
        }

        public void ResetHover() => hovered = 0;

        public Vector2 Size(SpriteFont font)
        {
            return font.MeasureString(Text);
        }

        public override string ToString() => Text;

        public static SectionedText operator +(MulticolorText thisText, IDrawableText other)
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