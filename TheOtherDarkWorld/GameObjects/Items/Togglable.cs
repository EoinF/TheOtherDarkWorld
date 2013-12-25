using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class Togglable : Item
    {
        protected bool IsActive { get; set; }

        public override int Type
        {
            get
            {
                return IsActive ? _type + 1 : _type; //1 texture is the active state. The other is the inactive state
            }
            set
            {
                if (value < GameData.GameItems.Length && value >= 0)
                    _type = value;
            }
        }

        public Togglable(int type, Entity owner = null)
            : base(type, owner: owner)
        {
            Togglable characteristics = GameData.GameItems[type] as Togglable;

            //this.c = characteristics.c;

        }

        public Togglable()  //Parameterless constructor for Xml serialization
        {
        }

        public virtual void Deactivate()
        {

        }

        protected virtual void Toggle()
        {
            IsActive = !IsActive;
        }

        protected override void ApplyActive()
        {
            Toggle();
        }

        protected override void ApplyPassive()
        {
            if (IsActive)
            {
                Item fuel = (Owner as IItemHolder).GetItem(Consumes);
                if (Consumes == -1 //If consumes is set to -1 it's free to use
                    || (fuel != null && fuel.Consume(this.ConsumeRate))) //otherwise check for fuel
                {
                    ApplyToggledPassive();
                }
                else
                {
                    Toggle();
                }
            }
        }

        protected virtual void ApplyToggledPassive()
        {
        }
    }
}