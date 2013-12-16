using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{

    public class MainMenuApp : PhoneApp
    {
        private List<PhoneApp> InstalledApps;
        private List<int> HeldApps; //The apps in the owner's inventory

        public MainMenuApp(int type, SmartPhone phone, int consumeRate, List<PhoneApp> InstalledApps, Entity owner, string Title = "Main Menu")
            : base(type, phone, consumeRate, owner, Title)
        {
            this.InstalledApps = InstalledApps;
            this.HeldApps = new List<int>();
            for (int i = 0; i < Owner.Inventory.Length; i++)
            {
                if (Owner.Inventory[i] != null)
                    if (Owner.Inventory[i].Type >= APP_TYPE_START && Owner.Inventory[i].Type <= APP_TYPE_END)
                    {
                        HeldApps.Add(Owner.Inventory[i].Type);
                    }
            }
            UpdateUI();
        }

        //NOTE: Could change this to the Activate method to improve performance
        public override void Update()
        {
            //
            //First, check if the inventory of the player has changed
            //
            bool isChanged = false;

            int a = 0; //The first item in the heldapps list
            for (int i = 0; i < Owner.Inventory.Length; i++)
            {
                if (Owner.Inventory[i] != null)
                    if (Owner.Inventory[i].Type >= APP_TYPE_START && Owner.Inventory[i].Type <= APP_TYPE_END)
                    {
                        if (a < HeldApps.Count)
                        {
                            if (Owner.Inventory[i].Type == HeldApps[a])
                                a++;
                            else //The order has changed, or a new app is in the inventory
                            {
                                isChanged = true;
                                HeldApps[a] = Owner.Inventory[i].Type;
                            }
                        }
                        else //More apps than there were in the last frame
                        {
                            isChanged = true;
                            HeldApps.Add(Owner.Inventory[i].Type);
                        }
                    }
            }
            while (a < HeldApps.Count - 1) //Fewer apps than there were in the last frame
            {
                isChanged = true;
                HeldApps.RemoveAt(HeldApps.Count - 1);
            }

            if (isChanged)
            {
                PageContent.ClearElements();
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            //Start with 1, to skip over the main menu app
            for (int i = 1; i < InstalledApps.Count; i++)
            {
                PageContent.AddElement(CreateSwitchAppButton(InstalledApps[i].Name, InstalledApps[i].Type));
            }

            for (int i = 0; i < Owner.Inventory.Length; i++)
            {
                //Check if this is actually an app item
                if (Owner.Inventory[i] != null
                    && Owner.Inventory[i].Type >= APP_TYPE_START && Owner.Inventory[i].Type <= APP_TYPE_END)
                {
                    //
                    //First, make sure this app is not already installed
                    //
                    if (!phone.IsInstalled(Owner.Inventory[i].Type))
                    {
                        string appname = Owner.Inventory[i].Name.Split('(')[1].Split(')')[0]; //App items come in the form "SD Card(app_name)"

                        int apptype = Owner.Inventory[i].Type;
                        CreateInstallAppButton(appname, apptype, phone, Owner, InstalledApps);
                    }
                }
            }
            if (InstalledApps.Count <= 1 && HeldApps.Count == 0)
            {
                PageContent.AddElement(new TextSprite("There are no Apps installed on this phone", 1, Color.White, PageContent.Width, CursorType: CursorType.Cursor));
            }
        }

        protected void CreateInstallAppButton(string appname, int apptype, SmartPhone phone, Entity owner, List<PhoneApp> InstalledApps)
        {
            UIContainer btncontainer = new UIContainer(CursorType: CursorType.Cursor, Width: SmartPhone.SCREEN_RECT.Width, CentreHorizontal: true);

            Button button = new Button(Color.LightBlue, Color.LightCyan, Color.Turquoise, Textures.SmartPhoneButton, CursorType: CursorType.Cursor, Text: "Install " + appname);

            button.OnPressed += (obj) =>
            {
                int itemIndex = owner.GetItemIndex(apptype);
                if (itemIndex != -1)
                {
                    int consumeRate = owner.GetItem(apptype).ConsumeRate;
                    owner.TrashItem(itemIndex);
                    //Remove the item from the HUD as well
                    ((UI.Inventory_UI[itemIndex] as UIContainer)[0] as InventoryElement).Item = null;

                    //Remove this button, since it's no longer needed
                    (button.Parent as UIContainer).RemoveElement(button);

                    //Also create the button for launching the new app
                    phone.InstallApp(apptype, consumeRate);
                    PageContent.AddElement(CreateSwitchAppButton(appname, apptype));
                }
            };
            btncontainer.AddElement(button);
            PageContent.AddElement(btncontainer);
        }

    }
}