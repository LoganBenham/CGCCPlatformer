using Microsoft.Xna.Framework;

//Written by Logan Benham

namespace CGCCPlatformer.Helpers.Math
{
    public static class GeometryExtensions
    {
        public static Rectangle BoundedBy(this Rectangle inner, Rectangle boundary)
        {
            if (inner.Size.X > boundary.Size.X || inner.Size.Y > boundary.Size.Y)
            {
                Logging.WriteLine(Logging.Level.Warning, "Try not to contain a rectangle with a smaller rectangle");
                return boundary;
            }

            var x = inner.X;
            if (x < boundary.X)
                x = boundary.X;
            else if (x + inner.Width > boundary.Right)
                x = boundary.Right - inner.Width;

            var y = inner.Y;
            if (y < boundary.Y)
                y = boundary.Y;
            else if (y + inner.Height > boundary.Bottom)
                y = boundary.Bottom - inner.Height;

            return new Rectangle(x, y, inner.Width, inner.Height);
        }

        public static Rectangle CutBy(this Rectangle inner, Rectangle cutter)
        {
            var left = inner.Left.ClampMin(cutter.Left);
            var right = inner.Right.ClampMax(cutter.Right);
            var top = inner.Top.ClampMin(cutter.Top);
            var bottom = inner.Bottom.ClampMax(cutter.Bottom);

            return new Rectangle(left, top, right - left, bottom - top);
        }
    }
}
