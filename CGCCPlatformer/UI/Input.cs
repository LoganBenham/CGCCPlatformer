using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Written by Logan Benham

namespace CGCCPlatformer.UI
{
    public class Input
    {
        public static readonly Keys[] Letters =
        {
            Keys.A,
            Keys.B,
            Keys.C,
            Keys.D,
            Keys.E,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.I,
            Keys.J,
            Keys.K,
            Keys.L,
            Keys.M,
            Keys.N,
            Keys.O,
            Keys.P,
            Keys.Q,
            Keys.R,
            Keys.S,
            Keys.T,
            Keys.U,
            Keys.V,
            Keys.W,
            Keys.X,
            Keys.Y,
            Keys.Z
        };

        private string pressedChar = "";
        /// <summary> consecutive frames with a single typing input </summary>
        private int pressedFrames;
        public uint Frame { get; private set; }

        public bool Hovered { get; private set; }
        public bool Clicked { get; private set; }
        public bool Scrolled { get; private set; }
        public bool Backed { get; private set; }
        public bool Entered { get; private set; }
        public bool KeyboardUsed { get; private set; }

        private bool press
        {
            get
            {
                if (pressedFrames == 1)
                    return true;
                if (pressedFrames >= 260)
                    return true;
                if (pressedFrames >= 160)
                    return pressedFrames % 2 == 0;
                if (pressedFrames >= 80)
                    return pressedFrames % 3 == 0;
                if (pressedFrames >= 30)
                    return pressedFrames % 4 == 0;
                return false;
            }
        }

        public Input()
        {
            Frame = 0;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }

        public void CaptureHover()
        {
            Hovered = true;
        }

        public void CaptureEnter()
        {
            Entered = true;
        }

        public void CaptureClick()
        {
            //used for safety to easily specify that the mouse click has been captured and used
            Clicked = true;
            LeftPress = false;
            MiddlePress = false;
            RightPress = false;
        }

        public void CaptureScroll()
        {
            MouseScroll = 0;
            Scrolled = true;
        }

        public void CaptureBack()
        {
            Backed = true;
        }

        public void Update(GameTime gameTime, bool isActive, bool debug = false)
        {
            Frame += 1;
            Entered = false;
            Hovered = false;
            Clicked = false;
            Scrolled = false;
            Backed = false;
            KeyboardUsed = false;
            //pressedKeys = null;

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            LeftPress = mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released &&
                        isActive;
            MiddlePress = mouseState.MiddleButton == ButtonState.Pressed &&
                          prevMouseState.MiddleButton == ButtonState.Released &&
                          isActive;
            RightPress = mouseState.RightButton == ButtonState.Pressed &&
                         prevMouseState.RightButton == ButtonState.Released &&
                         isActive;

            MouseMovement = MousePos - prevMouseState.Position;

            //Adjusting scroll value by multiplying by the average of the last 4 frames of scroll values
            //Result is a more smooth scrolling where small scrolls do very little and large scrolls do very much
            //I could also have done Math.Pow(MouseScroll, 2) or something similar
            //but using 'momentum' smooths over the innacuracy of the scrolling input
            MouseScroll = mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue;
            scrollQeueue.Enqueue(MouseScroll);
            if (scrollQeueue.Count > 4)
                scrollQeueue.Dequeue();
            MouseMomentum = scrollQeueue.Average() / 30;
            MouseScroll *= Math.Abs((int) MouseMomentum);
            if (!isActive)
                MouseScroll = 0;
            


            //Debug Code
            //key logger !!
            //not really useful and kinda creepy
            /*
            if (PressedKeys.Length > 0)
            {
                int pressed = 0;
                foreach (var key in PressedKeys)
                {
                    if (KeyPress(key))
                    {
                        Logging.WriteLine("Key Pressed: " + key + " at frame " + Frame);
                        pressed += 1;
                    }
                }
                if (pressed > 1)
                    Logging.WriteLine("---Multiple key pressed in a single frame---");
            }

            if (LeftPress)
                Logging.WriteLine("Left clicked at x=" + mouseState.X + "  y=" + mouseState.Y);
            if (RightPress)
                Logging.WriteLine("Right clicked at x=" + mouseState.X + "  y=" + mouseState.Y);*/
        }

