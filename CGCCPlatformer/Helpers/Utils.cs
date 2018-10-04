using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CGCCPlatformer.Helpers.Math;
using Microsoft.Xna.Framework;

namespace CGCCPlatformer.Helpers
{
    public static class Utils
    {
        public delegate void EventHandler();

        public delegate void UsingFloat(float input);

        public static Rectangle PadIn(this Rectangle rect, int pad)
        {
            return new Rectangle(rect.X + pad, rect.Y + pad, rect.Width - 2 * pad, rect.Height - 2 * pad);
        }

        public static Rectangle PadInX(this Rectangle rect, int leftPad, int rightPad)
        {
            return new Rectangle(rect.X + leftPad, rect.Y, rect.Width - leftPad - rightPad, rect.Height);
        }

        public static Rectangle PadInY(this Rectangle rect, int topPad, int bottomPad)
        {
            return new Rectangle(rect.X, rect.Y + topPad, rect.Width, rect.Height - topPad - bottomPad);
        }

        public static Rectangle Translate(this Rectangle rect, int x, int y)
        {
            return new Rectangle(rect.X + x, rect.Y + y, rect.Width, rect.Height);
        }

        [Pure]
        public static double Ceiling(this double value, double divisor)
        {
            return divisor * System.Math.Ceiling(value / divisor);
        }

        [Pure]
        public static double Floor(this double value, double divisor)
        {
            return divisor * System.Math.Floor(value / divisor);
        }

        [Pure]
        public static ulong GreatestCommonDivisor(this IEnumerable<ulong> numbers)
        {
            return numbers.Aggregate(GreatestCommonDivisor);
        }

        [Pure]
        public static ulong GreatestCommonDivisor(ulong a, ulong b)
        {
            while (true)
            {
                if (a == 0 || b == 0) return a | b;
                ulong a1 = a;
                a = System.Math.Min(a, b);
                b = System.Math.Max(a1, b) % System.Math.Min(a1, b);
            }
        }

        [Pure]
        public static T Greatest<T>(params T[] values) where T : IComparable<T>
        {
            return values.Max();
        }

        [Pure]
        public static T Lowest<T>(params T[] values) where T : IComparable<T>
        {
            return values.Min();
        }

        [Pure]
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            return value.CompareTo(max) > 0 ? max : value;
        }

        /// <summary> Returns the greater of the two values. </summary>
        [Pure]
        public static T ClampMin<T>(this T value, T min) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value;
        }

        /// <summary> Returns the lesser of the two values. </summary>
        [Pure]
        public static T ClampMax<T>(this T value, T max) where T : IComparable<T>
        {
            return value.CompareTo(max) > 0 ? max : value;
        }

        public static void Clamp<T>(ref T value, T min, T max) where T : IComparable<T>
        {
            value = value.Clamp(min, max);
        }

        /// <summary> Returns the greater of the two values. </summary>
        public static void ClampMin<T>(ref T value, T min) where T : IComparable<T>
        {
            value = value.ClampMin(min);
        }

        /// <summary> Returns the lesser of the two values. </summary>
        public static void ClampMax<T>(ref T value, T max) where T : IComparable<T>
        {
            value = value.ClampMax(max);
        }

        public static bool BetweenInclusive<T>(this T value, T bound1, T bound2) where T : IComparable<T>
        {
            var compare1 = value.CompareTo(bound1);
            var compare2 = value.CompareTo(bound2);
            return compare1 >= 0 && compare2 <= 0 ||
                   compare1 <= 0 && compare2 >= 0;
        }

        public static bool BetweenExclusive<T>(this T value, T bound1, T bound2) where T : IComparable<T>
        {
            var compare1 = value.CompareTo(bound1);
            var compare2 = value.CompareTo(bound2);
            return compare1 > 0 && compare2 < 0 ||
                   compare1 < 0 && compare2 > 0;
        }
        
        public static bool IsRounded(this double value, double maxDiff = -1)
        {
            if (maxDiff < 0)
                maxDiff = 0.000001 * value;
            ClampMax(ref maxDiff, 0.1);
            return Equals(value, System.Math.Round(value), maxDiff);
        }

        public static void Shuffle<T>(this List<T> list, Random rand)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rand.Next(n--);
                var temp = list[n];
                //swap objects at index n and index k
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static void Shuffle<T>(this T[] array, Random rand)
        {
            //https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
            int n = array.Length;
            while (n > 1)
            {
                int k = rand.Next(n--);
                var temp = array[n];
                //swap objects at index n and index k
                array[n] = array[k];
                array[k] = temp;
            }
        }

        [Pure]
        public static bool Equals(this double d1, double d2, double delta = 0.00001)
        {
            delta = System.Math.Abs(delta);
            return System.Math.Abs(d1 - d2) < delta;
        }

        public static double Atanh(double x)
        {
            return (System.Math.Log(1 + x) - System.Math.Log(1 - x)) / 2;
        }

        /// <summary> Generates a random angle </summary>
        public static Angle NextAngle(this Random rand)
        {
            return new Angle(rand.NextDouble() * System.Math.PI * 2);
        }

        /// <summary> Generates a random number between the given min and max. Spaced evenly on the log scale. </summary>
        public static double LogRand(this Random rand, double min, double max)
        {
            return System.Math.Pow(System.Math.E, rand.LinearRand(System.Math.Log(min), System.Math.Log(max)));
        }

        /// <summary> Generates a random number between the given min and max </summary>
        public static double LinearRand(this Random rand, double min, double max)
        {
            double r = rand.NextDouble();
            return min + (max - min) * r;
        }
    }
}
