using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheOtherDarkWorld
{
    public class Player : GameObject
    {
        public static Player[] PlayerList;
        private static Vector2 PanelPosition = new Vector2(800 - Textures.SidePanel.Width, 0);
        private static Vector2 HealthPosition = PanelPosition + new Vector2(11, 10);
        public int ID;
        private int Health { get; set; }
        private int MaxHealth { get; set; }

        /// <summary>
        /// The first 2 items are the equipped items
        /// </summary>
        public Item[] Inventory { get; protected set; }
        private byte WeaponStability;
        private bool ObserverMode { get; set; }


        public Player(Vector2 startPosition, int MaxHealth, float walkSpeed, Weapon primaryWeapon, Weapon secondaryWeapon, Color colour, Vector2 startVelocity, int ID)
            : base(startPosition, walkSpeed, colour, startVelocity, new Vector2(Textures.Player.Width / 2, Textures.Player.Height / 2))
        {
            this.Health = 100 * (this.MaxHealth = MaxHealth);
            this.ID = ID;
            Inventory = new Item[5];
            Inventory[0] = primaryWeapon;
            Inventory[1] = secondaryWeapon;
        }

        public Vector2 Origin
        {
            get { return new Vector2(Textures.Player.Width / 2, Textures.Player.Height / 2); }
        }

        public static Vector2 CrosshairOrigin
        {
            get { return new Vector2(Textures.Crosshair.Width / 2, Textures.Crosshair.Height / 2); }
        }

        public override Rectangle Rect
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Textures.Player.Height, Textures.Player.Width); }
        }

        public Rectangle BaseRect
        {
            get { return new Rectangle(0, 0, Textures.Player.Height, Textures.Player.Width); }
        }

        public float Rotation
        {
            get
            {
                //float vectorX = CrosshairOrigin.X;
                //float vectorY = CrosshairOrigin.Y;

                float rot = (float)Math.Atan((MousePosition.Y - Rect.Center.Y) / (MousePosition.X - Rect.Center.X)) - MathHelper.PiOver2;

                if (MousePosition.X >= Rect.Center.X)
                    rot += MathHelper.Pi;

                return rot;
            }
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(InputManager.mouseState[0].X, InputManager.mouseState[0].Y); }
        }
        public Vector2 MousePositionVector
        {
            get { return new Vector2(InputManager.mouseState[0].X, InputManager.mouseState[0].Y); }
        }

        public void Reload()
        {
            if (Inventory[0].GetType() == typeof(Weapon))
            {
                //Starts at 1, because the weapon can't be it's own ammo
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

                        //The weapon must be reloaded at this stage. The item shouldn't be in the inventory if the amount is 0
                        return;
                    }
                }
            }
        }


        public void Update()
        {
            ObserverMode = InputManager.keyboardState[0].IsKeyDown(Keys.Space);
            if (Velocity != Vector2.Zero)
            {
                Colour = Color.White;
                List<Point> BlocksHit;
                CheckCollisions();
                //CheckBlockCollisions(BaseRect, out BlocksHit, false);
            }

            if (Inventory[0] != null && Inventory[0].Cooldown >= 0)
                Inventory[0].Cooldown -= Inventory[0].UseRate;

            if (Inventory[1] != null && Inventory[1].Cooldown >= 0)
                Inventory[1].Cooldown -= Inventory[1].UseRate;

            Velocity = Vector2.Zero;
        }


        public void Activate_Primary()
        {
            if (Inventory[0] != null)
            {
                Vector2 direction = new Vector2((float)Math.Cos(Rotation - MathHelper.PiOver2), (float)Math.Sin(Rotation - MathHelper.PiOver2));
                direction.Normalize();
                Inventory[0].Activate(Rotation, direction, new Vector2(Rect.Center.X + (Textures.Bullet.Width / 2), Rect.Center.Y + (Textures.Bullet.Height / 2)) + (direction * 3));
            }
        }

        public void Activate_Secondary()
        {
            if (Inventory[1] != null)
            {
                Vector2 direction = new Vector2((float)Math.Cos(Rotation - MathHelper.PiOver2), (float)Math.Sin(Rotation - MathHelper.PiOver2));
                direction.Normalize();
                Inventory[1].Activate(Rotation, direction, new Vector2(Rect.Center.X + (Textures.Bullet.Width / 2), Rect.Center.Y + (Textures.Bullet.Height / 2)) + (direction * 3));
            }
        }

        public void UseSkill()
        {


        }


        public static void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < PlayerList.Length; i++)
            {
                spriteBatch.Draw(Textures.Player, PlayerList[i].Position + PlayerList[i].Origin, null, PlayerList[i].Colour, PlayerList[i].Rotation, PlayerList[i].Origin, 1, SpriteEffects.None, 0.1f);
            }
            
            Player.PlayerList[0].DrawHUD(spriteBatch);

            //spriteBatch.DrawString(Textures.MediumFont, "Projectiles: " + Projectile.ProjectileList.Count, new Vector2(600, 50), Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            //
            //Draw the two equipped item slots
            //
            spriteBatch.Draw(Textures.UITextures[0], UI.Inventory[0].OriginalPosition, null, UI.Inventory[0].Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
            spriteBatch.Draw(Textures.UITextures[0], UI.Inventory[1].OriginalPosition, null, UI.Inventory[1].Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);

            /*if (Player.PlayerList[0].Inventory[0] != null)
            {
                spriteBatch.Draw(Textures.Items, UI.Inventory[0].Position + new Vector2(2, 2), Textures.GetItemRectangle(Player.PlayerList[0].Inventory[0].Type), Color.PaleVioletRed, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                spriteBatch.DrawString(Textures.Fonts[0], PlayerList[0].Inventory[0].Amount.ToString(), UI.Inventory[0].Position + new Vector2(2, 23), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.82f);
            }
            if (Player.PlayerList[0].Inventory[1] != null)
            {
                spriteBatch.Draw(Textures.Items, UI.Inventory[1].Position + new Vector2(2, 2), Textures.GetItemRectangle(Player.PlayerList[0].Inventory[1].Type), Color.PaleVioletRed, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                spriteBatch.DrawString(Textures.Fonts[0], PlayerList[0].Inventory[1].Amount.ToString(), UI.Inventory[1].Position + new Vector2(2, 23), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.82f);
            }*/


            spriteBatch.Draw(Textures.SidePanel, PanelPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);

            spriteBatch.DrawString(Textures.Fonts[1], "Health", HealthPosition ,Color.Violet, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            //Each health point is worth 100
            int HealthCutoff = (int)(Health / 100);
            int HealthTip = (Health / HealthCutoff);
            for (int i = 0; i < MaxHealth - 1; i++)
            {
                //First, this checks for the special case when a health point is not completely full or empty
                if (i == HealthCutoff)
                    spriteBatch.Draw(Textures.HealthPoint, PanelPosition + new Vector2(7 + (((int)(Health / 100) % 4) - 1) * 15, 30 + (int)((Health / 400) * 20)), null, new Color((byte)(255 - (HealthTip * 2.55f)), (byte)(HealthTip * 2.55f), 0), 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f); 


                Color HPColour = new Color(0, 255, 0);
                if (i * 100 > Health)
                    HPColour = new Color(20, 20, 20);
                spriteBatch.Draw(Textures.HealthPoint, PanelPosition + new Vector2(7 + (i % 4) * 15, 30 + (int)(i/4) * 20), null, HPColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
            }
            
            //
            //Next, draw the inventory Items
            //
            for (int i = 0; i < Inventory.Length; i++)
            {
                spriteBatch.Draw(Textures.UITextures[0], UI.Inventory[i].OriginalPosition , null, UI.Inventory[i].Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);

                if (Inventory[i] != null)
                {
                    if (Textures.Items != null)
                        spriteBatch.Draw(Textures.Items, UI.Inventory[i].Position + new Vector2(2, 2), Textures.GetItemRectangle(Inventory[i].Type), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                    spriteBatch.DrawString(Textures.Fonts[0], Inventory[i].Amount.ToString(), UI.Inventory[i].Position + new Vector2(4, 22) , Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
                }
            }

            if (InputManager.CursorMode)
                spriteBatch.Draw(Textures.Cursor, PlayerList[0].MousePositionVector, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            else
                spriteBatch.Draw(Textures.Crosshair, PlayerList[0].MousePositionVector, null, Color.YellowGreen, 0, CrosshairOrigin, 1, SpriteEffects.None, 0.9f);

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
