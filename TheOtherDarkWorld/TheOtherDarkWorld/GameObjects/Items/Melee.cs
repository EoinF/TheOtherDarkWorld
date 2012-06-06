using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.Items
{
    public class Melee : Item
    {
        public int Reach { get; private set; }
        public MeleeType AttackType { get; private set; }

        public Melee(int Type, int StartingDurability = -1)
            : base(Type, StartingDurability)
        {
            Melee characteristics = (Melee)GameData.GameItems[Type];
            this.Reach = characteristics.Reach;
            this.AttackType = characteristics.AttackType;
        }

        public Melee(int Type, bool IsConsumable, int Consumes, int MaxAmount, string Name, int UseRate, int Power, int Reach, MeleeType AttackType, bool IsAutomatic, string Description)
            : base(Type, IsConsumable, Consumes, MaxAmount, Name, UseRate, Power, IsAutomatic, Description)
        {
            this.Reach = Reach;
            this.AttackType = AttackType;
        }

        /// <summary>
        /// The acrivate method of the Melee class; It reduces the durability of the weapon(if its consumable).
        /// </summary>
        /// <returns>Returns true if the weapon has been destroyed</returns>
        public override bool Activate(float rotation, Vector2 Direction, Vector2 startPosition)
        {
            if (IsConsumable)
            {
                //Find out what this item does and trigger it
                Cooldown = UseCooldown;
                Amount--;
            }

            return (Amount == 0);

        }
    }


    public enum MeleeType
    {
        Swing
    }
}
