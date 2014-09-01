using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class MedicApp : PhoneApp
    {
        public MedicApp(int type, SmartPhone phone, int consumeRate, Entity owner = null, string Title = "Medic App v1.0")
            : base(type, phone, consumeRate, owner, Title)
        {
            UIGrid statusEffects = new UIGrid(Width: SmartPhone.SCREEN_RECT.Width, GridColumns: 1, CursorType: CursorType.Cursor, MarginBottom: 20);

            PageContent.AddElement(statusEffects);

            PageContent.AddElement(CreateSwitchAppButton("Main Menu", phone.GetMainMenu()));
        }

        ~MedicApp()
        {
        }

        public override void Update()
        {
        
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }
    }
}
