using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

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
                int UseRate = -1;
                int Consumes = -1;
                int MaxAmount = -1;
                int Power = 0;
                int Penetration = -1;
                Color BulletColour = Color.White;
                float BulletSpeed = -1;
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
                        case "UseRate":
                            UseRate = int.Parse(atr.Value);
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
                        case "Penetration":
                            Penetration = int.Parse(atr.Value);
                            break;
                        case "BulletColour":
                            BulletColour = GetColourFromName(atr.Value);
                            break;
                        case "BulletSpeed":
                            BulletSpeed = float.Parse(atr.Value);
                            break;
                        case "IsAutomatic":
                            IsAutomatic = bool.Parse(atr.Value);
                            break;
                        case "Description":
                            Description = atr.Value;
                            break;

                    }
                }
                GameItems[Type] = new Weapon(Type, false, Consumes, MaxAmount, Name, UseRate, Power, Penetration, BulletColour, BulletSpeed, IsAutomatic, Description);
               
            }
        }

        private static void LoadItems(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int Type = -1;
                string Name = "";
                int UseRate = -1;
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
                        case "UseRate":
                            UseRate = int.Parse(atr.Value);
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
                GameItems[Type] = new Item(Type, false, Consumes, MaxAmount, Name, UseRate, Power, IsAutomatic, Description);
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
                case "Black":
                    return Color.Black;
                case "Orange":
                    return Color.Orange;
                default:
                    return Color.White;
            }
        }
    }

}
