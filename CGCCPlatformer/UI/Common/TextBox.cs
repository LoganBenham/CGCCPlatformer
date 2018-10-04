using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CGCCPlatformer.UI.Common
{
    public class TextBox : InteractiveWindow
    {
        //TODO: textbox wordwrap types: space, tab, unspaced, none

        public enum XAlignType
        {
            Left,
            Center,
            Right
        }

        public enum YAlignType
        {
            Top,
            Center,
            Bottom
        }

        private SpriteFont font;
        private ObservableCollection<IDrawableText> lines;
        private int scrolled;
        public XAlignType XAlign;
        public YAlignType YAlign;
        
        public override Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                SetScale();
            }
        }

        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                LineHeight = (int) font.MeasureString("|").Y;
                SetScale();
            }
        }

        public int LineHeight { get; protected set; }
        public int VerticalSpacing { get; set; }

        public ObservableCollection<IDrawableText> Lines
        {
            get
            {
                lock (linesLock)
                {
                    return lines;
                }
            }
            set
            {
                lines = value;
                SetScale();
            }
        }

        private readonly object linesLock = new object();

        public void ThreadSafeSetLine(int index, IDrawableText text)
        {
            lock (linesLock)
            {
                if (index == lines.Count)
                    lines.Add(text);
                else
                    lines[index] = text;
            }
        }

        public int LineCount => Lines.Count;

        public event Action ScrollChange;

        public float Scale { get; private set; }
        public float DefaultScale { get; set; }

        public int FilledHeight => 4 + (int) (LineHeight * Scale * Lines.Count);

        public int FilledWidth
        {
            get
            {
                lock (linesLock)
                {
                    return 4 + lines.Concat(new List<IDrawableText> { new PlainText(" ") })
                                      .Max(line => (int)(line.Size(font).X * Scale));
                }
            }
        }

        public Rectangle FilledBound
        {
            get
            {
                int y = StartY;
                int w = FilledWidth;
                int h = FilledHeight;
                switch (XAlign)
                {
                    case XAlignType.Center:
                        return new Rectangle(Bounds.Center.X - w / 2, y, w, h);
                    case XAlignType.Left:
                        return new Rectangle(Bounds.X - 2, y, w, h);
                    case XAlignType.Right:
                        return new Rectangle(Bounds.Right - w - 2, y, w, h);
                    default:
                        return new Rectangle(Bounds.Center.X - w / 2, y, w, h);
                }
            }
        }

        public int StartY
        {
            get
            {
                int y;
                switch (YAlign)
                {
                    case YAlignType.Top:
                        y = Bounds.Y;
                        break;
                    case YAlignType.Center:
                        y = Bounds.Y;
                        int diff = Bounds.Height - FilledHeight;
                        if (diff > 0)
                            y += diff / 2;
                        break;
                    case YAlignType.Bottom:
                        y = Bounds.Bottom - FilledHeight;
                        if (y < Bounds.Y)
                            y = Bounds.Y;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(YAlign), "YAlign is invalid");
                }
                return y;
            }
        }

        public TextBox(Rectangle bounds, XAlignType xAlign = XAlignType.Left,
            YAlignType yAlign = YAlignType.Top,
            float defaultScale = 1, int vertSpacing = 0, bool scrollable = false, TextBox scrollLink = null)
            : base(bounds)
        {
            Lines = new ObservableCollection<IDrawableText>();
            Lines.CollectionChanged += (sender, args) => SetScale();
            XAlign = xAlign;
            YAlign = yAlign;
            if (defaultScale > 1)
                Logging.WriteLine(Logging.Level.Warning, "Scaling a font up by default makes it look bad.");
            DefaultScale = defaultScale;
            Scale = defaultScale;
            VerticalSpacing = vertSpacing;
        }

        protected void SetScale()
        {

            if (font == null)
                return;
            float heightScale = (float) Bounds.Height / (6 + LineHeight * Lines.Count);
            //Bounds.Width / Max line width
            float widthScale = Bounds.Width /
                               Lines.Concat(new List<IDrawableText> {new PlainText("")}).Max(line => line.Size(font).X);
            Scale = Utils.Lowest(heightScale, widthScale, DefaultScale); //smaller scale so it fits. clamp at DefaultScale
            ScrollChange?.Invoke();
        }

        public void AddString(string text)
        {
            Lines.Add(new ColorText(text, Gfx.DefaultTextColor));
        }

        public void AddOption(Option option)
        {
            var colored = option as ColorOption;
            if (colored != null)
                Lines.Add(colored.DrawableText);
            else
                AddString(option.Text);
        }

        public int LineSelection(Input input) => LineSelection(input.MousePos, input.Hovered);

        public int LineSelection(Point mousePos, bool hovered = false)
        {
            if (!FilledBound.Contains(mousePos) || hovered)
                return -1;

            int y = StartY;
            for (int i = 0; i < Lines.Count; i++)
            {
                if (y + LineHeight * Scale + i * LineHeight * Scale > Bounds.Bottom)
                    break;
                var hitbox = new Rectangle(Bounds.X, y + (int) (i * LineHeight * Scale), Bounds.Width,
                    (int) (LineHeight * Scale));
                if (hitbox.Contains(mousePos))
                    return i;
            }
            return -1;
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            Draw(gameTime, -1, mousePos);
        }

        public void Draw(GameTime gameTime, int cursorIndex, Point mousePos = default(Point))
        {
            if (mousePos != default(Point))
            {
                int j = LineSelection(mousePos);
                if (j > -1) cursorIndex = j;
            }

            int y = StartY;

            switch (XAlign)
            {
                case XAlignType.Left:
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (y + LineHeight * Scale + i * LineHeight * Scale > Bounds.Bottom)
                            break;
                        Lines[i].Draw(font, new Vector2(Bounds.X, y + i * LineHeight * Scale),
                            Scale, i == cursorIndex);
                    }
                    break;
                case XAlignType.Center:
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (y + LineHeight * Scale + i * LineHeight * Scale > Bounds.Bottom)
                            break;
                        Lines[i].Draw(font, new Vector2(Bounds.Center.X - (int) (Lines[i].Size(font).X * Scale / 2),
                            y + i * LineHeight * Scale), Scale, i == cursorIndex);
                    }
                    break;
                case XAlignType.Right:
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (y + LineHeight * Scale + i * LineHeight * Scale > Bounds.Bottom)
                            break;
                        Lines[i].Draw(font, new Vector2(Bounds.Right - (int) (Lines[i].Size(font).X * Scale),
                            y + i * LineHeight * Scale), Scale, i == cursorIndex);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(XAlign), "XAlign is invalid");
            }

            //Debug
            //Bounds.Draw(Color.Aqua, Game.Bounds);
            //FilledBound.Draw(Color.Red, Game.Bounds);
        }

        public override void Update(GameTime gameTime, Input input)
        {

        }

        public void LoadContent(SpriteFont _font)
        {
            Font = _font;
        }
    }
}