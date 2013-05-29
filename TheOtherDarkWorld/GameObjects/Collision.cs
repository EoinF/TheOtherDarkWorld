using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class Collision
    {
        public Point location { get; private set; }
        public double pct { get; private set; } //The lower this number is, the earlier the collision will happen
        
        //
        //Distance from entity to block
        //

        private Vector2 _S;
        public Vector2 S { get { return _S; } }

        public bool IsHorizontal { get; private set; }

        public Collision(GameObject entitya, GameObject entityb, int i, int j)
        {
            location = new Point(i, j);

            //Get the velocity of a relative to b
            Vector2 Vab = entitya.Velocity - entityb.Velocity;

            if (Vab.X < 0)
                _S.X = entityb.Rect.Right - entitya.Rect.Left;
            else if (Vab.X > 0)
                _S.X = entityb.Rect.Left - entitya.Rect.Right;

            if (Vab.Y < 0)
                _S.Y = entityb.Rect.Bottom - entitya.Rect.Top;
            else if (Vab.Y > 0)
                _S.Y = entityb.Rect.Top - entitya.Rect.Bottom;


            //If the time taken for the y component to reach the block is higher than
            //for the x component to reach the block, then it has collided horiztontally
            //The lower time is the percentage of the velocity that was used up before colliding

            double px = Math.Abs(_S.X / Vab.X);
            double py = Math.Abs(_S.Y / Vab.Y);

            if (double.IsNaN(px))
                pct = py;
            else if (double.IsNaN(py))
                pct = px;
            else
                pct = Math.Min(px, py);

            //The collision happened along the left or right side of the objects, if horizontal is true
            IsHorizontal = (px == pct);

            if (IsHorizontal)
                _S.Y = (float)(Vab.Y * pct);
            else
                _S.X = (float)(Vab.X * pct);

            //
            //If pct > 1 then the entitya is probably already inside the entityb so let them go where they want
            //until they aren't colliding anymore. The pct is set to -1, so this will be the only collision that 
            //is actually executed
            //
            if (pct > 1)
            {
                pct = -1;
                _S.X = Vab.X;
                _S.Y = Vab.Y;
            }
        }


        /*public Collision(GameObject entity, Tile tile, int i, int j)
        {
            location = new Point(i, j);


            if (entity.Velocity.X < 0)
                _S.X = tile.Rect.Right - entity.Rect.Left;
            else if (entity.Velocity.X > 0)
                _S.X = tile.Rect.Left - entity.Rect.Right;

            if (entity.Velocity.Y < 0)
                _S.Y = tile.Rect.Bottom - entity.Rect.Top;
            else if (entity.Velocity.Y > 0)
                _S.Y = tile.Rect.Top - entity.Rect.Bottom;


            //If the time taken for the y component to reach the block is higher than
            //for the x component to reach the block, then it has collided horiztontally
            //The lower time is the percentage of the velocity that was used up before colliding

            double px = Math.Abs(_S.X / entity.Velocity.X);
            double py = Math.Abs(_S.Y / entity.Velocity.Y);

            if (double.IsNaN(px))
                pct = py;
            else if (double.IsNaN(py))
                pct = px;
            else
                pct = Math.Min(px, py);

            //The collision happened along the left or right side of the objects, if horizontal is true
            IsHorizontal = (px == pct);

            if (IsHorizontal)
                _S.Y = (float)(entity.Velocity.Y * pct);
            else
                _S.X = (float)(entity.Velocity.X * pct);

            //
            //If pct > 1 then the entity is probably already inside the tile so let it go where it wants
            //until they aren't colliding anymore. The pct is set to -1, so this will be the only collision that 
            //is actually executed
            //
            if (pct > 1)
            {
                pct = -1;
                _S.X = entity.Velocity.X;
                _S.Y = entity.Velocity.Y;
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1">The moving rectangle</param>
        /// <param name="r2">The stationary rectangle</param>
        /// <param name="Velocity">The velocity of r1</param>
        /// <returns>True if they collide</returns>
        public static bool SquareVsSquare(Rectanglef r1, Rectanglef r2, Vector2 VelocityA, Vector2 VelocityB)
        {
            //Get the velocity of A relative to B
            Vector2 Velocity = VelocityA - VelocityB;
            return (r2.Intersects(new Rectanglef(r1.Left, r1.Top + (Velocity.Y < 0 ? Velocity.Y : 0), r1.Width, r1.Height + Math.Abs(Velocity.Y)))
            || r2.Intersects(new Rectanglef(r1.Left + (Velocity.X < 0 ? Velocity.X : 0), r1.Top, r1.Width + Math.Abs(Velocity.X), r1.Height)));

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
