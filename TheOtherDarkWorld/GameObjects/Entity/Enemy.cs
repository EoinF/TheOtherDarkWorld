using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Enemy : Entity, IMelee, IItemHolder
    {
        public Swing Swing { get; set; }
        const int DEFAULT_MELEE_COOLDOWN = 20;
        public Item[] Inventory { get; set; }

        public int Type { get; private set; }
        public int MeleeDamage { get; private set; }
        private int MeleeCooldown { get; set; }
        public bool IsAttacking { get { return HitCooldown <= 0; } }

        public override Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Enemies[Type].Width, Textures.Enemies[Type].Height); }
        }


        public Enemy(Texture2D Texture, Vector2 startPosition, float speed, Vector2 startVelocity, int Type, int InventorySize, int MaxHealth, int Resistance, int Weight, int MeleeDamage)
            : base(Texture, MaxHealth, startPosition, speed, Color.White, Vector2.Zero, new Vector2(Textures.Enemies[Type].Width / 2, Textures.Enemies[Type].Height / 2), Resistance, Texture.Width, Texture.Height)
        {
            Inventory = new Item[InventorySize];
            this.Type = Type;
            Target = Vector2.Zero;
            this.Weight = Weight;
            HitVelocity = Vector2.Zero;
            this.MeleeDamage = MeleeDamage;
            this.LightColour = Color.White;
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
                        if (Inventory[i].TypeWhenEmpty == -1)
                            TrashItem(i);
                        else if (Inventory[i].TypeWhenEmpty == Inventory[i].Type)
                            ; //Leave it as it is
                        else
                            Inventory[i] = Item.Create(Inventory[i].TypeWhenEmpty, owner: this);

                    }
                }
            }
        }

        protected override void Intelligence()
        {
            //Target = StateManager.CurrentPlayer.Position + StateManager.CurrentPlayer.Origin;
            Vector2 Displacement = new Vector2(Target.X - (this.Position.X + this.Origin.X), Target.Y - (this.Position.Y + this.Origin.Y));
            
            Velocity = Vector2.Normalize(Displacement);
            
            if (Swing == null)
            {
                if (MeleeCooldown <= 0)
                {
                    if (Displacement.Length() < 20)
                    {
                        Swing = new Swing(Rotation, 20, MeleeDamage, (Weight * 2), Weight * 2, this.Texture.Width, this);
                        MeleeCooldown = DEFAULT_MELEE_COOLDOWN;
                    }
                }
                else
                    MeleeCooldown--;
            }
        }

        public virtual bool PickUpItem(Item item)
        {
            //Check if there's space to fit the new item in the inventory
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null)
                {
                    Inventory[i] = item;
                    item.Owner = this;
                    return true;
                }
            }
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
                    item = null;
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
        public bool TrashItem(int index)
        {
            if (index >= 0 && index < Inventory.Length)
            {
                Inventory[index] = null;
                return true;
            }
            return false;
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
            ApplyLighting();
            if (IsVisible)
                spriteBatch.Draw(Textures.Enemies[Type], Position + Origin - StateManager.Offset, null, Colour, Rotation, Origin, 1, SpriteEffects.None, UI.ENEMY_DEPTH_DEFAULT);
            if (Swing != null)
            {
                Swing.Draw(spriteBatch, StateManager.Offset);
            }
        }

    }
}
