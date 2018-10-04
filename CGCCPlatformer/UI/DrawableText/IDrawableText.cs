using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.UI.DrawableText
{
    public static class DrawableText
    {
        public const int HoverFrames = 60;
        public const int TotalFrames = 52;
    }

    public interface IDrawableText
    {
        string Text { get; }
        void Draw(SpriteFont font, Vector2 pos, float scale, bool hover = false);

        Vector2 Size(SpriteFont font);

        void ResetHover();
    }
}