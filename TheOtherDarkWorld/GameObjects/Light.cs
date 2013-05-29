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

        public Vector2 Position { get; private set; }
        private Vector2 direction;
        private float span;

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
                blocksLit.Pop().Brightness = 0;
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
        public Light(float Brightness, float Radius, Vector2 Position, Vector2 Direction, float Span)
        {
            this.direction = Vector2.Normalize(Direction);
            this.Position = Position;
            this.Brightness = Brightness;
            this.Radius = Radius;
            this.span = Span;
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
            //Out of Bounds checking
            //
            if (startX < 0)
                startX = 0;
            if (endX > tiles.GetLength(0))
                endX = tiles.GetLength(0);
            if (startY < 0)
                startY = 0;
            if (endY > tiles.GetLength(1))
                endY = tiles.GetLength(1);

            //
            //The coordinates of the light source itself
            //
            centreX = (int)(Position.X / 10);
            centreY = (int)(Position.Y / 10);
        }

        /// <summary>
        /// Calculates what tiles are affected by this light in this frame
        /// </summary>
        public void Cast(Tile[,] tiles)
        {
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    //First calculate the light from the top left corner of the tile
                    float light = calculateTileLight(tiles, i, j, 1, 1);

                    if (light == 0) //If the path was blocked, try again for the bottom right corner of the tile
                        light = calculateTileLight(tiles, i, j, Tile.WIDTH - 1, Tile.HEIGHT - 1);
                    

                    if (light != 0)
                    {
                        float spread = light * 0.1f;

                        tiles[i, j].Brightness += light;
                        blocksLit.Push(tiles[i, j]); //Add it to the list of lit tiles
                    }
                }
            }
        }

        private float calculateTileLight(Tile[,] tiles, int tileX, int tileY, float tileOffsetX, float tileOffsetY)
        {
            //Get the offset from the tile to the centre of the light
            Vector2 voffset = new Vector2((tileX * Tile.WIDTH) - Position.X, Position.Y - (tileY * Tile.HEIGHT));
            //Calculate the angle between the direction and this offset
            double angle = Math.Acos(Vector2.Dot(voffset, direction) / (voffset.Length() * direction.Length()));

            //If the angle is small enough, this tile is within the span of the light
            if (angle <= span)
            {
                //
                //Check each square on the way back to the source until a blocking square is found
                //
                Vector2 current = new Vector2((tileX * Tile.WIDTH) + tileOffsetX, (tileY * Tile.HEIGHT) + tileOffsetY);

                float Sx = Position.X - current.X;
                float Sy = Position.Y - current.Y;
                float Tx, Ty;


                if (Sx != 0 || Sy != 0) //Make sure that the distance from this block to the centre isn't 0
                {
                    do
                    {
                        //Find the time taken to reach the next block
                        Tx = Math.Abs(((int)((current.X / Tile.WIDTH) + 1) - (current.X / Tile.WIDTH)) / (Sx / Tile.WIDTH));
                        Ty = Math.Abs(((int)((current.Y / Tile.HEIGHT) + 1) - (current.Y / Tile.HEIGHT)) / (Sy / Tile.HEIGHT));

                        //Whichever direction reaches the next block first, increment current by that
                        if (Tx < Ty)
                        {
                            current.X += Sx * Tx;
                            current.Y += Sy * Tx;
                        }
                        else
                        {
                            current.X += Sx * Ty;
                            current.Y += Sy * Ty;
                        }

                        if (tiles[(int)(current.X / 10), (int)(current.Y / 10)].Block != null)
                        {
                            //There is a block in the way
                            return 0;
                        }
                    }
                    //Keep checking until we are inside the centre block
                    while (Math.Abs(current.X - Position.X) > Tile.WIDTH || Math.Abs(current.Y - Position.Y) > Tile.HEIGHT);
                }

                return getLightOnTile(voffset);
            }
            return 0;
        }

        private float getLightOnTile(Vector2 voffset)
        {
            return Brightness * (Radius - voffset.Length()) / Radius;
        }

        private double getAngleBetweenVectors(Vector2 Va, Vector2 Vb)
        {
            return (Math.Abs(Math.Atan2(Va.X, Va.Y) - Math.Atan2(Vb.X, Vb.Y)));
        }

    }
}
