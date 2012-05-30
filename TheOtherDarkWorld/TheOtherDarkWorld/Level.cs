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
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Seed { get; private set; }
        public string maxHeight;

        public void Draw(SpriteBatch spriteBatch)
        {
            //These variables find out which blocks are currently on screen
            //This saves drawing off screen objects
            int startX = (int)(Player.PlayerList[0].Offset.X / 10);
            int startY = (int)(Player.PlayerList[0].Offset.Y / 10);

            for (int i = startX; i < startX + (UI.ScreenX / 10) + 1 && i < Width; i++)
                for (int j = startY; j < startY + (UI.ScreenY / 10) + 1 && j < Height; j++)
                {
                    if (BlockList[i, j] != null)
                        BlockList[i, j].Draw(spriteBatch);
                }
        }


        private Level(Block[,] blockList, int Seed)
        {
            BlockList = blockList;
            Width = blockList.GetLength(0);
            Height = blockList.GetLength(1);
            this.Seed = Seed;
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

        public enum LevelType
        {
            Box,
            Hallways,
            Scattered
        }

        public static void GenerateLevel(LevelType type, int seed, int width, int height)
        {
            switch (type)
            {
                case LevelType.Box:
                    Generate_Box(width, height, seed);
                    break;
                case LevelType.Hallways:
                    Generate_Hallways(width, height, seed);
                    break;
            }

        }

        private static Block[,] CreateBorders(int width, int height)
        {
            Block[,] blocks = new Block[width, height];
            byte type = 0;

            int j = 0;
            int i = 1;

            while (j < height)
            {
                type ^= 1; //
                blocks[0, j] = new Block(0, j, (type));
                blocks[width-1, j] = new Block(width-1, j, (type));
                j++;
            }

            while (i < width)
            {
                blocks[i, 0] = new Block(i, 0, (type));
                blocks[i, height - 1] = new Block(i, height - 1, (type));
                type ^= 1;
                i++;
            }

            return blocks;
        }

        private static void Generate_Box(int width, int height, int seed)
        {
            Block[,] blockList = CreateBorders(width, height);

            CurrentLevel = new Level(blockList, seed);
        }

        private static void Generate_Hallways(int width, int height, int seed)
        {
            Block[,] blockList = CreateBorders(width, height);
            //Block[,] blockList = new Block[width, height];

            int minimumRoomWidth = 5;
            int minimumRoomHeight = 8;
            int maximumRoomWidth = 40;
            int maximumRoomHeight = 20;

            int maximumCorridorWidth = 7;
            int minimumCorridorWidth = 4;

            int corridorWidth = minimumCorridorWidth + Math.Abs((seed + (int)Math.Pow(seed + 3, 3) + (1 / (Math.Abs(seed) + 1))) / 15) % (maximumCorridorWidth - minimumCorridorWidth);

            int widthTaken = 1;
            int heightTaken = 1;

            for (int i = 0; heightTaken < height; i++)
            {
                int roomHeight = minimumRoomHeight + Math.Abs((seed + i + (int)Math.Pow(seed + 3 + i, 3) + (1 / (Math.Abs(seed) + 1 + i))) / 15) % (maximumRoomHeight - minimumRoomHeight);

                if (roomHeight + heightTaken > height - 2)
                    roomHeight = height - 2 - heightTaken;

                CreateHallway(widthTaken, blockList, seed, minimumRoomWidth, maximumRoomWidth, width, roomHeight, heightTaken, i, new bool[]{i % 2 == 1, i % 2 == 0, false, false}); //Every corridor, the door changes sides
                heightTaken += roomHeight + (corridorWidth * ((i+1) % 2)) + 1; //There is only a corridor every 2 rooms
            }

            string x = "";
            if (CurrentLevel != null)
                x = CurrentLevel.maxHeight + ",";

            CurrentLevel = new Level(blockList, seed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widthTaken"></param>
        /// <param name="blockList"></param>
        /// <param name="seed"></param>
        /// <param name="minimumRoomWidth"></param>
        /// <param name="maximumRoomWidth"></param>
        /// <param name="Levelwidth"></param>
        /// <param name="roomHeight"></param>
        /// <param name="heightTaken"></param>
        /// <param name="corridorNum"></param>
        /// <param name="doors">Tells whether a door should exist on a particular side. Indexed from 0 to 3 - Top, Bottom, Left, Right </param>
        private static void CreateHallway(int widthTaken, Block[,] blockList, int seed, int minimumRoomWidth, int maximumRoomWidth, int Levelwidth, int roomHeight, int heightTaken, int corridorNum, bool[] doors)
        {
            //
            //Makes an entire hallway
            //
            for (int room = 0; widthTaken < Levelwidth - 1; room++)
            {
                //
                //Makes one room
                //
                int roomWidth = minimumRoomWidth + Math.Abs((seed + room + (int)Math.Pow(room + (seed % 50), 4 + corridorNum) + (3 / (Math.Abs(seed + corridorNum) + 1))) / ((room % 10) + 5 + corridorNum)) % (maximumRoomWidth - minimumRoomWidth);

                if (roomWidth + widthTaken > Levelwidth - 1)
                    roomWidth = Levelwidth - 1 - widthTaken;
                if (roomWidth < minimumRoomWidth)
                    return;

                //Add the left and right hand walls
                for (int i = widthTaken; i < roomWidth + widthTaken; i++)
                {
                    blockList[i, heightTaken] = new Block(i, heightTaken, 2);
                    blockList[i, heightTaken + roomHeight] = new Block(i, heightTaken + roomHeight, 2);
                }
                //Add the top and bottom walls
                for (int j = heightTaken; j < roomHeight + heightTaken + 1; j++)
                {
                    blockList[widthTaken, j] = new Block(widthTaken, j, 2);
                    blockList[widthTaken + roomWidth, j] = new Block(widthTaken + roomWidth, j, 2);
                }

                if (doors[0])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((seed + 3) * (room + 2) * (int)Math.Pow(corridorNum + 4, 5)) % (roomWidth - 3));
                    blockList[doorStart, heightTaken] = null;
                    blockList[doorStart + 1, heightTaken] = null;
                    blockList[doorStart + 2, heightTaken] = null;
                }
                if (doors[1])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((seed + 5) * (room + 7) * (int)Math.Pow(corridorNum + 2, 3)) % (roomWidth - 3));
                    blockList[doorStart, heightTaken + roomHeight] = null;
                    blockList[doorStart + 1, heightTaken + roomHeight] = null;
                    blockList[doorStart + 2, heightTaken + roomHeight] = null;
                }
                widthTaken += roomWidth;
            }

        }

    }
}
