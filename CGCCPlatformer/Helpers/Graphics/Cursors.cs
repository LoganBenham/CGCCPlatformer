using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.Helpers.Graphics
{
    public static partial class Gfx
    {
        public static class Cursors
        {
            public static Texture2D Default { get; private set; }
            public static Texture2D Hand { get; private set; }
            public static Texture2D HorizontalAdjust { get; private set; }
            public static Texture2D VerticalAdjust { get; private set; }
            public static Texture2D OmniAdjust { get; private set; }
            public static Texture2D Test { get; private set; }

            public static void LoadCursorContent(ContentManager content)
            {
                Default = content.Load<Texture2D>("Cursors/Default");
                Hand = content.Load<Texture2D>("Cursors/Hand");
                HorizontalAdjust = content.Load<Texture2D>("Cursors/HorizontalAdjust");
                VerticalAdjust = content.Load<Texture2D>("Cursors/VerticalAdjust");
                OmniAdjust = content.Load<Texture2D>("Cursors/OmniAdjust");
                Test = content.Load<Texture2D>("Cursors/Test");
            }
        }
    }
}