        public bool Type(ref string text, ref int cursor)
        {
            bool shift = Shift;
            
            var pressed = keyboardState.GetPressedKeys();
            if (pressed.Length == 0)
            {
                pressedChar = "";
                return false;
            }

            var key = pressed[0]; //only 1 at a time pls T.T

            var i = 0; //shift doesn't count. i think it's usually last in the list anyways though
            while ((key == Keys.LeftShift || key == Keys.RightShift)
                   && i + 1 < pressed.Length)
            {
                i++; //so I move through the list to seek a non-shift key
                key = pressed[i];
            }

            if (key == Keys.Tab)
                return false;

            //Logging.WriteLine("Typed " + key);

            KeyboardUsed = true;

            if (key == Keys.Back && text.Length > 0 && cursor != 0)
            {
                if (pressedChar != "bk")
                    pressedFrames = 0;
                pressedFrames += 1;
                pressedChar = "bk";
                if (press)
                {
                    string start = text;
                    string end = "";
                    if (cursor != text.Length)
                    {
                        start = text.Remove(cursor);
                        end = text.Substring(cursor);
                    }
                    text = start.Remove(start.Length - 1) + end;
                    cursor -= 1;
                    return true;
                }
                return false;
            }
            if (key == Keys.Delete && text.Length > 0 && cursor != text.Length)
            {
                if (pressedChar != "dl")
                    pressedFrames = 0;
                pressedFrames += 1;
                pressedChar = "dl";
                if (press)
                {
                    string start = text;
                    string end = "";
                    if (cursor != text.Length)
                    {
                        start = text.Remove(cursor);
                        end = text.Substring(cursor);
                    }
                    text = start + end.Substring(1);
                    return true;
                }
                return false;
            }

            var chr = "";

            #region Characters

            if (Letters.Contains(key))
                chr = key.ToString().ToLower();
            else if (key == Keys.OemTilde) chr = "`";
            else if (key == Keys.D1) chr = "1";
            else if (key == Keys.D2) chr = "2";
            else if (key == Keys.D3) chr = "3";
            else if (key == Keys.D4) chr = "4";
            else if (key == Keys.D5) chr = "5";
            else if (key == Keys.D6) chr = "6";
            else if (key == Keys.D7) chr = "7";
            else if (key == Keys.D8) chr = "8";
            else if (key == Keys.D9) chr = "9";
            else if (key == Keys.D0) chr = "0";
            else if (key == Keys.OemMinus) chr = "-";
            else if (key == Keys.OemPlus) chr = "=";
            else if (key == Keys.OemOpenBrackets) chr = "[";
            else if (key == Keys.OemCloseBrackets) chr = "]";
            else if (key == Keys.OemPipe) chr = "\\";
            else if (key == Keys.OemSemicolon) chr = ";";
            else if (key == Keys.OemQuotes) chr = "'";
            else if (key == Keys.OemComma) chr = ",";
            else if (key == Keys.OemPeriod || key == Keys.Decimal) chr = ".";
            else if (key == Keys.OemQuestion) chr = "/";

            if (shift)
                chr = ShiftChar(chr);


            //Space and Num pad
            if (key == Keys.Space) chr = " ";
            else if (key == Keys.Divide) chr = "/";
            else if (key == Keys.Multiply) chr = "*";
            else if (key == Keys.Subtract) chr = "-";
            else if (key == Keys.Add) chr = "+";
            else if (key == Keys.NumPad0) chr = "0";
            else if (key == Keys.NumPad1) chr = "1";
            else if (key == Keys.NumPad2) chr = "2";
            else if (key == Keys.NumPad3) chr = "3";
            else if (key == Keys.NumPad4) chr = "4";
            else if (key == Keys.NumPad5) chr = "5";
            else if (key == Keys.NumPad6) chr = "6";
            else if (key == Keys.NumPad7) chr = "7";
            else if (key == Keys.NumPad8) chr = "8";
            else if (key == Keys.NumPad9) chr = "9";

            #endregion

            if (chr != "")
            {
                if (pressedChar != chr)
                {
                    pressedFrames = 0;
                    if (ShiftChar(chr) == pressedChar)
                        return false; //releasing shift won't start entering the unshifted version
                }

                pressedFrames += 1;
                pressedChar = chr;
                if (press)
                {
                    text = text.Insert(cursor, chr);
                    cursor += 1;
                    return true;
                }
                return false;
            }
            pressedChar = "";

            return false;
        }

