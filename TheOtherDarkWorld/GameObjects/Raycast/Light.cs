using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{

    public class Light : Raycaster
    {
        public float Brightness { get; set; }
        public Color Colour { get; set; }

        private Corners[,] _lightmap; //The lightmap is a list of the corners of each tile hit by the light

        [Flags()]
        public enum Corners : byte
        {
            Unlit = 0,
            TopLeft = 1,
            BottomLeft = 2,
            TopRight = 4,
            BottomRight = 8,
            FullyLit = (TopLeft | TopRight | BottomLeft | BottomRight)
        }

        private const float LIGHT_SCALE_DOWN = 250;
        public static Color DEFAULT_LIGHT = Color.Transparent;

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
                Tile tile = blocksLit.Pop();
                tile.ResetLighting();
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
        public Light(float Brightness, float Radius, Vector2 Position, Vector2 Direction, float Span, Color Colour, Tile[,] tiles, bool IsActive = true)
             : base(Radius, Position, Direction, Span, tiles, IsActive)
        {
            this.Brightness = Brightness;
            this.Colour = Colour;

            _lightmap = new Corners[(int)Math.Ceiling((Radius * 2) / Tile.WIDTH) + 1, (int)Math.Ceiling((Radius * 2) / Tile.HEIGHT) + 1];
        }

        protected override void OnCasted()
        {
            //
            //Apply the light from the lightmap
            //
            for (int x = startX < 0 ? -startX : 0; x < _lightmap.GetLength(0) && startX + x < _tiles.GetLength(0); x++)
            {
                for (int y = startY < 0 ? -startY : 0; y < _lightmap.GetLength(1) && startY + y < _tiles.GetLength(1); y++)
                {
                    if (_lightmap[x, y] != Corners.Unlit)
                    {
                        int combX = x + startX;
                        int combY = y + startY;


                        float blockBrightness = getLightOnTile(combX, combY);

                        if (blockBrightness > 0)
                        {
                            blocksLit.Push(_tiles[combX, combY]);

                            if (_tiles[combX, combY].Block != null)
                            {
                                //Setup a 2x2 array to store which parts of the block are lit up
                                float[,] brightnessmap = new float[2, 2];
                                if ((_lightmap[x, y] & Corners.TopLeft) > 0)
                                {
                                    brightnessmap[0, 0] = blockBrightness;
                                }
                                if ((_lightmap[x, y] & Corners.BottomLeft) > 0)
                                {
                                    brightnessmap[0, 1] = blockBrightness;
                                }
                                if ((_lightmap[x, y] & Corners.TopRight) > 0)
                                {
                                    brightnessmap[1, 0] = blockBrightness;
                                }
                                if ((_lightmap[x, y] & Corners.BottomRight) > 0)
                                {
                                    brightnessmap[1, 1] = blockBrightness;
                                }

                                //
                                //Next, superimpose this 2x2 array onto the blocks 2x2 brightness array
                                //
                                for (int i = 0; i < 2; i++)
                                    for (int j = 0; j < 2; j++)
                                    {
                                        ApplyLightToSegment(_tiles[combX, combY].Block, i, j, brightnessmap[i, j], Colour);
                                    }
                            }
                            else
                            {
                                _tiles[combX, combY].LightColour = Color.Lerp(_tiles[combX, combY].LightColour, Colour, Brightness);
                                _tiles[combX, combY].Brightness = blockBrightness;
                            }
                        }
                    }
                }
            }

            //
            //Reset the lightmap back to it's original state
            //
            _lightmap = new Corners[_lightmap.GetLength(0), _lightmap.GetLength(1)];
        }

        private void ApplyLightToSegment(Block block, int i, int j, float blockBrightness, Color lightColour)
        {
            //Lerp is used to limit the amount of colour that this light introduces based on the brightness of this light
            block.SetLightColour(i, j , Color.Lerp(lightColour, Colour, blockBrightness));
            if (block.GetBrightness(i, j) > 0)
            {
                block.SetBrightness(i, j,
                    Math.Max(blockBrightness, block.GetBrightness(i, j)) +
                    Math.Min(blockBrightness, block.GetBrightness(i, j)) / 3);
            }
            else
                block.SetBrightness(i, j, blockBrightness);
        }

        /// <summary>
        ///  Action to be performed upon each tile while the light is raycasted
        /// </summary>
        /// <param name="tileX">The x coordinate of the tile</param>
        /// <param name="tileY">The y coordinate of the tile</param>
        /// <returns>False if a blocking tile is found. This terminates the raycast algorithm</returns>
        protected override bool OnNextTile(int tileX, int tileY)
        {
            if (tileX - startX >= 0 && tileX - startX < _lightmap.GetLength(0)
                        && tileY - startY >= 0 && tileY - startY < _lightmap.GetLength(1))
            {
                if (_tiles[tileX, tileY].Block != null) // We hit a wall
                {
                    bool HitsBottom = currentRayDirection.Y < 0;
                    bool HitsTop = !HitsBottom;
                    bool HitsRight = currentRayDirection.X < 0;
                    bool HitsLeft = !HitsRight;

                    HitsLeft &= (tileX <= 0) || _tiles[tileX - 1, tileY].Block == null; //Check if the light can actually reach the left side of the block
                    HitsRight &= (tileX + 1 >= _tiles.GetLength(0)) || _tiles[tileX + 1, tileY].Block == null; //''
                    HitsTop &= (tileY <= 0) || _tiles[tileX, tileY - 1].Block == null; //''
                    HitsBottom &= (tileY + 1 >= _tiles.GetLength(1)) || _tiles[tileX, tileY + 1].Block == null; //''

                    //
                    //Check if any of the sides are being blocked by neighbouring tiles
                    //And set the bit that corresponds to the corner
                    //
                    if (HitsLeft || HitsTop)
                        _lightmap[tileX - startX, tileY - startY] |= Corners.TopLeft;
                    if (HitsLeft || HitsBottom)
                        _lightmap[tileX - startX, tileY - startY] |= Corners.BottomLeft;
                    if (HitsRight || HitsTop)
                        _lightmap[tileX - startX, tileY - startY] |= Corners.TopRight;
                    if (HitsRight || HitsBottom)
                        _lightmap[tileX - startX, tileY - startY] |= Corners.BottomRight;
                }
                else
                {
                    _lightmap[tileX - startX, tileY - startY] = Corners.FullyLit; //Every bit of the tile is hit with light
                }
            }

            //If we hit a wall, stop casting the light
            return (_tiles[tileX, tileY].Block == null);
        }

        private float getLightOnTile(int x, int y)
        {
            //Get the offset from the tile to the centre of the light
            Vector2 voffset = new Vector2((x * Tile.WIDTH) - Position.X, Position.Y - (y * Tile.HEIGHT));

            if (voffset.Length() <= Radius)
            {
                if (Radius > LIGHT_SCALE_DOWN)
                //If the radius is larger, and we scale down by only LIGHT_SCALE_DOWN then the function
                //will start increasing when we get past the value of LIGHT_SCALE_DOWN
                {
                    return Math.Abs(Brightness * (1 - voffset.Length() / Radius));
                }
                else
                {
                    return Math.Abs(Brightness * (1 - voffset.Length() / LIGHT_SCALE_DOWN));
                }
            }
            else return 0;
        }

    }
}
