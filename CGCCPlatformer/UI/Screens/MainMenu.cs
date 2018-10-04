using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.ExternalUtils;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI.Common;
using CGCCPlatformer.UI.Common.Bar;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CGCCPlatformer.UI.Screens
{
    public sealed class MainMenu : Menu
    {
        private const float ColorInterval = 500;

        public enum MainMenuState
        {
            Main,
            About,
            Help
        }

        private readonly Option[] aboutOptions;

        private readonly List<Option> mainOptions;
        private readonly TextBox optionsBox;

        private Rectangle escReminderRect;
        private readonly TextBox titleBox;
        
        private MainMenuState state;

        public override Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                int middle = value.Y + value.Height / 2;

                if (!Gfx.Loaded)
                    return;

                optionsBox.Bounds = new Rectangle(value.X, middle, value.Width, value.Bottom - middle);
                /*while (optionsBox.Scale < 1)
                {
                    middle -= 5;
                    optionsBox.Bounds = new Rectangle(value.X, middle, value.Width, value.Bottom - middle);
                }*/

                titleBox.Bounds = new Rectangle(value.X, Bounds.Y, value.Width, middle - Bounds.Y);

                var size = Gfx.Fonts.SmallFont.MeasureString("Press Esc or F1 at any time");
                escReminderRect = new Rectangle(0, 0, (int) size.X + 2, (int) size.Y + 2);
            }
        }

        public override Option[] Options
        {
            get
            {
                switch (State)
                {
                    case MainMenuState.Main:
                        return mainOptions.ToArray();
                    case MainMenuState.About:
                        return aboutOptions;
                    default:
                        throw new InvalidDataException("MainMenuState [" + State + "] not accounted for");
                }
            }
        }

        public MainMenuState State
        {
            get { return state; }
            set
            {
                state = value;
                optionsBox.Lines.Clear();

                switch (value)
                {
                    case MainMenuState.Main:
                        //optionsBox.YAlign = TextBox.YAlignType.Top;
                        if (Gfx.Loaded)
                            optionsBox.LoadContent(Gfx.Fonts.LargeFont);
                        break;
                    case MainMenuState.About:
                        //optionsBox.YAlign = TextBox.YAlignType.Center;
                        optionsBox.LoadContent(Gfx.Fonts.MediumFont);
                        break;
                }

                foreach (var opt in Options)
                    optionsBox.AddOption(opt);
                Bounds = Bounds;
            }
        }

        public MainMenu()
        {
            var titleBounds = new Rectangle(bounds.X, bounds.Y + 2, bounds.Width, bounds.Height / 2 - 4);
            titleBox = new TextBox(titleBounds, TextBox.XAlignType.Center, TextBox.YAlignType.Center);
            titleBox.Lines.Add(new ColorText("CGCC", Gfx.CodingClubPurple));
            titleBox.Lines.Add(new ColorText("Platformer", Gfx.CodingClubPurple));

            mainOptions = new List<Option>
            {
                new ColorOption(new ColorText("New Game", Gfx.DefaultTextColor, Gfx.DefaultTextHoverColor),
                    () => Game.SetState(TheGame.GameState.NewGame)),
                new ColorOption(new ColorText("About", Gfx.DefaultTextColor, Gfx.DefaultTextHoverColor),
                    () => State = MainMenuState.About),
                new ColorOption(new ColorText("Quit", Color.Red, Color.DarkRed), () =>
                {
                    Logging.WriteLine("Quit game.", 3);
                    Game.Exit();
                })
            };

            var optionsBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2 + 2, bounds.Width,
                bounds.Height / 2 - 16);
            optionsBox = new TextBox(optionsBounds, TextBox.XAlignType.Center);
            foreach (var option in mainOptions)
                optionsBox.AddOption(option);

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = Assembly.GetExecutingAssembly().GetLinkerTime();

            aboutOptions = new[]
            {
                new Option("Created by the CGCC Coding Club"),
                new Option("Font is Comic James by James Frazier"),
                new Option("Build Version: " + version),
                new Option("Build Date: " + buildDate.ToShortDateString()),
                new ColorOption(new ColorText("Back", Color.Red, Color.DarkRed),
                    () => State = MainMenuState.Main),
            };

            State = MainMenuState.Main;
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            Gfx.SpriteBatch.DrawString(Gfx.Fonts.SmallFont, "Press Esc or F1 at any time", new Vector2(2, 2),
                escReminderRect.Contains(mousePos)
                    ? Color.Black
                    : Gfx.DefaultTextColor);
            
            titleBox.Draw(gameTime, mousePos);
            optionsBox.Draw(gameTime, mousePos);

            Gfx.CgccLogo.Draw(new Vector2(Bounds.Right - 100, 0));
        }

        public override void Update(GameTime gameTime, Input input)
        {
            bool keyboardSelect = KeyboardSelect(input); //select with number keys, and don't do it twice if you click
            
            if (input.LeftPress && escReminderRect.Contains(input.MousePos))
            {
                Game.EscMenuBool = true;
                return;
            }

            if (!input.Hovered)
            {
                int lineNum = optionsBox.LineSelection(input);
                if (lineNum > -1)
                {
                    if (input.LeftPress && !keyboardSelect)
                        Options[lineNum].Execute();
                }
            }

            if (State != MainMenuState.Main && input.KeyPress(Keys.Back) && !input.Backed)
            {
                State = MainMenuState.Main;
                input.CaptureBack();
            }
        }

        public override void LoadContent()
        {
            titleBox.LoadContent(Gfx.Fonts.LargeFont);
            State = State; //auto set options font and bounds
        }
    }
}