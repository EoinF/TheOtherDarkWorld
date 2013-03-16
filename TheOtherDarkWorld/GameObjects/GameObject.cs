using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
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


        public void CheckCollisions(Tile[,] Tiles)
        {
            for (int checks = 0; checks < 2 && Velocity != Vector2.Zero; checks++)
            {
                List<Collision> collisions = new List<Collision>();
                //This rectangle is initialised here because it doesn't change until all the blocks have been checked
                Rectanglef RoughRect = new Rectanglef(Rect.Left + (Velocity.X < 0 ? Velocity.X : 0), Rect.Top + (Velocity.Y < 0 ? Velocity.Y : 0), Rect.Width + Math.Abs(Velocity.X), Rect.Height + Math.Abs(Velocity.Y));


                //The index of the tile in the array that the rectangle begins at
                int startX = (int)(RoughRect.Left / 10);
                int startY = (int)(RoughRect.Top / 10);


                //Check if the start index is out of bounds
                if (startX < 0)
                    startX = 0;
                if (startY < 0)
                    startY = 0;

                //The index of the tile in the array that the rectangle ends at
                int endX = startX + (int)(RoughRect.Width / 10) + 1;
                int endY = startY + (int)(RoughRect.Height / 10) + 1;

                //Check if the end index is out of bounds
                if (endX >= Tiles.GetLength(0))
                    endX = Tiles.GetLength(0);
                if (endY >= Tiles.GetLength(1))
                    endY = Tiles.GetLength(1);


                //
                //The index should never be out of bounds at this stage
                //No further checking is necessary (so that should improve performance)
                //

                for (int i = startX; i < endX; i++)
                    for (int j = startY; j < endY; j++)
                    {
                        if (Tiles[i, j].Block == null) //If there is no block here,
                            continue; //go to the next block

                        //TODO: Decide if this rough check is actually necessary when only the
                        //nearby blocks are checked anyway
                        if (RoughRect.Intersects(Tiles[i, j].Rect))
                        {
                            collisions.Add(GetCollisionDetails(Tiles[i, j].Rect, i, j));
                        }
                    }
                

                if (collisions.Count == 0) //There were no collisions
                {
                    Position += Velocity; //The object has moved the full distance available without colliding
                    return;
                }
                else
                {
                    //
                    //One important thing to note about the following is that a vertical and horizontal collision
                    //can happen at the same time. So one or the other is not enough. This has to be checked for.
                    //

                    //The least distance. (The soonest collision will get processed first)
                    int lowestIndex = 0;
                    bool collidingDiagonal = false;

                    //Start at 1, because we assume that the 0 index is the lowest first.
                    for (int i = 1; i < collisions.Count; i++)
                    {
                        if (collisions[i].pct < collisions[lowestIndex].pct)
                        {
                            lowestIndex = i;

                            //At this point, we know i is the index of soonest collision. We assume that there is
                            //no collision perpendicular to this one that happens at the same time so we set
                            //this variable to false
                            collidingDiagonal = false;
                        }
                        else if (collisions[i].pct == collisions[lowestIndex].pct)
                        {
                            //If they are colliding in different directions
                            if (collisions[i].IsHorizontal != collisions[lowestIndex].IsHorizontal)
                                collidingDiagonal = true;
                        }
                    }

                    if (collidingDiagonal)
                        CollideDiagonal(collisions[lowestIndex]);
                    else if (collisions[lowestIndex].IsHorizontal)
                        CollideHorizontal(collisions[lowestIndex]);
                    else
                        CollideVertical(collisions[lowestIndex]);

                    if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                        System.Diagnostics.Debugger.Break();
                }
            }

        }


        public virtual void CollideHorizontal(Collision col)
        {
            Position += new Vector2(col.Sx, col.Sy);
            Velocity = new Vector2(0, Velocity.Y - (Math.Sign(Velocity.X) * col.Sy));
        }
        public virtual void CollideVertical(Collision col)
        {
            Position += new Vector2(col.Sx, col.Sy);
            Velocity = new Vector2(Velocity.X - (Math.Sign(Velocity.X) * col.Sx), 0); 
        }
        public virtual void CollideDiagonal(Collision col)
        {
            Position += new Vector2(col.Sx, col.Sy);
            Velocity = new Vector2(Velocity.X - (Math.Sign(Velocity.X) * col.Sx), 0);
        }

        public virtual Collision GetCollisionDetails(Rectanglef rect, int i, int j)
        {
            float Sx = 0, Sy = 0; //Distance of the player from the block

            if (Velocity.X < 0)
                Sx = rect.Right - this.Rect.Left;
            else if (Velocity.X > 0)
                Sx = rect.Left - this.Rect.Right;

            if (Velocity.Y < 0)
                Sy = rect.Bottom - this.Rect.Top;
            else if (Velocity.Y > 0)
                Sy = rect.Top - this.Rect.Bottom;


            //If the time taken for the y component to reach the block is higher than
            //for the x component to reach the block, then it has collided horiztontally
            //The lower time is the percentage of the velocity that was used up before colliding

            double pct = 0;

            double px = Math.Abs(Sx / Velocity.X);
            double py = Math.Abs(Sy / Velocity.Y);

            if (double.IsNaN(px))
                pct = py;
            else if (double.IsNaN(py))
                pct = px;
            else
                pct = Math.Min(px, py);

            //The collision happened along the left or right side of the objects, if horizontal is true
            bool Horizontal = (px == pct);

            if (Horizontal)
                Sy = (float)(Velocity.Y * pct);
            else
                Sx = (float)(Velocity.X * pct);

            //
            //The player is probably already inside a block so let them go where they want
            //The pct passed in is -1, so this will be the only collision that is actually executed
            //
            if (pct > 1)// || (Velocity.Y < 0))
                return new Collision(new Point(i, j), -1, Velocity.X, Velocity.Y, Horizontal);

            //if (Math.Abs(pct) <= Math.Abs(Velocity.X) || Math.Abs(pct) <= Math.Abs(Velocity.Y))
            return new Collision(new Point(i, j), pct, Sx, Sy, Horizontal);
        }
    }
}
