using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Item
    {
        const int CONSUMETICKS_LIMIT = 600; //1 second to consume a 10 ConsumeRate item if consumed every frame for 1 second

        //
        //Default values for an item when deserialized from the xml
        //
        const int CONSUMES_DEFAULT = -1;
        const int CONSUMERATE_DEFAULT = 600; //Consumed once per use
        const int USECOOLDOWN_DEFAULT = 1;
        const int MAXAMOUNT_DEFAULT = 1;
        const bool DESTROYED_WHEN_EMPTY_DEFAULT = true;
        const string DESCRIPTION_DEFAULT = "This item is a mystery to me!";

        #region Fields & Properties

        [System.Xml.Serialization.XmlIgnore()]
        public Entity Owner { get; set; }

        public float Power { get; set; }
        public bool IsAutomatic { get; set; }
        public int MaxAmount { get; set; }

        public int Consumes { get; set; }
        private int _consumeRate;
        public virtual int ConsumeRate
        {
            get
            {
                return _consumeRate;
            }
            set
            {
                _consumeRate = value;
            }
        } //The amount of the item that has been consumed
        protected int ConsumeTicks { get; set; }

        public bool IsConsumable { get; set; } //If true, this item consumes itself based on the consume rate
        public bool DestroyedWhenEmpty { get; set; } //The item is discarded when amount = 0

        public ItemEffect[] ActiveEffects { get; set; }
        public ItemEffect[] PassiveEffects { get; set; }

        /// <summary>
        /// The time remaining before this item can be used again
        /// </summary>
        public int UseCooldown { get; set; }

        /// <summary>
        /// The original cooldown for this item(This should not be modified)
        /// </summary>
        public int BaseCooldown { get; set; }

        public string Name { get; set; }
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

        //private int _amount;
        public int Amount { get; set; }

        #endregion

        /// <summary>
        /// The base method of the Item class; It reduces the amount of the item if its consumable. Otherwise, it consumes the fuel
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
                        if (Consume(this.ConsumeRate))
                            ApplyActive();
                    }
                    else //A different item is consumed when used
                    {
                        Item fuel = (Owner as IItemHolder).GetItem(Consumes);
                        if (fuel != null)
                        {
                            if (fuel.Consume(this.ConsumeRate)) //If there is enough fuel
                            {
                                ApplyActive();
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

        public bool Consume(int rate)
        {
            if (Amount > 0)
            {
                ConsumeTicks += rate;
                if (ConsumeTicks >= CONSUMETICKS_LIMIT)
                {
                    ConsumeTicks -= CONSUMETICKS_LIMIT;
                    Amount--;
                }
                return true;
            }
            else
                return false; //Nothing left to consume
        }


        public Item(int type, int amount = -1, Entity owner = null)
        {
            this.Type = type;
            this.Owner = owner;
            Item characteristics = GameData.GameItems[type];

            if (characteristics == null)
            {
                DebugManager.WriteError("Item type " + type + " not defined in GameContent.xml");
                SetDefaults();
            }
            else
            {
                this.IsConsumable = characteristics.IsConsumable;
                this.Consumes = characteristics.Consumes;
                this.ConsumeRate = characteristics.ConsumeRate;
                this.PassiveEffects = characteristics.PassiveEffects;
                this.ActiveEffects = characteristics.ActiveEffects;
                this.MaxAmount = characteristics.MaxAmount;
                this.Name = characteristics.Name;
                this.BaseCooldown = characteristics.BaseCooldown;
                this.Power = characteristics.Power;
                this.IsAutomatic = characteristics.IsAutomatic;
                this.Description = characteristics.Description;
                this.DestroyedWhenEmpty = characteristics.DestroyedWhenEmpty;
            }


            if (amount < 0)
                Amount = MaxAmount;
            else
                Amount = amount;
        }

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public Item()
        {
            //
            //Apply the default values for an item while it's being deserialized from the xml(in case they aren't specified in the xml)
            //
            SetDefaults();
        }

        private void SetDefaults()
        {
            Consumes = CONSUMES_DEFAULT;
            ConsumeRate = CONSUMERATE_DEFAULT;
            UseCooldown = USECOOLDOWN_DEFAULT;
            Name = "";
            Description = DESCRIPTION_DEFAULT;
            MaxAmount = MAXAMOUNT_DEFAULT;
            DestroyedWhenEmpty = DESTROYED_WHEN_EMPTY_DEFAULT;
            PassiveEffects = new ItemEffect[0];
            ActiveEffects = new ItemEffect[0];
        }

        public virtual void Update()
        {
            if (UseCooldown > 0)
                UseCooldown--;

            ApplyPassive();
        }

        protected virtual void ApplyPassive()
        {
            foreach (ItemEffect ie in PassiveEffects)
            {
                ie.Activate(this.Owner);
            }
        }

        protected virtual void ApplyActive()
        {
            foreach (ItemEffect ie in ActiveEffects)
            {
                ie.Activate(this.Owner);
            }
        }
    }
}