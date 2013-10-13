using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Enemy : Entity
    {
        const int DEFAULT_MELEE_COOLDOWN = 20;

        public int Type { get; private set; }
        public int MeleeDamage { get; private set; }
        private int MeleeCooldown { get; set; }
        public bool IsAttacking { get { return HitCooldown <= 0; } }

        public override Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Enemies[Type].Width, Textures.Enemies[Type].Height); }
        }


        public Enemy(Vector2 startPosition, float speed, Vector2 startVelocity, int Type, int MaxHealth, int Resistance, int Weight, int MeleeDamage)
            : base(MaxHealth, 4,startPosition, speed, Color.White, Vector2.Zero, new Vector2(Textures.Enemies[Type].Width / 2, Textures.Enemies[Type].Height / 2), Resistance, (int)Textures.Enemies[Type].Width, (int)Textures.Enemies[Type].Height)
        {
            this.Type = Type;
            Target = Vector2.Zero;
            this.Weight = Weight;
            HitVelocity = Vector2.Zero;
            this.MeleeDamage = MeleeDamage;
        }

        protected override void Intelligence()
        {
            Target = Level.CurrentLevel.Players[0].Position + Level.CurrentLevel.Players[0].Origin;
            Vector2 Displacement = new Vector2(Target.X - (this.Position.X + this.Origin.X), Target.Y - (this.Position.Y + this.Origin.Y));
            
            Velocity = Vector2.Normalize(Displacement);
            if (Swing == null)
            {
                if (MeleeCooldown <= 0)
                {
                    if (Displacement.Length() < 20)
                    {
                        Swing = new Swing(Rotation, 20, MeleeDamage, (Weight * 2), Weight * 2, this.Texture.Width, this);
                        MeleeCooldown = DEFAULT_MELEE_COOLDOWN;
                    }
                }
                else
                    MeleeCooldown--;
            }
        }



        public override Texture2D Texture
        {
            get { return Textures.Enemies[Type]; }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
                spriteBatch.Draw(Textures.Enemies[Type], Position + Origin - Level.CurrentLevel.Players[0].Offset, null, getLightColour(), Rotation, Origin, 1, SpriteEffects.None, 0.12f);
            if (Swing != null)
            {
                Swing.Draw(spriteBatch, Level.CurrentLevel.Players[Level.CurrentLevel.PlayerIndex].Offset);
            }
        }

    }
}
