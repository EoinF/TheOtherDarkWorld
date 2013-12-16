using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheOtherDarkWorld.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class InventoryContainer : UIGrid
    {
        public InventoryContainer(Color Colour, Color HighlightColour,
            Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            int GridColumns = UI_AUTO, int GridRows = UI_AUTO,
            int RowHeight = UI_AUTO, int ColWidth = UI_AUTO,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, GridColumns, GridRows, RowHeight, ColWidth, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, DragAndDropType, DataBinding)
        { }

        public InventoryContainer(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
         int Width = UI_AUTO, int Height = UI_AUTO,
         bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
         int GridColumns = UI_AUTO, int GridRows = UI_AUTO,
         int RowHeight = UI_AUTO, int ColWidth = UI_AUTO,
         float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
         float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
         DragAndDropType DragAndDropType = DragAndDropType.None,
         object[] DataBinding = null)
            : base(Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, GridColumns, GridRows, RowHeight, ColWidth, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, DragAndDropType, DataBinding)
        { }

        public override void SwapChildren(UIElement e1, UIElement e2)
        {
            Item[] binding = DataBinding as Item[];
            int i1 = Children.IndexOf(e1);
            int i2 = Children.IndexOf(e2);

            if (i1 == -1 || i2 == -1)
            {
                //
                // At least one of the child elements don't exist in this container
                // so check higher up
                //
                if (this.Parent != null)
                    (this.Parent as UIContainer).SwapChildren(e1.Parent, e2.Parent);
            }
            else
            {
                if (binding[i1] != null && binding[i2] != null 
                    && binding[i1].Type == binding[i2].Type)
                {
                    int amountMissing = binding[i1].MaxAmount - binding[i1].Amount;
                    if (amountMissing > 0)
                    {
                        if (binding[i2].Amount > amountMissing) //If we have more than enough to fill the missing amount
                        {
                            binding[i1].Amount = binding[i1].MaxAmount;
                            binding[i2].Amount -= amountMissing;
                        }
                        else //We don't have enough to fill up the missing amount
                        {
                            binding[i1].Amount += binding[i2].Amount;
                            binding[i2].Amount = 0;
                        }
                    }
                }
                else if (e1.SwapWith(e2)) //If they actually swap
                {
                    UIElement tempEl = Children[i1];
                    Children[i1] = Children[i2];
                    Children[i2] = tempEl;

                    if (i1 < DataBinding.Length && i2 < DataBinding.Length)
                    {
                        var tempobj = DataBinding[i1];
                        DataBinding[i1] = DataBinding[i2];
                        DataBinding[i2] = tempobj;
                    }
                }
            }
        }
    }
}
