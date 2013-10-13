using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheOtherDarkWorld
{
    public static partial class InputManager
    {
        private static int ItemHeld = -1;
        private static int ItemHoveringOver = -1; //The item the mouse is hovering over
        private static int ItemRestingOver = -1; //The item the mouse was left on in the last frame
        
        private static void InventoryInput()
        {
            CheckTooltip();
            ItemHoveringOver = -1;
            ItemHeld = -1;

            if (UI.Inventory_UI != null)
            {
                for (int i = 0; i < UI.Inventory_UI.Count; i++)
                {
                    UI.Inventory_UI[i].Update();

                    if (UI.Inventory_UI[i].IsMouseOver)
                    {
                        ItemHoveringOver = i;
                    }
                    if (UI.Inventory_UI[i].IsHeld)
                    {
                        ItemHeld = i;
                        UI.Inventory_UI[ItemHeld].Position = new Vector2(MousePositionP.X - 10, MousePositionP.Y - 10);
                    }
                }
            }

            if (DoubleLeftClicked) //The player is double clicking
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver >= 0)
                {
                    UI.Inventory_UI[ItemHoveringOver].DoubleClick();
                }
            }
            else if (JustLeftClicked)
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver >= 0) //The player has just started clicking the left mouse button
                    UI.Inventory_UI[ItemHoveringOver].JustLeftClicked();
            }
            else if (mouseState[0].LeftButton == ButtonState.Released) //This means the player isn't clicking
            {
                if (ItemHeld >= 0)
                {
                    UI.Inventory_UI[ItemHeld].JustLeftReleased(ItemHeld, ItemHoveringOver);
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
            if (ItemHoveringOver != -1 && Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver] != null)
            {
                UI.IncrementTooltipCounter();

                if (ItemRestingOver != ItemHoveringOver) //The Item the player is hovering over has changed
                    UI.ResetTooltipCounter(); //This prevents the tooltip from appearing too quickly for the new item
                //else
                //{
                    //If the mouse is still over the item, while the tooltip is being displayed
                    //UI.RefreshTooltip(60);
                //}


                if (UI.TooltipCounterLimitReached())
                {
                    string Header;
                    string Text;

                    if (Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver] != null)
                    {
                        Header = Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver].Name;
                        Text = Level.CurrentLevel.Players[0].Inventory[ItemHoveringOver].Description;
                    }
                    else
                        return; //If there is no item, then we can't display a tooltip

                    int ttPosX = MousePositionP.X;
                    int ttPosY = MousePositionP.Y;

                    //Make sure that the tooltip is always visible, if it's being shown
                    if (MousePositionP.X + Textures.Tooltip.Width > UI.ScreenX)
                        ttPosX = MousePositionP.X - Textures.Tooltip.Width;
                    if (MousePositionP.Y + Textures.Tooltip.Height > UI.ScreenY)
                        ttPosY = MousePositionP.Y - Textures.Tooltip.Height;

                    UI.EnableTooltip(new Vector2(ttPosX, ttPosY), Header, Text, 0);
                }
            }
        }
    
    }
}
