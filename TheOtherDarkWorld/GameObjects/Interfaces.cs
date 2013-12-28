using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public interface IItemHolder
    {
        Item[] Inventory { get; set; }
        bool DropItem(Item item);
        bool PickUpItem(Item item);
        bool TrashItem(Item item);
        bool TrashItem(int index);
        void UpdateInventory();
        Item GetItem(int type);
        int GetItemIndex(int type);
    }

    public interface IEnergyBased
    {
        float Energy { get; set; }
        float MaxEnergy { get; set; }
        void UpdateEnergy();
        StatusEffect exhaustionExhaust { get; set; }
        StatusEffect exhaustionSlow { get; set; }
        bool IsExhaustedCompletely { get; set; }
        float ExhaustPercent { get; set; }
    }

    public interface IMelee
    {
        Swing Swing { get; set; }
    }
}
