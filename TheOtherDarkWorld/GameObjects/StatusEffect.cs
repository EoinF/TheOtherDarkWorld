using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class StatusEffect
    {
        public const int ID_EXAUSTION_SLOW = 0;
        public const int ID_EXAUSTION_EXHAUST = 1;

        public static Color COLOUR_PROTECTED = Color.WhiteSmoke;
        public static Color COLOUR_POISON = Color.Green;
        public static Color COLOUR_BURNING = Color.PaleVioletRed;
        public static Color COLOUR_FROZEN = Color.Aqua;
        public static Color COLOUR_INVISIBLE = Color.Transparent;

        public StatusType Type { get; set; }
        public int ID { get; set; }
        public float Potency { get; set; }

        /// <summary>
        /// If there are a whole number of seconds remaining
        /// </summary>
        public bool OnSecondTick
        {
            get { return (RemainingTicks % 60) == 0; }
        }
        public int RemainingTicks;
        public string Description { get; private set; }

        public StatusEffect(StatusType Type, float Potency, int Duration_Ticks, string Description, int ID = ItemEffect.DEFAULT_EFFECT_ID)
        {
            this.ID = ID;
            this.Type = Type;
            this.Potency = Potency;
            RemainingTicks = Duration_Ticks;
            this.Description = Description;
        }

        public void Update()
        {
            RemainingTicks--;
        }
    }

    public enum StatusType
    {
        Blinded, //Screen goes white and can't see any in game objects
        Stunned, //Intelligence method isn't called. Therefore movement and vision is disabled as well
        Confused, //The character's direction is altered
        Burning, //Health degenerates. Flammable objects have a chance to burst into flames
        Poison, //Health degenerates. (TODO: Reduces healing effects)
        Frozen, //Can't move or activate items
        Invisible, //Can't be seen by other entities
        Binded, //Can't move
        Slowed, //Slower movement speed
        Cursed, //Item cooldowns never refresh
        Invincible, //Takes 0 damage
        Healing, //Recovers x health per tick
        Injured, //Reduces Max Health cap by x
        Bleeding, //Health slowly degenerates. Can attract certain monsters without them having vision
        QuickFooted, //Increased Speed
        Perceptive, //Displays hp and status effects of other entities(Player only)
        Ghost, //Can walk through walls and enemies
        Hardened, //Reduces damage received
        Benediction, //Health cannot fall below 1 (Costs -1 potency each time the entity is prevented from falling below 1 health)
        Dexterity, //Reduces item usage cooldowns
        Exhausted //Reduces energy recovery rate
    }
}
