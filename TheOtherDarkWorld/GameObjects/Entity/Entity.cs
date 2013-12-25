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

        //public int ID;
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
        public virtual bool IsAlive { get { return Health > 0; } }
        public int HitCooldown { get; set; }
        public int Weight { get; protected set; }


        public Light AuraLight { get; protected set; }
        public List<StatusEffect> StatusEffects;
        public bool IsBlinded { get; protected set; }
        public bool IsStunned { get; protected set; }
        public bool IsConfused { get; protected set; }
        public Vector2 ConfusionVector { get; protected set; }
        public bool IsPoisoned { get; protected set; }
        public bool IsBurning { get; protected set; }
        public bool IsFrozen { get; protected set; }
        public bool IsInvisible { get; protected set; }
        public bool IsBinded { get; protected set; }
        public bool IsProtected { get; protected set; }
        public bool IsBleeding { get; protected set; }
        public float SpeedBonus { get; protected set; }
        public override float Speed 
        {
            get { return base.Speed + SpeedBonus; } 
            set { base.Speed = value; } 
        }

        private static Color COLOUR_PROTECTED = Color.WhiteSmoke;
        private static Color COLOUR_POISON = Color.Green;
        private static Color COLOUR_BURNING = Color.PaleVioletRed;
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

        /// <summary>
        /// This is the velocity that is added to the standard velocity of the entity. e.g. When pushed by a bullet or other entity
        /// </summary>
        public Vector2 HitVelocity { get; set; }

        protected virtual Vector2 Target { get; set; }

        public Vector2 Direction
        {
            get
            {
                return new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation));
            }
        }

        public virtual float Rotation
        {
            get
            {
                return (float)Math.Atan2(Target.Y - Rect.Center.Y, Target.X - Rect.Center.X) + MathHelper.PiOver2;
            }
            protected set { throw new NotImplementedException("This method exists so that the Projectile class can derive its own Rotation setter method"); }
        }

        public Entity(int MaxHealth, Vector2 startPosition, float speed, Color colour, Vector2 startVelocity, Vector2 Origin, int Resistance, int width, int height)
            : base(startPosition, speed, colour, startVelocity, Origin, Resistance, width, height)
        {
            this.MaxHealth = MaxHealth;
            this.Health = this.MaxHealth;
            StatusEffects = new List<StatusEffect>();
            StatusColour = Color.White;
            AuraLight = StateManager.CreateLight(0, 30, Vector2.Zero, Vector2.UnitX, MathHelper.Pi, Color.White, false);
        }

        static Entity()
        {
            statusGen = new Random();
        }

        
        public void Update(Tile[,] Tiles, List<Entity> Entities)
        {
            if (this is IItemHolder)
            {
                (this as IItemHolder).UpdateInventory();
            }

            UpdateStatusEffects();
            UpdateVisibility(Tiles);

            Velocity = Vector2.Zero; //Reset velocity every frame because previous velocity is recorded in HitVelocity anyway


            if (!IsStunned) //The entity can't do anything intelligent while stunned
            {
                if (IsAlive)
                    Intelligence();
            }
            HitCooldown--;

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
        }

        protected void UpdateVisibility(Tile[,] Tiles)
        {
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
            SpeedBonus = 0;


            for (int i = 0; i < StatusEffects.Count; i++)
            {
                if (StatusEffects[i].RemainingTicks < 0)
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
                            AuraLight.Brightness = StatusEffects[i].Potency;
                            break;
                        case StatusType.Poison:
                            IsPoisoned = true;
                            ApplyDamage(StatusEffects[i].Potency);
                            break;
                        case StatusType.Healing:
                            ApplyHealing(StatusEffects[i].Potency);
                            break;
                        case StatusType.QuickFooted:
                            SpeedBonus += StatusEffects[i].Potency;
                            break;
                        case StatusType.Benediction:
                            AuraLight.Brightness = StatusEffects[i].Potency;
                            IsProtected = true;
                            break;
                        case StatusType.Binded:
                            IsBinded = true;
                            break;
                        case StatusType.Bleeding:
                            IsBleeding = true;
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
            if (IsProtected)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_PROTECTED, 0.5f);
                AuraLight.Colour = COLOUR_PROTECTED;
                AuraLight.IsActive = true;
                AuraLight.Update(Position + (Origin / 2), Vector2.UnitX);
            }
            else if (IsBurning)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_BURNING, 0.5f);
                AuraLight.Colour = COLOUR_BURNING;
                AuraLight.IsActive = true;
                AuraLight.Update(Position + (Origin / 2), Vector2.UnitX);
            }
            else
                AuraLight.IsActive = false;

            if (IsFrozen)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_FROZEN, 0.5f);
            }
            if (IsInvisible)
            {
                StatusColour = Color.Lerp(StatusColour, COLOUR_INVISIBLE, 0.5f);
            }

        }

        protected abstract void Intelligence();

        public void CheckEntityCollision(Entity entity)
        {
            if ((this.Position - entity.Position).Length() < 70)
            {
                if (Collision.SquareVsSquare(this, entity))
                {
                    OnCollideWith(entity);
                }
            }
        }

        protected virtual void OnCollideWith(Entity entity)
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

    }
}

