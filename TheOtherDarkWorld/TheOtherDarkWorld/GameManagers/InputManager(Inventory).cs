using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheOtherDarkWorld.Items;

namespace TheOtherDarkWorld
{
    public static partial class InputManager
    {
        private static int ItemHeld = -1;
        private static int ItemHoveringOver = -1; //The item the mouse is hovering over
        private static int ItemRestingOver = -1; //The item the mouse was left on in the last frame
        public static int TooltipCounter; //When this reaches 60(1 second), a tooltip appears for the item
        
        private static void InventoryInput()
        {
            CheckTooltip();
            ItemHoveringOver = -1;
            ItemHeld = -1;

            if (UI.Inventory != null)
            {
                for (int i = 0; i < UI.Inventory.Count; i++)
                {
                    if (UI.Inventory[i].IsMouseOver)
                    {
                        ItemHoveringOver = i;
                    }
                    if (UI.Inventory[i].IsHeld)
                    {
                        ItemHeld = i;
                        UI.Inventory[ItemHeld].Position = new Vector2(MousePositionP.X - 10, MousePositionP.Y - 10);
                    }
                }
            }

            if (DoubleLeftClicked) //The player is double clicking
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver >= 0)
                {
                    UI.Inventory[ItemHoveringOver].DoubleClick(ItemHoveringOver);
                }
            }
            else if (JustLeftClicked)
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver >= 0) //The player has just started clicking the left mouse button
                    UI.Inventory[ItemHoveringOver].JustLeftClicked(ItemHoveringOver);
            }
            else if (mouseState[0].LeftButton == ButtonState.Released) //This means the player isn't clicking
            {
                if (ItemHeld >= 0)
                {
                    UI.Inventory[ItemHeld].JustLeftReleased(ItemHeld, ItemHoveringOver);
                }
            }
            else
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed
            }

            ItemRestingOver = ItemHoveringOver;
        }

        private static void CheckTooltip()
        {
            if (ItemHoveringOver != -1 && Player.PlayerList[0].Inventory[ItemHoveringOver] != null)
            {
                TooltipCounter++;

                if (ItemRestingOver != ItemHoveringOver) //The Item the player is hovering over has changed
                    TooltipCounter = 0; //This prevents the counter from appearing too quickly for the new item
                else
                {
                    //If the mouse is still over the item, while the tooltip is being displayed
                    if (UI.Tooltip != null)
                        UI.Tooltip.Timeout = 60;
                }


                if (TooltipCounter > 60)
                {
                    string Header;
                    string Text;

                    if (Player.PlayerList[0].Inventory[ItemHoveringOver] != null)
                    {
                        Header = Player.PlayerList[0].Inventory[ItemHoveringOver].Name;
                        Text = Player.PlayerList[0].Inventory[ItemHoveringOver].Description;
                    }
                    else
                        return; //If there is no item, then we can't display a tooltip

                    int ttPosX = MousePositionP.X;
                    int ttPosY = MousePositionP.Y;

                    //Make sure that the tooltip is always visible, if it's being shown
                    if (MousePositionP.X + Textures.UITextures[1].Width > UI.ScreenX)
                        ttPosX = MousePositionP.X - Textures.UITextures[1].Width;
                    if (MousePositionP.Y + Textures.UITextures[1].Height > UI.ScreenY)
                        ttPosY = MousePositionP.Y - Textures.UITextures[1].Height;

                    UI.Tooltip = new Tooltip(new Vector2(ttPosX, ttPosY), Header, Text);
                    TooltipCounter = 0;
                }
            }
        }
    
    }
}
