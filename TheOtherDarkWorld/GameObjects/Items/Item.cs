using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Item : Action
    {
        //
        //Default values for an item when deserialized from the xml
        //
        const int CONSUMES_DEFAULT = -1;
        const int CONSUMETIME_DEFAULT = 1;
        const int USECOOLDOWN_DEFAULT = 1;
        const string DESCRIPTION_DEFAULT = "This item is a mystery to me!";

        #region Fields & Properties

        [System.Xml.Serialization.XmlIgnore()]
        public Entity Owner { get; set; }

        public float Power { get; set; }
        public bool IsAutomatic { get; set; }
        public int MaxAmount { get; set; }

        public int Consumes { get; set; }
        public int ConsumeTime { get; set; } //The number of ticks before the item it consumes is consumed again
        protected int ConsumeTicks { get; set; }

        public bool IsConsumable { get; set; } //If true, this item consumes itself based on the consume rate

        /// <summary>
        /// The time remaining before this item can be used again
        /// </summary>
        public int UseCooldown { get; set; }

        /// <summary>
        /// The original cooldown for this item(This should not be modified)
        /// </summary>
        public int BaseCooldown { get; set; }
        public string Description { get; set; }


        protected int _type;
        public virtual int Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value < GameData.GameItems.Length && value >= 0)
                    _type = value;
            }
        }

        private int _amount;
        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }

        #endregion

        /// <summary>
        /// The base method of the Item class; It reduces the amount of the item if its consumable.
        /// </summary>
        /// <returns>Returns true if the item has been used up completely</returns>
        public virtual void Activate()
        {
            if (UseCooldown <= 0)
            {
                if (Consumes >= 0 || IsConsumable) //Fuel required to use this item
                {
                    if (IsConsumable) //This item is consumed when used
                    {
                        if (Amount > 0)
                        {
                            ApplyActive();

                            ConsumeTicks++;
                            if (ConsumeTicks >= ConsumeTime)
                            {
                                ConsumeTicks = 0;
                                Amount--;
                            }
                        }
                    }
                    else //A different item is consumed when used
                    {
                        Item fuel = Owner.GetItem(Consumes);
                        if (fuel != null)
                        {
                            ApplyActive();
                            if (ConsumeTicks > ConsumeTime)
                            {
                                ConsumeTicks = 0;
                                fuel.Amount--;
                            }
                            else
                            {
                                ConsumeTicks++;
                            }
                        }
                    }
                }
                else //No fuel required to use this item
                {
                    ApplyActive();
                }
                UseCooldown += BaseCooldown;
            }
        }



        public Item(int type, int amount = -1, Entity owner = null)
        {
            Type = type;
            Item characteristics = GameData.GameItems[type];

            this.IsConsumable = characteristics.IsConsumable;
            this.Consumes = characteristics.Consumes;
            this.ConsumeTime = characteristics.ConsumeTime;
            this.MaxAmount = characteristics.MaxAmount;
            this.Name = characteristics.Name;
            this.Owner = owner;
            this.BaseCooldown = characteristics.BaseCooldown;
            this.Power = characteristics.Power;
            this.IsAutomatic = characteristics.IsAutomatic;
            this.Description = characteristics.Description;

            if (amount < 0)
                _amount = MaxAmount;
            else
                _amount = amount;
        }

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public Item()
        {
            //
            //Apply the default values for an item while it's being deserialized from the xml(in case they aren't specified in the xml)
            //
            Consumes = CONSUMES_DEFAULT;
            ConsumeTime = CONSUMETIME_DEFAULT;
            UseCooldown = USECOOLDOWN_DEFAULT;
            Description = DESCRIPTION_DEFAULT;
        }

        public virtual void Update()
        {
            if (UseCooldown > 0)
                UseCooldown--;

            ApplyPassive();
        }

        protected virtual void ApplyPassive()
        {
            //TODO: Apply passive effect
            switch (Type)
            {
                case 0:
                    break;
            }
        }

        protected virtual void ApplyActive()
        {
            //TODO: Apply active effect
            switch (Type)
            {
                case 0:
                    break;
            }
        }
    }
}