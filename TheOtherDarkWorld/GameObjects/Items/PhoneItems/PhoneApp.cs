using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class PhoneApp : Item
    {
        public const int APP_MAINMENU = 119; //No item contains the main menu app. This exists on the phone by default
        public const int APP_TYPE_START = 120; //The lowest item type that contains an app
        public const int APP_FLASHLIGHT = 120;

        public const int APP_TYPE_END = 120; //The highest item type that contains an app

        public UIContainer HUD;
        protected UIContainer PageContent;

        public SmartPhone phone;

        public PhoneApp(int type, SmartPhone phone, int consumeRate, Entity owner = null, string Title = "")
            : base(type, -1, owner)
        {
            this.ConsumeRate = consumeRate;
            this.phone = phone;
            this.ActiveEffects = new ItemEffect[0];
            this.PassiveEffects = new ItemEffect[0];


            //This container allows the TextSprite to be centred
            UIContainer PageTitleContainer = new UIContainer(Color.White, Color.White, Width: SmartPhone.SCREEN_RECT.Width, CursorType: CursorType.Cursor, MarginTop: 10, MarginBottom: 10, CentreHorizontal: true);

            TextSprite PageTitle = new TextSprite(Title, 2, Color.White, SmartPhone.SCREEN_RECT.Width, CursorType: CursorType.Cursor);

            PageTitleContainer.AddElement(PageTitle);
            HUD = new UIGrid(Width: SmartPhone.SCREEN_RECT.Width, Height: SmartPhone.SCREEN_RECT.Height, CursorType: CursorType.Cursor, GridColumns: 1, ColWidth: SmartPhone.SCREEN_RECT.Width);
            HUD.AddElement(PageTitleContainer);

            PageContent = new UIGrid(Width: SmartPhone.SCREEN_RECT.Width, Height: SmartPhone.SCREEN_RECT.Height - PageTitleContainer.Height, CursorType: CursorType.Cursor, GridColumns: 1, ColWidth: SmartPhone.SCREEN_RECT.Width);
            HUD.AddElement(PageContent);

        }

        protected UIContainer CreateSwitchAppButton(string appname, int apptype)
        {
            UIContainer btncontainer = new UIContainer(CursorType: CursorType.Cursor, Width: SmartPhone.SCREEN_RECT.Width, CentreHorizontal: true);
            Button button = new Button(Color.LightBlue, Color.LightCyan, Color.Turquoise, Textures.SmartPhoneButton, CursorType: CursorType.Cursor, Text: appname);
            btncontainer.AddElement(button);

            button.OnPressed += obj =>
            {
                phone.SwitchTo(apptype);
            };
            return btncontainer;
        }

        public virtual void Deactivate()
        {
        }
    }
}
