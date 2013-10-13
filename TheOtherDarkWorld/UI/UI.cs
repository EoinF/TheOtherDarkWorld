using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public static class UI
    {
        private static Vector2 PANEL_POSITION = new Vector2(800 - Textures.SidePanel.Width, 0);
        private static Vector2 HEALTH_POSITION = PANEL_POSITION + new Vector2(11, 10);
        private static Vector2 HEALTH_BAR_POSITION = PANEL_POSITION + new Vector2(6, 30);
        private static Vector2 STATUS_EFFECTS_POSITION = PANEL_POSITION + new Vector2(12, 50);
        private static Vector2 INVENTORY_POSITION = PANEL_POSITION + new Vector2(14, 120);
        private const int TOOLTIP_LIMIT = 60;


        public static int TooltipCounter { get; private set; } //When this reaches 60(1 second), a tooltip can be created

        public static List<InventoryElement> Inventory_UI;

        public static int Kills;
        public static int HighScore;
        /// <summary>
        /// The list of actions that the player can perform that are nearby
        /// </summary>
        public static List<UIElement> Actions;
        public static List<TextSprite> HUDText;
        public static List<UIElement> HUDImage;

        private static Tooltip Tooltip;
        public static bool CursorMode;

        public static int ScreenX { get; set; }
        public static int ScreenY { get; set; }


        public static void InitializeHUD(int InventorySize)
        {
            HUDImage = new List<UIElement>();
            HUDText = new List<TextSprite>();
            Inventory_UI = new List<InventoryElement>();

            HUDText.Add(new TextSprite("Status", 1, Color.Violet, -1, STATUS_EFFECTS_POSITION));
            HUDText.Add(new TextSprite("Items", 1, Color.Violet, -1, INVENTORY_POSITION));
            HUDText.Add(new TextSprite("Health", 1, Color.Violet, -1, HEALTH_POSITION));

            HUDImage.Add(new UIElement(HEALTH_BAR_POSITION, Textures.HealthBar, Color.White, Color.White));

            //The two equipped items
            Inventory_UI.Add(new InventoryElement(INVENTORY_POSITION + new Vector2(-5, 20), Textures.Items, null, Textures.ItemSlot));
            Inventory_UI.Add(new InventoryElement(INVENTORY_POSITION + new Vector2(20, 20), Textures.Items, null, Textures.ItemSlot));

            //Starts at 2, because it skips over the two equipped items
            for (int i = 2; i < InventorySize; i++)
            {
                Inventory_UI.Add(new InventoryElement(INVENTORY_POSITION + new Vector2((i % 2) * 25 - 5, 30 + (int)(i / 2) * 40), Textures.Items, null, Textures.ItemSlot));
            }
        }

        public static void Update()
        {
            //Only allow the mode to change so that an item isn't activated
            //when the mouse goes into a non cursor mode area of the screen
            if (!InputManager.LeftClicking)
                CursorMode = false;

            //
            //Next, perform actions based on what state the game is in
            //
            if (StateManager.State == 0) //Main Menu
            {

            }
            else if (StateManager.State == 1) //In Game
            {
                if (InputManager.MousePositionV.X > (800 - Textures.SidePanel.Width))
                {
                    //Only allow the mode to change so that an item isn't activated
                    //when the mouse goes into a non cursor mode area of the screen
                    if (!InputManager.LeftClicking)
                    {
                        CursorMode = true;
                    }
                }
            }
            else if (StateManager.State == 2) //Pause Menu
            {
            }

            if (Tooltip != null)
            {
                if (Tooltip.Timeout < 0)
                    DisableTooltip();
                else
                    Tooltip.Update();
            }
        }

        public static void ResetTooltipCounter()
        {
            TooltipCounter = 0;
        }

        public static void IncrementTooltipCounter()
        {
            TooltipCounter++;
        }

        public static bool TooltipCounterLimitReached()
        {
            return TooltipCounter > TOOLTIP_LIMIT;
        }

        public static void EnableTooltip(Vector2 Position, string Title, string Content, int timeout)
        {
            UI.Tooltip = new Tooltip(Position, Title, Content);
            TooltipCounter = 0;
        }

        public static void DisableTooltip()
        {
            UI.Tooltip = null; //If the player is clicking, the tooltip should be removed
            TooltipCounter = 0;
        }


        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Actions != null)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    //spriteBatch.Draw(UI.Actions[i].Texture, UI.Inventory[i].OriginalPosition, null, UI.Actions[i].Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
                }
            }
            for (int i = 0; i < HUDText.Count; i++)
            {
                HUDText[i].Draw(spriteBatch);
                //spriteBatch.DrawString(HUDText[i].Spritefont, HUDText[i].Text, HUDText[i].Position, Color.Violet, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            }

            for (int i = 0; i < HUDImage.Count; i++)
            {
                HUDImage[i].Draw(spriteBatch);
            }
        }


        public static void DrawTooltip(SpriteBatch spriteBatch)
        {
            if (Tooltip != null)
                Tooltip.Draw(spriteBatch);
        }

        public static void DrawHUD(SpriteBatch spriteBatch, Player player)
        {
            //spriteBatch.DrawString(Textures.Fonts[1], "Seed = " + Level.CurrentLevel.Seed, new Vector2(400, 400), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Enemies Killed = " + Kills, new Vector2(250, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Wave: " + Level.CurrentLevel.wave, new Vector2(100, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[0], "Remaining: " + (Level.CurrentLevel.Entities.Count - 1), new Vector2(95, 33), Color.Aqua, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "High Score = " + UI.HighScore, new Vector2(250, 40), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            //spriteBatch.DrawString(Textures.Fonts[1], "Projectiles: " + Projectile.ProjectileList.Count, new Vector2(100, 100), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);

            spriteBatch.Draw(Textures.SidePanel, PANEL_POSITION, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);

            //
            //Calculate the percentage of health remaining for the player
            //
            float PctHealth = ((float)player.Health / (float)player.MaxHealth);
            float HealthBarSize = PctHealth * 50;

            Color HPColour = new Color((byte)(255 - (PctHealth * 255)), (byte)(PctHealth * 255), 0);
            spriteBatch.Draw(Textures.HealthBarPiece, HEALTH_BAR_POSITION + Vector2.One, new Rectangle(0, 0, (int)(PctHealth * 50), Textures.HealthBarPiece.Height), HPColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.82f);
            
            //
            //Next, draw the inventory Items
            //
            for (int i = 0; i < Inventory_UI.Count; i++)
            {
                Inventory_UI[i].Draw(spriteBatch);
            }

            if (UI.CursorMode)
                spriteBatch.Draw(Textures.Cursor, InputManager.MousePositionV, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            else
                spriteBatch.Draw(Textures.Crosshair, InputManager.MousePositionV, null, Color.YellowGreen, 0, Player.CrosshairOrigin, 1, SpriteEffects.None, 0.9f);

        }
    }
}