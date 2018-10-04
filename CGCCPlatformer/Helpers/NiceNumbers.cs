using CGCCPlatformer.Helpers.Math;

//Written by Logan Benham

namespace CGCCPlatformer.Helpers
{
    public static class NiceNumbers
    {
        public const string NumberFormat = "####,###,###,###,##0.##########";

        //Time
        public const double Year = 31540000;
        public const double Month = Day * 30.44;
        public const double Day = 86400;

        public static double RoundToSigFigs(double number, int sigFigs = 2)
        {
            //range -> decimalPoint -> multiplier -> centered
            //10-99.99.. -> 1 -> 10 -> 1-9.99..
            //1-9.99... -> 0 -> 1 -> 1-9.99..
            //0.1-0.99... -> -1 -> 0.1 -> 1-9.99..
            var decimalPoint = (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(number)));
            double multiplier = FuncFuncs.IntPower(10, decimalPoint);
            double centered = number / multiplier;
            if (sigFigs < 1)
                sigFigs = 1;
            return System.Math.Round(centered, sigFigs - 1) * multiplier;
        }

        public static string SigFig(double number, int sigFigs = 2)
        {
            return RoundToSigFigs(number, sigFigs).ToString(NumberFormat);
        }

        public static string Scientific(double number, int sigFigs = 2)
        {
            var decimalPoint = (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(number)));
            double multiplier = FuncFuncs.IntPower(10, decimalPoint);
            double centered = number / multiplier;
            if (sigFigs < 1)
                sigFigs = 1;
            if (decimalPoint == 0)
                return System.Math.Round(centered, sigFigs - 1).ToString();
            return System.Math.Round(centered, sigFigs - 1) + "e" + decimalPoint;
        }

        public static string NiceNumber(double number, int sigFigs = 2, int maxZeros = 2, bool space = false)
        {
            if (number == 0)
                return 0 + (space ? " " : "");
            var decimalPoint = (int) System.Math.Floor(System.Math.Log10(System.Math.Abs(number)));
            double multiplier = FuncFuncs.IntPower(10, decimalPoint);
            double centered = number / multiplier;
            if (sigFigs < 1)
                sigFigs = 1;

            if (decimalPoint > sigFigs + maxZeros - 1 ||
                decimalPoint < -1 - maxZeros)
                return System.Math.Round(centered, sigFigs - 1) + "e" + decimalPoint + (space ? " " : "");
            return (System.Math.Round(centered, sigFigs - 1) * multiplier).ToString(NumberFormat) + (space ? " " : "");
        }

        public static string PercentString(double portion, int sigFigs = 2)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (1 - portion < 0.01 && portion < 1) //don't round 99% up
                return 100 - 100 * RoundToSigFigs(1 - portion, sigFigs) + "%";
            return NiceNumber(100 * portion, sigFigs) + "%";
        }

        public static string BitRateString(double bytesPerSec, int digits = 2, bool space = true)
        {
            if (bytesPerSec < 1000)
                return NiceNumber(bytesPerSec, digits, space: space) + "bytes/s";
            if (bytesPerSec < 1000000)
                return NiceNumber(bytesPerSec / 1000, digits, space: space) + "KB/s";
            if (bytesPerSec < 1000000000)
                return NiceNumber(bytesPerSec / 1000000, digits, space: space) + "MB/s";
            return NiceNumber(bytesPerSec / 1000000000, digits, space: space) + "GB/s";
        }

        public static string FrequencyString(double hertz, string action, int digits = 2, bool space = true)
        {
            if (hertz >= 0.5)
                return NiceNumber(hertz, digits, space: space) + (space ? " " : "") + action + "/s";
            if (hertz >= 0.5 / 60d)
                return NiceNumber(hertz / 60, digits, space: space) + (space ? " " : "") + action + "/min";
            if (hertz >= 0.5 / 3600)
                return NiceNumber(hertz / 3600, digits, space: space) + (space ? " " : "") + action + "/hour";
            if (hertz >= 0.5 / Day)
                return NiceNumber(hertz / Day, digits, space: space) + (space ? " " : "") + action + "/day";
            if (hertz >= 0.5 / Month)
                return NiceNumber(hertz / Month, digits, space: space) + (space ? " " : "") + action + "/month";
            return NiceNumber(hertz / Year, digits, space: space) + (space ? " " : "") + action + "/year";
        }

        public static string TimeString(double s, int digits = 2, bool space = true)
        {
            double abs = System.Math.Abs(s);
            if (abs > 1000 * Year)
                return NiceNumber(s / (1000 * Year), digits, space: space) + "kyrs";
            if (abs == 1000 * Year)
                return "1" + (space ? " " : "") + "kyr";
            if (abs > Year)
                return NiceNumber(s / Year, digits, space: space) + "years";
            if (abs == Year)
                return "1" + (space ? " " : "") + "year";
            if (abs > Month)
                return NiceNumber(s / Month, digits, space: space) + "months";
            if (abs == Month)
                return "1" + (space ? " " : "") + "month";
            if (abs > Day)
                return NiceNumber(s / Day, digits, space: space) + "days";
            if (abs == Day)
                return "1" + (space ? " " : "") + "day";
            if (abs > 3600)
                return NiceNumber(s / 3600, digits, space: space) + "hours";
            if (abs == 3600)
                return "1" + (space ? " " : "") + "hour";
            if (abs > 60)
                return NiceNumber(s / 60, digits, space: space) + "minutes";
            if (abs == 60)
                return "1" + (space ? " " : "") + "minute";
            if (abs == 1)
                return "1" + (space ? " " : "") + "second";
            return NiceNumber(s, digits, space: space) + "seconds";
        }

        public static string TimeStringMultiUnit(double s, int units = 2, bool space = true)
        {
            if (units <= 0)
                return "";
            units--;

            double abs = System.Math.Abs(s);

            if (abs == 1000 * Year)
                return "1" + (space ? " " : "") + "kyr";
            if (abs == Year)
                return "1" + (space ? " " : "") + "year";
            if (abs == Month)
                return "1" + (space ? " " : "") + "month";
            if (abs == Day)
                return "1" + (space ? " " : "") + "day";
            if (abs == 3600)
                return "1" + (space ? " " : "") + "hour";
            if (abs == 60)
                return "1" + (space ? " " : "") + "minute";
            if (abs == 1)
                return "1" + (space ? " " : "") + "second";


            string str;
            if (abs > 1000 * Year)
            {
                var kyrs = System.Math.Floor(s / (1000 * Year));
                str = kyrs + (space ? " " : "") + "kyr";
                s -= kyrs * 1000 * Year;
            }
            else if (abs > Year)
            {
                var yrs = System.Math.Floor(s / Year);
                str = yrs + (space ? " " : "") + "yr";
                s -= yrs * Year;
            }
            else if (abs > Month)
            {
                var months = System.Math.Floor(s / Month);
                str = months + (space ? " " : "") + "mon";
                s -= months * Month;
            }
            else if (abs > Day)
            {
                var days = System.Math.Floor(s / Day);
                str = days + (space ? " " : "") + "days";
                s -= days * Day;
            }
            else if (abs > 3600)
            {
                var hrs = System.Math.Floor(s / 3600);
                str = hrs + (space ? " " : "") + "hrs";
                s -= hrs * 3600;
            }
            else if (abs > 60)
            {
                var min = System.Math.Floor(s / 60);
                str = min + (space ? " " : "") + "min";
                s -= min * 60;
            }
            else
                return System.Math.Floor(s) + (space ? " " : "") + "s";

            if (units > 0)
                str += ", ";

            return str + TimeStringMultiUnit(s, units, space);
        }
    }
}