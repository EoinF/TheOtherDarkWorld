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
        /*private static void CheckTooltip()
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
    */
    }
}
