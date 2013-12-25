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

        private bool[,] _lightmap;

        private const float LIGHT_SCALE_DOWN = 250;
        private static Color DEFAULT_LIGHT = Color.Transparent;

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
                tile.Brightness = 0;
                tile.LightColour = DEFAULT_LIGHT;
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

            _lightmap = new bool[(int)Math.Ceiling((Radius * 2) / Tile.WIDTH) + 1, (int)Math.Ceiling((Radius * 2) / Tile.HEIGHT) + 1];
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
                    if (_lightmap[x, y])
                    {
                        int combX = x + startX;
                        int combY = y + startY;

                        float tileBrightness = getLightOnTile(combX, combY);
                        if (tileBrightness > 0)
                        {
                            _tiles[combX, combY].LightColour = Color.Lerp(_tiles[combX, combY].LightColour, Colour, 0.5f);
                            //Lerp is used to limit the amount of colour that this light introduces based on the brightness of this light

                            if (_tiles[combX, combY].Brightness > 0)
                            {
                                _tiles[combX, combY].Brightness =
                                    Math.Max(tileBrightness, _tiles[combX, combY].Brightness) +
                                    Math.Min(tileBrightness, _tiles[combX, combY].Brightness) / 3;
                            }
                            else
                                _tiles[combX, combY].Brightness = tileBrightness;

                            blocksLit.Push(_tiles[combX, combY]);
                        }
                    }
                }
            }



            //
            //Reset the lightmap back to it's original state
            //
            _lightmap = new bool[_lightmap.GetLength(0), _lightmap.GetLength(1)];
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
                _lightmap[tileX - startX, tileY - startY] = true;
            }

            //If we hit a wall, stop casting the light
            return (_tiles[tileX, tileY].Block == null);
        }

        private float getLightOnTile(int x, int y)
        {
            //Get the offset from the tile to the centre of the light
            Vector2 voffset = new Vector2((x * Tile.WIDTH) - Position.X, Position.Y - (y * Tile.HEIGHT));

            if (voffset.Length() > Radius)
                return 0;//The tile is outside the radius of the light so it has 0 brightness
            else
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
        }

    }
}
