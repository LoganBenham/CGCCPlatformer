using System;
using System.Diagnostics;
using System.Threading;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI.Common;
using CGCCPlatformer.UI.Common.Buttons;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CGCCPlatformer.UI.Screens
{
    public sealed class NewGame : Menu
    {
        private readonly TextBox titleBox;

        private Rectangle mainArea;
        private readonly TextButton startButton;
        private readonly TextButton backButton;
        
        public override Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                titleBox.Bounds = bounds.PadInY(5, 0);

                var bottomHeight = (int)Gfx.Fonts.MediumFont.MeasureString("|").Y + 20;
                
                startButton.Bounds = new Rectangle(bounds.Center.X - startButton.FilledBound.Width / 2,
                    bounds.Bottom - bottomHeight,
                    startButton.FilledBound.Width, bottomHeight);
                startButton.Bounds = startButton.FilledBound.PadIn(-6);

                mainArea = new Rectangle(bounds.X + 5 + (bounds.Width - 10) / 4, titleBox.FilledBound.Bottom + 5,
                    (bounds.Width - 10) / 2, startButton.Bounds.Y - titleBox.FilledBound.Bottom - 10);
                var backBounds = new Rectangle(mainArea.Right - backButton.FilledBound.Width - 6,
                    startButton.Bounds.Y + 6,
                    backButton.FilledBound.Width + 6, backButton.FilledBound.Height);
                backButton.Bounds = backBounds.PadIn(-5);


            }
        }

        public NewGame()
        {
            titleBox = new TextBox(bounds, TextBox.XAlignType.Center);
            titleBox.Lines.Add(new PlainText("New Game"));

            startButton = new TextButton(new ColorText("Start", Gfx.DefaultTextColor, Gfx.DefaultTextHoverColor))
            {
                BorderColor = Gfx.DefaultTextColor,
                BorderHoverColor = Gfx.DefaultTextHoverColor,
                BorderThickness = 4
            };
            //startButton.Click += StartCreate;

            backButton = new TextButton(new ColorText("Back", Color.Red, Color.DarkRed))
            {
                BorderColor = Color.Red,
                BorderHoverColor = Color.DarkRed,
            };
            backButton.Click += () => Game.SetState(TheGame.GameState.MainMenu);
        }

        public override void Update(GameTime gameTime, Input input)
        {
            backButton.Update(gameTime, input);
            
            if (input.KeyPress(Keys.Back))
                Game.SetState(TheGame.GameState.MainMenu);
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            titleBox.Draw(gameTime, -1);
            Drawing.DrawHorizontalLine(titleBox.FilledBound.Bottom, Color.Gray, Bounds);

            mainArea.Draw(Color.Gray, Bounds);
            

            startButton.Draw(gameTime, mousePos);
            backButton.Draw(gameTime, mousePos);
        }

        public override void LoadContent()
        {
            titleBox.Font = Gfx.Fonts.LargeFont;
            startButton.Font = Gfx.Fonts.MediumFont;
            backButton.Font = Gfx.Fonts.MediumFont;

            Options = new []
            {
                //habitabilityBox.Opt(),
                startButton.Option,
                backButton.Option
            };
        }
    }
}