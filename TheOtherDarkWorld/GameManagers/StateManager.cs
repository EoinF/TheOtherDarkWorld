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
        public static int State { get; set; }
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
            if (StateManager.State == 1)
            {
                CurrentLevel.Update();
                UI.Update();
                Offset = CurrentPlayer.Position - new Vector2(UI.ScreenX / 2, UI.ScreenY / 2);
            }
        }

        public static void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (State == 0)
            {
                string prompt = "Press Enter to Begin...\n\n    Double click items to activate them\n    Press n to send the next wave\n    Press enter to restart the game";
                spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(UI.ScreenX / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White);

            }
            if (State == 1)
            {
                if (CurrentPlayer != null && CurrentPlayer.IsBlinded)
                    device.Clear(Color.White);

                string prompt = "Press Enter to Retry...\n\n    Double click items to activate them\n    Press n to send the next wave\n    Press enter to restart the game";
                if (!CurrentPlayer.IsAlive)
                    spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(UI.ScreenX / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                CurrentLevel.Draw(spriteBatch);
                UI.DrawHUD(spriteBatch, CurrentPlayer);
            }

        }

        public static void StartLevel()
        {
            Random rand = new Random();
            CurrentLevel = new Level(Level.LevelType.Hallways, rand.Next(), 76, 60);
            CurrentLevel.Generate();
            CurrentLevel.Entities = new List<Entity>();

            int InventorySize = 14;

            CurrentLevel.Players = new Player[1] { new Player(new Vector2(230, 200), 500, 3, 14, Vector2.Zero, 0, 5) };
            PlayerIndex = 0;
            CurrentLevel.Entities.Add(CurrentPlayer);

            UI.InitializeHUD(InventorySize);
            CurrentPlayer.PickUpItem(new Gun(0, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Melee(11, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Melee(12, -1, CurrentPlayer));

            CurrentPlayer.PickUpItem(new Item(101, 999, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(50, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(50, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Torch(110, CurrentPlayer));
            CurrentPlayer.PickUpItem(new SmartPhone(112, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Gun(1, Owner: CurrentPlayer));
            CurrentPlayer.PickUpItem(new Gun(4, Owner: CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(130, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(131, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(132, -1, CurrentPlayer));
            CurrentPlayer.PickUpItem(new Item(120, -1, CurrentPlayer));

            /*
             * An idea for later on:
             *      When the player starts, pick 3 random weapons in the game. The player gets to choose one of them
             *      to start with. Then give 3 random utility items(like medic packs) and then 3 other random items
             *      
             *      Could also include classes. Like bonus health tank. Or ability to carry more items. Or being able
             *      to see a wider radius. Or being able to see in the dark better
             */
        }

        public static Projectile CreateProjectile(float damage, int penetration, float speed, Color colour, GameObject owner, Vector2 startVelocity, Vector2 startPosition, float rotation)
        {
            Projectile newP = new Projectile(damage, penetration, speed, colour, owner, startVelocity, startPosition, rotation,
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
    }
}
