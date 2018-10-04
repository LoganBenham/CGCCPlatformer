using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI.Common;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CGCCPlatformer.UI.Screens
{
    public sealed class EscMenu : Menu
    {
        public enum EscMenuState
        {
            Main,
            Help
        }
        
        private readonly Option[] helpOptions;

        private readonly List<Option> mainOptions;
        private readonly TextBox textBox;

        private readonly TextBox titleBox;

        private EscMenuState state;

        public override Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                titleBox.Bounds = new Rectangle(value.X, value.Y + 4, value.Width, titleBox.FilledHeight);
                textBox.Bounds = new Rectangle(value.X, titleBox.Bounds.Bottom, Bounds.Width,
                    Bounds.Height - titleBox.Bounds.Height - 4);
            }
        }

        public EscMenuState State
        {
            get { return state; }
            set
            {
                state = value;

                if (!Gfx.Loaded) return;

                titleBox.Lines.Clear();
                textBox.Lines.Clear();
                
                switch (state)
                {
                    case EscMenuState.Main:
                        titleBox.Lines.Add(new PlainText("Esc Menu"));
                        textBox.LoadContent(Gfx.Fonts.LargeFont);
                        break;
                    case EscMenuState.Help:
                        titleBox.Lines.Add(new PlainText("Esc Menu > Help"));
                        textBox.LoadContent(Gfx.Fonts.MediumFont);
                        break;
                    default:
                        throw new InvalidDataException("EscMenuState [" + State + "] not accounted for");
                }

                foreach (var opt in Options)
                    textBox.AddOption(opt);
            }
        }

        public override Option[] Options
        {
            get
            {
                switch (State)
                {
                    case EscMenuState.Main:
                        return mainOptions.ToArray();
                    case EscMenuState.Help:
                        return helpOptions;
                    default:
                        throw new InvalidDataException("EscMenuState [" + State + "] not accounted for");
                }
            }
        }
        
        public EscMenu()
        {
            titleBox = new TextBox(Bounds, TextBox.XAlignType.Center);
            titleBox.Lines.Add(new PlainText("Esc Menu"));

            textBox = new TextBox(Bounds, TextBox.XAlignType.Center, TextBox.YAlignType.Center);

            state = EscMenuState.Main;
            mainOptions = new List<Option>
            {
                new ColorOption(new ColorText("Main Menu", Gfx.DefaultTextColor, Gfx.DefaultTextHoverColor),
                    () => Game.SetState(TheGame.GameState.MainMenu)),
                new ColorOption(new ColorText("Help", Gfx.DefaultTextColor, Gfx.DefaultTextHoverColor), () => State = EscMenuState.Help),
                
                new ColorOption(new ColorText("Back", Color.Red, Color.DarkRed),
                    () => Game.EscMenuBool = false)
            };

            foreach (var opt in Options)
                textBox.AddOption(opt);

            helpOptions = new Option[]
            {
                new ColorOption(new PlainText("Nothing yet :[")), 
                new ColorOption(new ColorText("Back", Color.Red, Color.DarkRed),
                    () => State = EscMenuState.Main)
            };
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            titleBox.Draw(gameTime, -1);
            textBox.Draw(gameTime, mousePos);
        }

        public override void Update(GameTime gameTime, Input input)
        {
            bool pressed = KeyboardSelect(input);

            if (input.Hovered)
                return;

            int lineNum = textBox.LineSelection(input);
            if (lineNum > -1)
            {
                if (input.LeftPress && !pressed)
                    Options[lineNum].Execute();
            }

            if (input.KeyPress(Keys.Back) && !input.Backed)
            {
                input.CaptureBack();
                if (State != EscMenuState.Main)
                    State = EscMenuState.Main;
                else
                    Game.EscMenuBool = false;
            }
        }

        public override void LoadContent()
        {
            titleBox.LoadContent(Gfx.Fonts.LargeFont);
            State = State; //auto load correct textBox font
        }
    }
}