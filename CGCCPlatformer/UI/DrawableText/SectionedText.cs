using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.UI.DrawableText
{
    public struct SectionedText : IDrawableText
    {
        public IDrawableText[] Sections { get; }
        public string Text { get; }

        public SectionedText(params IDrawableText[] sections)
        {
            Sections = sections;
            var t = new StringBuilder();
            foreach (var section in sections)
                t.Append(section.Text);
            Text = t.ToString();
        }

        public Vector2 Size(SpriteFont font)
        {
            float h = 0, w = 0;
            foreach (var section in Sections)
            {
                var size = section.Size(font);
                w += size.X;
                if (size.Y > h)
                    h = size.Y;
            }
            return new Vector2(w, h);
        }

        public void ResetHover()
        {
            foreach (var section in Sections)
                section.ResetHover();
        }

        public void Draw(SpriteFont font, Vector2 pos, float scale, bool hover = false)
        {
            float x = pos.X;
            foreach (var section in Sections)
            {
                section.Draw(font, new Vector2(x, pos.Y), scale, hover);
                x += section.Size(font).X * scale;
            }
        }

        public override string ToString() => Text;

        public static SectionedText operator +(SectionedText thisText, IDrawableText other)
        {
            if (other is SectionedText)
            {
                var otherSectioned = (SectionedText)other;
                var arr = new IDrawableText[thisText.Sections.Length + otherSectioned.Sections.Length];
                for (var i = 0; i < thisText.Sections.Length; i++)
                    arr[i] = thisText.Sections[i];
                for (var i = 0; i < otherSectioned.Sections.Length; i++)
                    arr[i + thisText.Sections.Length] = otherSectioned.Sections[i];
                return new SectionedText(arr);
            }

            var arr2 = new IDrawableText[thisText.Sections.Length + 1];
            for (var i = 0; i < thisText.Sections.Length; i++)
                arr2[i] = thisText.Sections[i];
            arr2[arr2.Length - 1] = other;
            return new SectionedText(arr2);
        }
    }
}