using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class PhoneApp : Item
    {
        public const int APP_FLASHLIGHT = 110;

        public PhoneApp(int type, Entity owner = null)
            : base(type, owner: owner)
        {
            PhoneApp characteristics = GameData.GameItems[type] as PhoneApp;
            //this.x = characteristics.x;
        }

        /// <summary>
        /// Parameterless constructor for Xml deserialization
        /// </summary>
        public PhoneApp()
        {

        }

        public override void Update()
        {
            switch (Type)
            {
                case APP_FLASHLIGHT:
                    break;
            }
        }
    }
}
