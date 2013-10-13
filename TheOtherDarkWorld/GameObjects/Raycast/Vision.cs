using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Vision : Raycaster
    {
        private int sizeX, sizeY;

        #region Internal Reset Logic

        static Vision()
        {
            //Static constructor ensures this stack is only initialized once ever in the program
            blocksVisible = new Stack<Tile>();
        }

        private static Stack<Tile> blocksVisible;

        public static void ResetVision()
        {
            while (blocksVisible.Count > 0)
            {
                Tile tile = blocksVisible.Pop();
                tile.IsVisible = false;
            }
        }
        #endregion

        public Vision(float Radius, Vector2 Position, Vector2 Direction, float Span, Tile[,] tiles, bool IsActive = true)
            : base(Radius, Position, Direction, Span, tiles, IsActive)
        {
            sizeX = (int)Math.Ceiling((Radius * 2) / Tile.WIDTH) + 1;
            sizeY = (int)Math.Ceiling((Radius * 2) / Tile.HEIGHT) + 1;
        }

        protected override void OnCasted()
        {
            //No need to do anything (Already set visibility in the OnNextTile method)
        }

        protected override bool OnNextTile(int tileX, int tileY)
        {
            if (tileX - startX >= 0 && tileX - startX < sizeX
                        && tileY - startY >= 0 && tileY - startY < sizeY)
            {
                //Get the offset from the tile to the vision source
                Vector2 voffset = new Vector2((tileX * Tile.WIDTH) - Position.X, Position.Y - (tileY * Tile.HEIGHT));

                if (voffset.Length() <= Radius) //Check if this tile is within the range of vision
                {
                    _tiles[tileX, tileY].IsVisible = true;
                    blocksVisible.Push(_tiles[tileX, tileY]);
                }
            }

            //If we hit a wall, stop casting the ray
            return (_tiles[tileX, tileY].Block == null);
        }

    }
    
}
