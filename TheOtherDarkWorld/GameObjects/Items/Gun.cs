using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;


namespace TheOtherDarkWorld.GameObjects
{
    public class Gun : Item
    {
        #region Properties & Fields

        //State variables
        public int ReloadCooldown { get; set; }

        //Characteristics
        public int Penetration { get; set; }
        public int ReloadTime { get; set; }
        public float BulletSpeed { get; set; }
        public Color BulletColour { get; set; }
        public int BulletType { get; set; }
        public GunType AttackType { get; set; }

        #endregion


        public Gun(int Type, int StartingAmmo = -1)
            : base(Type, StartingAmmo)
        {
            Gun characteristics = (Gun)GameData.GameItems[Type];
            this.Penetration = characteristics.Penetration;
            this.BulletColour = characteristics.BulletColour;
            this.BulletSpeed = characteristics.BulletSpeed;
            this.ReloadTime = characteristics.ReloadTime;
            this.AttackType = characteristics.AttackType;
        }

        public Gun()
        {
        }


        public override bool Activate(float rotation, Vector2 Direction, Vector2 startPosition)
        {
            Random rand = new Random();
            switch (AttackType)
            {
                case GunType.Single:
                    if (ReloadCooldown <= 0)
                    {
                        Projectile.ProjectileList.Add(new Projectile(Power, Penetration, BulletSpeed, BulletColour, Owner, Direction, startPosition, rotation));
                        UseCooldown += Cooldown;
                        Amount--;
                    }
                    break;
                case GunType.Shotgun:
                    if (ReloadCooldown <= 0)
                    {
                        for (int numShots = 0; numShots < 20; numShots++)
                        {
                            //Creates a new rotation that is close to the old rotation
                            float newRotation = rotation + (float)(rand.NextDouble() / 4) - 0.125f;
                            Direction = new Vector2((float)Math.Cos(newRotation - MathHelper.PiOver2), (float)Math.Sin(newRotation - MathHelper.PiOver2));
                            Direction.Normalize();
                            Projectile.ProjectileList.Add(new Projectile(Power, Penetration, BulletSpeed, BulletColour, Owner, Direction, startPosition, newRotation));
                        }
                        UseCooldown += Cooldown;
                        Amount--;
                    }
                    break;
            }
            return false;
        }
    }

    public enum GunType
    {
        Single,
        Shotgun,
        SemiAuto,
        Rocket,
        GrenadeLauncher,
        Grenade
    }
}