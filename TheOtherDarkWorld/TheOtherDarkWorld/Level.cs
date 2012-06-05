using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.Items;

namespace TheOtherDarkWorld
{
    public class Level
    {
        public static Level CurrentLevel;
        public Block[,] BlockList;
        public List<FloorItem> FloorItems;
        public List<Enemy> Enemies;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Seed { get; private set; }
        public string maxHeight;

        public int wave;

        public void Draw(SpriteBatch spriteBatch)
        {
            //These variables find out which blocks are currently on screen
            //This saves drawing off screen objects
            int startX = (int)(Player.PlayerList[0].Offset.X / 10);
            int startY = (int)(Player.PlayerList[0].Offset.Y / 10);

            //Makes sure i doesn't start at a negative index
            for (int i = startX < 0 ? 0 : startX; i < startX + (UI.ScreenX / 10) + 2; i++)
            {
                //Checks if the index is out of bounds
                if (i >= Width)
                    break;

                //Makes sure j doesn't start at a negative index
                for (int j = startY < 0 ? 0 : startY ; j < startY + (UI.ScreenY / 10) + 2; j++)
                {
                    //Checks if the index is out of bounds
                    if (j >= Level.CurrentLevel.Height)
                        break;

                    if (BlockList[i, j] != null)
                        BlockList[i, j].Draw(spriteBatch);
                }
            }

            for (int i = 0; i < FloorItems.Count; i++)
            {
                FloorItems[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Draw(spriteBatch);
            }
        }


        private Level(Block[,] BlockList, int Seed, List<FloorItem> FloorItems)
        {
            this.FloorItems = FloorItems;
            this.BlockList = BlockList;
            Width = BlockList.GetLength(0);
            Height = BlockList.GetLength(1);
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

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].Update())
                {
                    Enemies.RemoveAt(i);
                    //Reduce the index by 1, because the next item will replace the position of the item that
                    //was just removed from the list
                    i--;
                }
            }

            if (Enemies.Count == 0)
            {
                Random rand = new Random();
                wave++;
                switch (wave)
                {
                    case 4:
                    case 5:
                        for (int i = 0; i < 70 + (5 * wave); i++)
                        {
                            Level.CurrentLevel.Enemies.Add(new Enemy(new Vector2((float)(4 * UI.ScreenX * rand.NextDouble()) - (2 * UI.ScreenX), -50 - (float)(100 * rand.NextDouble()) + (2 * UI.ScreenY * (rand.Next(0, 9) % 3))), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                        }
                        break;

                    default:
                        for (int i = 0; i < (10 * wave); i++)
                        {
                            Level.CurrentLevel.Enemies.Add(new Enemy(new Vector2((float)(UI.ScreenX * rand.NextDouble()), -50 - (float)(100 * rand.NextDouble()) + (2 * UI.ScreenY * (rand.Next(1, 3) % 2))), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                        }
                        for (int i = 0; i < (10 * (wave - 1)); i++)
                        {
                            Level.CurrentLevel.Enemies.Add(new Enemy(new Vector2(2 * (UI.ScreenY * (rand.Next(1,3) % 2 )) - (float)(UI.ScreenX * rand.NextDouble()), - 50 - (float)(100 * rand.NextDouble()) + (0.5f * UI.ScreenY * (rand.Next(0,9) % 3 ))), 1 +(wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                        }
                        break;
                }

                if (wave != 1)
                {
                    for (int i = 0; i < Player.PlayerList[0].Inventory.Length; i++)
                    {
                        Item item = Player.PlayerList[0].Inventory[i];

                        if (item == null || item.Type > 100)
                        {
                            int type = 101 + rand.Next(0, 2);
                            Player.PlayerList[0].Inventory[i] = new Item(type, (int)(GameData.GameItems[type].MaxAmount * (1 + (wave / 20f))));
                        }
                        else if (item.Type != 100)
                            item.Amount = item.MaxAmount;
                    }
                }
            }
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
            Open,
            Box,
            Hallways,
            Scattered
        }

        public static void GenerateLevel(LevelType type, int seed, int width, int height)
        {
            switch (type)
            {
                case LevelType.Open:
                    Generate_Open(width, height, seed);
                    break;
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

        private static void Generate_Open(int width, int height, int seed)
        {
            Block[,] blockList = new Block[width, height];

            List<FloorItem> FloorItems = new List<FloorItem>();
            CurrentLevel = new Level(blockList, seed, FloorItems);
        }

        private static void Generate_Box(int width, int height, int seed)
        {
            Block[,] blockList = CreateBorders(width, height);

            List<FloorItem> FloorItems = new List<FloorItem>();
            CurrentLevel = new Level(blockList, seed, FloorItems);
        }

        private static void Generate_Hallways(int width, int height, int seed)
        {
            Block[,] blockList = CreateBorders(width, height);
            List<FloorItem> FloorItems = new List<FloorItem>();

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
                int roomHeight = minimumRoomHeight + Math.Abs((seed + i + (int)Math.Pow(seed + 3 + i, 3) + (1 / (Math.Abs(seed) + 1 + i))) / (5 + i)) % (maximumRoomHeight - minimumRoomHeight);

                if (roomHeight + heightTaken > height - 2)
                    roomHeight = height - 2 - heightTaken;

                CreateHallway(widthTaken, blockList, seed, minimumRoomWidth, maximumRoomWidth, width, roomHeight, heightTaken, i, new bool[]{i % 2 == 1, i % 2 == 0, false, false}); //Every corridor, the door changes sides
                heightTaken += roomHeight + (corridorWidth * ((i+1) % 2)) + 1; //There is only a corridor every 2 rooms
            }

            string x = "";
            if (CurrentLevel != null)
                x = CurrentLevel.maxHeight + ",";

            CurrentLevel = new Level(blockList, seed, FloorItems);
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
