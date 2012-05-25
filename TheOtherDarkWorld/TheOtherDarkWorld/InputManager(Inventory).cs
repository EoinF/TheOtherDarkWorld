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
        public static int TooltipCounter; //When this reaches 60(1 second), a tooltip appears for the item
        
        private static void InventoryInput()
        {
            CheckTooltip();
            ItemHoveringOver = -1;

            if (UI.Inventory != null)
            {
                for (int i = 0; i < UI.Inventory.Count; i++)
                {
                    if (UI.Inventory[i].IsMouseOver())
                    {
                        //if (Player.PlayerList[0].Inventory[i] != null)
                        ItemHoveringOver = i;
                        UI.Inventory[i].Colour = Color.Orange;
                    }
                    else //If the mouse is not hovering over it, then it should be displayed normally
                        UI.Inventory[i].Colour = Color.White;
                }
            }

            if (DoubleLeftClicked) //The player is double clicking
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver >= 0)
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
                    else //An unequipped item is being equipped(If there's space)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (Player.PlayerList[0].Inventory[i] == null)
                            {
                                Player.PlayerList[0].Inventory[i] = Player.PlayerList[0].Inventory[ItemHoveringOver];
                                Player.PlayerList[0].Inventory[ItemHoveringOver] = null;
                            }
                        }
                    }
                }
            }
            else if (JustLeftClicking) //The player is picking up an item
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed

                if (ItemHoveringOver != -1)
                    ItemHeld = ItemHoveringOver;
            }
            else if (mouseState[0].LeftButton == ButtonState.Released) //We'll assume this means the player is holding down the mouse button
            {
                if (ItemHeld >= 0)
                {
                    UI.Inventory[ItemHeld].Position = UI.Inventory[ItemHeld].OriginalPosition;

                    //They swap places only if they are different
                    if (ItemHoveringOver != -1)
                    {
                        if (ItemHoveringOver != ItemHeld)
                        {
                            //The item is being dropped onto an item slot
                            Item temp = Player.PlayerList[0].Inventory[ItemHoveringOver];
                            Player.PlayerList[0].Inventory[ItemHoveringOver] = Player.PlayerList[0].Inventory[ItemHeld];
                            Player.PlayerList[0].Inventory[ItemHeld] = temp;
                        }
                    }
                }

                ItemHeld = -1; //The player is no longer holding the item
            }
            else
            {
                UI.DisableTooltip(); //If the player is clicking, the tooltip should be removed
            }

            if (ItemHeld >= 0)
            {
                UI.Inventory[ItemHeld].Position = new Vector2(mouseState[0].X - 10, mouseState[0].Y - 10);
                UI.Inventory[ItemHeld].Colour = Color.Orange;
            }
            
            ItemRestingOver = ItemHoveringOver;
        }

        private static void CheckTooltip()
        {
            if (ItemHoveringOver != -1)
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

                    int ttPosX = mouseState[0].X;
                    int ttPosY = mouseState[0].Y;
                    if (mouseState[0].X + Textures.UITextures[1].Width > UI.ScreenX)
                        ttPosX = mouseState[0].X - Textures.UITextures[1].Width;
                    if (mouseState[0].Y + Textures.UITextures[1].Height > UI.ScreenY)
                        ttPosY = mouseState[0].Y - Textures.UITextures[1].Height;

                    UI.Tooltip = new Tooltip(new Vector2(ttPosX, ttPosY), Header, Text);
                    TooltipCounter = 0;
                }
            }
        }
    
    }
}
