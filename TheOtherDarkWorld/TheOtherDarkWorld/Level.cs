using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public struct Level
    {
        public static Level CurrentLevel;
        public Tile[,] Tiles;
        public Player[] Players;
        public List<FloorItem> FloorItems;
        public List<Enemy> Enemies;
        public int Width;
        public int Height;
        public int Seed;
        public Stack<Light> LightStack;

        public int wave;

        /// <summary>
        /// The darkness that will be drawn over everything
        /// </summary>
        private Color[] ShroudArray;
        private Color[] PlainShroud;

        /// <summary>
        /// This is used when a tile's brightness has changed. This returns
        /// the value back to 0
        /// </summary>
        public Stack<Tile> UpdatedTiles;
        
        public void Draw(SpriteBatch spriteBatch)
        {
            
            //These variables find out which blocks are currently on screen
            //This saves drawing off screen objects
            int startX = (int)(Player.PlayerList[0].Offset.X / 10);
            int startY = (int)(Player.PlayerList[0].Offset.Y / 10);

            int endX = startX + (UI.ScreenX / 10) + 2;
            int endY = startY + (UI.ScreenY / 10) + 2;

            if (endX >= Width)
                endX = Width;
            //
            //Prevent the index from being out of bounds
            //
            if (endY >= Height)
                endY = Height;

            //Makes sure it doesn't start at a negative index
            for (int i = startX < 0 ? 0 : startX; i < endX; i++)
            {
                //Makes sure j doesn't start at a negative index
                for (int j = startY < 0 ? 0 : startY ; j < endY; j++)
                {
                    Tiles[i, j].Draw(spriteBatch);
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

            //Draw the foreground texture, which represents the darkness
            //spriteBatch.Draw(Textures.Foreground, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.4f);
        }


        private Level(Tile[,] Tiles, int Seed, List<FloorItem> FloorItems)
        {
            this.FloorItems = FloorItems;
            this.Tiles = Tiles;
            Width = Tiles.GetLength(0);
            Height = Tiles.GetLength(1);
            this.Seed = Seed;
            Enemies = new List<Enemy>();
            wave = 0;
            Players = new Player[1];

            UpdatedTiles = new Stack<Tile>();
            LightStack = new Stack<Light>();

            PlainShroud = new Color[UI.ScreenX * UI.ScreenY];

            for (int i = 0; i < PlainShroud.Length; i++)
            {
                PlainShroud[i] = Color.Black;
            }

            ShroudArray = (Color[])PlainShroud.Clone();
        }

        public static bool DamageBlock(int x, int y, int dmg)
        {
            if ((CurrentLevel.Tiles[x, y].Block.Health -= dmg) <= 0) //Decreases the blocks health by the damage specified and checks if the result is less than 0, indicating that the block is destroyed
            {
                CurrentLevel.Tiles[x, y].Block = null;
                return true;
            }
            return false;
        }

        
        public void LightTile(int i, int j, float Brightness)
        {
            if (i >= 0 && i < Width && j >= 0 && j < Height)
            {
                if (Tiles[i, j].Block != null)
                {
                    if (Tiles[i, j].Brightness < (Brightness / 2f))
                        Tiles[i, j].Brightness = (Brightness / 2f);
                }
                else
                    if (Tiles[i, j].Brightness < Brightness)
                        Tiles[i, j].Brightness = Brightness;
                UpdatedTiles.Push(Tiles[i, j]);
            }
        }
        

        private void UpdateLights()
        {
            while (LightStack.Count > 0)
            {
                CalculateLight(LightStack.Pop());
            }
            Textures.UpdateForeground(ShroudArray);
            //ShroudArray = (Color[])PlainShroud.Clone();
            //if (Player.PlayerList[0].IsAlive)
                //ShroudArray = (Color[])PlainShroud.Clone();
        }

        private void CalculateLight(Light light)
        {
            //If the brightness is 0, the light will have no effect on anything
            if (light.Brightness <= 0)
                return;

            //
            //The pixel in the array that the light originates
            //
            int arrayPositionX = (int)(light.Position.X - Player.PlayerList[0].Offset.X);
            int arrayPositionY = (int)(light.Position.Y - Player.PlayerList[0].Offset.Y);

            int radius = (int)light.Depth;

            int startX = arrayPositionX - radius;
            if (startX < 10)
                startX = 10;

            int endX = arrayPositionX + radius;
            if (endX >= (Level.CurrentLevel.Width * 10))
                endX = (Level.CurrentLevel.Width * 10) - 1;

            int startY = arrayPositionY - radius;
            if (startY < 0)
                startY = 0;

            int endY = arrayPositionY + radius;
            if (endY >= (Level.CurrentLevel.Height * 10))
                endY = (Level.CurrentLevel.Height * 10) - 1;

            //float brightness = (float)Math.Sqrt(1 - light.Brightness);
            float brightness = (1 - (light.Brightness / 10f));

            Vector2 lightScreenPosition = light.Position - Player.PlayerList[0].Offset;
            Vector2 CurrentCheck;
            for (CurrentCheck.Y = startY; CurrentCheck.Y < endY; CurrentCheck.Y++)
            {
                for (CurrentCheck.X = startX; CurrentCheck.X < endX; CurrentCheck.X++)
                {
                    float DistanceFromCentre =  Vector2.Distance(CurrentCheck, lightScreenPosition);
                    if (DistanceFromCentre <= light.Depth)
                        ShroudArray[((int)CurrentCheck.Y * UI.ScreenX) + (int)CurrentCheck.X].A = (byte)((255 * Math.Sqrt(DistanceFromCentre / light.Depth) * brightness));
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].Update())
                {
                    Enemies.RemoveAt(i);
                    UI.Kills++;
                    //Reduce the index by 1, because the next item will replace the position of the item that
                    //was just removed from the list
                    i--;
                }
            }

            UpdateBlocks();

            if (Enemies.Count == 0 || (InputManager.keyboardState[0].IsKeyDown(Microsoft.Xna.Framework.Input.Keys.N) && InputManager.keyboardState[1].IsKeyUp(Microsoft.Xna.Framework.Input.Keys.N)))
            {
                Random rand = new Random();
                wave++;

                if (wave % 5 == 0)
                {
                    for (int i = 0; i < 2 + (wave); i++)
                    {
                        Enemies.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) + Level.CurrentLevel.Height * 5 * rand.NextDouble())), 1.6f, Vector2.Zero, 0, 1000, 20, 40, 60 + (wave * 4)));
                        Enemies.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 5 * rand.NextDouble())), 1.6f, Vector2.Zero, 0, 1000, 20, 40, 60 + (wave * 4)));
                    }
                } 
                else if ((wave + 2) % 5 == 0)
                {
                    for (int i = 0; i < 10 + (7 * wave); i++)
                    {
                        Enemies.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 1 + (wave * 0.5f), Vector2.Zero, 0, 100, 20, 8, 15));
                        Enemies.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 1 + (wave * 0.5f), Vector2.Zero, 0, 100, 20, 8, 15));
                    }
                }
                else if ((wave + 3) % 5 == 0)
                {
                    for (int i = 0; i < 10 + (4 * wave); i++)
                    {
                        Enemies.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                        Enemies.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 5, Vector2.Zero, 0, 50, 20, 2, 2));
                    }
                }
                else
                {
                    for (int i = 0; i < (3 * wave); i++)
                    {
                        Enemies.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                    }
                    for (int i = 0; i < (3 * (wave - 1)); i++)
                    {
                        Enemies.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (Level.CurrentLevel.Height * 5) + (float)(2f * Level.CurrentLevel.Height * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                    }
                }

                if (wave != 1)
                {
                    Player.PlayerList[0].PlusOneLife();
                }
            }
        }

        private void UpdateBlocks()
        {
            while (UpdatedTiles.Count > 0)
                UpdatedTiles.Pop().Brightness = 0;

            for (int i = 0; i < Level.CurrentLevel.Width; i++)
                for (int j = 0; j < Level.CurrentLevel.Height; j++)
                {
                    if (Tiles[i,j].Block != null)
                        if (Tiles[i, j].Block.Health < 0)
                            Tiles[i, j].Block = null;
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

        private static Tile[,] InitializeTiles(int width, int height)
        {
            Tile[,] Tiles = new Tile[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tiles[i, j] = new Tile(i, j);
                }
            }

            return Tiles;
        }

        private static void CreateBorders(Tile[,] Tiles)
        {
            int width = Tiles.GetLength(0);
            int height = Tiles.GetLength(1);

            byte type = 0;

            int j = 0;
            int i = 1;

            while (j < height)
            {
                type ^= 1; //
                Tiles[0, j].Block = new Block(0, j, (type));
                Tiles[width - 1, j].Block = new Block(width - 1, j, (type));
                j++;
            }

            while (i < width)
            {
                Tiles[i, 0].Block = new Block(i, 0, (type));
                Tiles[i, height - 1].Block = new Block(i, height - 1, (type));
                type ^= 1;
                i++;
            }
        }

        private static void Generate_Open(int width, int height, int seed)
        {
            Tile[,] Tiles = InitializeTiles(width, height);

            List<FloorItem> FloorItems = new List<FloorItem>();
            CurrentLevel = new Level(Tiles, seed, FloorItems);
        }

        private static void Generate_Box(int width, int height, int seed)
        {
            Tile[,] Tiles = InitializeTiles(width, height);
            CreateBorders(Tiles);

            List<FloorItem> FloorItems = new List<FloorItem>();
            CurrentLevel = new Level(Tiles, seed, FloorItems);
        }

        private static void Generate_Hallways(int width, int height, int seed)
        {
            Tile[,] Tiles = InitializeTiles(width, height);
            CreateBorders(Tiles);

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

                CreateHallway(widthTaken, Tiles, seed, minimumRoomWidth, maximumRoomWidth, width, roomHeight, heightTaken, i, new bool[]{i % 2 == 1, i % 2 == 0, false, false}); //Every corridor, the door changes sides
                heightTaken += roomHeight + (corridorWidth * ((i+1) % 2)) + 1; //There is only a corridor every 2 rooms
            }


            CurrentLevel = new Level(Tiles, seed, FloorItems);
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
        private static void CreateHallway(int widthTaken, Tile[,] Tiles, int seed, int minimumRoomWidth, int maximumRoomWidth, int Levelwidth, int roomHeight, int heightTaken, int corridorNum, bool[] doors)
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
                    Tiles[i, heightTaken].Block = new Block(i, heightTaken, 2);
                    Tiles[i, heightTaken + roomHeight].Block = new Block(i, heightTaken + roomHeight, 2);
                }
                //Add the top and bottom walls
                for (int j = heightTaken; j < roomHeight + heightTaken + 1; j++)
                {
                    Tiles[widthTaken, j].Block = new Block(widthTaken, j, 2);
                    Tiles[widthTaken + roomWidth, j].Block = new Block(widthTaken + roomWidth, j, 2);
                }

                if (doors[0])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((seed + 3) * (room + 2) * (int)Math.Pow(corridorNum + 4, 5)) % (roomWidth - 3));
                    Tiles[doorStart, heightTaken].Block = null;
                    Tiles[doorStart + 1, heightTaken].Block = null;
                    Tiles[doorStart + 2, heightTaken].Block = null;
                }
                if (doors[1])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((seed + 5) * (room + 7) * (int)Math.Pow(corridorNum + 2, 3)) % (roomWidth - 3));
                    Tiles[doorStart, heightTaken + roomHeight].Block = null;
                    Tiles[doorStart + 1, heightTaken + roomHeight].Block = null;
                    Tiles[doorStart + 2, heightTaken + roomHeight].Block = null;
                }
                widthTaken += roomWidth;
            }

        }

    }
}
