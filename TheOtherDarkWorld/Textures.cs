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
        public static Texture2D Swipe { get; private set; }

        public static List<Texture2D> UITextures { get; private set; }

        public static Texture2D Foreground { get; private set; }


        public static Texture2D Items { get; private set; }
        public static Texture2D Blocks { get; private set; }

        public static Texture2D[] Enemies { get; private set; }
        public static Texture2D[] MenuTextures { get; private set; }
        public static SpriteFont[] Fonts { get; private set; }

        public static void LoadTextures(ContentManager Content, GraphicsDevice device)
        {
            MenuTextures = new Texture2D[10];
            Crosshair = Content.Load<Texture2D>("Crosshair");
            SidePanel = Content.Load<Texture2D>("SidePanel");
            HealthPoint = Content.Load<Texture2D>("HealthPoint");
            Cursor = Content.Load<Texture2D>("Cursor");
            Swipe = Content.Load<Texture2D>("swipe");

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
            Enemies = new Texture2D[1];
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = Content.Load<Texture2D>("Enemies/" + i);
            }
        }

        public static Rectangle GetItemRectangle(int Type)
        {
             return new Rectangle((Type * 16) % Items.Width, ((Type * 16) / Items.Width) * 16, 16, 16);
        }
    }

}
