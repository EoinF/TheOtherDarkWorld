using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class Level
    {
        public static Level CurrentLevel;

        public int PlayerIndex;
        public Tile[,] Tiles;
        public Player[] Players;
        public List<FloorItem> FloorItems;
        public List<Entity> Entities;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Seed { get; private set; }
        public LevelType Type { get; private set; }
        private List<Light> Lights;
        public static Vision PlayerVision;

        public int wave;

        public void Draw(SpriteBatch spriteBatch)
        {
            //These variables find out which blocks are currently on screen
            //This saves drawing off screen objects
            int startX = (int)(Players[0].Offset.X / 10);
            int startY = (int)(Players[0].Offset.Y / 10);

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

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Draw(spriteBatch);
            }    
        }


        public Level(LevelType Type, int Seed, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            this.Tiles = new Tile[Width, Height];
            this.Seed = Seed;
            this.Type = Type;
            Entities = new List<Entity>();
            wave = 0;
            Players = new Player[1];
            PlayerIndex = 0;
            Entities.Add(Players[0]);

            Lights = new List<Light>();
            PlayerVision = new Vision(500, Vector2.Zero, Vector2.Zero, 0.8f, Tiles);
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

        public void AddLight(Light light)
        {
            if (light.Position.X < (CurrentLevel.Width * Tile.WIDTH) && light.Position.X >= 0
                && light.Position.Y < (CurrentLevel.Height * Tile.HEIGHT) && light.Position.Y >= 0)
                Lights.Add(light);
        }

        public void RemoveLight(Light light)
        {
            Lights.Remove(light);
        }

        private void UpdateLights()
        {
            Light.ResetBrightness(); //Reset the brightness on all lit tiles

            for (int i = 0; i < Lights.Count; i++)
            {
                Lights[i].CastAll();
            }
        }
       

        public void Update()
        {
            UpdateBlocks();
            UpdateLights();
            Vision.ResetVision();

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(Tiles, Entities);

                for (int e = i + 1; e < Level.CurrentLevel.Entities.Count; e++)
                {
                    Entities[i].CheckEntityCollision(Entities[e]);
                }

                if (!Entities[i].IsAlive)
                {
                    Entities.RemoveAt(i);
                    UI.Kills++;
                    //Reduce the index by 1, because the next item will replace the position of the item that
                    //was just removed from the list
                    i--;
                }

            }

            for (int i = 0; i < FloorItems.Count; i++)
            {
                FloorItems[i].Update(Tiles);
            }

            if ((InputManager.keyboardState[0].IsKeyDown(Microsoft.Xna.Framework.Input.Keys.N) && InputManager.keyboardState[1].IsKeyUp(Microsoft.Xna.Framework.Input.Keys.N)))
            {
                Random rand = new Random();
                wave++;

                if (wave % 5 == 0)
                {
                    for (int i = 0; i < 2 + (wave); i++)
                    {
                        Entities.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) + Level.CurrentLevel.Height * 5 * rand.NextDouble())), 1.6f, Vector2.Zero, 0, 1000, 20, 40, 60 + (wave * 4)));
                        Entities.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 5 * rand.NextDouble())), 1.6f, Vector2.Zero, 0, 1000, 20, 40, 60 + (wave * 4)));
                    }
                } 
                else if ((wave + 2) % 5 == 0)
                {
                    for (int i = 0; i < 10 + (7 * wave); i++)
                    {
                        Entities.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 1 + (wave * 0.5f), Vector2.Zero, 0, 100, 20, 8, 15));
                        Entities.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 1 + (wave * 0.5f), Vector2.Zero, 0, 100, 20, 8, 15));
                    }
                }
                else if ((wave + 3) % 5 == 0)
                {
                    for (int i = 0; i < 10 + (4 * wave); i++)
                    {
                        Entities.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                        Entities.Add(new Enemy(new Vector2((float)((Level.CurrentLevel.Width * 5) + (Level.CurrentLevel.Width * 5) * rand.NextDouble()), (float)(Level.CurrentLevel.Height * 10 * rand.NextDouble())), 5, Vector2.Zero, 0, 50, 20, 2, 2));
                    }
                }
                else
                {
                    for (int i = 0; i < (3 * wave); i++)
                    {
                        Entities.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (float)((Level.CurrentLevel.Height * 5) * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 800, 30));
                    }
                    for (int i = 0; i < (3 * (wave - 1)); i++)
                    {
                        Entities.Add(new Enemy(new Vector2((float)(Level.CurrentLevel.Width * 10 * rand.NextDouble()), (Level.CurrentLevel.Height * 5) + (float)(2f * Level.CurrentLevel.Height * rand.NextDouble())), 1 + (wave * 0.3f), Vector2.Zero, 0, 100, 20, 8, 30));
                    }
                }

                if (wave != 1)
                {
                    Players[PlayerIndex].PlusOneLife();
                }
            }
        }

        private void UpdateBlocks()
        {
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

        public void Generate()
        {
            InitializeTiles();
            FloorItems = new List<FloorItem>();

            switch (Type)
            {
                case LevelType.Open:
                    Generate_Open();
                    break;
                case LevelType.Box:
                    Generate_Box();
                    break;
                case LevelType.Hallways:
                    Generate_Hallways();
                    break;
            }
            /*
            CurrentLevel.AddLight(new Light(0.5f, 200, new Vector2(200, 200), Vector2.One, MathHelper.TwoPi, Color.Yellow, Level.CurrentLevel.Tiles));
            CurrentLevel.AddLight(new Light(0.5f, 200, new Vector2(400, 500), Vector2.One, MathHelper.TwoPi, Color.Red, Level.CurrentLevel.Tiles));
            CurrentLevel.AddLight(new Light(0.5f, 200, new Vector2(600, 300), Vector2.One, MathHelper.TwoPi, Color.Blue, Level.CurrentLevel.Tiles));

            CurrentLevel.AddLight(new Light(0.3f, 100, new Vector2(400, 200), Vector2.One, MathHelper.TwoPi, Color.Green, Level.CurrentLevel.Tiles));
            CurrentLevel.AddLight(new Light(0.3f, 100, new Vector2(400, 100), Vector2.One, MathHelper.TwoPi, Color.Violet, Level.CurrentLevel.Tiles));
            CurrentLevel.AddLight(new Light(0.3f, 100, new Vector2(300, 600), Vector2.One, MathHelper.TwoPi, Color.Magenta, Level.CurrentLevel.Tiles));
            */
        }

        private void InitializeTiles()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Tiles[i, j] = new Tile(i, j);
                }
            }
        }

        private void CreateBorders()
        {
            byte type = 0;

            int j = 0;
            int i = 1;

            while (j < Height)
            {
                type ^= 1; //Alernate between 2 different types of block
                Tiles[0, j].Block = new Block(0, j, (type));
                Tiles[Width - 1, j].Block = new Block(Width - 1, j, (type));
                j++;
            }

            while (i < Width)
            {
                Tiles[i, 0].Block = new Block(i, 0, (type));
                Tiles[i, Height - 1].Block = new Block(i, Height - 1, (type));
                type ^= 1;
                i++;
            }
        }
        
        private void Generate_Open()
        {
        }

        private void Generate_Box()
        {
            CreateBorders();
        }

        private void Generate_Hallways()
        {
            CreateBorders();

            int minimumRoomWidth = 5;
            int minimumRoomHeight = 8;
            int maximumRoomWidth = 40;
            int maximumRoomHeight = 20;

            int maximumCorridorWidth = 7;
            int minimumCorridorWidth = 4;

            int corridorWidth = minimumCorridorWidth + Math.Abs((Seed + (int)Math.Pow(Seed + 3, 3) + (1 / (Math.Abs(Seed) + 1))) / 15) % (maximumCorridorWidth - minimumCorridorWidth);

            int widthTaken = 1;
            int heightTaken = 1;

            for (int i = 0; heightTaken + minimumRoomHeight < Height; i++)
            {
                int roomHeight = minimumRoomHeight + Math.Abs((Seed + i + (int)Math.Pow(Seed + 3 + i, 3) + (1 / (Math.Abs(Seed) + 1 + i))) / (5 + i)) % (maximumRoomHeight - minimumRoomHeight);

                if (roomHeight + heightTaken > Height - 2)
                    roomHeight = Height - 2 - heightTaken;

                CreateHallway(widthTaken, minimumRoomWidth, maximumRoomWidth, Width, roomHeight, heightTaken, i, new bool[] { i % 2 == 1, i % 2 == 0, true, false }); //Every corridor, the door changes sides
                heightTaken += roomHeight + (corridorWidth * ((i + 1) % 2)) + 1; //There is only a corridor every 2 rooms
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widthTaken"></param>
        /// <param name="blockList"></param>
        /// <param name="Seed"></param>
        /// <param name="minimumRoomWidth"></param>
        /// <param name="maximumRoomWidth"></param>
        /// <param name="Levelwidth"></param>
        /// <param name="roomHeight"></param>
        /// <param name="heightTaken"></param>
        /// <param name="corridorNum"></param>
        /// <param name="doors">Tells whether a door should exist on a particular side. Indexed from 0 to 3 - Top, Bottom, Left, Right </param>
        private void CreateHallway(int widthTaken, int minimumRoomWidth, int maximumRoomWidth, int Levelwidth, int roomHeight, int heightTaken, int corridorNum, bool[] doors)
        {
            //
            //Makes an entire hallway
            //
            for (int room = 0; widthTaken < Levelwidth - 2; room++)
            {
                //
                //Makes one room
                //
                int roomWidth = minimumRoomWidth + Math.Abs((Seed + room + (int)Math.Pow(room + (Seed % 50), 4 + corridorNum) + (3 / (Math.Abs(Seed + corridorNum) + 1))) / ((room % 10) + 5 + corridorNum)) % (maximumRoomWidth - minimumRoomWidth);

                if (roomWidth + widthTaken > Levelwidth - 1)
                    roomWidth = Levelwidth - 1 - widthTaken;

                int widthForNextRoom = (Levelwidth - 2) - (roomWidth + widthTaken);
                if (widthForNextRoom < minimumRoomWidth) //If the space left for next room is smaller than the minimum width
                {
                    //Then check if halving the amount will result in 2 sufficiently sized rooms
                    if (widthForNextRoom + roomWidth > 2 * minimumRoomWidth)
                    {
                        roomWidth = widthForNextRoom + roomWidth / 2;
                    }
                    else //Otherwise, take the remaining width and make 1 room with it
                    {
                        roomWidth += widthForNextRoom;
                    }
                }

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
                    int doorStart = widthTaken + 1 + Math.Abs(((Seed + 3) * (room + 2) * (int)Math.Pow(corridorNum + 4, 5)) % (roomWidth - 3));
                    Tiles[doorStart, heightTaken].Block = null;
                    Tiles[doorStart + 1, heightTaken].Block = null;
                    Tiles[doorStart + 2, heightTaken].Block = null;
                }
                if (doors[1])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((Seed + 5) * (room + 7) * (int)Math.Pow(corridorNum + 2, 3)) % (roomWidth - 3));
                    Tiles[doorStart, heightTaken + roomHeight].Block = null;
                    Tiles[doorStart + 1, heightTaken + roomHeight].Block = null;
                    Tiles[doorStart + 2, heightTaken + roomHeight].Block = null;
                }
                if (doors[2])
                {
                    int doorStart = heightTaken + 1 + Math.Abs(((Seed + 3) * (room + 2) * (int)Math.Pow(corridorNum + 2, 3)) % (roomHeight - 3));
                    Tiles[widthTaken, doorStart].Block = null;
                    Tiles[widthTaken, doorStart + 1].Block = null;
                    Tiles[widthTaken, doorStart + 2].Block = null;
                }
                if (doors[3])
                {
                    int doorStart = widthTaken + 1 + Math.Abs(((Seed + 5) * (room + 7) * (int)Math.Pow(corridorNum + 2, 3)) % (roomHeight - 3));
                    Tiles[widthTaken + roomWidth, doorStart].Block = null;
                    Tiles[widthTaken + roomWidth, doorStart + 1].Block = null;
                    Tiles[widthTaken + roomWidth, doorStart + 2].Block = null;
                }

                //Add in a light for the room
                //AddLight(new Light(0.7f, Math.Max(roomWidth * Tile.WIDTH, roomHeight * Tile.HEIGHT),
                 //   new Vector2((widthTaken + roomWidth / 2) * Tile.WIDTH, (heightTaken + roomHeight / 2) * Tile.HEIGHT), Vector2.UnitX,
                 //   MathHelper.TwoPi, Color.Orange, Tiles));
                
                widthTaken += roomWidth;
            }

        }
    }
}
