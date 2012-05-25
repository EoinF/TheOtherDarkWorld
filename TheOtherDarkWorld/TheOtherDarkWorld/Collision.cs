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

        public static bool SquareVsSquare_Quick(Rectangle r1, Rectangle r2)
        {
            return (r1.Left < r2.Right
                        && r1.Right > r2.Left
                        && r1.Top < r2.Bottom
                        && r1.Bottom > r2.Top);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1">The moving rectangle</param>
        /// <param name="r2">The stationary rectangle</param>
        /// <param name="Velocity">The velocity of r1</param>
        /// <returns>True if they collide</returns>
        public static bool SquareVsSquare_Moving(Rectangle r1, Rectangle r2, Vector2 Velocity)
        {
            return (r2.Intersects(new Rectangle(r1.Left, r1.Top + (Velocity.Y < 0 ? (int)Velocity.Y : 0), r1.Width, r1.Height + (Velocity.Y > 0 ? (int)Velocity.Y : 0)))
            || r2.Intersects(new Rectangle(r1.Left + (Velocity.X < 0 ? (int)Velocity.X : 0), r1.Top, r1.Width + (Velocity.X > 0 ? (int)Velocity.X : 0), r1.Height)));
        

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
            }*/
        }
    }
}
