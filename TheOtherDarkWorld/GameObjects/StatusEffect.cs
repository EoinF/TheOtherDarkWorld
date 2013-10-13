using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class StatusEffect
    {
        public StatusType Type { get; set; }

        public float Potency { get; set; }

        /// <summary>
        /// If there are a whole number of seconds remaining
        /// </summary>
        public bool OnSecondTick
        {
            get { return (RemainingTicks % 60) == 0; }
        }
        public int RemainingSeconds
        {
            get { return RemainingTicks / 60; }
        }
        private int RemainingTicks;
        public string Description { get; private set; }

        public StatusEffect(StatusType Type, float Potency, int Duration_Seconds, string Description )
        {
            this.Type = Type;
            this.Potency = Potency;
            RemainingTicks = Duration_Seconds * 60;
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
        Stunned, //Intelligence method isn't called
        Confused, //The character's direction is altered
        Burning, //Health quickly degenerates. Flammable objects have a chance to burst into flames
        Poison, //Health slowly degenerates
        Frozen, //Can't move or activate items
        Invisible, //Can't be seen by other entities
        Binded, //Can't move
        Slowed, //Slower movement speed
        Cursed, //Item cooldowns never refresh
        Invincible, //Takes 0 damage
        Healing, //Recovering 1% of health per second
        Injured, //Reduces Max Health cap by 20%
        Bleeding, //Health slowly degenerates. Can attract certain monsters without having vision
        QuickFooted, //Increased Speed
        Perceptive, //Displays hp and status effects of other entities(Player only)
        Ghost, //Can walk through walls and enemies
        Hardened, //Reduces damage received
        DivineProtection, //Health cannot fall below 1
        FastHands //Reduces item usage cooldowns
    }
}
