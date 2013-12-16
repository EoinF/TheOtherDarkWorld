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
    public class Player : Entity
    {

        public override Texture2D Texture { get { return Textures.Player; } }
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

        private bool ObserverMode { get; set; }


        public Player(Vector2 startPosition, int MaxHealth, float walkSpeed, int inventorySize, Vector2 startVelocity, int ID, int Resistance)
            : base(MaxHealth, inventorySize, startPosition, walkSpeed, Color.White, startVelocity, new Vector2(Textures.Player.Width / 2, Textures.Player.Height / 2), Resistance, (int)Textures.Player.Height, (int)Textures.Player.Width)
        {
            Weight = 5;
            this.ID = ID;
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

        protected override Vector2 Target
        {
            get
            {
                return InputManager.MousePositionV + Offset; 
            }
        }


        private void StatusEffectDebugMethod()
        {
            if (InputManager.JustPressed(Keys.D1))
                StatusEffects.Add(new StatusEffect(StatusType.Blinded, 1, 180, "Blinded"));
            if (InputManager.JustPressed(Keys.D2))
                StatusEffects.Add(new StatusEffect(StatusType.Stunned, 1, 180, "Stunned"));
            if (InputManager.JustPressed(Keys.D3))
                StatusEffects.Add(new StatusEffect(StatusType.Confused, 30, 180, "Confused"));
            if (InputManager.JustPressed(Keys.D4))
                StatusEffects.Add(new StatusEffect(StatusType.Burning, 0.7f, 180, "Burning"));
            if (InputManager.JustPressed(Keys.D5))
                StatusEffects.Add(new StatusEffect(StatusType.Poison, 0.5f, 180, "Poison"));
        }

        protected override void Intelligence()
        {
            StatusEffectDebugMethod();

            if (IsStunned || IsFrozen)
                return;

            if (InputManager.keyboardState[0].IsKeyDown(Keys.R))
            {
                if (Inventory[0].GetType() == typeof(Gun))
                {
                    (Inventory[0] as Gun).Reload();
                }
            }

            Level.PlayerVision.Update(Position + (Origin / 2), Direction);
            Level.PlayerVision.CastAll();

            if (UI.CursorMode == CursorType.Crosshair 
                && !ObserverMode) //Make sure the user is clicking on the actual map and not the ui
            {
                Item itemP = Inventory[0];
                if (itemP != null)
                {
                    if ((!itemP.IsAutomatic && InputManager.JustLeftClicked)
                        || (itemP.IsAutomatic && InputManager.LeftClicking))
                        itemP.Activate();
                }

                Item itemS = Inventory[1];
                if (itemS != null)
                {
                    if ((!itemS.IsAutomatic && InputManager.JustRightClicked)
                        || (itemS.IsAutomatic && InputManager.RightClicking))
                        itemS.Activate();
                }
            }

            ObserverMode = InputManager.keyboardState[0].IsKeyDown(Keys.Space);

            if (Velocity != Vector2.Zero)
            {
                CheckCollisions(Level.CurrentLevel.Tiles);
                Offset = Position - new Vector2(UI.ScreenX / 2, UI.ScreenY / 2);
            }

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
        }

        public void PlusOneLife()
        {
            MaxHealth += 100;
            ApplyHealing(-1);
        }


        public override bool PickUpItem(Item item)
        {
            //Check if there's space to fit the new item in the inventory
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null)
                {
                    Inventory[i] = item;
                    UIContainer con = UI.Inventory_UI[i] as UIContainer; //First get the container that holds the inventory
                    (con[0] as InventoryElement).Item = item;
                    item.Owner = this;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Removes an item from the game
        /// </summary>
        /// <param name="item">A pointer to the item to be trashed(Must be the actual pointer to the item in the Inventory array)</param>
        /// <returns>True if the item actually exists in the entity's inventory</returns>
        public override bool TrashItem(Item item)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Equals(item))
                {
                    Inventory[i] = null;
                    UIContainer con = UI.Inventory_UI[i] as UIContainer; //First get the container that holds the inventory
                    (con[0] as InventoryElement).Item = null;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Removes an item from the game
        /// </summary>
        /// <param name="item">A pointer to the item to be trashed(Must be the actual pointer to the item in the Inventory array)</param>
        /// <returns>True if the item actually exists in the entity's inventory</returns>
        public override bool TrashItem(int index)
        {
            if (index >= 0 && index < Inventory.Length)
            {
                Inventory[index] = null;
                UIContainer con = UI.Inventory_UI[index] as UIContainer; //First get the container that holds the inventory
                (con[0] as InventoryElement).Item = null;
                return true;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Player, Position + Origin - Offset, null, StatusColour, Rotation, Origin, 1, SpriteEffects.None, UI.PLAYER_DEPTH_DEFAULT);
            if (Swing != null)
                Swing.Draw(spriteBatch, Offset);
        }
        
    }
}
