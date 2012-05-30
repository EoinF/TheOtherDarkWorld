using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class Projectile : GameObject
    {
        public static List<Projectile> ProjectileList;

        private int Health { get; set; }
        private int Owner { get; set; }
        public int Damage { get; private set; }
        private float Rotation { get; set; }

        public override Rectangle Rect
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Textures.Bullet.Width, Textures.Bullet.Height); }
        }

        public Rectangle BaseRect
        {
            get { return new Rectangle(0, 0, Textures.Bullet.Width, Textures.Bullet.Height); }
        }

        public Vector2 Origin
        {
            get { return new Vector2(Textures.Bullet.Width / 2, Textures.Bullet.Height / 2); } 
        }

        public Projectile(int damage, int penetration, float speed, Color colour, int owner, Vector2 startVelocity, Vector2 startPosition, float rotation)
            : base(startPosition, speed, colour, startVelocity * speed, new Vector2(Textures.Bullet.Width / 2, Textures.Bullet.Height / 2))
        {

            Damage = damage;
            Health = penetration * 40;
            Owner = owner;
            Rotation = rotation;
        }

        public bool Update()
        {
            CheckCollisions();
            return (Health <= 0);
        }


        public static void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < ProjectileList.Count; i++)
            {
                ProjectileList[i].Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Bullet, Position - Player.PlayerList[0].Offset, null, Colour, Rotation, Origin * Vector2.UnitX, 1, SpriteEffects.None, 0.2f);
        }

        public override void CollideHorizontal(Collision col)
        {
            Block BlkHit = Level.CurrentLevel.BlockList[col.block.X, col.block.Y];
            Position += Velocity / 2; //There are two collisions per frame, so it adds half the velocity twice.
            BlkHit.Health -= (int)(this.Damage * Speed);
            this.Health -= (int)(BlkHit.Resistance * Speed);
        }
        public override void CollideVertical(Collision col)
        {
            CollideHorizontal(col); //Doesn't matter which side of the block it collides with
        }
    }
}
