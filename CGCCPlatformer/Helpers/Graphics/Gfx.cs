using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CGCCPlatformer.Helpers.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.Helpers.Graphics
{
    public static partial class Gfx
    {
        public static Color DefaultTextColor => CgccGreen;
        public static Color BackgroundColor => Color.White;
        public static Color DefaultTextHoverColor { get; } = Color.Black;
        
        public static bool EssentialsLoaded { get; private set; }
        public static bool Loaded { get; private set; }

        private static Stopwatch timer;

        public static SpriteBatch SpriteBatch { get; private set; }
        private static RasterizerState ScissorRasterizerState { get; set; }
        public static SamplerState SamplerState { get; set; }
        
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Circle { get; private set; }
        public static Texture2D CodingClubLogo { get; private set; }
        public static Texture2D CgccLogo { get; private set; }

        public static Color CgccGreen { get; } = new Color(0, 140, 149);
        public static Color CodingClubGreen { get; } = new Color(3, 205, 205);
        public static Color CodingClubMagenta { get; } = new Color(179, 13, 217);
        public static Color CodingClubPurple { get; } = new Color(80, 37, 241);

        private static bool CropOn { get; set; }
        private static Stack<Rectangle> Croppers { get; set; }

        public static void BeginSpriteBatch()
        {
            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState,
                rasterizerState: CropOn ? ScissorRasterizerState : null);
        }

        public static void RestartSpriteBatch()
        {
            SpriteBatch.End();
            BeginSpriteBatch();
        }

        public static void SetSpriteBatchCrop(Rectangle bounds)
        {
            if (CropOn) //In case this is called from a nested function
                Croppers.Push(SpriteBatch.GraphicsDevice.ScissorRectangle);
            CropOn = true;
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.ScissorRectangle
                = bounds.CutBy(SpriteBatch.GraphicsDevice.ScissorRectangle);
            BeginSpriteBatch();
        }

        public static void EndSpriteBatchCrop()
        {
            SpriteBatch.End();
            CropOn = Croppers.Any();
            SpriteBatch.GraphicsDevice.ScissorRectangle = 
                Croppers.Any() ? Croppers.Pop() : TheGame.Bounds;
            BeginSpriteBatch();
        }

        public static void SetSpriteBatch(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Croppers = new Stack<Rectangle>();
            CropOn = false;
        }

        public static void LoadEssentials(ContentManager content)
        {
            if (EssentialsLoaded)
                return;
            
            Fonts.LoadFontContent(content);
            Cursors.LoadCursorContent(content);
            Pixel = content.Load<Texture2D>("Images/pixel");

            ScissorRasterizerState = new RasterizerState { ScissorTestEnable = true };
            SamplerState = SamplerState.AnisotropicClamp;

            EssentialsLoaded = true;
        }

        public static void Load(ContentManager content)
        {
            if (Loaded)
                return;

            timer = new Stopwatch();
            
            Circle = content.LoadTimed<Texture2D>("Images/fullcircle");
            CodingClubLogo = content.LoadTimed<Texture2D>("Images/codingclub");
            CgccLogo = content.LoadTimed<Texture2D>("Images/cgcclogo");
            
            if (i != TotalItems)
                throw new InvalidDataException("Please Update TotalItems to " + i);
            Loaded = true;

            timer.Stop();
        }

        public static void Dispose()
        {
            Pixel.Dispose();
            Circle.Dispose();
        }

        private static int i = 0;
        private const int TotalItems = 3;

        public static T LoadTimed<T>(this ContentManager content, string path)
        {
            i++;
            TheGame.Game.LoadingScreen.SetTaskMessage("Loading " + path.Substring(7) + "  (" + i + "/" + TotalItems + ")", false);
            timer.Restart();
            var result = content.Load<T>(path);
            TheGame.Game.LoadingScreen.SetProgress((float)i / TotalItems);
            TheGame.Game.LoadingScreen.SetTaskMessage("Done loading " + path.Substring(7) + "  (" + i + "/" + TotalItems + ")", false);
            if (timer.ElapsedMilliseconds > 10)
                Logging.WriteLine("Loaded " + path + " in " + timer.ElapsedMilliseconds + "ms", 0);
            return result;
        }
    }
}