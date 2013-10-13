using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public abstract class Entity : GameObject
    {
        private static Random statusGen;

        public int ID;
        public int Health
        {
            get 
            {
                return (int)Math.Ceiling(_health);
            }
            private set
            { 
                _health = value;
            }
        }
        private float _health;

        public int MaxHealth { get; protected set; }
        public bool IsAlive { get { return Health > 0; } }
        public int HitCooldown { get; set; }
        public int Weight { get; protected set; }
        public Item[] Inventory { get; protected set; }

        public List<StatusEffect> StatusEffects;
        public bool IsBlinded { get; protected set; }
        public bool IsStunned { get; protected set; }
        public bool IsConfused { get; protected set; }
        public Vector2 ConfusionVector { get; protected set; }
        public bool IsPoisoned { get; protected set; }
        public bool IsBurning { get; protected set; }
        public Light BurnLight { get; protected set; }
        public bool IsFrozen { get; protected set; }
        public bool IsInvisible { get; protected set; }
        public bool IsBinded { get; protected set; }

        private static Color COLOUR_POISON = Color.Green;
        private static Color COLOUR_BURNING = Color.Red;
        private static Color COLOUR_FROZEN = Color.Aqua;
        private static Color COLOUR_INVISIBLE = Color.Transparent;


        public override Color Colour
        {
            get
            {
                return HitCooldown > 0 ? Color.Red : StatusColour;
            }
        }

        public Color StatusColour
        {
            get;
            set;
        }

        public abstract Texture2D Texture { get; }

        public Swing Swing;

        /// <summary>
        /// This is the velocity that is added to the standard velocity of the entity. e.g. When pushed by a bullet or other entity
        /// </summary>
        public Vector2 HitVelocity { get; set; }

        protected virtual Vector2 Target { get; set; }

        public float Rotation
        {
            get
            {
                return (float)Math.Atan2(Target.Y - Rect.Center.Y, Target.X - Rect.Center.X) + MathHelper.PiOver2;
            }
        }

        public Entity(int MaxHealth, int inventorySize, Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin, int Resistance, int width, int height)
            : base(startPosition, speed, colour, startVelocity, Origin, Resistance, width, height)
        {
            this.MaxHealth = MaxHealth;
            this.Health = this.MaxHealth;
            Inventory = new Item[inventorySize];
            StatusEffects = new List<StatusEffect>();
            BurnLight = new Light(0, 30, Vector2.Zero, Vector2.UnitX, MathHelper.Pi, Color.PaleVioletRed, Level.CurrentLevel.Tiles, false);
            Level.CurrentLevel.Lights.Add(BurnLight);
                            
        }

        static Entity()
        {
            statusGen = new Random();
        }

        public virtual bool PickUpItem(Item item)
        {
            //Check if there's space to fit the new item in the inventory
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null)
                {
                    Inventory[i] = item;
                    item.Owner = this;
                    return true;
                }
            }
            return false;
        }

        public void Update(Tile[,] Tiles, List<Entity> Entities)
        {
            UpdateStatusEffects();

            //
            //Check if the entity is lit up and/or visible
            //
            int tilex = (int)(Position.X / 10);
            int tiley = (int)(Position.Y / 10);

            if (tilex >= 0 && tilex + 1 < Tiles.GetLength(0)
                && tiley >= 0 && tiley + 1 < Tiles.GetLength(1))
            {
                float averageBrightness = (Tiles[tilex, tiley].Brightness
                    + Tiles[tilex + 1, tiley].Brightness
                    + Tiles[tilex, tiley + 1].Brightness
                    + Tiles[tilex + 1, tiley + 1].Brightness) 
                        / 4f;

                Brightness = averageBrightness;

                IsVisible = Tiles[tilex, tiley].IsVisible
                    || Tiles[tilex + 1, tiley].IsVisible
                    || Tiles[tilex, tiley + 1].IsVisible
                    || Tiles[tilex + 1, tiley + 1].IsVisible;
            }

            if (IsBurning)
            {
                BurnLight.IsActive = true;
                BurnLight.Update(Position + (Origin / 2), Vector2.UnitX);
            }
            else
                BurnLight.IsActive = false;

            Velocity = Vector2.Zero; //Reset velocity every frame because previous velocity is recorded in HitVelocity anyway

            if (!IsStunned) //The entity can't do anything intelligent while stunned
            {
                if (IsAlive)
                    Intelligence();
            }
            HitCooldown--;

            //Update the swing of the melee weapon if the weapon is being swung
            if (Swing != null)
            {
                if (Swing.Update(Position))
                    Swing = null;
            }

            if (IsFrozen)
            {
                Velocity = Vector2.Zero;
                HitVelocity = Vector2.Zero;
            }
            else
            {
                //The velocity can become NaN if the enemy is in the exact same position as the target
                if (float.IsNaN(Velocity.X))
                    Velocity = Vector2.Zero;
                Velocity *= Speed;
                Velocity += HitVelocity;

                //Attenuates the hit velocity
                HitVelocity *= 0.9f;

                //When the hit velocity is low, it won't reach zero very quickly. So this makes 
                //it look smoother when the entity is stationary
                if (HitVelocity.Length() < 0.001f)
                    HitVelocity = Vector2.Zero;

                if (IsConfused)
                {
                    Velocity = ConfusionVector * Velocity.Length();
                }
            }

            CheckCollisions(Tiles);
            CheckProjectileCollisions();
        }

        private void UpdateStatusEffects()
        {
            //
            //Reset all effects from the last frame to default
            //
            IsBlinded = false;
            IsStunned = false;
            IsConfused = false;
            IsPoisoned = false;
            IsBurning = false;


            for (int i = 0; i < StatusEffects.Count; i++)
            {
                if (StatusEffects[i].RemainingSeconds < 0)
                {
                    StatusEffects.RemoveAt(i);
                    i--;
                    continue;
                }
                else
                {
                    switch (StatusEffects[i].Type)
                    {
                        case StatusType.Blinded:
                            IsBlinded = true;
                            break;
                        case StatusType.Stunned:
                            IsStunned = true;
                            break;
                        case StatusType.Confused:
                            IsConfused = true;
                            if (StatusEffects[i].OnSecondTick)
                            {
                                ConfusionVector = new Vector2((float)(statusGen.NextDouble() * 2) - 1, (float)(statusGen.NextDouble() * 2) - 1);
                                ConfusionVector.Normalize(); //This will be the new direction vector
                            }
                            break;
                        case StatusType.Burning:
                            IsBurning = true;
                            ApplyDamage(StatusEffects[i].Potency);
                            BurnLight.Brightness = StatusEffects[i].Potency;
                            break;
                        case StatusType.Poison:
                            IsPoisoned = true;
                            ApplyDamage(StatusEffects[i].Potency);
                            break;
                    }
                }
                StatusEffects[i].Update();
            }

            //
            //Reset the status colour and then update it based on current status effects
            //
            StatusColour = Color.White;
            if (IsPoisoned)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_POISON, 0.5f);
            }
            if (IsBurning)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_BURNING, 0.5f);
            }
            if (IsFrozen)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_FROZEN, 0.5f);
            }
            if (IsInvisible)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_INVISIBLE, 0.5f);
            }
        }


        private void CheckProjectileCollisions()
        {
            for (int i = 0; i < Projectile.ProjectileList.Count; i++)
            {
                Projectile p = Projectile.ProjectileList[i];

                if (p.Owner != this) //The owner of a projectile can't hit itself
                {
                    if (Collision.SquareVsSquare(this, p))
                    {
                        ApplyDamage(p.Damage);
                        p.Health -= this.Resistance;
                        this.HitVelocity += p.Velocity / Weight;
                        HitCooldown = 30;

                        //The entity won't absorb too many bullets if we stop the checks early
                        return; //By returning, rather than moving on to the next projectile
                    }
                }
            }
        }

        protected abstract void Intelligence();

        public void CheckEntityCollision(Entity entity)
        {
            if ((this.Position - entity.Position).Length() < 70)
            {
                if (Collision.SquareVsSquare(this, entity))
                {
                    //Collision c = GetCollisionDetails(e.Rect, 0, 0);

                    Vector2 Bounce = this.Position - entity.Position;
                    Bounce.Normalize();
                    if (float.IsNaN(Bounce.X))
                        Bounce = Vector2.UnitX;
                    //Bounce *= (Velocity.Length() + e.Velocity.Length()) / 2f;

                    float push = ((float)this.Weight / (float)(this.Weight + entity.Weight)) / 2f;
                    //push += ((float)this.Velocity.Length() / (e.Velocity.Length() + this.Velocity.Length())) / 2f;

                    //Add the percentage of push that this entity should recieve
                    this.HitVelocity += Bounce * (1 - push);
                    //Add the remainder to the other entity
                    entity.HitVelocity -= Bounce * push;

                    if (float.IsNaN(HitVelocity.X) || float.IsNaN(HitVelocity.Y))
                        System.Diagnostics.Debugger.Break();
                     
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">Submit a positive number to damage by that much or a negative number to set health to 0</param>
        public void ApplyDamage(float amount)
        {
            if (amount < 0)
                Health = 0;

            //
            //TODO: Apply buffs/debuffs when they're implemented
            //


            if (amount > 0) //Damage must be at least greater than 0
            {
                _health -= amount;

                if (Health <= 0)
                    Health = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">Submit a positive number to heal by that much or a negative number to recover health to maximum</param>
        public void ApplyHealing(float amount)
        {
            if (amount < 0)
                Health = MaxHealth;

            //
            //TODO: Apply buffs/debuffs when they're implemented
            //

            if (amount > 0) //Healing must be at least greater than 0
            {
                _health += amount;
                if (Health > MaxHealth)
                    Health = MaxHealth;
            }
        }

        /// <summary>
        /// Get's the first occurrence of an item within this entity's inventory, or if none is found, returns null
        /// </summary>
        /// <param name="type">The type of item required</param>
        /// <returns>The item with the requested type, or if there is no item, returns null</returns>
        public Item GetItem(int type)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].Type == type && Inventory[i].Amount != 0)
                    return Inventory[i];
            }
            return null;
        }
    }
}
