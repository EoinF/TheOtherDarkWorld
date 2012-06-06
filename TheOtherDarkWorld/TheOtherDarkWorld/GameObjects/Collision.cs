using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld
{
    public class Collision
    {
        public Point block { get; private set; }
        public float pct { get; private set; } //The lower this number is, the earlier the collision will happen
        public bool IsHorizontal { get; set; }

        public Collision(Point block, float pct, bool IsHorizontal)
        {
            this.block = block;
            this.pct = pct;
            this.IsHorizontal = IsHorizontal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1">The moving rectangle</param>
        /// <param name="r2">The stationary rectangle</param>
        /// <param name="Velocity">The velocity of r1</param>
        /// <returns>True if they collide</returns>
        public static bool SquareVsSquare_OneMoving(Rectanglef r1, Rectanglef r2, Vector2 Velocity)
        {

            return (r2.Intersects(new Rectanglef(r1.Left, r1.Top + (Velocity.Y < 0 ? Velocity.Y : 0), r1.Width, r1.Height + Math.Abs(Velocity.Y)))
            || r2.Intersects(new Rectanglef(r1.Left + (Velocity.X < 0 ? Velocity.X : 0), r1.Top, r1.Width + Math.Abs(Velocity.X), r1.Height)));


            /*
            if (Velocity.X == 0)
            {
                return r2.Intersects(new Rectangle(r1.Left, r1.Top + (Velocity.Y < 0 ? (int)Velocity.Y : 0), r1.Width, r1.Height + (Velocity.Y > 0 ? (int)Velocity.Y : 0)));
            }
            else if (Velocity.Y == 0)
            {
                return r2.Intersects(new Rectangle(r1.Left + (Velocity.X < 0 ? (int)Velocity.X : 0), r1.Top, r1.Width + (Velocity.X > 0 ? (int)Velocity.X : 0), r1.Height));
            }
            else
            {
                float slope = Math.Abs(Velocity.Y / Velocity.X);

                //Using the perpendicular distance formula we can find out if the vector is between the top right and bottom left vertices.
                float d1 = ((slope * r2.Right) - r2.Top + r1.Center.Y - (r1.Center.X * slope)) / (float)Math.Sqrt((slope * slope) + 1);
                float d2 = ((slope * r2.Left) - r2.Bottom + +r1.Center.Y - (r1.Center.X * slope)) / (float)Math.Sqrt((slope*slope) + 1);

                if (d1 + r1.Width / 2 >= 0 && d2 - r1.Width / 2 <= 0)
                    return true;

                //Using the perpendicular distance formula we can find out if the vector is between the top left and bottom right vertices.
                d1 = ((slope * r2.Left) - r2.Top + r1.Center.Y - (r1.Center.X * slope)) / (float)Math.Sqrt((slope * slope) + 1);
                d2 = ((slope * r2.Right) - r2.Bottom + r1.Center.Y - (r1.Center.X * slope)) / (float)Math.Sqrt((slope * slope) + 1);

                if (d1 + r1.Width / 2 < 0 && d1 - r2.Width / 2 > 0)
                    return true;

                return false;
            }
             */
        } /// <summary>
        /// 
        /// </summary>
        /// <param name="r1">The moving rectangle</param>
        /// <param name="r2">The stationary rectangle</param>
        /// <param name="Velocity">The velocity of r1</param>
        /// <returns>True if they collide</returns>
        public static bool SquareVsSquare_TwoMoving(Rectanglef r1, Rectanglef r2, Vector2 Velocity1, Vector2 Velocity2)
        {
            Rectanglef r2V = new Rectanglef(r2.Left + (Velocity2.X < 0 ? 0 : Velocity2.X), r2.Top + (Velocity2.Y < 0 ? 0 : Velocity2.Y), r2.Width + Math.Abs(Velocity2.X), r2.Height + Math.Abs(Velocity2.Y));

            return (r2V.Intersects(new Rectanglef(r1.Left, r1.Top + (Velocity1.Y < 0 ? Velocity1.Y : 0), r1.Width, r1.Height + Math.Abs(Velocity1.Y)))
            || r2V.Intersects(new Rectanglef(r1.Left + (Velocity1.X < 0 ? Velocity1.X : 0), r1.Top, r1.Width + Math.Abs(Velocity1.X), r1.Height)));
        }
    }


    public struct Rectanglef
    {
        public float Left;
        public float Top;
        public float Width;
        public float Height;
        

        public float Right;
        public float Bottom;

        public Vector2 Center;

        public Rectanglef(float Left, float Top, float Width, float Height)
        {
            this.Left = Left;
            this.Top = Top;
            this.Bottom = Top + Height;
            this.Right = Left + Width;

            Center = new Vector2(Left + (Width / 2f), Top + (Height / 2f));

            this.Width = Width;
            this.Height = Height;
        }

        public bool Intersects(Rectanglef r2)
        {
            return this.Right > r2.Left
                && this.Left < r2.Right
                && this.Bottom > r2.Top
                && this.Top < r2.Bottom;
        }
    }
}
