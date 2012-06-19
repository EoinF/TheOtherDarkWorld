using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public partial class Player : GameObject
    {
        public static Player[] PlayerList;
        private static Vector2 PanelPosition = new Vector2(800 - Textures.SidePanel.Width, 0);
        private static Vector2 HealthPosition = PanelPosition + new Vector2(11, 10);
        public int ID;
        private int Health { get; set; }
        private int MaxHealth { get; set; }
        public Swing Swing;
        public Light Vision;
        public Light PeripheralVision;

        public new Color Colour
        {
            get
            {
                return HitCooldown > 0 ? Color.Red : Color.White;
            }
        }

        private int HitCooldown { get; set; }
        private Vector2 _offset;
        public Vector2 Offset
        {
            get { return _offset;}
            set 
            {
                //We only need to alter the offset in this way if the level is larger than the actual screen
                if ((Level.CurrentLevel.Width * 10) < UI.ScreenX
                    || value.X < 0)
                    value.X = 0;
                else if (value.X > (Level.CurrentLevel.Width * 10) - UI.ScreenX)
                    value.X = (Level.CurrentLevel.Width * 10) - UI.ScreenX;

                if ((Level.CurrentLevel.Height * 10) < UI.ScreenY
                    || value.Y < 0)
                    value.Y = 0;
                else if (value.Y > (Level.CurrentLevel.Height * 10) - UI.ScreenY)
                    value.Y = (Level.CurrentLevel.Height * 10) - UI.ScreenY;

                _offset = value;
            }
        }


        /// <summary>
        /// The first 2 items are the equipped items
        /// </summary>
        public Item[] Inventory { get; protected set; }
        private byte WeaponStability;
        private bool ObserverMode { get; set; }


        public Player(Vector2 startPosition, int MaxHealth, float walkSpeed, int inventorySize, Vector2 startVelocity, int ID, int Resistance)
            : base(startPosition, walkSpeed, Color.White, startVelocity, new Vector2(Textures.Player.Width / 2, Textures.Player.Height / 2), Resistance)
        {
            this.Health = 100 * (this.MaxHealth = MaxHealth);
            this.ID = ID;
            Inventory = new Item[inventorySize];
            Vision = new Light(0.4f, 100, startPosition, Vector2.One, 0.7f);
            //PeripheralVision = new Light(0.1f, startPosition, Vector2.One, MathHelper.Pi);
        }

        public static Vector2 CrosshairOrigin
        {
            get { return new Vector2(Textures.Crosshair.Width / 2, Textures.Crosshair.Height / 2); }
        }

        public override Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Player.Height, Textures.Player.Width); }
        }

        public Rectanglef BaseRect
        {
            get { return new Rectanglef(0, 0, Textures.Player.Height, Textures.Player.Width); }
        }

        public float Rotation
        {
            get
            {
                float rot = (float)Math.Atan((InputManager.MousePositionP.Y + Offset.Y - Rect.Center.Y) / (InputManager.MousePositionP.X + Offset.X - Rect.Center.X)) - MathHelper.PiOver2;

                if (InputManager.MousePositionP.X + Offset.X >= Rect.Center.X)
                    rot += MathHelper.Pi;

                return rot;
            }
        }

        public void Reload()
        {
            //If the gun doesn't need to be reloaded
            if (Inventory[0].Amount == Inventory[0].MaxAmount)
                return;

            //Starts at 1, because the gun can't be it's own ammo
            for (int i = 1; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type == Inventory[0].Consumes)
                {
                    //If there's not enough ammo to fill it completely, then just add as much as possible
                    if (Inventory[i].Amount < Inventory[0].MaxAmount - Inventory[0].Amount)
                    {
                        Inventory[0].Amount += Inventory[i].Amount;
                        Inventory[i].Amount = 0;
                    }
                    else //Otherwise fill it to the max
                    {
                        Inventory[i].Amount -= (Inventory[0].MaxAmount - Inventory[0].Amount);
                        Inventory[0].Amount = Inventory[0].MaxAmount;
                    }
                    //Check if the ammo has been emptied since reloading. [You can't hold 0 of an item!]
                    if (Inventory[i].Amount == 0)
                        Inventory[i] = null;

                    //The weapon must be reloaded at this stage. The item doesn't exist in the inventory if the amount is 0
                    Gun gun = Inventory[0] as Gun;
                    gun.ReloadCooldown = gun.ReloadTime;
                    return;
                }
            }
        }


        public void Update()
        {
            if (Health <= 0)
                return;

            if (InputManager.keyboardState[0].IsKeyDown(Keys.R))
            {
                if (Player.PlayerList[0].Inventory[0].GetType() == typeof(Gun))
                {
                    Reload();
                }
            }

            Vision.Update(Level.CurrentLevel.Tiles, Position + Origin, Rotation);
            Level.CurrentLevel.LightStack.Push(Vision);
            //PeripheralVision.Update(Level.CurrentLevel.Tiles, Position + Origin, Rotation);

            if (InputManager.LeftClicking && !UI.CursorMode)
                Activate_Primary(Inventory[0]);
            if (InputManager.RightClicking && !UI.CursorMode)
                Activate_Secondary(Inventory[1]);

            ObserverMode = InputManager.keyboardState[0].IsKeyDown(Keys.Space);

            if (Velocity != Vector2.Zero)
            {
                CheckCollisions(Level.CurrentLevel.Tiles);
                Offset = Position - new Vector2(UI.ScreenX / 2, UI.ScreenY / 2);
            }

            if (Swing != null)
            {
                if (Swing.Update(Position))
                    Swing = null;
            }

            //
            //Reduce the cooldown of equipped items
            //
            if (Inventory[0] != null)
            {
                if (Inventory[0].GetType() == typeof(Gun))
                {
                    Gun gun = Inventory[0] as Gun;
                    if (gun.ReloadCooldown >= 0)
                        gun.ReloadCooldown--;
                }

                if (Inventory[0].Cooldown >= 0)
                    Inventory[0].Cooldown--;
            }

            if (Inventory[1] != null && Inventory[1].Cooldown >= 0)
                Inventory[1].Cooldown--;

            if (HitCooldown >= 0)
                HitCooldown--;

            UpdateMovement();
            CheckEnemyCollisions();

            UpdateMovement();
        }

        void UpdateMovement()
        {
            Velocity = Vector2.Zero;

            if (InputManager.keyboardState[0].IsKeyDown(Keys.W))
            {
                Velocity -= Vector2.UnitY;
            }
            if (InputManager.keyboardState[0].IsKeyDown(Keys.S))
            {
                Velocity += Vector2.UnitY;
            }
            if (InputManager.keyboardState[0].IsKeyDown(Keys.A))
            {
                Velocity -= Vector2.UnitX;
            }
            if (InputManager.keyboardState[0].IsKeyDown(Keys.D))
            {
                Velocity += Vector2.UnitX;
            }

            if (Velocity != Vector2.Zero)
                Velocity = Vector2.Normalize(Velocity);
            Velocity *= Speed;
            //Velocity = new Vector2(Velocity.X + 0.1f, Velocity.Y + 0.1f);
        }

        public void Activate_Primary(Item item)
        {
            if (item != null && !ObserverMode)
            {
                if ((!item.IsAutomatic && InputManager.JustLeftClicked) || item.IsAutomatic)
                {
                    if (item.Cooldown < 0)
                    {
                        if (item.Amount == 0)
                            Reload();

                        if (item.Amount > 0)
                        {
                            Vector2 direction = new Vector2((float)Math.Cos(Rotation - MathHelper.PiOver2), (float)Math.Sin(Rotation - MathHelper.PiOver2));
                            direction.Normalize();
                            item.Activate(Rotation, direction, new Vector2(Rect.Center.X + (Textures.Bullet.Width / 2), Rect.Center.Y + (Textures.Bullet.Height / 2)) + (direction * 3));
                        }
                    }
                }
            }
        }

        public void Activate_Secondary(Item item)
        {
            if (item != null && !ObserverMode)
            {
                if ((!item.IsAutomatic && InputManager.JustRightClicked) || item.IsAutomatic)
                {
                    if (item.Cooldown < 0 && item.GetType() != typeof(Gun))
                    {
                        Vector2 direction = new Vector2((float)Math.Cos(Rotation - MathHelper.PiOver2), (float)Math.Sin(Rotation - MathHelper.PiOver2));
                        direction.Normalize();
                        item.Activate(Rotation, direction, new Vector2(Rect.Center.X + (Textures.Bullet.Width / 2), Rect.Center.Y + (Textures.Bullet.Height / 2)) + (direction * 3));
                    }
                }
            }
        }

        public void CheckEnemyCollisions()
        {
            for (int i = 0; i < Level.CurrentLevel.Enemies.Count; i++)
            {
                Enemy e = Level.CurrentLevel.Enemies[i];

                if (Collision.SquareVsSquare_TwoMoving(Rect, e.Rect, Velocity, e.Velocity) && HitCooldown < 0)
                {
                    if (e.IsAttacking)
                    {
                        Health -= e.MeleeDamage;
                        HitCooldown = 10;
                    }
                }
            }

        }

        public void PlusOneLife()
        {
            MaxHealth++;
            Health = MaxHealth * 100;
        }


        public static void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < PlayerList.Length; i++)
            {
                spriteBatch.Draw(Textures.Player, PlayerList[i].Position + PlayerList[i].Origin - PlayerList[i].Offset, null, PlayerList[i].Colour, PlayerList[i].Rotation, PlayerList[i].Origin, 1, SpriteEffects.None, 0.5f);
                
                if (PlayerList[i].Swing != null)
                    PlayerList[i].Swing.Draw(spriteBatch, PlayerList[i].Offset);
            }
            
            Player.PlayerList[0].DrawHUD(spriteBatch);

            //spriteBatch.DrawString(Textures.MediumFont, "Projectiles: " + Projectile.ProjectileList.Count, new Vector2(600, 50), Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

        }

        /// <summary>
        /// Gets the colour the item should be displayed as. It should be darker if the gun is reloading
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private Color GetReloadColour(int x, Color original)
        {
            if (Player.PlayerList[0].Inventory[x] != null
                && Player.PlayerList[0].Inventory[x].GetType() == typeof(Gun))
            {
                Color changed = original;
                Gun gun = Player.PlayerList[0].Inventory[x] as Gun;

                changed *=  1 - ((float)gun.ReloadCooldown / (gun.ReloadTime * 1.3f));
                //Restore the alpha
                changed.A = original.A;
                return changed;
            }
            return original;
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            //spriteBatch.DrawString(Textures.Fonts[1], "Seed = " + Level.CurrentLevel.Seed, new Vector2(400, 400), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Enemies Killed = " + UI.Kills, new Vector2(250, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Wave: " + Level.CurrentLevel.wave, new Vector2(100, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "High Score = " + UI.HighScore, new Vector2(250, 40), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            //spriteBatch.DrawString(Textures.Fonts[1], "Projectiles: " + Projectile.ProjectileList.Count, new Vector2(100, 100), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            

            spriteBatch.Draw(Textures.SidePanel, PanelPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);

            //Each health point is worth 100
            int HealthCutoff = (int)(Health / 100);
            int HealthTip = (Health % 100);
            for (int i = 0; i < MaxHealth; i++)
            {
                Color HPColour = new Color(0, 255, 0);

                //First, this checks for the special case when a health point is not completely full or empty
                if (i == HealthCutoff)
                {
                    if (HealthTip == 0)
                        HPColour = new Color(20, 20, 20);
                    else
                        HPColour = new Color((byte)(255 - (HealthTip * 2.55f)), (byte)(HealthTip * 2.55f), 0);

                }
                if (i * 100 > Health)
                    HPColour = new Color(20, 20, 20);
                spriteBatch.Draw(Textures.HealthPoint, PanelPosition + new Vector2(7 + (i % 4) * 15, 30 + (int)(i/4) * 20), null, HPColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
            }
            
            //
            //Next, draw the inventory Items
            //
            for (int i = 0; i < Inventory.Length; i++)
            {
                spriteBatch.Draw(UI.Inventory[i].Texture, UI.Inventory[i].OriginalPosition, null, GetReloadColour(i, UI.Inventory[i].Colour), 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);

                if (Inventory[i] != null)
                {
                    if (Textures.Items != null)
                        spriteBatch.Draw(Textures.Items, UI.Inventory[i].Position + new Vector2(2, 2), Textures.GetItemRectangle(Inventory[i].Type), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                    spriteBatch.DrawString(Textures.Fonts[0], Inventory[i].Amount.ToString(), UI.Inventory[i].Position + new Vector2(4, 22) , Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                }
            }

            if (UI.CursorMode)
                spriteBatch.Draw(Textures.Cursor, InputManager.MousePositionV, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            else
                spriteBatch.Draw(Textures.Crosshair, InputManager.MousePositionV, null, Color.YellowGreen, 0, CrosshairOrigin, 1, SpriteEffects.None, 0.9f);

        }

        public int GetAmountOf(int type)
        {
            int total = 0;
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].Type == type)
                    total += Inventory[i].Amount;
            }
            return total;
        }
    }
}
