using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using TheOtherDarkWorld.Items;

namespace TheOtherDarkWorld
{
    public static class GameData
    {
        public static Item[] GameItems = new Item[1024];
        public static Block[] GameBlocks = new Block[1024];

        public static void LoadGameData(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode node = doc.FirstChild.NextSibling;

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                switch (node.ChildNodes[i].Name)
                {
                    case "Weapons":
                        LoadWeapons(node.ChildNodes[i]);
                        break;
                    case "Items":
                        LoadItems(node.ChildNodes[i]);
                        break;
                    case "Blocks":
                        LoadBlocks(node.ChildNodes[i]);
                        break;
                }
            }

        }


        private static void LoadWeapons(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int Type = -1;
                string Name = "";
                int UseCooldown = -1;
                bool IsConsumable = false;
                MeleeType AttackTypeM = MeleeType.Swing;
                GunType AttackTypeG = GunType.Single;
                int Consumes = -1;
                int MaxAmount = -1;
                int Power = 0;
                int Penetration = -1;
                int Reach = 0;
                Color BulletColour = Color.White;
                float BulletSpeed = -1;
                int ReloadTime = -1;
                bool IsAutomatic = false;
                string Description = "This weapon is a mystery to me...";

                for (int a = 0; a < node.ChildNodes[i].Attributes.Count; a++)
                {
                    XmlAttribute atr = node.ChildNodes[i].Attributes[a];
                    switch (atr.Name)
                    {
                        case "Type":
                            Type = int.Parse(atr.Value);
                            break;
                        case "Name":
                            Name = atr.Value;
                            break;
                        case "Cooldown":
                            UseCooldown = int.Parse(atr.Value);
                            break;
                        case "Consumes":
                            Consumes = int.Parse(atr.Value);
                            break;
                        case "IsConsumable":
                            IsConsumable = bool.Parse(atr.Value);
                            break;
                        case "MaxAmount":
                            MaxAmount = int.Parse(atr.Value);
                            break;
                        case "Power":
                            Power = int.Parse(atr.Value);
                            break;
                        case "Penetration":
                            Penetration = int.Parse(atr.Value);
                            break;
                        case "BulletColour":
                            BulletColour = GetColourFromName(atr.Value);
                            break;
                        case "BulletSpeed":
                            BulletSpeed = float.Parse(atr.Value);
                            break;
                        case "ReloadTime":
                            ReloadTime = int.Parse(atr.Value);
                            break;
                        case "IsAutomatic":
                            IsAutomatic = bool.Parse(atr.Value);
                            break;
                        case "Description":
                            Description = atr.Value;
                            break;
                        case "Reach":
                            Reach = int.Parse(atr.Value);
                            break;
                        case "AttackType":
                            AttackTypeM = getAttackTypeFromNameM(atr.Value);
                            AttackTypeG = getAttackTypeFromNameG(atr.Value);
                            break;
                    }
                }

                if (node.ChildNodes[i].Name == "Gun")
                {
                    GameItems[Type] = new Gun(Type, IsConsumable, Consumes, MaxAmount, Name, UseCooldown, Power, Penetration, BulletColour, BulletSpeed, ReloadTime, AttackTypeG, IsAutomatic, Description);
                }
                else if (node.ChildNodes[i].Name == "Melee")
                {
                    GameItems[Type] = new Melee(Type, IsConsumable, Consumes, MaxAmount, Name, UseCooldown, Power, Reach, AttackTypeM, IsAutomatic, Description);
                }
            }
        }

        private static void LoadItems(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int Type = -1;
                string Name = "";
                int UseCooldown = -1;
                bool IsConsumable = false;
                int Consumes = -1;
                int MaxAmount = -1;
                int Power = 0;
                bool IsAutomatic = false;
                string Description = "This item is a mystery to me...";

                for (int a = 0; a < node.ChildNodes[i].Attributes.Count; a++)
                {
                    XmlAttribute atr = node.ChildNodes[i].Attributes[a];
                    switch (atr.Name)
                    {
                        case "Type":
                            Type = int.Parse(atr.Value);
                            break;
                        case "Name":
                            Name = atr.Value;
                            break;
                        case "Cooldown":
                            UseCooldown = int.Parse(atr.Value);
                            break;
                        case "IsConsumable":
                            IsConsumable = bool.Parse(atr.Value);
                            break;
                        case "Consumes":
                            Consumes = int.Parse(atr.Value);
                            break;
                        case "MaxAmount":
                            MaxAmount = int.Parse(atr.Value);
                            break;
                        case "Power":
                            Power = int.Parse(atr.Value);
                            break;
                        case "IsAutomatic":
                            IsAutomatic = bool.Parse(atr.Value);
                            break;
                        case "Description":
                            Description = atr.Value;
                            break;

                    }
                }
                GameItems[Type] = new Item(Type, IsConsumable, Consumes, MaxAmount, Name, UseCooldown, Power, IsAutomatic, Description);
            }
        }

        private static void LoadBlocks(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int Type = -1;
                string Name = "";
                Color Colour = Color.White;
                int Health = -1;
                List<Item> Items = new List<Item>();
                int Resistance = -1;
                List<Trigger> Triggers = new List<Trigger>();

                if (node.ChildNodes[i].InnerText != "")
                {
                    //TODO: Add some extra parameters here; Such as triggers and events 
                }


                for (int a = 0; a < node.ChildNodes[i].Attributes.Count; a++)
                {
                    XmlAttribute atr = node.ChildNodes[i].Attributes[a];
                    switch (atr.Name)
                    {
                        case "Type":
                            Type = int.Parse(atr.Value);
                            break;
                        case "Name":
                            Name = atr.Value;
                            break;
                        case "Colour":
                            Colour = GetColourFromName(atr.Value);
                            break;
                        case "Health":
                            Health = int.Parse(atr.Value);
                            break;
                        case "Resistance":
                            Resistance = int.Parse(atr.Value);
                            break;

                    }
                }
                GameBlocks[Type] = new Block(Type, Colour, Health, Items, Resistance, Triggers);
            }
        }

        private static Color GetColourFromName(string Name)
        {
            switch (Name)
            {
                case "White":
                    return Color.White;
                case "Cream":
                    return Color.AntiqueWhite;
                case "Grey":
                case "Gray":
                    return Color.Gray;
                case "Black":
                    return Color.Black;
                case "Red":
                    return Color.Red;
                case "Orange":
                    return Color.Orange;
                default:
                    return Color.White;
            }
        }

        private static MeleeType getAttackTypeFromNameM(string name)
        {
            switch (name.ToLower())
            {
                case "swing":
                    return MeleeType.Swing;
                default:
                    return MeleeType.Swing;
            }
        }
        private static GunType getAttackTypeFromNameG(string name)
        {
            switch (name.ToLower())
            {
                case "single":
                    return GunType.Single;
                case "shotgun":
                    return GunType.Shotgun;
                default:
                    return GunType.Single;
            }
        }
    }

}
