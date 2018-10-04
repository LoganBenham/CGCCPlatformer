using System;
using CGCCPlatformer.Helpers.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Written by Logan Benham

namespace CGCCPlatformer.Helpers.Graphics
{
    public static class Drawing
    {
        public static Texture2D Blit(this Color[] textureData, Texture2D overlay)
        {
            BlitInPlace(textureData, overlay);
            var texture = new Texture2D(overlay.GraphicsDevice, overlay.Width, overlay.Height);
            texture.SetData(textureData);
            //texture.GetData(textureData);
            //foreach (var color in textureData)
            //    Debug.WriteLine(color);
            return texture;
        }

        public static void BlitInPlace(this Color[] textureData, Texture2D overlay)
        {
            if (textureData.Length != overlay.Height * overlay.Width)
                throw new ArgumentOutOfRangeException();
            var foregroundData = new Color[textureData.Length];
            overlay.GetData(foregroundData);
            // var newData = new Vector4[textureData.Length];
            for (var i = 0; i < textureData.Length; i++)
            {
                textureData[i] = textureData[i].Blit(foregroundData[i]);
                //Debug.WriteLine(textureData[i]);
            }
            //Debug.WriteLine("-----Set and Get data to texture-----");
        }

        public static void Draw(this Texture2D texture, float radius, Vector2 position, Color color, bool centered = true)
        {
            if (radius >= 0.5f)
            {
                float r = centered ? radius : 0;
                Gfx.SpriteBatch.Draw(texture,
                    new Vector2(position.X - r, position.Y - r),
                    color: color,
                    scale: new Vector2(radius / (texture.Width / 2f), radius / (texture.Height / 2f)));
            }
            else
            {
                Gfx.SpriteBatch.Draw(Gfx.Pixel,
                    new Vector2((int) position.X, (int) position.Y),
                    color);
            }
        }

        public static void Draw(this Texture2D texture, Vector2 position, Color color)
        {
            Gfx.SpriteBatch.Draw(texture, position, color);
        }

        public static void Draw(this Texture2D texture, Vector2 position)
        {
            texture.Draw(position, Color.White);
        }

        public static void Draw(this Rectangle rect, Color color, Rectangle bounds)
        {
            var topLeft = new Vector2(rect.Left, rect.Top);
            var topRight = new Vector2(rect.Right - 1, rect.Top);
            var bottomLeft = new Vector2(rect.Left, rect.Bottom - 1);
            var bottomRight = new Vector2(rect.Right - 1, rect.Bottom - 1);
            DrawLine(topLeft, topRight, color, bounds); //top
            DrawLine(topRight, bottomRight, color, bounds); //right
            DrawLine(bottomLeft, bottomRight + new Vector2(1, 0), color, bounds); //bottom
            DrawLine(topLeft, bottomLeft, color, bounds); //left
        }

        public static void DrawFilled(this Rectangle rect, Color color)
        {
            Gfx.SpriteBatch.Draw(Gfx.Pixel, rect, color);
        }

        public static void DrawCircumference(Vector2 pos, float radius, Color color, Rectangle bounds,
            float lineLength = 5, int maxPoints = 200)
        {
            var points = GetCircumferencePoints(pos, radius, lineLength, maxPoints);
            DrawPath(points, color, bounds, true);
        }

        public static Vector2[] GetCircumferencePoints(Vector2 pos, float radius, float lineLength = 5, int maxPoints = 200)
        {
            if (radius < 0 || lineLength < 0)
                throw new ArgumentOutOfRangeException();
            var circumference = 2 * System.Math.PI * radius;
            var n = (int)System.Math.Ceiling(circumference / lineLength).ClampMax(maxPoints);

            var points = new Vector2[n];
            for (var i = 0; i < n; i++)
            {
                var ang = new Angle(2 * System.Math.PI * i / n);
                points[i] = pos + new Vector2(radius * (float) ang.Cosine(), radius * (float) ang.Sine());
            }

            return points;
        }

        public static void DrawPath(Vector2[] points, Color color, Rectangle bounds, bool closed,
            bool drawPoints = false)
        {
            if (drawPoints)
            {
                var inverseColor = color.Invert();
                for (var i = 0; i < points.Length - 1; i++)
                {
                    if (DrawLine(points[i], points[i + 1], color, bounds))
                        Gfx.SpriteBatch.Draw(Gfx.Pixel,
                            new Vector2((int) points[i].X, (int) points[i].Y),
                            inverseColor);
                }
                Gfx.SpriteBatch.Draw(Gfx.Pixel,
                    new Vector2((int) points[points.Length - 1].X, (int) points[points.Length - 1].Y),
                    inverseColor);
                if (closed)
                    DrawLine(points[points.Length - 1], points[0], color, bounds);
            }
            else
            {
                for (var i = 0; i < points.Length - 1; i++)
                {
                    DrawLine(points[i], points[i + 1], color, bounds);
                }
                if (closed)
                    DrawLine(points[points.Length - 1], points[0], color, bounds);
            }
        }

