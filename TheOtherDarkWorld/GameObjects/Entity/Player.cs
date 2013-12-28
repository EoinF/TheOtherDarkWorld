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
    public class Player : Entity, IMelee, IItemHolder, IEnergyBased
    {
        public override Texture2D Texture { get { return Textures.Player; } }
        private bool ObserverMode { get; set; }
        public Swing Swing { get; set; }
        public Item[] Inventory { get; set; }

        public StatusEffect exhaustionExhaust { get; set; }
        public StatusEffect exhaustionSlow { get; set; }

        private bool _isExhaustedCompletely;
        public bool IsExhaustedCompletely //This is set when the player completely runs out of energy and unset when energy is fully regained
        {
            get 
            {
                return _isExhaustedCompletely;
            }
            set
            {
                if (_isExhaustedCompletely)
                {
                    if (value == false) //The player has fully recovered from exhaustion
                    {
                        StatusEffects.Remove(exhaustionExhaust);
                        StatusEffects.Remove(exhaustionSlow);
                    }
                }
                else
                {
                    if (value == true) //The player is now exhausted
                    {
                        StatusEffects.Add(exhaustionExhaust);
                        StatusEffects.Add(exhaustionSlow);
                    }
                }
                _isExhaustedCompletely = value;
            }
        }
        private float _energy;
        public float Energy
        {
            get 
            {
                return _energy;
            }
            set
            {
                _energy = value;
                if (_energy < 0)
                {
                    _energy = 0;
                    IsExhaustedCompletely = true;
                }
                else if (_energy > MaxEnergy)
                {
                    _energy = MaxEnergy;
                    IsExhaustedCompletely = false;
                }
            }
        }
        public float MaxEnergy { get; set; }
        private float _exhaustPercent;
        public float ExhaustPercent
        {
            get
            {
                return _exhaustPercent;
            }
            set
            {
                if (value > 1)
                    _exhaustPercent = 1;
                else if (value < 0)
                    _exhaustPercent = 0;
                else
                    _exhaustPercent = value;
            }
        }

        public Player(Vector2 startPosition, int MaxHealth, int MaxEnergy, float walkSpeed, int inventorySize, Vector2 startVelocity, int ID, int Resistance)
            : base(MaxHealth, startPosition, walkSpeed, Color.White, startVelocity, new Vector2(Textures.Player.Width / 2, Textures.Player.Height / 2), Resistance, (int)Textures.Player.Height, (int)Textures.Player.Width)
        {
            this.Energy = MaxEnergy;
            this.MaxEnergy = MaxEnergy;
            Inventory = new Item[inventorySize];
            Weight = 5;
            exhaustionSlow = new StatusEffect(StatusType.Slowed, 0.5f, -1, "Out of energy!");
            exhaustionExhaust = new StatusEffect(StatusType.Exhausted, 0.5f, -1, "Out of energy!");
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
                return InputManager.MousePositionV + StateManager.Offset; 
            }
        }

        protected override void UpdateStatusEffects()
        {
            base.UpdateStatusEffects();

            ExhaustPercent = 0;

            for (int i = 0; i < StatusEffects.Count; i++)
            {
                if (StatusEffects[i].Type == StatusType.Exhausted)
                {
                    ExhaustPercent += StatusEffects[i].Potency;
                }
            }
        }

        public void UpdateEnergy()
        {
            if (Swing != null)
                Energy -= 2;

            if (Velocity != Vector2.Zero)
            {
                Energy -= 0.5f;
            }
            else
            {
                Energy += (1 - ExhaustPercent);
            }
        }

        protected override void Intelligence()
        {
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

            CheckInput();

            if (Velocity != Vector2.Zero)
                Velocity = Vector2.Normalize(Velocity);
        }

        private void CheckInput()
        {
            if (!CommandManager.IsActive) //Only accept player keyboard input when the command box isn't active
            {
                ObserverMode = InputManager.keyboardState[0].IsKeyDown(Keys.Space);

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
            }
        }

        public bool PickUpItem(Item item)
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
            UI.QueueMessage("Inventory is full!");
            return false;
        }

        /// <summary>
        /// Places an item onto the floor of the map near the entity's position
        /// </summary>
        /// <param name="item">A pointer to the item to be dropped(Must be the actual pointer to the item in the Inventory array)</param>
        /// <returns>True if the item actually exists in the entity's inventory</returns>
        public virtual bool DropItem(Item item)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Equals(item))
                {
                    StateManager.DropItem(item, Position + Origin);
                    Inventory[i] = null;
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
        public bool TrashItem(Item item)
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
        public bool TrashItem(int index)
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

        public void UpdateInventory()
        {
            //
            //Apply passive effects of items and restore cooldowns
            //
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null)
                {
                    Inventory[i].Update();
                    if (Inventory[i].Amount == 0)
                    {
                        if (Inventory[i].DestroyedWhenEmpty)
                            TrashItem(i);
                    }
                }
            }
        }

        /// <summary>
        /// Get's the first occurrence of an item within this entity's inventory, or if none is found, returns null
        /// </summary>
        /// <param name="type">The type of item required</param>
        /// <returns>The item with the requested type, or if there is no item, returns null</returns>
        public Item GetItem(int type)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type == type && Inventory[i].Amount != 0)
                    return Inventory[i];
            }
            return null;

        }
        
        /// <summary>
        /// Get's the index of the first occurrence of an item within this entity's inventory, or if none is found, returns -1
        /// </summary>
        /// <param name="type">The type of item required</param>
        /// <returns>The index of the item with the requested type, or if there is no item, returns -1</returns>
        public int GetItemIndex(int type)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type == type && Inventory[i].Amount != 0)
                    return i;
            }
            return -1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Player, Position + Origin - StateManager.Offset, null, StatusColour, Rotation, Origin, 1, SpriteEffects.None, UI.PLAYER_DEPTH_DEFAULT);
            if (Swing != null)
                Swing.Draw(spriteBatch, StateManager.Offset);
        }
        
    }
}
