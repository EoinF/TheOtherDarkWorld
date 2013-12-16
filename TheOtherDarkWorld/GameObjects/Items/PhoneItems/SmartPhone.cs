using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class SmartPhone : Togglable
    {
        public const int MAINMENU_CONSUMERATE_DEFAULT = 1;
        public static int SMARTPHONE_EDGE_WIDTH = 35;
        public static int SMARTPHONE_EDGE_HEIGHT = 26;
        public static Rectangle SCREEN_RECT =
            new Rectangle(SMARTPHONE_EDGE_WIDTH, SMARTPHONE_EDGE_HEIGHT, Textures.SmartPhoneExterior.Width - (SMARTPHONE_EDGE_WIDTH * 2), Textures.SmartPhoneExterior.Height - (SMARTPHONE_EDGE_HEIGHT * 2));
            
        public Color Colour { get; set; }
        public static Color DEFAULT_COLOUR = Color.MidnightBlue;

        private UIContainer HUD;
        private UIContainer Phone;

        public override int ConsumeRate
        {
            get 
            {
                if (SelectedApp != null)
                {
                    return SelectedApp.ConsumeRate + base.ConsumeRate;
                }
                else 
                    return base.ConsumeRate;
            }
        }

        /// <summary>
        /// USB sticks contain apps. When these are installed onto the phone
        /// the id of the app is stored here
        /// </summary>
        private List<PhoneApp> InstalledApps { get; set; }

        protected PhoneApp SelectedApp { get; set; }

        public PhoneApp GetApp(int type)
        {
            for (int i = 0; i < InstalledApps.Count; i++)
            {
                if (InstalledApps[i].Type == type)
                    return InstalledApps[i];
            }
            return null;
        }

        public SmartPhone(int type, Entity owner = null)
            : base(type, owner: owner)
        {
            SmartPhone characteristics = GameData.GameItems[type] as SmartPhone;
            this.InstalledApps = new List<PhoneApp>();
            this.Colour = characteristics.Colour;

            Phone = new UIContainer(Colour, Colour, null, new Vector2((UI.ScreenX - SCREEN_RECT.Width) / 2, (UI.ScreenY - SCREEN_RECT.Height) / 2), Width: Textures.SmartPhoneExterior.Width, Height: SMARTPHONE_EDGE_HEIGHT, IsActive: false, IsDraggable: true, CursorType: CursorType.Cursor, opacity: 0.5f);
            UIContainer Exterior = new UIContainer(Colour, Colour, Textures.SmartPhoneExterior, CursorType: CursorType.Cursor);
            Exterior.OnMouseEnter += (x) => { Phone.Opacity = 1; };
            Exterior.OnMouseLeave += (x) => { Phone.Opacity = 0.5f; };

            HUD = new UIContainer(null, new Vector2(SCREEN_RECT.Left, SCREEN_RECT.Top), null, SCREEN_RECT.Width, SCREEN_RECT.Height, CursorType: CursorType.Cursor, MarginLeft: SMARTPHONE_EDGE_WIDTH);

            Phone.AddElement(Exterior);
            Phone.AddElement(HUD);
            UI.UIControls.AddElement(Phone);

            InstallApp(PhoneApp.APP_MAINMENU, MAINMENU_CONSUMERATE_DEFAULT);
            SelectedApp = GetApp(PhoneApp.APP_MAINMENU);
        }

        public SmartPhone() //Parameterless constructor for xml deserialization
        {
            InstalledApps = new List<PhoneApp>();
            Colour = DEFAULT_COLOUR;
        }

        ~SmartPhone()
        {
            if (Phone != null)
                UI.UIControls.RemoveElement(Phone);
        }

        public override void Deactivate()
        {
            IsActive = false;
            Phone.IsActive = false;
            SelectedApp.Deactivate();
            HUD.ClearElements();
        }

        protected override void Toggle()
        {
            IsActive = !IsActive;
            Phone.IsActive = IsActive;

            if (IsActive)
            {
                SelectedApp.Activate();
                this.HUD.AddElement(SelectedApp.HUD);
            }
            else
            {
                SelectedApp.Deactivate();
                this.HUD.ClearElements();
            }
        }

        public void SwitchTo(int apptype)
        {
            SelectedApp.Deactivate();
            SelectedApp = GetApp(apptype);
            SelectedApp.Activate();
            this.HUD.ClearElements();
            this.HUD.AddElement(SelectedApp.HUD);
        }

        protected override void ApplyToggledPassive()
        {
            SelectedApp.Update();
        }

        public bool InstallApp(int type, int ConsumeRate)
        {
            switch (type)
            {
                case PhoneApp.APP_MAINMENU:
                    InstalledApps.Add(new MainMenuApp(type, this, ConsumeRate, InstalledApps, Owner));
                    break;
                case PhoneApp.APP_FLASHLIGHT:
                    InstalledApps.Add(new FlashlightApp(type, this, ConsumeRate, Owner));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public bool IsInstalled(int type)
        {
            for (int i = 0; i < InstalledApps.Count; i++)
            {
                if (InstalledApps[i].Type == type)
                    return true;
            }
            return false;
        }
    }
}
