using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class ItemEffect
    {
        public EffectType Type;
        public float Potency;
        public int Duration;
        public string Description;

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public ItemEffect()
        {
            Type = EffectType.Heal_Self;
            Potency = 0;
        }

        public virtual void Activate(Entity Owner)
        {
            switch (Type)
            {
                case EffectType.Heal_Self:
                    Owner.ApplyHealing(Potency);
                    break;
                case EffectType.Healing_Self:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Healing, Potency, Duration, Description));
                    break;
                case EffectType.Poison_Self:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Poison, Potency, Duration, Description));
                    break;
                case EffectType.QuickFooted_Self:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.QuickFooted, Potency, Duration, Description));
                    break;
                default:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Healing, Potency, Duration, Description));
                    break;
            }
        }
    }



    public class RangedItemEffect : ItemEffect
    {
        int Range;

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public RangedItemEffect()
        {
            Range = 0;
        }

        public override void Activate(Entity Owner)
        {
        }
    }

    public enum EffectType
    {
        Heal_Self, //Instant heal
        Healing_Self, //Heal over time
        Poison_Self,
        QuickFooted_Self,
        Cure_Injured_Self,
        Cure_Bleeding_Self,

    }
}
