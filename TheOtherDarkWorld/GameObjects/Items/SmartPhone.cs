using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class SmartPhone : Togglable
    {
        private static Vector2 SMART_PHONE_POSITION = 
            new Vector2((UI.ScreenX - Textures.SmartPhoneExterior.Width) / 2,
                (UI.ScreenY - Textures.SmartPhoneExterior.Height) / 2);

        public Color Colour { get; set; }
        public static Color DEFAULT_COLOUR = Color.MidnightBlue;

        private UIElement Exterior;

        /// <summary>
        /// USB sticks contain apps. When these are installed onto the phone
        /// the usb item is stored here
        /// </summary>
        protected List<PhoneApp> Apps;
        private int SelectedApp;

        public SmartPhone(int type, Entity owner = null)
            : base(type, owner: owner)
        {
            SelectedApp = -1;
            SmartPhone characteristics = GameData.GameItems[type] as SmartPhone;
            this.Apps = characteristics.Apps;
            this.Colour = characteristics.Colour;

            Apps.Add(new PhoneApp(120, owner));
            Exterior = new UIElement(SMART_PHONE_POSITION, Textures.SmartPhoneExterior, Colour, Colour, IsActive: false);
            UI.HUDImage.Add(Exterior);
        }

        public SmartPhone() //Parameterless constructor for xml deserialization
        {
            Apps = new List<PhoneApp>();
            Colour = DEFAULT_COLOUR;
        }


        public override void Deactivate()
        {
            IsActive = false;
            Exterior.IsActive = false;
        }

        protected override void Toggle()
        {
            IsActive = !IsActive;
            Exterior.IsActive = IsActive;
        }

        protected override void ApplyToggledPassive()
        {
            if (SelectedApp != -1) //An app is active
            {
                int AppId = Apps[SelectedApp].Type;

            }
            else //Main Menu is active
            {
                MainMenu();
            }
        }

        private void MainMenu()
        {

        }
    }
}
