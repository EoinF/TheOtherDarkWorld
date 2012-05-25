using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace TheOtherDarkWorld
{
    public class Textures
    {
        public static Texture2D Block { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Crosshair { get; private set; }
        public static Texture2D SidePanel { get; private set; }
        public static Texture2D ItemSlot { get; private set; }
        public static Texture2D HealthPoint { get; private set; }
        public static Texture2D Cursor { get; private set; }

        public static List<Texture2D> UITextures { get; private set; }


        private static ContentManager Content;

        public Textures(ContentManager content)
        {
            Content = content;
        }

        public static Texture2D Items { get; set; }
        public static Texture2D[] Blocks { get; set; }
        public static Texture2D[] MenuTextures { get; set; }
        public static SpriteFont[] Fonts { get; set; }

        public static void LoadTextures()
        {
            MenuTextures = new Texture2D[10];
            Crosshair = Content.Load<Texture2D>("Crosshair");
            SidePanel = Content.Load<Texture2D>("SidePanel");
            HealthPoint = Content.Load<Texture2D>("HealthPoint");
            Cursor = Content.Load<Texture2D>("Cursor");


            Block = Content.Load<Texture2D>("Block");
            Bullet = Content.Load<Texture2D>("Bullet");
            Player = Content.Load<Texture2D>("Characters");

            Fonts = new SpriteFont[4];
            Fonts[0] = Content.Load<SpriteFont>("Small");
            Fonts[1] = Content.Load<SpriteFont>("Medium");
            Fonts[2] = Content.Load<SpriteFont>("Large");
            Fonts[3] = Content.Load<SpriteFont>("Huge");

            UITextures = new List<Texture2D>();
            UITextures.Add(Content.Load<Texture2D>("ItemSlot"));
            UITextures.Add(Content.Load<Texture2D>("Tooltip"));


            Items = Content.Load<Texture2D>("ItemSheet");


            Blocks = new Texture2D[1024];
        }

        public static Rectangle GetItemRectangle(int Type)
        {
             return new Rectangle((Type % (Textures.Items.Width / 16) * 16),(Type / (Textures.Items.Width / 16)) * 16, 16, 16);
        }
    }

}
