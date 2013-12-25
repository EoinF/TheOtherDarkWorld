using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Projectile : Entity
    {
        public Action<Projectile, Collision> OnBlockHit;
        public GameObject Owner { get; private set; }
        public float Damage { get; private set; }

        private float _rotation;
        public override float Rotation
        {
            get { return _rotation; }
            protected set { _rotation = value; }
        }
        public override Texture2D Texture { get { return Textures.Bullet; } }
        private bool Collided;

        public override bool IsAlive
        {
            get
            {
                return Health > 0 && !IsOutOfBounds;
            }
        }
        private bool IsOutOfBounds { get; set; }

        
        public override Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Bullet.Width, Textures.Bullet.Height); }
        }

        public Rectanglef BaseRect
        {
            get { return new Rectanglef(0, 0, Textures.Bullet.Width, Textures.Bullet.Height); }
        }

        public new Vector2 Origin
        {
            get { return new Vector2(Textures.Bullet.Width / 2, Textures.Bullet.Height / 2); } 
        }
        

        public Projectile(float damage, int penetration, float speed, Color colour, GameObject owner, Vector2 startVelocity, Vector2 startPosition, float rotation, Action<Projectile, Collision> OnBlockHit)
            : base(penetration, startPosition, speed, colour, startVelocity * speed, new Vector2(Textures.Bullet.Width / 2, Textures.Bullet.Height / 2), 0, (int)Textures.Bullet.Width, (int)Textures.Bullet.Height)
        {
            Damage = damage;
            Owner = owner;
            Rotation = rotation;
            this.OnBlockHit = OnBlockHit;
        }

        public void Update(Tile[,] Tiles)
        {
            Collided = false;
            CheckCollisions(Tiles);
            UpdateVisibility(Tiles);

            if (Collided)
            {
                Position += Velocity;
            }

            IsOutOfBounds = Position.X > 50 + (Tiles.GetLength(0) * 10)
            || Position.X < -50
            || Position.Y > 50 + (Tiles.GetLength(1) * 10)
            || Position.Y < -50;
        }

        protected override void Intelligence() { }

        protected override void OnCollideWith(Entity entity)
        {
            entity.ApplyDamage(Damage);
            ApplyDamage(entity.Resistance);
            entity.HitVelocity += Velocity / entity.Weight;
            entity.HitCooldown = 30;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Bullet, Position - StateManager.Offset, null, getLightColour(), Rotation, Origin * Vector2.UnitX, 1, SpriteEffects.None, UI.PROJECTILE_DEPTH_DEFAULT);
        }

        public override void CollideHorizontal(Collision col)
        {
            OnBlockHit(this, col);
            Collided = true;
        }
        public override void CollideVertical(Collision col)
        {
            OnBlockHit(this, col);
            Collided = true;
        }

        public override void CollideDiagonal(Collision col)
        {
            OnBlockHit(this, col);
            Collided = true;
        }
    }
}
