using System;

namespace CGCCPlatformer.Helpers.Math
{
    //Written by Logan Benham

    public class NormalRandom
    {
        public readonly Random Rand;
        private double cacheValue;
        private bool requireNew;

        public NormalRandom(Random rand)
        {
            Rand = rand;
            requireNew = true;
            cacheValue = 0;
        }

        public NormalRandom()
            : this(new Random()) { }

        public double Next(double mean = 0, double stdDev = 1)
        {
            return next() * stdDev + mean;
        }

        private double next()
        {
            if (requireNew)
            {
                requireNew = false;
                double r1 = Rand.NextDouble();
                double r2 = Rand.NextDouble();
                //Box-Muller Transform
                double part1 = System.Math.Sqrt(-2 * System.Math.Log(r1));
                cacheValue = part1 * System.Math.Cos(2 * System.Math.PI * r2);
                return part1 * System.Math.Sin(2 * System.Math.PI * r2);
            }
            requireNew = true;
            return cacheValue;
        }

        public double ChiSquared(uint k = 2, bool aveOf1 = true)
        {
            double sum = 0;
            for (var i = 1; i <= k; i++)
            {
                double x = Next();
                sum += x * x;
            }
            if (aveOf1)
                sum /= k;
            return sum;
        }
    }
}