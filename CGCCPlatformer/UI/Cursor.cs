using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.UI
{
    public static class Cursor
    {
        public enum CursorType
        {
            Default,
            Hand,
            HorizontalAdjust,
            VerticalAdjust,
            OmniAdjust,
            Test
        }

        public static CursorType Type { get; set; }

        static Cursor()
        {
            TheGame.Game.IsMouseVisible = false;
            Reset();
        }

        public static void Reset() => Type = CursorType.Default;

        public static void Draw(Point mousePos)
        {
            if (!TheGame.Game.IsMouseVisible)
            {
                Texture2D texture;
                switch (Type)
                {
                    case CursorType.Default:
                        texture = Gfx.Cursors.Default;
                        break;
                    case CursorType.Hand:
                        texture = Gfx.Cursors.Hand;
                        break;
                    case CursorType.HorizontalAdjust:
                        texture = Gfx.Cursors.HorizontalAdjust;
                        break;
                    case CursorType.VerticalAdjust:
                        texture = Gfx.Cursors.VerticalAdjust;
                        break;
                    case CursorType.OmniAdjust:
                        texture = Gfx.Cursors.OmniAdjust;
                        break;
                    case CursorType.Test:
                        texture = Gfx.Cursors.Test;
                        break;
                    default:
                        texture = Gfx.Cursors.Default;
                        break;
                }
                var offset = new Point((texture.Width - 1) / 2, (texture.Height - 1) / 2);
                Gfx.SpriteBatch.Draw(texture, (mousePos - offset).ToVector2(), Color.White);
            }
                
            //Gfx.SpriteBatch.Draw(Gfx.Pixel, mousePos.ToVector2(), Color.HotPink); //Debug
        }
    }
}
