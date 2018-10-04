using CGCCPlatformer.Helpers;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.UI
{
    public class Option
    {
        public delegate Option OptGetter();

        protected Option[] SubOpts;
        
        public string Name
            => "\"" + Text + "\"" + (GetSubOptions() != null ? " with " + GetSubOptions().Length + " subOpts" : "");

        public IDrawableText DrawableText { get; protected set; }
        public string Text { get; protected set; }
        public bool HasFunction { get; protected set; }
        public bool Activatable { get; protected set; }
        public Utils.EventHandler Click { get; protected set; }
        public OptGetter Updater { get; }

        public Option(string text, Utils.EventHandler function = null, Option[] subOptions = null,
            OptGetter update = null)
        {
            Text = text;
            DrawableText = new PlainText(text);
            SubOpts = subOptions;
            Click = function;
            HasFunction = function != null;
            Activatable = HasFunction || SubOpts != null;
            Updater = update;
        }

        protected Option(string text)
        {
            Text = text;
        }

        public virtual Option[] GetSubOptions() => SubOpts;
        public Option Opt() => this;

        public override bool Equals(object o)
        {
            return GetType() == o?.GetType() && Text == (o as Option)?.Text;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + Text.GetHashCode();
            return hash;
        }

        public virtual bool Update()
        {
            if (Updater == null)
            {
                if (GetSubOptions() == null)
                    return false;

                var updated = false;
                foreach (var subOpt in GetSubOptions())
                {
                    if (subOpt.Update())
                        updated = true;
                }

                HasFunction = Click != null;
                Activatable = HasFunction || SubOpts != null;
                return updated;
            }
            var newOpt = Updater.Invoke();
            Text = newOpt.Text;
            DrawableText = new PlainText(Text);
            HasFunction = newOpt.HasFunction;
            SubOpts = newOpt.GetSubOptions();
            Activatable = newOpt.Activatable;
            Click = newOpt.Click;
            //Updater = newOpt.Updater;
            return true;
        }

        public virtual void Execute()
        {
            Click?.Invoke();
        }

        public override string ToString() => Text;

        //useful!
        public static implicit operator Option[](Option input)
        {
            return new [] {input};
        }
    }

    
    public class ColorOption : Option
    {
        public delegate ColorOption ColorOptGetter();

        public new ColorOptGetter Updater { get; }

        public ColorOption(IDrawableText text, Utils.EventHandler function = null,
            Option[] subOptions = null, ColorOptGetter update = null) : base(text.Text, function, subOptions)
        {
            DrawableText = text;
            Updater = update;
        }

        protected ColorOption(IDrawableText text) : base(text.Text)
        {
            DrawableText = text;
        }

        public override bool Update()
        {
            if (Updater == null)
            {
                if (GetSubOptions() == null)
                    return false;

                var updated = false;
                foreach (var subOpt in GetSubOptions())
                {
                    if (subOpt.Update())
                        updated = true;
                }
                return updated;
            }
            var newOpt = Updater.Invoke();
            DrawableText = newOpt.DrawableText;
            HasFunction = newOpt.HasFunction;
            SubOpts = newOpt.GetSubOptions();
            Activatable = newOpt.Activatable;
            Click = newOpt.Click;
            //Updater = newOpt.Updater;
            return true;
        }

        public void Draw(SpriteFont font, Vector2 pos, float scale, bool hover)
        {
            DrawableText.Draw(font, pos, scale, hover);
        }

        public Vector2 Size(SpriteFont font)
        {
            return DrawableText.Size(font);
        }
    }
}