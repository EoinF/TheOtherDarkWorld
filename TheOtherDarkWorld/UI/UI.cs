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
        private static Vector2 PANEL_POSITION = new Vector2(800 - PANEL_WIDTH, 0);
        private static Vector2 HEALTH_POSITION = new Vector2(0, 10);
        private static Vector2 HEALTH_BAR_POSITION = new Vector2(0, 30);
        private static Vector2 STATUS_EFFECTS_POSITION = new Vector2(0, 50);
        private static Vector2 INVENTORY_POSITION = new Vector2(0, 120);
        private static int PANEL_WIDTH { get { return Textures.SidePanel.Width; } }
        private static int PANEL_HEIGHT { get { return Textures.SidePanel.Height; } }
        private const int TOOLTIP_LIMIT = 60;
        public const CursorType CURSOR_DEFAULT = CursorType.Crosshair;

        public const float CURSOR_DEPTH_DEFAULT = 0.9f;
        public const float TOOLTIP_DEPTH_DEFAULT = 0.89f;
        public const float UIELEMENT_DEPTH_DEFAULT = 0.8f;
        public const float MELEE_DEPTH_DEFAULT = 0.17f;
        public const float FLOORITEM_DEPTH_DEFAULT = 0.14f;
        public const float PROJECTILE_DEPTH_DEFAULT = 0.13f;
        public const float ENEMY_DEPTH_DEFAULT = 0.12f;
        public const float PLAYER_DEPTH_DEFAULT = 0.11f;
        public const float GAMEOBJECT_DEPTH_DEFAULT = 0.1f;

        public static UIElement ItemHeld;
        public static Vector2 DragOffset;

        public static InventoryContainer Inventory_UI;

        public static int Kills;
        public static int HighScore;
        public static UIContainer HUDElements;
        public static UIContainer UIControls;

        private static UIGrid Tooltip;
        public static CursorType CursorMode;

        public static int ScreenX { get; set; }
        public static int ScreenY { get; set; }


        public static void InitializeHUD(int InventorySize)
        {
            HUDElements = new UIContainer(Width: ScreenX - PANEL_WIDTH, Height: ScreenY, layerDepth: UIElement.UI_AUTO);
            
            TextSprite ts = new TextSprite("Items", 1, Color.Violet, PANEL_WIDTH, INVENTORY_POSITION, CursorType.Cursor);
            UIContainer SidePanel = new UIContainer(Textures.SidePanel, PANEL_POSITION, null, PANEL_WIDTH, PANEL_HEIGHT, CursorType: CursorType.Cursor, CentreHorizontal: true);
            Tooltip = new UIGrid(Color.DarkRed, Color.DarkRed, Textures.Tooltip, GridColumns: 1, GridRows: 2, CursorType: CursorType.Cursor, IsActive: false, layerDepth: TOOLTIP_DEPTH_DEFAULT);
            Tooltip.OnLeftClicked += x => { Tooltip.IsActive = false; };

            UIControls = new UIContainer(Width: (int)PANEL_POSITION.X, Height: ScreenY, DragAndDropType: DragAndDropType.DropElement);
            Inventory_UI = new InventoryContainer(Position: INVENTORY_POSITION + Vector2.UnitY * ts.Height, Width: PANEL_WIDTH, Height: (int)(400 - INVENTORY_POSITION.Y), GridColumns: 2, GridRows: 7, RowHeight: Textures.ItemSlot.Height, CursorType: CursorType.Cursor, DragAndDropType: DragAndDropType.SwapElement, DataBinding: Level.CurrentLevel.Players[Level.CurrentLevel.PlayerIndex].Inventory );

            SidePanel.AddElement(ts);
            SidePanel.AddElement(new TextSprite("Status", 1, Color.Violet, PANEL_WIDTH, STATUS_EFFECTS_POSITION, CursorType.Cursor));
            SidePanel.AddElement(new TextSprite("Health", 1, Color.Violet, PANEL_WIDTH, HEALTH_POSITION, CursorType.Cursor));

            SidePanel.AddElement(new UIElement(Textures.HealthBar, HEALTH_BAR_POSITION, Color.White, Color.White, CursorType: CursorType.Cursor));

            //The two equipped items
            UIContainer con = new UIContainer(Textures.ItemSlot, CursorType: CursorType.Cursor);
            con.AddElement(new InventoryElement(Textures.Items, CursorType: CursorType.Cursor));
            Inventory_UI.AddElement(con);
            con = new UIContainer(Textures.ItemSlot, CursorType: CursorType.Cursor);
            con.AddElement(new InventoryElement(Textures.Items, CursorType: CursorType.Cursor));
            Inventory_UI.AddElement(con);

            //Starts at 2, because it skips over the two equipped items
            for (int i = 2; i < InventorySize; i++)
            {
                con = new UIContainer(Textures.ItemSlot, new Vector2(0, 10), CursorType: CursorType.Cursor);
                con.AddElement(new InventoryElement(Textures.Items, CursorType: CursorType.Cursor));
                Inventory_UI.AddElement(con);
            }

            //
            // Add event handlers to the inventory items
            //
            for (int i = 0; i < InventorySize; i++)
            {
                //Add the event handler for when the player wants to activate an item by double clicking
                Inventory_UI[i].OnDoubleClick += OnDoubleClickInventoryHandler;
                Inventory_UI[i].OnHover += OnHoverInventoryHandler;
                Inventory_UI[i].OnMouseLeave += x => { Tooltip.IsActive = false; };
            }

            //
            //Include the event handler for when an item is dropped
            //Add it before the current handler, because this is a special case
            //
            UIControls.OnLeftReleased = obj =>
            {
                if (ItemHeld is InventoryElement) //Check if it is an inventory element
                {
                    InventoryElement ie = ItemHeld as InventoryElement;
                    //Inventory items can be dropped onto the map
                    Level.CurrentLevel.Players[Level.CurrentLevel.PlayerIndex].DropItem(ie.Item);
                    ie.Item = null;
                    ItemHeld.IsHeld = false;
                    ItemHeld = null;
                }
            } + UIControls.OnLeftReleased;

            SidePanel.AddElement(Inventory_UI);
            HUDElements.AddElement(SidePanel);
            HUDElements.AddElement(UIControls);
            HUDElements.AddElement(Tooltip);
        }

        public static void Update()
        {
            //Only allow the mode to change so that an item isn't activated
            //when the mouse goes into a non cursor mode area of the screen
            if (!InputManager.LeftClicking)
            {
                CursorMode = CURSOR_DEFAULT;
            }

            HUDElements.Update();

            UIElement ItemToCheck = ItemHeld;

            while (!InputManager.LeftClicking && ItemToCheck != null) //The item was released
            {
                ItemToCheck.OnLeftReleased(ItemToCheck);
                ItemToCheck = ItemToCheck.Parent;
            }
            //
            //Next, perform actions based on what state the game is in
            //
            if (StateManager.State == 0) //Main Menu
            {

            }
            else if (StateManager.State == 1) //In Game
            {
            }
            else if (StateManager.State == 2) //Pause Menu
            {
            }
        }

        public static void EnableTooltip(Vector2 Position, string Title, string Content)
        {
            Tooltip.IsActive = true;
            Tooltip.Position = Position;
            Tooltip.ClearElements();
            Tooltip.AddElement(new TextSprite(Title, 2, Color.Aqua, Tooltip.Width - 20, CursorType: CursorType.Cursor, MarginBottom: 0));
            Tooltip.AddElement(new TextSprite(Content, 1, Color.LightCyan, Tooltip.Width - 20, CursorType: CursorType.Cursor, MarginTop: 0));

            if (Tooltip.Position.X < 0)
                Tooltip.Position = new Vector2(0, Tooltip.Position.Y);
            if (Tooltip.Position.X + Tooltip.Width > ScreenX)
                Tooltip.Position = new Vector2(ScreenX - Tooltip.Width, Tooltip.Position.Y);
            if (Tooltip.Position.Y < 0)
                Tooltip.Position = new Vector2(Tooltip.Position.X, 0);
            if (Tooltip.Position.Y + Tooltip.Height > ScreenY)
                Tooltip.Position = new Vector2(Tooltip.Position.X, ScreenY - Tooltip.Height);
        }

        public static void DrawHUD(SpriteBatch spriteBatch, Player player)
        {
            HUDElements.Draw(spriteBatch);
            //spriteBatch.DrawString(Textures.Fonts[1], "Seed = " + Level.CurrentLevel.Seed, new Vector2(400, 400), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Enemies Killed = " + Kills, new Vector2(250, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "Wave: " + Level.CurrentLevel.wave, new Vector2(100, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[0], "Remaining: " + (Level.CurrentLevel.Entities.Count - 1), new Vector2(95, 33), Color.Aqua, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(Textures.Fonts[1], "High Score = " + UI.HighScore, new Vector2(250, 40), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            //spriteBatch.DrawString(Textures.Fonts[1], "Projectiles: " + Projectile.ProjectileList.Count, new Vector2(100, 100), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);

            //spriteBatch.Draw(Textures.SidePanel, PANEL_POSITION, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);

            //
            //Calculate the percentage of health remaining for the player
            //
            float PctHealth = ((float)player.Health / (float)player.MaxHealth);
            float HealthBarSize = PctHealth * 50;

            Color HPColour = new Color((byte)(255 - (PctHealth * 255)), (byte)(PctHealth * 255), 0);
            spriteBatch.Draw(Textures.HealthBarPiece, PANEL_POSITION + HEALTH_BAR_POSITION + new Vector2(6, 0) + Vector2.One, new Rectangle(0, 0, (int)(PctHealth * 50), Textures.HealthBarPiece.Height), HPColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.82f);


            if (UI.CursorMode == CursorType.Cursor)
                spriteBatch.Draw(Textures.Cursor, InputManager.MousePositionV, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, CURSOR_DEPTH_DEFAULT);
            else if (UI.CursorMode == CursorType.Crosshair)
                spriteBatch.Draw(Textures.Crosshair, InputManager.MousePositionV, null, Color.YellowGreen, 0, Player.CrosshairOrigin, 1, SpriteEffects.None, CURSOR_DEPTH_DEFAULT);

        }

        private static void OnDoubleClickInventoryHandler(object sender)
        {
            InventoryElement ie = (sender as UIContainer)[0] as InventoryElement;
            if (ie.Item != null)
                ie.Item.Activate();
        }

        private static void OnHoverInventoryHandler(object sender)
        {
            InventoryElement ie = (sender as UIContainer)[0] as InventoryElement;
            if (ie.Item != null)
            {
                EnableTooltip(InputManager.MousePositionV, ie.Item.Name, ie.Item.Description);
            }
        }
    }
}