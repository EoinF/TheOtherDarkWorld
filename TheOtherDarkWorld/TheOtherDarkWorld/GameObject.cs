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
        public virtual Rectangle Rect { get; private set; }

        public GameObject(Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin)
        {
            Position = startPosition - Origin;
            Speed = speed;
            Colour = colour;
            Velocity = startVelocity;
        }

        
        public void CheckCollisions()
        {
            Block blk;
            for (int checks = 0; checks < 2 && Velocity != Vector2.Zero; checks++)
            {
                List<Collision> collisions = new List<Collision>();

                //This rectangle is initialised here because it doesn't change until all the blocks have been checked
                Rectangle RoughRect = new Rectangle(Rect.X + (Velocity.X < 0 ? (int)Velocity.X : 0), Rect.Y + (Velocity.Y < 0 ? (int)Velocity.Y : 0), Rect.Width + (Velocity.X > 0 ? (int)Velocity.X : 0), Rect.Height + (Velocity.Y > 0 ? (int)Velocity.Y : 0));

                for (int i = 0; i < Level.CurrentLevel.BlockList.GetLength(0); i++)
                    for (int j = 0; j < Level.CurrentLevel.BlockList.GetLength(1); j++)
                    {
                        blk = Level.CurrentLevel.BlockList[i, j];

                        if (blk == null) //If there is no block here,
                            continue; //go to the next block

                        if (Collision.SquareVsSquare_Quick(RoughRect, blk.Rect))
                        {
                            if (Collision.SquareVsSquare_Moving(Rect, blk.Rect, Velocity))
                            {
                                float Sx = 0, Sy = 0; //Distance of the player from the block


                                if (Velocity.X > 0)
                                    Sx = blk.Rect.Left - (this.Position.X + (this.Rect.Width));
                                else if (Velocity.X < 0)
                                    Sx = (this.Position.X) - blk.Rect.Right;

                                if (Velocity.Y < 0)
                                    Sy = (this.Position.Y) - blk.Rect.Bottom;
                                else if (Velocity.Y > 0)
                                    Sy = blk.Rect.Top - (this.Position.Y + (this.Rect.Height));


                                //If the time taken for the y component to reach the block is higher than
                                //for the x component to reach the block, then it has collided horiztontally
                                //The lower time is the percentage of the velocity that was used up before colliding

                                float pct = 0;

                                float px = Sx / Velocity.X;
                                float py = Sy / Velocity.Y;

                                if (float.IsNaN(px))
                                    pct = py;
                                else if (float.IsNaN(py))
                                    pct = px;
                                else
                                    pct = Math.Min(Math.Abs(px), Math.Abs(py));

                                
                                if (float.IsNaN(pct) || float.IsInfinity(pct) || (pct > 3.9f && pct < 5))// || (Velocity.Y < 0))
                                    pct += 0;

                                //if (Math.Abs(pct) <= Math.Abs(Velocity.X) || Math.Abs(pct) <= Math.Abs(Velocity.Y))
                                   collisions.Add(new Collision(new Point(i, j), pct, px == pct));
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
                    
                    //Count the number that result in the same amount of distance moved
                    int identicalHor = 0;
                    int identicalVer = 0;

                    //Start at 1, because we assume that the 0 index is the lowest first.
                    for (int i = 1; i < collisions.Count; i++)
                    {
                        if (collisions[i].pct < collisions[lowestIndex].pct)
                            lowestIndex = i;
                        else if (collisions[i].pct == collisions[lowestIndex].pct)
                        {
                            if (collisions[i].IsHorizontal)
                                identicalHor++;
                            else
                                identicalVer++;
                        }
                    }


                    if (collisions.Count - 1 == identicalHor + identicalVer && identicalHor != identicalVer)
                    {
                        collisions[lowestIndex].IsHorizontal = identicalHor > identicalVer;
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
            Velocity = new Vector2(0, Velocity.Y);
        }

        public virtual void CollideVertical(Collision col)
        {
            Position += Velocity * col.pct;
            Velocity = new Vector2(Velocity.X, 0);
        }

    }
}
