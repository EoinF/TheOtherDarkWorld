﻿using System;
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


        public Gun(int Type, int StartingAmmo = -1, Entity Owner = null)
            : base(Type, StartingAmmo, Owner)
        {
            Gun characteristics = (Gun)GameData.GameItems[Type];
            this.Penetration = characteristics.Penetration;
            this.BulletColour = characteristics.BulletColour;
            this.BulletSpeed = characteristics.BulletSpeed;
            this.ReloadTime = characteristics.ReloadTime;
            this.AttackType = characteristics.AttackType;
        }

        /// <summary>
        /// Parameterless Constructor for XML deserialization
        /// </summary>
        public Gun()
        {
        }

        public override void Activate()
        {
            if (Amount <= 0)
                Reload();
            else
                base.Activate();
        }

        protected override void ApplyActive()
        {
            Vector2 direction = new Vector2((float)Math.Cos(Owner.Rotation - MathHelper.PiOver2), (float)Math.Sin(Owner.Rotation - MathHelper.PiOver2));
            direction.Normalize();
            Vector2 startPosition = new Vector2(Owner.Rect.Center.X - 5, Owner.Rect.Center.Y - 5) + (direction * 3);

            Random rand = new Random();
            switch (AttackType)
            {
                case GunType.Single:
                    if (ReloadCooldown <= 0)
                    {
                        Projectile.ProjectileList.Add(new Projectile(Power, Penetration, BulletSpeed, BulletColour, Owner, direction, startPosition, Owner.Rotation));
                    }
                    break;
                case GunType.Shotgun:
                    if (ReloadCooldown <= 0)
                    {
                        for (int numShots = 0; numShots < 20; numShots++)
                        {
                            //Creates a new rotation that is close to the original rotation
                            float newRotation = Owner.Rotation + (float)(rand.NextDouble() / 4) - 0.125f;
                            direction = new Vector2((float)Math.Cos(newRotation - MathHelper.PiOver2), (float)Math.Sin(newRotation - MathHelper.PiOver2));
                            direction.Normalize();
                            Projectile.ProjectileList.Add(new Projectile(Power, Penetration, BulletSpeed, BulletColour, Owner, direction, startPosition, newRotation));
                        }
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (ReloadCooldown >= 0)
                ReloadCooldown--;
            else if (UseCooldown > 0) //Only reduce the cooldown while it's not reloading
                UseCooldown--;

            ApplyPassive();
        }
        public void Reload()
        {
            //If the gun doesn't need to be reloaded
            if (Amount == MaxAmount)
                return;

            Item ammo = Owner.GetItem(Consumes);
            if (ammo != null)
            {
                //If there's not enough ammo to fill it completely, then just add as much as possible
                if (ammo.Amount < MaxAmount - Amount)
                {
                    Amount += ammo.Amount;
                    ammo.Amount = 0;
                }
                else //Otherwise fill it to the max
                {
                    ammo.Amount -= (MaxAmount - Amount);
                    Amount = MaxAmount;
                }
                //NOTE: This temporary reference will just point to null. But the ammo will still exist in the inventory of the owner
                //Check if the ammo has been emptied since reloading. [You can't hold 0 of an item!]
                //if (ammo.Amount == 0)
                //    ammo = null;

                //The weapon has been reloaded at this stage
                ReloadCooldown = ReloadTime;
                UseCooldown = 1; //Prevent the item from being activated while being reloaded
                return;
            }
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