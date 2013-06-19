using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Light
    {
        public float Brightness { get; private set; }
        public float Radius { get; private set; }
        public Color Colour { get; private set; }

        public Vector2 Position { get; private set; }
        private Vector2 direction;
        private float span;
        bool[,] lightmap;

        private int startX, startY, centreX, centreY, endX, endY;

        #region Internal Reset Logic

        static Light()
        {
            //Static constructor ensures this stack is only initialized once ever in the program
            blocksLit = new Stack<Tile>();
        }

        private static Stack<Tile> blocksLit;

        public static void ResetBrightness()
        {
            while (blocksLit.Count > 0)
            {
                Tile block = blocksLit.Pop();
                block.Brightness = 0;
                block.LightColour = block.Colour;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Brightness">The depth the light will reach before having no effect</param>
        /// <param name="Position">The location the light is projected from</param>
        /// <param name="Direction">The direction the light will go in</param>
        /// <param name="Span">The amount of space the light will span, i.e. higher values -> wider area covered
        /// \n []</param>
        public Light(float Brightness, float Radius, Vector2 Position, Vector2 Direction, float Span, Color Colour)
        {
            this.direction = Vector2.Normalize(Direction);
            this.Position = Position;
            this.Brightness = Brightness;
            this.Radius = Radius;
            this.span = Span;
            this.Colour = Colour;

            lightmap = new bool[(int)Math.Ceiling((Radius * 2) / Tile.WIDTH) + 1, (int)Math.Ceiling((Radius * 2) / Tile.HEIGHT) + 1];

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            startX = (int)((Position.X - Radius) / Tile.WIDTH);
            startY = (int)((Position.Y - Radius) / Tile.HEIGHT);

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            endX = (int)Math.Ceiling((Position.X + Radius) / Tile.WIDTH);
            endY = (int)Math.Ceiling((Position.Y + Radius) / Tile.HEIGHT);

            //
            //The coordinates of the light source itself
            //
            centreX = (int)(Position.X / 10);
            centreY = (int)(Position.Y / 10);
        }

        /// <summary>
        /// Update the position and direction of the light
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="newDirection"></param>
        /// <param name="tiles"></param>
        public void Update(Vector2 newPosition, Vector2 newDirection, Tile[,] tiles)
        {
            this.direction = Vector2.Normalize(newDirection);
            this.Position = newPosition;

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            startX = (int)((Position.X - Radius) / Tile.WIDTH);
            startY = (int)((Position.Y - Radius) / Tile.HEIGHT);

            //
            //The upper left corner of the square that contains the circle projected by the light
            //
            endX = (int)Math.Ceiling((Position.X + Radius) / Tile.WIDTH);
            endY = (int)Math.Ceiling((Position.Y + Radius) / Tile.HEIGHT);

            //
            //The coordinates of the light source itself
            //
            centreX = (int)(Position.X / 10);
            centreY = (int)(Position.Y / 10);
        }

        /// <summary>
        /// Calculates what tiles are affected by this light during this frame
        /// </summary>
        public void Cast(Tile[,] tiles)
        {
            if (InputManager.JustReleased(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                StateManager.DebugMode = !StateManager.DebugMode;
            }

            //if (StateManager.DebugMode)
            //    rayCast(tiles, InputManager.MousePositionP.X / 10, InputManager.MousePositionP.Y / 10, 0, 0);
            //else
            {

                //
                //Use the raycast algorithm on the outer edges of the light source
                //
                for (int i = startX; i < endX; i++)
                {
                    if (StateManager.DebugMode)
                    {
                        rayCast(tiles, i, startY, 1, 1);
                        rayCast(tiles, i, endY - 1, 1, 1);
                    }
                    else
                    {
                        rayCast(tiles, i, startY, 50, 50);
                        rayCast(tiles, i, endY - 1, 50, 50);
                    }
                }

                for (int j = startY + 1; j < endY - 1; j++)
                {
                    if (StateManager.DebugMode)
                    {
                        rayCast(tiles, startX, j, 1, 1);
                        rayCast(tiles, endX - 1, j, 1, 1);
                    }
                    else
                    {
                        rayCast(tiles, startX, j, 50, 50);
                        rayCast(tiles, endX - 1, j, 50, 50);
                    }
                }
            }

            //
            //Next, apply the light from the lightmap
            //
            for (int x = startX < 0 ? -startX : 0; x < lightmap.GetLength(0) && startX + x < tiles.GetLength(0); x++)
            {
                for (int y = startY < 0 ? -startY : 0; y < lightmap.GetLength(1) && startY + y < tiles.GetLength(1); y++)
                {
                    if (lightmap[x, y])
                    {
                        int combX = x + startX;
                        int combY = y + startY;

                        tiles[combX, combY].LightColour = Color.Lerp(tiles[combX, combY].LightColour, Colour, 0.4f);
                        tiles[combX, combY].Brightness += getLightOnTile(combX, combY);
                        blocksLit.Push(tiles[combX, combY]);
                    }
                }
            }


            //
            //Reset the lightmap back to it's original state
            //
            lightmap = new bool[lightmap.GetLength(0), lightmap.GetLength(1)];
        }

        private void rayCast(Tile[,] tiles, int tileX, int tileY, float tileOffsetX, float tileOffsetY)
        {
            //Get the offset from the tile to the centre of the light
            Vector2 voffset = new Vector2((tileX * Tile.WIDTH) - Position.X, Position.Y - (tileY * Tile.HEIGHT));
            //Calculate the angle between the direction and this offset
            double angle = Math.Acos(Vector2.Dot(voffset, direction) / (voffset.Length() * direction.Length()));

            //If the angle is small enough, this tile is within the span of the light
            if (angle <= span)
            {
                //
                //Check each square from the light source to the destination tile until a blocking square is found; using the Bresenham line algorithm
                //
                Point dest = new Point(tileX + (int)(tileOffsetX / Tile.WIDTH), tileY + (int)(tileOffsetY / Tile.HEIGHT));
                Point src = new Point ((int)(Position.X / Tile.WIDTH), (int)(Position.Y / Tile.HEIGHT));

                bool isSteep = Math.Abs(dest.Y - src.Y) > Math.Abs(dest.X - src.X);
                if (isSteep)
                {
                    int temp = src.X;
                    src.X = src.Y;
                    src.Y = temp;

                    temp = dest.X;
                    dest.X = dest.Y;
                    dest.Y = temp;
                }

                float deltaX = Math.Abs(dest.X - src.X);
                float deltaY = Math.Abs(dest.Y - src.Y);

                float error = deltaX / 2;
                int y = src.Y;

                int xstep = src.X > dest.X ? -1 : 1;
                int ystep = src.Y > dest.Y ? -1 : 1;

                for (int x = src.X; x != dest.X; x+= xstep)
                {
                    int i;
                    int j;
                    if (isSteep)
                    {
                        i = y;
                        j = x;
                    }
                    else
                    {
                        i = x;
                        j = y;
                    }

                    if (i - startX >= 0 && i - startX < lightmap.GetLength(0)
                        && j - startY >= 0 && j - startY < lightmap.GetLength(1))
                    {
                        lightmap[i - startX, j - startY] = true;

                        //If we hit a wall, stop casting the light
                        if (tiles[i, j].Block != null)
                            return;

                    }
                    else
                        error += 0;
                    error -= deltaY;

                    if (error < 0)
                    {
                        y += ystep;
                        error += deltaX;
                    }

                }
            }
        }

        private float getLightOnTile(int x, int y)
        {
            //Get the offset from the tile to the centre of the light
            Vector2 voffset = new Vector2((x * Tile.WIDTH) - Position.X, Position.Y - (y * Tile.HEIGHT));
                        
            return Math.Abs(Brightness * (Radius - voffset.Length()) / Radius);
        }

        private double getAngleBetweenVectors(Vector2 Va, Vector2 Vb)
        {
            return (Math.Abs(Math.Atan2(Va.X, Va.Y) - Math.Atan2(Vb.X, Vb.Y)));
        }

    }
}