        private string ShiftChar(string chr)
        {
            if (chr == "`") return "~";
            if (chr == "1") return "!";
            if (chr == "2") return "@";
            if (chr == "3") return "#";
            if (chr == "4") return "$";
            if (chr == "5") return "%";
            if (chr == "6") return "^";
            if (chr == "7") return "&";
            if (chr == "8") return "*";
            if (chr == "9") return "(";
            if (chr == "0") return ")";
            if (chr == "-") return "_";
            if (chr == "=") return "+";
            if (chr == "[") return "{";
            if (chr == "]") return "}";
            if (chr == "\\") return "|";
            if (chr == ";") return ":";
            if (chr == "'") return "\"";
            if (chr == ",") return "<";
            if (chr == ".") return ">";
            if (chr == "/") return "?";
            return chr.ToUpper();
        }

        #region Keyboard

        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;
        public bool Shift => KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift);
        public bool Ctrl => KeyDown(Keys.LeftControl) || KeyDown(Keys.RightControl);

        public bool Enter => keyboardState[Keys.Enter] == KeyState.Down &&
                             prevKeyboardState[Keys.Enter] == KeyState.Up && !Entered;

        public bool KeyPress(Keys key)
            => !KeyboardUsed && keyboardState[key] == KeyState.Down && prevKeyboardState[key] == KeyState.Up;

        public bool KeyDown(Keys key) => !KeyboardUsed && keyboardState[key] == KeyState.Down;
        public bool KeyUp(Keys key) => keyboardState[key] == KeyState.Up;

        public int Vertical => (KeyDown(Keys.Up) || KeyDown(Keys.NumPad8) ||
                                KeyDown(Keys.NumPad7) || KeyDown(Keys.NumPad9)
                                   ? 1
                                   : 0) +
                               (KeyDown(Keys.Down) || KeyDown(Keys.NumPad2) ||
                                KeyDown(Keys.NumPad1) || KeyDown(Keys.NumPad3)
                                   ? -1
                                   : 0);

        public int Horizontal => (KeyDown(Keys.Right) || KeyDown(Keys.NumPad6) ||
                                  KeyDown(Keys.NumPad3) || KeyDown(Keys.NumPad9)
                                     ? 1
                                     : 0) +
                                 (KeyDown(Keys.NumPad4) || KeyDown(Keys.NumPad1) ||
                                  KeyDown(Keys.NumPad7) || KeyDown(Keys.Left)
                                     ? -1
                                     : 0);

        public int VerticalPress => (KeyPress(Keys.Up) || KeyPress(Keys.NumPad8) ||
                                     KeyPress(Keys.NumPad7) || KeyPress(Keys.NumPad9)
                                        ? 1
                                        : 0) +
                                    (KeyPress(Keys.Down) || KeyPress(Keys.NumPad2) ||
                                     KeyPress(Keys.NumPad1) || KeyPress(Keys.NumPad3)
                                        ? -1
                                        : 0);

        public int HorizontalPress => (KeyPress(Keys.Right) || KeyPress(Keys.NumPad6) ||
                                       KeyPress(Keys.NumPad3) || KeyPress(Keys.NumPad9)
                                          ? 1
                                          : 0) +
                                      (KeyPress(Keys.NumPad4) || KeyPress(Keys.NumPad1) ||
                                       KeyPress(Keys.NumPad7) || KeyPress(Keys.Left)
                                          ? -1
                                          : 0);

        #endregion

        #region Mouse

        private MouseState prevMouseState;
        private MouseState mouseState;

        public bool LeftDown => mouseState.LeftButton == ButtonState.Pressed;
        public bool MiddleDown => mouseState.MiddleButton == ButtonState.Pressed;
        public bool RightDown => mouseState.RightButton == ButtonState.Pressed;
        public Point MousePos => mouseState.Position;
        public Point MouseMovement { get; private set; }
        public int X => MousePos.X;
        public int Y => MousePos.Y;
        public Vector2 MousePosV2 => MousePos.ToVector2();

        public bool LeftPress { get; private set; }
        public bool MiddlePress { get; private set; }
        public bool RightPress { get; private set; }
        public bool AnyMousePress => LeftPress || MiddlePress || RightPress;

        private readonly Queue<int> scrollQeueue = new Queue<int>(4);
        public int MouseScroll { get; private set; }
        public double MouseMomentum { get; private set; }

        #endregion
    }
}