//Written by Logan Benham

namespace CGCCPlatformer.Helpers.Math
{
    public struct Angle
    {
        public static readonly Angle Zero = new Angle(0);

        public double Radians { get; private set; }

        public Angle(double radians)
        {
            Radians = radians < 0
                ? 2 * System.Math.PI - (-radians) % (System.Math.PI * 2)
                : Radians = radians % (System.Math.PI * 2);
        }

        public Angle(double x, double y)
            : this(System.Math.Atan2(y, x))
        {}

        public double Degrees() => Radians * 180 / System.Math.PI;

        public double Cosine()
        {
            return System.Math.Cos(Radians);
        }

        public double Sine()
        {
            return System.Math.Sin(Radians);
        }

        public Angle Opposite()
        {
            return new Angle(Radians + System.Math.PI);
        }

        public static Angle FromDegrees(double degrees) => new Angle(degrees * System.Math.PI / 180);

        public static Angle operator +(Angle ang1, Angle ang2)
        {
            return new Angle(ang1.Radians + ang2.Radians);
        }

        public static Angle operator -(Angle ang1, Angle ang2)
        {
            return new Angle(ang1.Radians - ang2.Radians);
        }

        public static Angle operator -(Angle ang)
        {
            return new Angle(-ang.Radians);
        }

        public static Angle operator +(Angle ang, double radians)
        {
            return new Angle(ang.Radians + radians);
        }

        public static Angle operator -(Angle ang1, double radians)
        {
            return new Angle(ang1.Radians - radians);
        }

        public static bool operator >(Angle ang, double radians)
        {
            return ang.Radians > radians && 2 * System.Math.PI - ang.Radians > radians;
        }

        public static bool operator <(Angle ang, double radians)
        {
            return ang.Radians < radians || 2 * System.Math.PI - ang.Radians < radians;
        }

        public override string ToString()
        {
            return System.Math.Round(Degrees(), 1) + "deg";
        }
    }
}
