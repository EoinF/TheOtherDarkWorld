using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class ItemEffect
    {
        public const EffectType DEFAULT_EFFECT_TYPE = EffectType.Heal;
        public const int DEFAULT_EFFECT_ID = -1;

        public int ID;
        public EffectType Type;
        public float Potency;
        public int Duration;
        public string Description;
        public bool Negate;

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public ItemEffect()
        {
            ID = DEFAULT_EFFECT_ID;
            Type = DEFAULT_EFFECT_TYPE;
            Potency = 0;
        }

        public virtual void Activate(Entity Owner)
        {
            switch (Type)
            {
                //
                //Special item effects
                //
                case EffectType.Heal:
                    Owner.ApplyHealing(Potency);
                    break;
                case EffectType.LifeUp:
                    Owner.MaxHealth += (int)Potency;
                    break;

                //
                //Status effects
                //
                case EffectType.Bind:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Binded, Potency, Duration, Description, ID));
                    break;
                case EffectType.Bleed:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Bleeding, Potency, Duration, Description, ID));
                    break;
                case EffectType.Blind:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Blinded, Potency, Duration, Description, ID));
                    break;
                case EffectType.Burn:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Burning, Potency, Duration, Description, ID));
                    break;
                case EffectType.Confuse:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Confused, Potency, Duration, Description, ID));
                    break;
                case EffectType.Curse:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Cursed, Potency, Duration, Description, ID));
                    break;
                case EffectType.FastHands:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.FastHands, Potency, Duration, Description, ID));
                    break;
                case EffectType.Freeze:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Frozen, Potency, Duration, Description, ID));
                    break;
                case EffectType.Ghost:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Ghost, Potency, Duration, Description, ID));
                    break;
                case EffectType.Harden:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Hardened, Potency, Duration, Description, ID));
                    break;
                case EffectType.Injure:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Injured, Potency, Duration, Description, ID));
                    break;
                case EffectType.Invincible:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Invincible, Potency, Duration, Description, ID));
                    break;
                case EffectType.Invisbile:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Invisible, Potency, Duration, Description, ID));
                    break;
                case EffectType.Perceptive:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Perceptive, Potency, Duration, Description, ID));
                    break;
                case EffectType.Poison:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Poison, Potency, Duration, Description, ID));
                    break;
                case EffectType.Protect:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Benediction, Potency, Duration, Description, ID));
                    break;
                case EffectType.Slow:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Slowed, Potency, Duration, Description, ID));
                    break;
                case EffectType.SpeedBoost:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.QuickFooted, Potency, Duration, Description, ID));
                    break;
                case EffectType.Stun:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Stunned, Potency, Duration, Description, ID));
                    break;
                case EffectType.Healing:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Healing, Potency, Duration, Description, ID));
                    break;
                case EffectType.Exhaust:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Exhausted, Potency, Duration, Description, ID));
                    break;
                default:
                    Owner.AddStatusEffect(new StatusEffect(StatusType.Healing, Potency, Duration, Description, ID));
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

    /// <summary>
    /// See the StatusType enumerator in the StatusEffect class for a full description of each effect 
    /// </summary>
    public enum EffectType
    {
        //
        //Special item effects
        //
        Heal, //Instant heal
        LifeUp, //Increase maximum health

        //
        //Status effects
        //
        Healing, //Heal over time
        Blind,
        Stun,
        Confuse,
        Burn, //Set on fire
        Poison,
        Freeze,
        Invisbile,
        Bind,
        Slow,
        Curse,
        Invincible,
        Injure,
        Bleed,
        SpeedBoost,
        Perceptive,
        Ghost,
        Harden,
        Protect,
        FastHands,
        Exhaust,
    }
}
