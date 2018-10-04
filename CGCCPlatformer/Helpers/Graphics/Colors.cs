using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

//Written by Logan Benham

namespace CGCCPlatformer.Helpers.Graphics
{
    public static class Colors
    {
        public static Color Blit(this Color back, Color front)
        {
            if (front.A == 255)
                return front;
            if (front.A == 0)
            {
                //Debug.WriteLine(back);
                return back;
            }
            return back.Interpolate(front, front.A / 255f, (front.A / 255f + back.A / 255f).Clamp(0, 1));

            /*
            var bV = back.ToVector3();
            var fV = front.ToVector3();
            var bA = back.A / 255f;
            var fA = front.A / 255f;
            //https://en.wikipedia.org/wiki/Alpha_compositing#Description
            var result = (fV * fA + bV * bA * (1 - fA)) / (fA + bA * (1 - fA));
            var resultA = fA + bA * (1 - fA);
            return new Color(new Vector4(result.X, result.Y, result.Z, resultA));*/
        }

        public static Color AverageColor(this IEnumerable<Color> colors)
        {
            int r = 0, g = 0, b = 0, n = 0;
            foreach (var color in colors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
                n++;
            }
            return new Color(r / n, g / n, b / n);
        }

        public static Color Interpolate(this Color zero, Color one, float value, float alpha = -1)
        {
            if (value <= 0)
                return zero;
            if (value >= 1)
                return one;
            float neg = 1 - value;
            var r = (byte) (zero.R * neg + one.R * value);
            var g = (byte) (zero.G * neg + one.G * value);
            var b = (byte) (zero.B * neg + one.B * value);
            byte a;
            if (alpha < 0 || alpha > 1)
                a = (byte) (zero.A * neg + one.A * value);
            else
                a = (byte) (alpha * 255);
            return new Color(r, g, b, a);
        }

        public static Color Random(Random rand)
        {
            return new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
        }

        public static Color RandomHsv(Random rand)
        {
            return ColorFromHsv(rand.NextDouble() * 360,
                rand.NextDouble(), rand.NextDouble());
        }

        public static Color SetAlpha(this Color input, byte alpha) => new Color(input, alpha);

        public static Color Invert(this Color color)
        {
            var vec = color.ToVector4();
            return new Color(new Vector4(1 - vec.X, 1 - vec.Y, 1 - vec.Z, vec.W));
        }

        public static Color Contrast(this Color color)
        {
            return (color.R + color.G + color.B) / 765f > 0.5f
                ? new Color(0, 0, 0, color.A)
                : new Color(255, 255, 255, color.A);
        }

        public static Color Brighten(this Color color, float minVal)
        {
            var val = color.GetHsvValue();
            return val >= minVal ? color : color.SetHsvValue(minVal);
            /*return new Color(
                Utils.Greatest(color.R, minVal),
                Utils.Greatest(color.G, minVal),
                Utils.Greatest(color.B, minVal),
                color.A);*/
        }


        //TODO: Use this in ColorOption to make sure that stuff highlighted with white isn't too white
        public static Color Darken(this Color color, float maxVal)
        {
            var val = color.GetHsvValue();
            return val <= maxVal ? color : color.SetHsvValue(maxVal);
        }

        public static Color SetHsvValue(this Color color, float value)
        {
            //value = color.GetHsvValue();
            var max = Utils.Greatest(color.R, color.G, color.B);
            if (max == 0)
            {
                var val = (int) (255 * value);
                return new Color(val, val, val);
            }
            var maxMult = 255f / max;
            var mult = value * maxMult;
            var color2 = new Color(
                (byte)(color.R * mult),
                (byte)(color.G * mult),
                (byte)(color.B * mult),
                color.A);
            /*Logging.WriteLine(Logging.Level.Info,
                System.Math.Round(color.GetHsvValue(), 3) + "  ->  " +
                System.Math.Round(color2.GetHsvValue(), 3));
            Logging.WriteLine(color + "  ->  " + color2);*/
            return color2;
        }

