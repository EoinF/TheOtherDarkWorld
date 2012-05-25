using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace TheOtherDarkWorld
{
	public class Weapon : Item
    {
        #region Properties & Fields

        public int Penetration { get; private set; }

        public int ReloadRate { get; private set; }
        public float BulletSpeed { get; private set; }
        public Color BulletColour { get; private set; }
        public int BulletType { get; private set; }

        #endregion


        public Weapon(int Type, int StartingAmmo = -1)
            :base (Type, StartingAmmo)
        {
            Weapon characteristics = (Weapon)GameData.GameItems[Type];
            this.Penetration = characteristics.Penetration;
            this.BulletColour = characteristics.BulletColour;
            this.BulletSpeed = characteristics.BulletSpeed;
        }

        public Weapon(int Type, bool IsConsumable, int Consumes, int MaxAmount, string Name, int UseRate, int Power, int Penetration, Color BulletColour, float BulletSpeed, bool IsAutomatic, string Description)
        :base(Type, IsConsumable, Consumes, MaxAmount, Name, UseRate, Power, IsAutomatic, Description)
        {
            this.Penetration = Penetration;
            this.BulletColour = BulletColour;
            this.BulletSpeed = BulletSpeed;
        }



        

        public override bool Activate(float rotation, Vector2 Direction, Vector2 startPosition)
        {
            if ((!IsAutomatic && InputManager.MousePreviouslyReleased) || IsAutomatic)
            {
                if (Cooldown < 0)
                {
                    if (Amount > 0)
                    {
                        Projectile.ProjectileList.Add(new Projectile(Power, Penetration, BulletSpeed, BulletColour, Owner, Direction, startPosition, rotation));
                        Cooldown += 100;
                        Amount--;
                    }
                }
            }
                return false;
        }

	}
}
