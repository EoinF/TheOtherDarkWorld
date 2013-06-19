using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Item:Action
    {
        #region Fields & Properties

        public int Power { get; set; }
        public bool IsAutomatic { get; set; }
        public int MaxAmount { get; set; }
        public int Consumes { get; set; } //This refers to the item type that is consumed when this item is activated
        public bool IsConsumable {get; set;} //If true, amount is reduced by 1 when activated
        public int UseCooldown { get; set; }
        public int Cooldown { get; set; }
        public int Owner { get; set; }
        public string Description { get; set; }


        private int _type;
        public int Type
        {
            get { return _type; }
            set
            {
                if (value <= GameData.GameItems.Length || value >= 0)
                    _type = value;
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                //if (value <= MaxAmount && value >= 0)
                    _amount = value;
                //else //We'll notice when debugging anyway
                    ; // throw new Exception("Tried to set amount of an item to higher than player can hold");
            }
        }

        #endregion

        /// <summary>
        /// The base method of the Item class; It reduces the amount of the item if its consumable.
        /// </summary>
        /// <returns>Returns true if the item has been used up completely</returns>
        public virtual bool Activate(float rotation, Vector2 Direction, Vector2 startPosition)
        {
            if (IsConsumable)
            {
                //Find out what this item does and trigger it
                UseCooldown = Cooldown;
                Amount--;
            }
            return (Amount == 0);
        }

        public Item(int type, int amount = -1, int owner = -1)
        {
            Type = type;
            Item characteristics = GameData.GameItems[type];

            this.IsConsumable = characteristics.IsConsumable;
            this.Consumes = characteristics.Consumes;
            this.MaxAmount = characteristics.MaxAmount;
            this.Name = characteristics.Name;
            this.Owner = owner;
            this.Cooldown = characteristics.Cooldown;
            this.Power = characteristics.Power;
            this.IsAutomatic = characteristics.IsAutomatic;
            this.Description = characteristics.Description;

            if (amount < 0)
                Amount = MaxAmount;
            else
                Amount = amount;
        }

        public Item()
        {
        }

    }
}
