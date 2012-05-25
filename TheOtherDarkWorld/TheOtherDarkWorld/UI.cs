using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld
{
    public static class UI
    {
        public static List<UIElement> Inventory;
        public static List<TextSprite> HUDText;
        public static Tooltip Tooltip;
        public static int ScreenX;
        public static int ScreenY;

        public static void Update()
        {
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
    }

    public class UIElement
    {
        public Vector2 OriginalPosition;
        public Vector2 Position;
        private int _textureID;
        public Color Colour;

        /// <summary>
        /// Types:
        ///     0 = ItemSlot
        ///     
        /// </summary>
        int Type;

        public UIElement(Vector2 Position, int Type, int textureID)
        {
            this.Position = OriginalPosition = Position;
            this.Type = Type;
            this._textureID = textureID;
            Colour = Color.White;
        }

        /// <summary>
        /// Important: Must be original position, so that the mouse isn't hovering over it while it's being held
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver()
        {
            return (InputManager.mouseState[0].X > this.OriginalPosition.X
                && InputManager.mouseState[0].X < this.OriginalPosition.X + Textures.UITextures[_textureID].Width
                && InputManager.mouseState[0].Y > this.OriginalPosition.Y
                && InputManager.mouseState[0].Y < this.OriginalPosition.Y + Textures.UITextures[_textureID].Height);
        }
    }
}
