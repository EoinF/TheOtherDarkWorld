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
        public const int ITEM_SCALE = 16;

        public static Texture2D Block { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Crosshair { get; private set; }
        public static Texture2D HealthBarPiece { get; private set; }
        public static Texture2D Cursor { get; private set; }
        public static Texture2D Swipe { get; private set; }

        //
        //  UI Textures
        //
        public static Texture2D SidePanel { get; private set; }
        public static Texture2D ItemSlot { get; private set; }
        public static Texture2D HealthBar { get; private set; }
        public static Texture2D Tooltip { get; private set; }
        public static Texture2D SmartPhoneExterior { get; private set; }

        public static Texture2D Foreground { get; private set; }


        public static Texture2D Items { get; private set; }
        public static Texture2D Blocks { get; private set; }
        public static Texture2D SmartPhoneButton { get; private set; }
        public static Texture2D SmartPhoneSlider { get; private set; }
        public static Texture2D SmartPhoneSliderPiece { get; private set; }

        public static Texture2D[] Enemies { get; private set; }
        public static SpriteFont[] Fonts { get; private set; }

        public static void LoadTextures(ContentManager Content, GraphicsDevice device)
        {
            Crosshair = Content.Load<Texture2D>("Crosshair");
            HealthBarPiece = Content.Load<Texture2D>("HealthBarPiece");
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

            SidePanel = Content.Load<Texture2D>("SidePanel");
            HealthBar = Content.Load<Texture2D>("HealthBar");
            ItemSlot = Content.Load<Texture2D>("ItemSlot");
            Tooltip = Content.Load<Texture2D>("Tooltip");
            SmartPhoneExterior = Content.Load<Texture2D>("SmartPhoneExterior");
            SmartPhoneButton = Content.Load<Texture2D>("SmartPhoneButton");
            SmartPhoneSlider = Content.Load<Texture2D>("SmartPhoneSlider");
            SmartPhoneSliderPiece = Content.Load<Texture2D>("SmartPhoneSliderPiece");


            Items = Content.Load<Texture2D>("ItemSheet");
            Enemies = new Texture2D[1];
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = Content.Load<Texture2D>("Enemies/" + i);
            }
        }

        public static Rectangle GetItemRectangle(int Type)
        {
             return new Rectangle((Type * ITEM_SCALE) % Items.Width, ((Type * ITEM_SCALE) / Items.Width) * ITEM_SCALE, ITEM_SCALE, ITEM_SCALE);
        }
    }

}
