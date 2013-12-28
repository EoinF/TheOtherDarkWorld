using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class ItemEffect
    {
        public const EffectType DEFAULT_EFFECT_TYPE = EffectType.Heal;

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
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Binded, Potency, Duration, Description));
                    break;
                case EffectType.Bleed:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Bleeding, Potency, Duration, Description));
                    break;
                case EffectType.Blind:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Blinded, Potency, Duration, Description));
                    break;
                case EffectType.Burn:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Burning, Potency, Duration, Description));
                    break;
                case EffectType.Confuse:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Confused, Potency, Duration, Description));
                    break;
                case EffectType.Curse:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Cursed, Potency, Duration, Description));
                    break;
                case EffectType.FastHands:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.FastHands, Potency, Duration, Description));
                    break;
                case EffectType.Freeze:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Frozen, Potency, Duration, Description));
                    break;
                case EffectType.Ghost:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Ghost, Potency, Duration, Description));
                    break;
                case EffectType.Harden:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Hardened, Potency, Duration, Description));
                    break;
                case EffectType.Injure:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Injured, Potency, Duration, Description));
                    break;
                case EffectType.Invincible:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Invincible, Potency, Duration, Description));
                    break;
                case EffectType.Invisbile:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Invisible, Potency, Duration, Description));
                    break;
                case EffectType.Perceptive:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Perceptive, Potency, Duration, Description));
                    break;
                case EffectType.Poison:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Poison, Potency, Duration, Description));
                    break;
                case EffectType.Protect:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Benediction, Potency, Duration, Description));
                    break;
                case EffectType.Slow:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Slowed, Potency, Duration, Description));
                    break;
                case EffectType.SpeedBoost:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.QuickFooted, Potency, Duration, Description));
                    break;
                case EffectType.Stun:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Stunned, Potency, Duration, Description));
                    break;
                case EffectType.Healing:
                    Owner.StatusEffects.Add(new StatusEffect(StatusType.Healing, Potency, Duration, Description));
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
    }
}
