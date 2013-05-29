using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Speed { get; private set; }
        public Color Colour { get; set; }
        public Vector2 Origin { get; private set; }
        public int Resistance { get; set; }
        public virtual Rectanglef Rect { get; private set; }

        private float _brightness;
        public float Brightness
        {
            get
            {
                if (_brightness > 1) //Brightness is capped at 1
                    return 1;
                else
                    return _brightness;
            }
            set
            {
                _brightness = value;
            }
        }

        protected Color getLightColour()
        {
                //Preserve the alpha component so that the object will actually be drawn
                byte alpha = Colour.A;
                Color c = Colour * Brightness;
                c.A = alpha;
                return c;
        }


        public GameObject(Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin, int Resistance, int width, int height)
        {
            Position = startPosition - Origin;
            this.Origin = Origin;
            Speed = speed;
            Colour = colour;
            Velocity = startVelocity;
            this.Resistance = Resistance;
            this.Rect = new Rectanglef(Position.X, Position.Y, width, height);
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
                //
                //The extra 2 is added to ensure that the area checked is big enough to detect all collisions
                //
                int endX = startX + (int)(RoughRect.Width / 10) + 2;
                int endY = startY + (int)(RoughRect.Height / 10) + 2;

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
                            continue; //go to the next tile

                        //TODO: Decide if this rough check is actually necessary when only the
                        //nearby blocks are checked anyway
                        if (RoughRect.Intersects(Tiles[i, j].Rect))
                        {
                            collisions.Add(new Collision(this, Tiles[i, j], i, j));
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

                            //At this point, we know i is the index of soonest collision. We assume(until 
                            //another collision is found) that there is no collision perpendicular to this 
                            //one that happens at the same time so we set this variable to false
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
                }
            }
        }


        public virtual void CollideHorizontal(Collision col)
        {
            Position += new Vector2(col.S.X, col.S.Y);
            Velocity = new Vector2(0, Velocity.Y - (Math.Sign(Velocity.Y) * col.S.Y));
        }
        public virtual void CollideVertical(Collision col)
        {
            Position += new Vector2(col.S.X, col.S.Y);
            Velocity = new Vector2(Velocity.X - (Math.Sign(Velocity.X) * col.S.X), 0);
        }
        public virtual void CollideDiagonal(Collision col)
        {
            Position += new Vector2(col.S.X , col.S.Y); //Add this small amount to prevent the player getting stuck
            Velocity = new Vector2(Velocity.X, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Block, Position + Origin - Player.PlayerList[0].Offset, null, getLightColour(), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
        }
    }
}
