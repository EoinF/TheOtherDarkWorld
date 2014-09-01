using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
using TheOtherDarkWorld.GameObjects;
using System.IO;

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

        /// <summary>
        /// Converts an xml node to an instance of a class of type T
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="node"></param>
        /// <returns>An instance of class T</returns>
        private static T ConvertNode<T>(XmlNode node) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            T result = (ser.Deserialize(stm) as T);

            return result;
        }

        private static void LoadWeapons(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int type = int.Parse(node.ChildNodes[i].SelectSingleNode("Type").InnerText);

                //
                //Check if it's a gun or melee type first
                //
                if (node.ChildNodes[i].Name == "Gun")
                {
                    GameItems[type] = ConvertNode<Gun>(node.ChildNodes[i]);
                }
                else if (node.ChildNodes[i].Name == "Melee")
                {
                    GameItems[type] = ConvertNode<Melee>(node.ChildNodes[i]);
                }
            }
        }

        private static void LoadItems(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int type = int.Parse(node.ChildNodes[i].SelectSingleNode("Type").InnerText);

                //
                //Go through all the sub classes of the Item type (except for weapons)
                //
                switch (node.ChildNodes[i].Name)
                {
                    case "Torch":
                        GameItems[type] = ConvertNode<Torch>(node.ChildNodes[i]);
                        break;
                    case "SmartPhone":
                        GameItems[type] = ConvertNode<SmartPhone>(node.ChildNodes[i]);
                        break;
                    case "Goggles":
                        GameItems[type] = ConvertNode<Goggles>(node.ChildNodes[i]);
                        break;
                    default:
                        GameItems[type] = ConvertNode<Item>(node.ChildNodes[i]);
                        break;
                }
            }
        }

        private static void LoadBlocks(XmlNode node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                int type = int.Parse(node.ChildNodes[i].SelectSingleNode("Type").InnerText);
                GameBlocks[type] = ConvertNode<Block>(node.ChildNodes[i]);
            }
        }
    }
}
