using System;
using System.Collections.Generic;
using System.Linq;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Written by Logan Benham

namespace CGCCPlatformer.UI.Common
{
    public sealed class FpsCounter : InteractiveWindow
    {
        public uint MaxSamples { get; set; } = 3;
        private readonly Queue<double> sampleBufferFps = new Queue<double>();
        private readonly Queue<double> sampleBufferUps = new Queue<double>();
        public double CurrentUps { get; private set; }
        public double AvgUps { get; private set; }
        public double CurrentFps { get; private set; }
        public double AvgFps { get; private set; }

        public Point Position { get; set; }
        public Vector2 Size { get; private set; }
        private SpriteFont Font => Gfx.Fonts.SmallFont;

        public bool Visible { get; private set; }

        public FpsCounter()
        {
            Visible = false;
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            CurrentFps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;

            sampleBufferFps.Enqueue(CurrentFps);

            if (sampleBufferFps.Count > MaxSamples)
            {
                sampleBufferFps.Dequeue();
                AvgFps = sampleBufferFps.Average();
            }
            else
            {
                AvgFps = CurrentFps;
            }

            if (!Visible)
                return;

            //string timeText = Math.Round(gameTime.TotalGameTime.TotalSeconds, 1).ToString();
            string fpsString = Math.Round(AvgFps) + " FPS"; // - " + timeText + " s";

            var color = Color.White;
            if (AvgFps < 5)
                color = Color.Red;
            else if (AvgFps < 20)
                color = Color.Orange;
            else if (AvgFps < 40)
                color = Color.Yellow;

            //Gfx.SpriteBatch.DrawString(Font, timeText,
            //        new Vector2(Bounds.X + 2, Bounds.Bottom - 2 * Height - 2), Color.White);
            new Rectangle(Position, Font.MeasureString(fpsString)
                .ToPoint() + new Point(4, 4)).DrawFilled(Color.Black * 0.5f);
            Gfx.SpriteBatch.DrawString(Font, fpsString,
                new Vector2(Position.X + 2, Position.Y + 2), color);

            /*if (Game.State == TotC.GameState.MainMenu || Game.EscMenuBool)
                Gfx.SpriteBatch.DrawString(Font, fpsString,
                    new Vector2(Game.Bounds.Right - Font.MeasureString(fpsString).X - 4, Game.Bounds.Y + 2), Color.White);
            else
                Gfx.SpriteBatch.DrawString(Font, fpsString,
                    new Vector2(Bounds.Right - Font.MeasureString(fpsString).X - 4, Bounds.Y + 2), Color.White);*/
        }

        public override void Update(GameTime gameTime, Input input)
        {
            CurrentUps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;

            sampleBufferUps.Enqueue(CurrentUps);

            if (sampleBufferUps.Count > MaxSamples)
            {
                sampleBufferUps.Dequeue();
                AvgUps = sampleBufferUps.Average();
            }
            else
            {
                AvgUps = CurrentUps;
            }

            if (input.KeyPress(Keys.F3))
                Visible = !Visible;
        }

        public override void LoadContent()
        {
            Size = Font.MeasureString("60 FPS") + new Vector2(4, 4);
            Position = new Point(Bounds.X, Bounds.Bottom - (int)Size.Y);
        }
    }
}