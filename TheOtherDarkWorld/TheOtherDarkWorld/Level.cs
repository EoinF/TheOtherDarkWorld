using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class Level
    {
        public static Level CurrentLevel;
        public Block[,] BlockList;

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < BlockList.GetLength(0); i++)
                for (int j = 0; j < BlockList.GetLength(1); j++)
                {
                    if (BlockList[i,j] != null)
                        BlockList[i, j].Draw(spriteBatch);
                }
        }


        public Level(Block[,] blockList)
        {
            BlockList = blockList;
        }


        public static bool DamageBlock(int x, int y, int dmg)
        {
            if ((CurrentLevel.BlockList[x, y].Health -= dmg) <= 0) //Decreases the blocks health by the damage specified and checks if the result is less than 0, indicating that the block is destroyed
            {
                CurrentLevel.BlockList[x, y] = null;
                return true;
            }
            return false;
        }

        public void Update()
        {
            UpdateBlocks();

        }

        private void UpdateBlocks()
        {
            for (int i = 0; i < BlockList.GetLength(0); i++)
                for (int j = 0; j < BlockList.GetLength(1); j++)
                {
                    if (BlockList[i,j] != null) 
                        if (BlockList[i, j].Health < 0)
                            BlockList[i, j] = null;
                }
        }

    }
}