        public static Color MultiplyHsvValue(this Color color, float mult)
        {
            var value = color.GetHsvValue() * mult;
            value = value.Clamp(0, 1);
            var max = Utils.Greatest(color.R, color.G, color.B);
            var maxMult = 255f / max;
            var byteMult = value * maxMult;
            var color2 = new Color(
                (byte)(color.R * byteMult),
                (byte)(color.G * byteMult),
                (byte)(color.B * byteMult),
                color.A);
            /*Logging.WriteLine(Logging.Level.Info,
                System.Math.Round(color.GetHsvValue(), 3) + "  ->  " +
                System.Math.Round(color2.GetHsvValue(), 3));
            Logging.WriteLine(color + "  ->  " + color2);*/
            return color2;
        }

        public static float GetHsvValue(this Color color)
        {
            return Utils.Greatest(color.R, color.G, color.B) / 255f;
        }

        public static float GetHsvSaturation(this Color color)
        {
            var max = Utils.Greatest(color.R, color.G, color.B);
            if (max == 0)
                return 0;
            var min = Utils.Lowest(color.R, color.G, color.B);
            var diff = max - min;
            return (float) diff / max;
        }

        public static Color SetHsvSaturation(this Color color, float saturation)
        {
            saturation = saturation.Clamp(0, 1);
            var max = Utils.Greatest(color.R, color.G, color.B);
            if (max == 0) //black
                return color;
            var min = Utils.Lowest(color.R, color.G, color.B);
            var diff = max - min;
            if (diff == 0) //grey or white. no color to change saturation from
                return color;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            //r == max ? r : (byte) (max * (1 - (max - r) * saturation / diff))
            var q = max * saturation / diff;
            return new Color(
                (byte) (max - (max - r) * q),
                (byte) (max - (max - g) * q),
                (byte) (max - (max - b) * q),
                color.A);
        }

        public static Color MultiplyHsvSaturation(this Color color, float mult)
        {
            var max = Utils.Greatest(color.R, color.G, color.B);
            if (max == 0) //black
                return color;
            var min = Utils.Lowest(color.R, color.G, color.B);
            var diff = max - min;
            if (diff == 0) //grey or white. no color to change saturation from
                return color;
            var currentSaturation = (float) diff / max;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            //r == max ? r : (byte) (max * (1 - (max - r) * saturation / diff))
            var saturation = (currentSaturation * mult).Clamp(0, 1);
            var q = max * saturation / diff;
            return new Color(
                (byte)(max - (max - r) * q),
                (byte)(max - (max - g) * q),
                (byte)(max - (max - b) * q),
                color.A);
        }

        public static Color Normalize(this Color color, float value = 0.5f, bool brightenGreys = false)
        {
            if (brightenGreys && color.R == color.G && color.G == color.B)
                value += 0.3f;
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            float fac = value / ((color.R + color.G + color.B) / 765f);
            return new Color((int) (color.R * fac), (int) (color.G * fac), (int) (color.B * fac));
        }

        public static Color ShiftHue(this Color color, double shift)
        {
            var clr = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            double hue, saturation, value;
            clr.ColorToHsv(out hue, out saturation, out value);
            hue += shift;
            hue = hue % 360;
            return ColorFromHsv(hue, saturation, value);
        }

        //https://stackoverflow.com/questions/359612/how-to-change-rgb-color-to-hsv

        public static void ColorToHsv(this System.Drawing.Color color, out double hue, out double saturation, out double value)
        {
            var max = Utils.Greatest(color.R, color.G, color.B);
            var min = Utils.Lowest(color.R, color.G, color.B);

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHsv(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(System.Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - System.Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return new Color(v, t, p);
                case 1:
                    return new Color(q, v, p);
                case 2:
                    return new Color(p, v, t);
                case 3:
                    return new Color(p, q, v);
                case 4:
                    return new Color(t, p, v);
                default: //5
                    return new Color(v, p, q);
            }
        }
    }
}