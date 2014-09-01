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
            
        public static Color DEFAULT_COLOUR = Color.MidnightBlue;

        public Color Colour { get; set; }
        private UIContainer _hud;
        private UIContainer _phone;

        private MainMenuApp _mainMenu;
        public MainMenuApp GetMainMenu()
        {
            return _mainMenu;
        }

        /// <summary>
        /// USB sticks contain apps. When these are installed onto the phone
        /// the id of the app is stored here
        /// </summary>
        private List<PhoneApp> InstalledApps { get; set; }

        protected PhoneApp SelectedApp { get; set; }

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

            _phone = new UIContainer(Colour, Colour, null, new Vector2((UI.ScreenX - SCREEN_RECT.Width) / 2, (UI.ScreenY - SCREEN_RECT.Height) / 2), Width: Textures.SmartPhoneExterior.Width, Height: SMARTPHONE_EDGE_HEIGHT, IsActive: false, IsDraggable: true, CursorType: CursorType.Cursor, opacity: 0.5f);
            UIContainer Exterior = new UIContainer(Colour, Colour, Textures.SmartPhoneExterior, CursorType: CursorType.Cursor);
            
            //Make the phone transparent when not mousing over it so it doesn't get in the way!
            Exterior.OnMouseEnter += (x) => { _phone.Opacity = 1; };
            Exterior.OnMouseLeave += (x) => { _phone.Opacity = 0.5f; };

            _hud = new UIContainer(null, new Vector2(SCREEN_RECT.Left, SCREEN_RECT.Top), null, SCREEN_RECT.Width, SCREEN_RECT.Height, CursorType: CursorType.Cursor);

            _phone.AddElement(Exterior);
            _phone.AddElement(_hud);
            UI.UIControls.AddElement(_phone);

            InstallApp(PhoneApp.APP_MAINMENU, MAINMENU_CONSUMERATE_DEFAULT);
            _mainMenu = (MainMenuApp)GetApp(PhoneApp.APP_MAINMENU);
            SelectedApp = _mainMenu;
        }

        public SmartPhone() //Parameterless constructor for xml deserialization
        {
            InstalledApps = new List<PhoneApp>();
            Colour = DEFAULT_COLOUR;
        }

        ~SmartPhone()
        {
            if (_phone != null)
                UI.UIControls.RemoveElement(_phone);
        }

        public override void Deactivate()
        {
            IsActive = false;
            _phone.IsActive = false;
            SelectedApp.Deactivate();
            _hud.ClearElements();
        }

        protected override void Toggle()
        {
            IsActive = !IsActive;
            _phone.IsActive = IsActive;

            if (IsActive)
            {
                SelectedApp.Activate();
                this._hud.AddElement(SelectedApp.HUD);
            }
            else
            {
                SelectedApp.Deactivate();
                this._hud.ClearElements();
            }
        }

        public void SwitchTo(PhoneApp app)
        {
            SelectedApp.Deactivate();
            SelectedApp = app;
            SelectedApp.Activate();
            this._hud.ClearElements();
            this._hud.AddElement(SelectedApp.HUD);
        }

        protected override void ApplyToggledPassive()
        {
            SelectedApp.Update();
        }

        /// <summary>
        /// Installs an app to the smart phone based on the given type
        /// </summary>
        /// <param name="type">The type of the class</param>
        /// <param name="ConsumeRate">The rate at which the app consumes fuel</param>
        /// <returns>The installed app</returns>
        public PhoneApp InstallApp(int type, int ConsumeRate)
        {
            switch (type)
            {
                case PhoneApp.APP_MAINMENU:
                    InstalledApps.Add(new MainMenuApp(type, this, ConsumeRate, InstalledApps, Owner));
                    break;
                case PhoneApp.APP_FLASHLIGHT:
                    InstalledApps.Add(new FlashlightApp(type, this, ConsumeRate, Owner));
                    break;
                case PhoneApp.APP_MEDIC:
                    InstalledApps.Add(new MedicApp(type, this, ConsumeRate, Owner));
                    break;
                default:
                    return null;
            }
            return InstalledApps[InstalledApps.Count - 1];
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
