using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld
{
    public class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Speed { get; private set; }
        public Color Colour { get; protected set; }
        public Vector2 Origin { get; private set; }
        public int Resistance { get; set; }
        public virtual Rectanglef Rect { get; private set; }

        public GameObject(Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin, int Resistance)
        {
            Position = startPosition - Origin;
            this.Origin = Origin;
            Speed = speed;
            Colour = colour;
            Velocity = startVelocity;
            this.Resistance = Resistance;
        }


        public void CheckCollisions()
        {
            Block blk;
            for (int checks = 0; checks < 2 && Velocity != Vector2.Zero; checks++)
            {
                List<Collision> collisions = new List<Collision>();

                //This rectangle is initialised here because it doesn't change until all the blocks have been checked
                Rectanglef RoughRect = new Rectanglef(Rect.Left + (Velocity.X < 0 ? Velocity.X : 0), Rect.Top + (Velocity.Y < 0 ? Velocity.Y : 0), Rect.Width + Math.Abs(Velocity.X), Rect.Height + Math.Abs(Velocity.Y));

                //
                //The square in the array that the object is contained in with 3 subtracted
                //
                int arrayPositionX = (int)(Position.X / 10) - 3;
                int arrayPositionY = (int)(Position.Y / 10) - 3;

                for (int i = arrayPositionX; i < arrayPositionX + 10; i++)
                {
                    //Check if the index is out of bounds
                    if (i >= Level.CurrentLevel.Width)
                        break;
                    if (i < 0)
                        continue;

                    for (int j = arrayPositionY; j < arrayPositionY + 6; j++)
                    {
                        //Check if the index is out of bounds
                        if (j >= Level.CurrentLevel.Height)
                            break;
                        if (j < 0)
                            continue;

                        blk = Level.CurrentLevel.BlockList[i, j];

                        if (blk == null) //If there is no block here,
                            continue; //go to the next block

                        //TODO: Decide if this rough check is actually necessary when only the
                        //nearby blocks are checked anyway
                        if (RoughRect.Intersects(blk.Rect))
                        {
                            //At this stage, there's a chance of a collision because they were close

                            if (Collision.SquareVsSquare_OneMoving(Rect, blk.Rect, Velocity))
                            {
                                //At this stage, there definitely is a collision
                                collisions.Add(GetCollisionDetails(blk.Rect, i, j));
                            }
                        }
                    }
                }

                    if (collisions.Count == 0) //There were no collisions
                    {
                        Position += Velocity; //The object has moved the full distance available without colliding
                        return;
                    }
                    else
                    {
                        int lowestIndex = 0;

                        //Start at 1, because we assume that the 0 index is the lowest first.
                        for (int i = 1; i < collisions.Count; i++)
                        {
                            if (collisions[i].pct < collisions[lowestIndex].pct)
                                lowestIndex = i;
                        }

                        if (collisions[lowestIndex].IsHorizontal)
                            CollideHorizontal(collisions[lowestIndex]);
                        else
                            CollideVertical(collisions[lowestIndex]);

                        Colour = Color.Red;
                    }
            }
        }


        
    
        public virtual void CollideHorizontal(Collision col)
        {
            Position += Velocity * col.pct;
            Velocity = new Vector2(0, Velocity.Y * (1 - col.pct));
        }

        public virtual void CollideVertical(Collision col)
        {
            Position += Velocity * col.pct;
            Velocity = new Vector2(Velocity.X * (1 - col.pct), 0);
        }

        public virtual Collision GetCollisionDetails(Rectanglef rect, int i, int j)
        {
            float Sx = 0, Sy = 0; //Distance of the player from the block

            if (Velocity.X < 0)
                Sx = this.Position.X - rect.Right;
            else if (Velocity.X > 0)
                Sx = rect.Left - (this.Position.X + this.Rect.Width);

            if (Velocity.Y < 0)
                Sy = this.Position.Y - rect.Bottom;
            else if (Velocity.Y > 0)
                Sy = rect.Top - (this.Position.Y + this.Rect.Height);


            //If the time taken for the y component to reach the block is higher than
            //for the x component to reach the block, then it has collided horiztontally
            //The lower time is the percentage of the velocity that was used up before colliding

            float pct = 0;

            float px = Math.Abs(Sx / Velocity.X);
            float py = Math.Abs(Sy / Velocity.Y);

            if (float.IsNaN(px))
                pct = py;
            else if (float.IsNaN(py))
                pct = px;
            else
                pct = Math.Min(px, py);


            if (pct > 1)// || (Velocity.Y < 0))
                pct += 0;

            //if (Math.Abs(pct) <= Math.Abs(Velocity.X) || Math.Abs(pct) <= Math.Abs(Velocity.Y))
            return new Collision(new Point(i, j), pct, px == pct);
        }
    }
}
