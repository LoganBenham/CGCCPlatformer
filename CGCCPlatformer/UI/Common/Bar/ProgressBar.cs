using System;
using CGCCPlatformer.Helpers.Graphics;
using Microsoft.Xna.Framework;

//Written by Logan Benham

namespace CGCCPlatformer.UI.Common.Bar
{
    public class ProgressBar : FillableBar
    {
        public int PulseDelay { get; set; }
        public Color PulseColor { get; set; }
        public float PulseSpeed { get; set; }
        public bool Pause { get; private set; }

        private float pulseWidthProportion;
        public float PulseWidthProportion
        {
            get { return pulseWidthProportion; }
            set
            {
                pulseWidthProportion = value;
                halfPulseWidth = (int)(Bounds.Width * PulseWidthProportion / 2);
            }
        }

        private int i;
        private float pulseX;
        private int halfPulseWidth;
        private bool showingPulse;

        public override Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
                halfPulseWidth = (int) (value.Width * PulseWidthProportion / 2);
            }
        }

        public ProgressBar(Rectangle bounds) : base(bounds)
        {
            PulseDelay = (int) (60 * 0.5f); //0.5 second
            PulseColor = Color.White;
            PulseWidthProportion = 1.05f;
            PulseSpeed = 5f;
            halfPulseWidth = (int) (bounds.Width * PulseWidthProportion / 2);
            showingPulse = false;
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            base.Draw(gameTime, mousePos);
            if (!showingPulse)
            {
                i++;
                if (i <= PulseDelay)
                    return;
                showingPulse = true;
                i = 0;
                pulseX = -halfPulseWidth;
                return;
            }

            var start = (int) (bounds.X + pulseX);
            var bound = new Rectangle(bounds.X + 1, bounds.Y + 1, Filled, bounds.Height - 2);
            //var halfPulseWidth = (int) (Filled * PulseWidthProportion / 2);
            for (var x = 0; x < halfPulseWidth; x++)
            {
                var whiten = 0.7f * (1 - (float) Math.Pow((double) x / halfPulseWidth, 1.25));
                var color = FillColor.Interpolate(PulseColor, whiten);
                Drawing.DrawVerticalLine(start + x, color, bound);
                Drawing.DrawVerticalLine(start - x, color, bound);
            }

            //pulse speeds up as it progresses
            if (!Pause)
                pulseX += PulseSpeed * (0.7f + 1f * (pulseX + halfPulseWidth) / (Filled + halfPulseWidth * 2));
            if (pulseX < Filled + halfPulseWidth)
                return;

            showingPulse = false;
        }

        public void Update(GameTime gameTime, Input input)
        {
            if (input.LeftPress && Bounds.Contains(input.MousePos))
                Pause = !Pause;
        }
    }
}
