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
        public static List<InventoryElement> Inventory;

        public static int Kills;
        public static int HighScore;
        /// <summary>
        /// The list of actions that the player can perform that are nearby
        /// </summary>
        public static List<UIElement> Actions;
        public static List<TextSprite> HUDText;
        public static Tooltip Tooltip;
        public static bool CursorMode;

        public static int ScreenX { get; set; }
        public static int ScreenY { get; set; }

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

        public static void DisableTooltip()
        {
            UI.Tooltip = null; //If the player is clicking, the tooltip should be removed
            InputManager.TooltipCounter = 0;
        }


        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Actions != null)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    spriteBatch.Draw(UI.Actions[i].Texture, UI.Inventory[i].OriginalPosition, null, UI.Actions[i].Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
                }
            }
            for (int i = 0; i < HUDText.Count; i++)
            {
                HUDText[i].Draw(spriteBatch);
                //spriteBatch.DrawString(HUDText[i].Spritefont, HUDText[i].Text, HUDText[i].Position, Color.Violet, 0, Vector2.Zero, 1, SpriteEffects.None, 0.85f);
            }
        }
    }

    public class UIElement
    {
        public Vector2 OriginalPosition;
        public Vector2 Position;
        public Color Colour 
        {
            get
            {
                return IsMouseOver ? Color.Orange : Color.White;
            }
        }
        public bool IsHeld { get; set; }

        /// <summary>
        /// Textures:
        ///     0 = ItemSlot
        ///     1 = ActionButton
        /// </summary>
        private int _textureID;

        public Texture2D Texture
        {
            get { return Textures.UITextures[_textureID]; }
        }

        public UIElement(Vector2 Position, int Type, int textureID)
        {
            this.Position = OriginalPosition = Position;
            //this.Type = Type;
            this._textureID = textureID;
        }

        public virtual void DoubleClick(int ItemHoveringOver) { }
        public virtual void JustLeftClicked(int ItemHoveringOver) { }
        public virtual void JustLeftReleased(int ItemHeld, int ItemHoveringOver) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver
        {
            get
            {
            
                //Important: Must be its original position, so that the mouse isn't counted as hovering over it while 
                //it's being held
                return (InputManager.MousePositionP.X > this.OriginalPosition.X
                    && InputManager.MousePositionP.X < this.OriginalPosition.X + Texture.Width
                    && InputManager.MousePositionP.Y > this.OriginalPosition.Y
                    && InputManager.MousePositionP.Y < this.OriginalPosition.Y + Texture.Height);
            }
        }
    }

    public class InventoryElement : UIElement
    {
        public InventoryElement(Vector2 Position, int Type, int textureID)
        : base(Position, Type, textureID)
        {
        }

        public override void DoubleClick(int ItemHoveringOver)
        {
            if (ItemHoveringOver < 2) //An equipped item is being unequipped(if there's space)
            {
                for (int i = 2; i < Player.PlayerList[0].Inventory.Length; i++)
                {
                    if (Player.PlayerList[0].Inventory[i] == null)
                    {
                        Player.PlayerList[0].Inventory[i] = Player.PlayerList[0].Inventory[ItemHoveringOver];
                        Player.PlayerList[0].Inventory[ItemHoveringOver] = null;
                    }
                }
            }
            else //An unequipped item is being equipped
            {
                Item temp = Player.PlayerList[0].Inventory[0];
                Player.PlayerList[0].Inventory[0] = Player.PlayerList[0].Inventory[ItemHoveringOver];
                Player.PlayerList[0].Inventory[ItemHoveringOver] = temp;
            }
        }

        public override void JustLeftClicked(int ItemHoveringOver)
        {
            //The player is picking up an item
            if (ItemHoveringOver != -1
                && Player.PlayerList[0].Inventory[ItemHoveringOver] != null) //Can only pick up non null items
                IsHeld = true;
        }

        public override void JustLeftReleased(int ItemHeld, int ItemHoveringOver)
        {
            if (ItemHoveringOver != -1)
            {
                //They swap places only if they are different
                if (ItemHoveringOver != ItemHeld)
                {
                    //The item is being dropped onto an item slot
                    Item temp = Player.PlayerList[0].Inventory[ItemHoveringOver];
                    Player.PlayerList[0].Inventory[ItemHoveringOver] = Player.PlayerList[0].Inventory[ItemHeld];
                    Player.PlayerList[0].Inventory[ItemHeld] = temp;
                }
            }

            if (InputManager.MousePositionP.X < UI.ScreenX && InputManager.MousePositionP.X > 0
                && InputManager.MousePositionP.Y < UI.ScreenY && InputManager.MousePositionP.Y > 0)
            {
                //Dropping the item onto the floor
                Level.CurrentLevel.FloorItems.Add(new FloorItem(Player.PlayerList[0].Inventory[ItemHeld], Player.PlayerList[0].Position + Player.PlayerList[0].Origin));

                Player.PlayerList[0].Inventory[ItemHeld] = null;
            }

            IsHeld = false; //The player is no longer holding the item


            this.Position = this.OriginalPosition;
        }
    }
}
