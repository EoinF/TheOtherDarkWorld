using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Enemy : GameObject
    {
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public int Type { get; private set; }
        public int HitCooldown { get; set; }
        public int Weight { get; private set; }
        public int MeleeDamage { get; private set; }
        public bool IsAttacking { get { return HitCooldown <= 0; } }
        private Color LightColour
        {
            get
            {
                //Preserve the alpha component so that the tiles will actually be drawn
                byte alpha = Colour.A;
                Color c = Colour * Brightness;
                c.A = alpha;
                return c;
            }

        }

        private float Brightness { get; set; }

        /// <summary>
        /// This is the velocity that is added to the standard velocity of the enemy. e.g. When pushed by a bullet
        /// </summary>
        public Vector2 HitVelocity { get; set; }

        public new Color Colour
        {
            get
            {
                return (HitCooldown > 0 ? Color.PaleVioletRed : Color.White);
            }
        }

        public override Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Enemies[Type].Width, Textures.Enemies[Type].Height); }
        }

        private Vector2 Target;
        public float Rotation
        {
            get
            {
                float rot = (float)Math.Atan((Target.Y - Rect.Center.Y) / (Target.X - Rect.Center.X)) - MathHelper.PiOver2;

                if (Target.X >= Rect.Center.X)
                    rot += MathHelper.Pi;

                return rot;
            }
        }

        public Enemy(Vector2 startPosition, float speed, Vector2 startVelocity, int Type, int MaxHealth, int Resistance, int Weight, int MeleeDamage)
            : base(startPosition, speed, Color.White, Vector2.Zero, new Vector2(Textures.Enemies[Type].Width / 2, Textures.Enemies[Type].Height / 2), Resistance)
        {
            this.Type = Type;
            this.Health = MaxHealth;
            this.MaxHealth = MaxHealth;
            Target = Vector2.Zero;
            this.Weight = Weight;
            HitVelocity = Vector2.Zero;
            this.MeleeDamage = MeleeDamage;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the enemy is dead</returns>
        public bool Update()
        {
            if (HitCooldown <= 0) //The enemy is stunned when hit, so it has no intelligence
                AI();

            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                System.Diagnostics.Debugger.Break();

            Velocity = Vector2.Normalize(new Vector2(Target.X - (this.Position.X + this.Origin.X), Target.Y - (this.Position.Y + this.Origin.Y)));

            //The velocity can become NaN if the enemy is in the exact same position as the target
            if (float.IsNaN(Velocity.X))
                Velocity = Vector2.Zero;
            Velocity *= Speed;
            Velocity += HitVelocity;

            //Attenuates the hit velocity
            HitVelocity *= 0.9f;

            //When the hit velocity is low, it won't reach zero very quickly. So this makes 
            //it look smoother when the enemy is stationary
            if (HitVelocity.Length() < 0.001f)
                HitVelocity = Vector2.Zero;

            CheckCollisions(Level.CurrentLevel.Tiles);
            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                System.Diagnostics.Debugger.Break();

            CheckProjectileCollisions();
            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                System.Diagnostics.Debugger.Break();

            CheckEnemyCollisions();
            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                System.Diagnostics.Debugger.Break();

            UpdateLighting(Level.CurrentLevel.Tiles);

            return (Health <= 0);
        }

        private void UpdateLighting(Tile[,] Tiles)
        {
            int startX = (int)(Position.X / 10);
            int endX = startX + (Texture.Width / 10) + 1;

            if (startX < 0)
                startX = 0;
            if (endX >= Level.CurrentLevel.Width)
                endX = Level.CurrentLevel.Width - 1;

            int startY = (int)(Position.Y / 10);
            int endY = startY + (Texture.Height / 10) + 1;

            if (startY < 0)
                startY = 0;
            if (endY >= Level.CurrentLevel.Height)
                endY = Level.CurrentLevel.Height - 1;

            float total = 0;
            int tilesChecked = 0;
            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    total += Tiles[i, j].Brightness;
                    tilesChecked++;
                }
            this.Brightness = total / tilesChecked;
        }

        protected virtual void AI()
        {
            Target = Player.PlayerList[0].Position + Player.PlayerList[0].Origin;
        }

        private void CheckProjectileCollisions()
        {
            for (int i = 0; i < Projectile.ProjectileList.Count; i++)
            {
                Projectile p = Projectile.ProjectileList[i];
                if (Collision.SquareVsSquare_TwoMoving(p.Rect, Rect, p.Velocity, Velocity))
                {
                    this.Health -= p.Damage;
                    p.Health -= this.Resistance;
                    this.HitVelocity += p.Velocity / Weight;
                    HitCooldown = 30;

                    if (this.Health <= MaxHealth) //The enemy won't absorb too many bullets if we stop the checks early
                        return;
                }
            }

            if (HitCooldown > 0)
                HitCooldown--;
        }

        private void CheckEnemyCollisions()
        {
            //Find the index of this enemy in the list. Collisions have already been detected for all enemies before 
            //this one in the list, so theres no need to check again
            int i = 0;
            while (!this.Equals(Level.CurrentLevel.Enemies[i]))
            {
                i++;
            }

            //Skip to the next enemy, because an enemy can't collide with itself
            i++;

            while (i < Level.CurrentLevel.Enemies.Count)
            {
                Enemy e = Level.CurrentLevel.Enemies[i];

                if ((this.Position - e.Position).Length() < 70)
                {
                    if (Collision.SquareVsSquare_TwoMoving(Rect, e.Rect, Velocity, e.Velocity))
                    {
                        //Collision c = GetCollisionDetails(e.Rect, 0, 0);

                        Vector2 Bounce = this.Position - e.Position;
                        Bounce.Normalize();
                        if (float.IsNaN(Bounce.X))
                            Bounce = Vector2.UnitX;
                        //Bounce *= (Velocity.Length() + e.Velocity.Length()) / 2f;

                        float push = ((float)this.Weight / (float)(this.Weight + e.Weight)) / 2f;
                        //push += ((float)this.Velocity.Length() / (e.Velocity.Length() + this.Velocity.Length())) / 2f;

                        //Add the percentage of push that this enemy should recieve
                        this.HitVelocity += Bounce * push;
                        //Add the remainder to the other enemy
                        e.HitVelocity -= Bounce * (1 - push);

                        if (float.IsNaN(HitVelocity.X) || float.IsNaN(HitVelocity.Y))
                            System.Diagnostics.Debugger.Break();
                    }
                }
                i++;
            }

        }


        public Texture2D Texture
        {
            get { return Textures.Enemies[Type];}
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Enemies[Type], Position + Origin - Player.PlayerList[0].Offset, null, LightColour, Rotation, Origin, 1, SpriteEffects.None, 0.12f);
        }

    }
}
