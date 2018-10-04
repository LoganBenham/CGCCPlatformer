using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.Helpers.Graphics
{
    public static partial class Gfx
    {
        public static class Fonts
        {
            public static SpriteFont SmallFont => ComicJames16;
            public static SpriteFont MediumFont => ComicJames36;
            public static SpriteFont LargeFont => ComicJames72;

            public static SpriteFont ComicJames16 { get; private set; }
            public static SpriteFont ComicJames36 { get; private set; }
            public static SpriteFont ComicJames72 { get; private set; }

            public static void LoadFontContent(ContentManager content)
            {
                ComicJames16 = content.Load<SpriteFont>("Fonts/ComicJames16");
                ComicJames36 = content.Load<SpriteFont>("Fonts/ComicJames36");
                ComicJames72 = content.Load<SpriteFont>("Fonts/ComicJames72");
            }
        }
    }
}