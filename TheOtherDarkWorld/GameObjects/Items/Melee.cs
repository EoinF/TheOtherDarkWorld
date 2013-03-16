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
        public int Reach { get; private set; }
        public MeleeType AttackType { get; private set; }
        private int Knockback { get; set; }


        public Melee(int Type, int StartingDurability = -1)
            : base(Type, StartingDurability)
        {
            Melee characteristics = (Melee)GameData.GameItems[Type];
            this.Reach = characteristics.Reach;
            this.AttackType = characteristics.AttackType;
            this.Knockback = characteristics.Knockback;
        }

        public Melee(int Type, bool IsConsumable, int Consumes, int MaxAmount, string Name, int UseRate, int Power, int Reach, int Knockback, MeleeType AttackType, bool IsAutomatic, string Description)
            : base(Type, IsConsumable, Consumes, MaxAmount, Name, UseRate, Power, IsAutomatic, Description)
        {
            this.Reach = Reach;
            this.AttackType = AttackType;
            this.Knockback = Knockback;
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
                Player.PlayerList[0].Swing = new Swing(Player.PlayerList[0].Rotation, this.Reach, this.Power, this.UseCooldown, Knockback, (int)Player.PlayerList[0].Rect.Width, Player.PlayerList[0].ID);
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
        private int Owner;
        private int Damage;
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
        /// <param name="Owner">The ID of the entity that initiated the swing</param>
        public Swing(float Rotation, int Reach, int Damage, int TotalCooldown, int Knockback, int OwnerDiameter, int Owner)
        {
            this.Rotation = Rotation + (MathHelper.Pi * 1.65f);
            this.Knockback = Knockback;
            this.Reach = Reach;
            this.Damage = Damage;

            //Two Pi is used, because we want the swing to end in half the time it takes the cooldown to end.
            this.SwingSpeed =  MathHelper.TwoPi / TotalCooldown;
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
            for (int i = 0; i < Level.CurrentLevel.Enemies.Count; i++)
            {
                Enemy e = Level.CurrentLevel.Enemies[i];
                if (e.HitCooldown <= 0)
                    if (Vector2.Distance(Position, e.Position + e.Origin) < Reach + (e.Texture.Width / 2))
                    {
                        e.Health -= Damage;
                        e.HitVelocity += (HitVector
                            * Knockback) / e.Weight;
                        e.HitCooldown = 50;
                    }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Offset)
        {
            spriteBatch.Draw(Textures.Swipe, Position - Offset, SourceRect, Color.White, Rotation, Vector2.Zero, 1, SpriteEffects.None, 0.7f);
        }
    }
}
