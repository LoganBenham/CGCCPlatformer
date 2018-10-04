namespace CGCCPlatformer.Helpers.Math
{
    public static class FuncFuncs
    {
        public delegate double Function(double input);

        public static double Integrate(Function func, double minInput, double maxInput, uint n = 70)
        {
            double range = maxInput - minInput;
            double sum = 0;
            double interval = range / n;
            //Console.WriteLine("Summing from " + minInput + " to " + maxInput + " in " + n + " steps.");
            for (var i = 0; i < n; i++)
            {
                //Middle reimann sum
                double input = minInput + range * ((i + 0.5) / n);
                double output = func(input);
                sum += output * interval;
                //Console.WriteLine("x = " + input + " -- y = " + output + " -- sum = " + sum);
            }
            return sum;
        }

        public static double Derivative(Function func, double input, double interval, double min = double.MinValue,
            double max = double.MaxValue)
        {
            double x1 = input - interval / 2;
            double x2 = input + interval / 2;
            if (x1 < min)
            {
                x1 = min;
                x2 = min + interval;
            }
            else if (x2 > max)
            {
                x1 = max - interval;
                x2 = max;
            }
            return (func(x2) - func(x1)) / interval;
        }

        public static double IntPower(this double _base, int power)
        {
            double output = 1;
            if (power > 0)
                for (var i = 0; i < power; i++)
                    output *= _base;
            else if (power < 0)
                for (var i = 0; i < -power; i++)
                    output /= _base;
            return output;
        }

        public static double Factorial(uint input)
        {
            uint output = 1;
            for (uint i = input; i > 0; i--)
            {
                output *= i;
            }
            return output;
        }

        public static double Limit(Function func, double input, double interval = 0.00001)
        {
            double right = func(input + interval / 2);
            double left = func(input - interval / 2);
            return (right + left) / 2;
        }

        public static double Asinh(double x)
        {
            return System.Math.Log(System.Math.Sqrt(x * x + 1) + x);
        }

        public static double Atanh(double x)
        {
            return (System.Math.Log(x + 1) - System.Math.Log(1 - x)) / 2;
        }

        public static double Acosh(double x)
        {
            return System.Math.Log(x + System.Math.Sqrt(x - 1) * System.Math.Sqrt(x + 1));
        }
    }
}