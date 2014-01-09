using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public static class StateManager
    {
        public static GameState State { get; set; }

        public static bool FULL_BRIGHT = false;
        public static bool FULL_VISION = false;
        public static bool DebugMode { get; set; }

        private static Level CurrentLevel;
        private static int PlayerIndex;
        public static Player CurrentPlayer
        {
            get { return CurrentLevel.Players[PlayerIndex]; }
        }

        private static Vector2 _offset;
        public static Vector2 Offset
        {
            get { return _offset; }
            set
            {
                //We only need to alter the offset in this way if the level is larger than the actual screen
                if ((CurrentLevel.Width * 10) < UI.ScreenX
                    || value.X < 0)
                    value.X = 0;
                else if (value.X > (CurrentLevel.Width * 10) - UI.ScreenX)
                    value.X = (CurrentLevel.Width * 10) - UI.ScreenX;

                if ((CurrentLevel.Height * 10) < UI.ScreenY
                    || value.Y < 0)
                    value.Y = 0;
                else if (value.Y > (CurrentLevel.Height * 10) - UI.ScreenY)
                    value.Y = (CurrentLevel.Height * 10) - UI.ScreenY;

                _offset = value;
            }
        }

        public static void Update()
        {
            if (StateManager.State == GameState.InGame)
            {
                CurrentLevel.Update();
                Offset = CurrentPlayer.Position - new Vector2(UI.ScreenX / 2, UI.ScreenY / 2);
            }
        }

        public static void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (State == GameState.MainMenu)
            {
                string prompt = "Press Enter to bring up the command prompt...\nType \"help\" for help\nType \"start\" to begin\n    Double click items to activate them";
                spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(UI.ScreenX / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White);

                UI.DrawHUD(spriteBatch);
            }
            if (State == GameState.InGame)
            {
                if (CurrentPlayer != null && CurrentPlayer.IsBlinded)
                    device.Clear(Color.White);

                string prompt = "Press Enter to bring up the command prompt...\n Type \"help\" for help\n    Double click items to activate them";
                if (!CurrentPlayer.IsAlive)
                    spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(UI.ScreenX / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                spriteBatch.End(); //End drawing the rest because the lighting shader is now to be applied

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

                CurrentLevel.Draw(spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
                UI.DrawHUD(spriteBatch);
            }
        }

        public static void StartLevel()
        {
            Random rand = new Random();
            CurrentLevel = new Level(Level.LevelType.Hallways, rand.Next(), 76, 60);
            CurrentLevel.Generate();
            CurrentLevel.Entities = new List<Entity>();

            int InventorySize = 14;

            CurrentLevel.Players = new Player[1] { new Player(Textures.Player, new Vector2(230, 200), 100, 100, 3, 14, Vector2.Zero, 0, 5) };
            PlayerIndex = 0;
            CurrentLevel.Entities.Add(CurrentPlayer);

            UI.InitializeInventory(InventorySize);
            CurrentPlayer.PickUpItem(Item.Create(0, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(11, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(12, -1, CurrentPlayer));

            CurrentPlayer.PickUpItem(Item.Create(101, 999, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(50, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(50, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(110, 1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(112, 1, CurrentPlayer));
            CurrentPlayer.PickUpItem(Item.Create(120, 1, CurrentPlayer));

            /*
             * An idea for later on:
             *      When the player starts, pick 3 random weapons in the game. The player gets to choose one of them
             *      to start with. Then give 3 random utility items(like medic packs) and then 3 other random items
             *      
             *      Could also include classes. Like bonus health tank. Or ability to carry more items. Or being able
             *      to see a wider radius. Or being able to see in the dark better
             */


            if (UI.Kills > UI.HighScore)
                UI.HighScore = UI.Kills;
            UI.Kills = 0;

            State = GameState.InGame;
        }

        public static Projectile CreateProjectile(float damage, int penetration, float speed, Color colour, GameObject owner, Vector2 startVelocity, Vector2 startPosition, float rotation)
        {
            Projectile newP = new Projectile(Textures.Bullet, damage, penetration, speed, colour, owner, startVelocity, startPosition, rotation,
                (p, col) //Collision
                =>
                {
                    Block BlkHit = CurrentLevel.Tiles[col.location.X, col.location.Y].Block;
                    //Position += Velocity / 2; //There are two collisions per frame, so it adds half the velocity twice.
                    BlkHit.Health -= p.Damage;
                    p.ApplyDamage(BlkHit.Resistance);
                });
            CurrentLevel.AddProjectile(newP);
            return newP;
        }

        public static void RemoveProjectile(Projectile projectile)
        {
            CurrentLevel.RemoveProjectile(projectile);
        }

        public static Light CreateLight(float Brightness, float Radius, Vector2 Position, Vector2 Direction, float Span, Color Colour, bool IsActive = true)
        {
            Light newL = new Light(Brightness, Radius, Position, Direction, Span, Colour, CurrentLevel.Tiles, IsActive);
            CurrentLevel.AddLight(newL);
            return newL;
        }

        public static void RemoveLight(Light light)
        {
            CurrentLevel.RemoveLight(light);
        }

        public static void DropItem(Item item, Vector2 Position)
        {
            CurrentLevel.FloorItems.Add(new FloorItem(item, Position));
        }

        public static void NextWave()
        {
            CurrentLevel.wave++;
            CurrentLevel.SpawnEnemies();
        }
    }

    public enum GameState
    {
        MainMenu,
        InGame
    }
}
