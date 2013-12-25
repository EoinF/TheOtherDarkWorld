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
        //
        //TODO: Fix bug where muzzle flash flickers twice instead of just once
        //      when trying to fire a non-automatic gun rapidly
        //
        private const float bright_begin = 0.2f;
        private const float bright_rate = 0.145f;
        private const GunType DEFAULT_ATTACKTYPE = GunType.Single;
        private const float DEFAULT_MUZZLE_FLASH = 0.3f;
        private static Color DEFAULT_BULLET_COLOUR = Color.White;
        private const int DEFAULT_PENETRATION = 10;
        private const int DEFAULT_BULLET_SPEED = 10;
        private const int DEFAULT_RELOAD_TIME = 200;

        #region Properties & Fields

        //State variables
        public int ReloadCooldown { get; set; }
        private Light flash { get; set; }
        private float currentLight { get; set; }

        //Characteristics
        public int Penetration { get; set; }
        public int ReloadTime { get; set; }
        public float BulletSpeed { get; set; }
        public Color BulletColour { get; set; }
        public int BulletType { get; set; }
        public GunType AttackType { get; set; }
        public float MuzzleFlashMagnitude { get; set; }

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
            this.MuzzleFlashMagnitude = characteristics.MuzzleFlashMagnitude;

            flash = StateManager.CreateLight(bright_begin, 1000 * MuzzleFlashMagnitude, Owner.Position, Vector2.Zero, MathHelper.Pi, Color.Yellow, false);
        }

        /// <summary>
        /// Parameterless Constructor for XML deserialization
        /// </summary>
        public Gun()
        {
            this.AttackType = DEFAULT_ATTACKTYPE;
            this.BulletColour = DEFAULT_BULLET_COLOUR;
            this.MuzzleFlashMagnitude = DEFAULT_MUZZLE_FLASH;
            this.Penetration = DEFAULT_PENETRATION;
            this.BulletSpeed = DEFAULT_BULLET_SPEED;
            this.ReloadTime = DEFAULT_RELOAD_TIME;
        }

        public override void Activate()
        {
            if (Amount <= 0)
                Reload();
            else if (ReloadCooldown <= 0)
                base.Activate();
        }

        protected override void ApplyActive()
        {
            Vector2 direction = new Vector2((float)Math.Cos(Owner.Rotation - MathHelper.PiOver2), (float)Math.Sin(Owner.Rotation - MathHelper.PiOver2));
            direction.Normalize();
            Vector2 startPosition = (Owner.Rect.Center + (Owner.Origin / 2));

            Random rand = new Random();
            switch (AttackType)
            {
                case GunType.Single:
                    StateManager.CreateProjectile(Power, Penetration, BulletSpeed, BulletColour, Owner, direction, startPosition, Owner.Rotation);
                    break;
                case GunType.Shotgun:
                    for (int numShots = 0; numShots < 20; numShots++)
                    {
                        //Creates a new rotation that is close to the original rotation
                        float newRotation = Owner.Rotation + (float)(rand.NextDouble() / 4) - 0.125f;
                        direction = new Vector2((float)Math.Cos(newRotation - MathHelper.PiOver2), (float)Math.Sin(newRotation - MathHelper.PiOver2));
                        direction.Normalize();
                        StateManager.CreateProjectile(Power, Penetration, BulletSpeed, BulletColour, Owner, direction, startPosition, newRotation);
                    }
                    break;
            }
            flash.IsActive = true;
            currentLight = bright_begin;
        }

        public override void Update()
        {
            if (ReloadCooldown >= 0)
                ReloadCooldown--;
            else if (UseCooldown > 0) //Only reduce the cooldown while it's not reloading
                UseCooldown--;

            ApplyPassive();
        }

        protected override void ApplyPassive()
        {
            base.ApplyPassive();
            if (currentLight < MuzzleFlashMagnitude)
            {
                flash.Brightness = currentLight;
                currentLight += bright_rate;
                flash.Update(Owner.Position, Owner.Direction);
            }
            else
            {
                flash.IsActive = false;
            }

        }

        public void Reload()
        {
            //If the gun doesn't need to be reloaded
            if (Amount == MaxAmount)
                return;

            Item ammo = (Owner as IItemHolder).GetItem(Consumes);
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