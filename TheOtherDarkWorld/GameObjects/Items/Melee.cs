using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Melee : Item
    {
        public int Reach { get; set; }
        public MeleeType AttackType { get; set; }
        public int Knockback { get; set; }
        public int SwingLength { get; set; }


        public Melee(int Type, int StartingDurability = -1, Entity Owner = null)
            : base(Type, StartingDurability, Owner)
        {
            Melee characteristics = (Melee)GameData.GameItems[Type];
            this.Reach = characteristics.Reach;
            this.AttackType = characteristics.AttackType;
            this.Knockback = characteristics.Knockback;
        }


        public Melee()
        {
        }

        protected override void ApplyActive()
        {
            Owner.Swing = new Swing(Owner.Rotation, this.Reach, this.Power, this.BaseCooldown, Knockback, (int)Owner.Rect.Width, Owner);


            //Melee items work differently than normal items. The time taken to swing the weapon is added
            //to the base cooldown
            UseCooldown += SwingLength;
        }
    }


    public enum MeleeType
    {
        Swing
    }

    /// <summary>
    /// 
    /// </summary>
    public class Swing
    {
        private float Rotation;

        /// <summary>
        /// The distance the swing reaches in pixels
        /// </summary>
        private int Reach;
        private Entity Owner;
        private float Damage;
        private int Knockback;
        private float SwingSpeed;
        private float AddedRotation;
        private Vector2 Position;
        private Vector2 RotationVector;
        private Vector2 HitVector;

        private Rectangle SourceRect;

        /// <summary>
        /// Note: The swing ends in half the time it takes to swing again. This makes timing the attack more important 
        /// </summary>
        /// <param name="Position">The Player's position</param>
        /// <param name="Rotation">The rotation of the Player</param>
        /// <param name="Reach">How many pixels the swing will reach</param>
        /// <param name="Damage">The damage caused by the swing</param>
        /// <param name="SwingSpeed">How quickly the swing ends</param>
        /// <param name="Owner">The entity that initiated the swing</param>
        public Swing(float Rotation, int Reach, float Damage, int SwingLength, int Knockback, int OwnerDiameter, Entity Owner)
        {
            this.Rotation = Rotation + (MathHelper.Pi * 1.65f);
            this.Knockback = Knockback;
            this.Reach = Reach;
            this.Damage = Damage;

            this.SwingSpeed = MathHelper.Pi / SwingLength;
            this.Owner = Owner;
            AddedRotation = 0;
            SourceRect = new Rectangle(0, 0, Textures.Swipe.Width, Reach);

            float pctx = (float)Math.Sin(Rotation);
            float pcty = -(float)Math.Cos(Rotation);

            HitVector = Vector2.Normalize(new Vector2(pctx, pcty));
            RotationVector = new Vector2(pctx + 1, pcty + 1) * (OwnerDiameter / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the swing has ended</returns>
        public bool Update(Vector2 newPosition)
        {
            AddedRotation += SwingSpeed;
            Rotation -= SwingSpeed;
            this.Position = newPosition + RotationVector;

            CheckCollisions();

            return (AddedRotation > MathHelper.Pi);
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < Level.CurrentLevel.Entities.Count; i++)
            {
                Entity e = Level.CurrentLevel.Entities[i];
                if (e != Owner) //An entity can't hit itself with its own melee attack
                {
                    if (e.HitCooldown <= 0)
                        if (Vector2.Distance(Position, e.Position + e.Origin) < Reach + (e.Texture.Width / 2))
                        {
                            e.ApplyDamage(Damage);
                            e.HitVelocity += (HitVector
                                * Knockback) / e.Weight;
                            e.HitCooldown = Knockback;
                        }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Offset)
        {
            spriteBatch.Draw(Textures.Swipe, Position - Offset, SourceRect, Color.White, Rotation, Vector2.Zero, 1, SpriteEffects.None, 0.7f);
        }
    }
}