        public static bool DrawVerticalLine(float x, Color color, Rectangle bounds) =>
            DrawLine(new Vector2(x, bounds.Y), new Vector2(x, bounds.Bottom), color, bounds);
        public static bool DrawHorizontalLine(float y, Color color, Rectangle bounds) =>
            DrawLine(new Vector2(bounds.X, y), new Vector2(bounds.Right, y), color, bounds);

        public static bool DrawLine(Vector2 p1, Vector2 p2, Color color, Rectangle bounds)
        {
            if (!bounds.Contains(p1))
            {
                if (!bounds.Contains(p2))
                {
                    //if both points are out of bounds, then i want to eliminate this line if it doesn't intersect the bounds
                    //won't eliminate lines which are diagonal and might intersect the bounds
                    //but will eliminate the lines which could intersect bounds if extended linearly
                    if (p1.X >= bounds.Right && p2.X >= bounds.Right ||
                        p1.X <= bounds.Left && p2.X <= bounds.Left ||
                        p1.Y <= bounds.Top && p2.Y <= bounds.Top ||
                        p1.Y >= bounds.Bottom && p2.Y >= bounds.Bottom)
                        return false;
                    if (!BoundLine(ref p1, p2, bounds))
                        return false;
                }
                else
                    BoundLine(ref p1, p2, bounds);
            }
            if (!bounds.Contains(p2))
                BoundLine(ref p2, p1, bounds);

            //Tried to do primitives
            /*var list1 = new VertexPositionColor[2];
            list1[0] = new VertexPositionColor(new Vector3(p1, 0), color);
            list1[1] = new VertexPositionColor(new Vector3(p2, 0), color);
            var indices = new short[] {0, 1};

            Gfx.SpriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, list1, 0, 2,
                indices, 0, 1);
            return true;*/

            float dy = p2.Y - p1.Y;
            float dx = p2.X - p1.X;

            if (dy == 0)
            {
                if (dx > 0)
                {
                    Gfx.SpriteBatch.Draw(Gfx.Pixel, p1,
                        color: color,
                        rotation: 0,
                        scale: new Vector2(dx, 1));
                    return true;
                }
                if (dx < 0)
                {
                    Gfx.SpriteBatch.Draw(Gfx.Pixel, p2,
                        color: color,
                        rotation: 0,
                        scale: new Vector2(-dx, 1));
                    return true;
                }
                Gfx.SpriteBatch.Draw(Gfx.Pixel, p1, color);
                return true;
            }
            if (dx == 0)
            {
                if (dy > 0)
                {
                    Gfx.SpriteBatch.Draw(Gfx.Pixel, p1,
                        color: color,
                        rotation: 0,
                        scale: new Vector2(1, dy));
                    return true;
                }
                if (dy < 0)
                {
                    Gfx.SpriteBatch.Draw(Gfx.Pixel, p2,
                        color: color,
                        rotation: 0,
                        scale: new Vector2(1, -dy));
                    return true;
                }
                throw new ApplicationException("How did you get here? Control flow must be wrong.");
            }

            var rotation = (float) System.Math.Atan2(dy, dx);
            var length = (float) System.Math.Sqrt(dx * dx + dy * dy);

            Gfx.SpriteBatch.Draw(Gfx.Pixel, p1,
                color: color,
                rotation: rotation,
                scale: new Vector2(length, 1));
            return true;
        }

        private static bool BoundLine(ref Vector2 vec, Vector2 lineEnd, Rectangle bounds)
        {
            //Basically make the line equation y=mx+b
            //Then move along line from vec to lineEnd along the longest axis, until you're within the bounds
            //Giving you vec within the bounds

            float xDiff = lineEnd.X - vec.X;
            float yDiff = lineEnd.Y - vec.Y;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (xDiff == 0)
                xDiff = float.Epsilon;

            float slope = yDiff / xDiff;
            float y0 = vec.Y - slope * vec.X;

            var intersects = false;

            float leftY = y0 + slope * bounds.Left;
            if (leftY > bounds.Top && leftY < bounds.Bottom)
            {
                intersects = true;
                if (xDiff > 0) //intersects left side and vec is on left
                    vec = new Vector2(bounds.Left, leftY);
            }

            float rightY = y0 + slope * bounds.Right;
            if (rightY > bounds.Top && rightY < bounds.Bottom)
            {
                intersects = true;
                if (xDiff < 0) //intersects right side and vec is on right
                    vec = new Vector2(bounds.Right, rightY);
            }

            float topX = (bounds.Top - y0) / slope;
            if (topX > bounds.Left && topX < bounds.Right)
            {
                intersects = true;
                if (yDiff > 0) //intersects top side and vec is  on top
                    vec = new Vector2(topX, bounds.Top);
            }

            float bottomX = (bounds.Bottom - y0) / slope;
            if (bottomX > bounds.Left && bottomX < bounds.Right && yDiff < 0)
            {
                intersects = true;
                if (yDiff < 0) //intersects bottom side and vec is on bottom
                    vec = new Vector2(bottomX, bounds.Bottom);
            }

            return intersects;
        }
    }
}