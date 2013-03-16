using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Projectile : GameObject
    {
        public static List<Projectile> ProjectileList;

        public int Health { get; set; }
        private int Owner { get; set; }
        public int Damage { get; private set; }
        private float Rotation { get; set; }
        private bool Collided;

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

        public Projectile(int damage, int penetration, float speed, Color colour, int owner, Vector2 startVelocity, Vector2 startPosition, float rotation)
            : base(startPosition, speed, colour, startVelocity * speed, new Vector2(Textures.Bullet.Width / 2, Textures.Bullet.Height / 2), 0)
        {
            Damage = damage;
            Health = penetration;
            Owner = owner;
            Rotation = rotation;
        }

        public bool Update()
        {
            Collided = false;
            CheckCollisions(Level.CurrentLevel.Tiles);

            if (Collided)
            {
                Position += Velocity;
            }

            return (Health <= 0
                || Position.X > 50 + (Level.CurrentLevel.Tiles.GetLength(0) * 10)
                || Position.X < -50
                || Position.Y > 50 + (Level.CurrentLevel.Tiles.GetLength(1) * 10)
                || Position.Y < -50
                );
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
            Block BlkHit = Level.CurrentLevel.Tiles[col.block.X, col.block.Y].Block;
            //Position += Velocity / 2; //There are two collisions per frame, so it adds half the velocity twice.
            BlkHit.Health -= this.Damage;
            this.Health -= BlkHit.Resistance;
            Collided = true;
        }
        public override void CollideVertical(Collision col)
        {
            CollideHorizontal(col); //Doesn't matter which side of the block it collides with
        }

        public override void CollideDiagonal(Collision col)
        {
            CollideHorizontal(col); //Doesn't matter which side of the block it collides with
        }
    }
}
