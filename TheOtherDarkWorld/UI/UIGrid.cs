using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class UIGrid : UIContainer
    {
        public override int Height
        {
            get
            {
                if (_height == UI_AUTO)
                {
                    if (Texture != null)
                        return Texture.Height;
                    else if (GridColumns == 1 && GridRows == UI_AUTO)
                    {
                        int total = 0;
                        for (int i = 0; i < Children.Count; i++)
                        {
                            total += (int)(Children[i].MarginTop + Children[i].Height + Children[i].MarginBottom);
                        }
                        return total;
                    }
                    else
                        return RowHeight * GridRows;
                }
                else
                {
                    return _height;
                }
            }
        }
        public override int Width
        {
            get
            {
                if (_width == UI_AUTO)
                {
                    if (Texture != null)
                        return Texture.Width;
                    else if (GridRows == 1 && GridColumns == UI_AUTO)
                    {
                        int total = 0;
                        for (int i = 0; i < Children.Count; i++)
                        {
                            total += (int)(Children[i].MarginLeft + Children[i].Width + Children[i].MarginRight);
                        }
                        return total;
                    }
                    else
                        return ColWidth * GridColumns;
                }
                else
                {
                    return _width;
                }
            }
        }

        public int GridRows { get; set; }
        public int GridColumns { get; set; }

        private int _minColWidth;
        private int _colWidth;
        public int ColWidth
        {
            get
            {
                if (_colWidth == UI_AUTO)
                {
                    if (_minColWidth == 0)
                        return Width / GridColumns;
                    else
                        return _minColWidth;

                }
                else
                    return _colWidth;
            }
            set
            {
                _colWidth = value;
            }
        }

        private int _minRowHeight;
        private int _rowHeight;
        public int RowHeight
        {
            get
            {
                if (_rowHeight == UI_AUTO)
                {
                    if (_minRowHeight == 0)
                    {
                        return Height / GridRows;
                    }
                    else
                        return _minRowHeight;
                }
                else
                    return _rowHeight;
            }
            set
            {
                _rowHeight = value;
            }
        }


        public UIGrid(Color Colour, Color HighlightColour,
            Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            int GridColumns = UI_AUTO, int GridRows = UI_AUTO,
            int RowHeight = UI_AUTO, int ColWidth = UI_AUTO,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, false, false, DragAndDropType, DataBinding)
        {
            Children = new List<UIElement>();
            this.GridRows = GridRows;
            this.GridColumns = GridColumns;
            this.RowHeight = RowHeight;
            this.ColWidth = ColWidth;
        }

        public UIGrid(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            int GridColumns = UI_AUTO, int GridRows = UI_AUTO,
            int RowHeight = UI_AUTO, int ColWidth = UI_AUTO,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
            : base(Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, false, false, DragAndDropType, DataBinding)
        {
            Children = new List<UIElement>();
            this.GridRows = GridRows;
            this.GridColumns = GridColumns;
            this.RowHeight = RowHeight;
            this.ColWidth = ColWidth;
        }

        /// <summary>
        /// </summary>
        /// <param name="newElement"></param>
        public override void AddElement(UIElement newElement)
        {
            newElement.Parent = this;
            Children.Add(newElement);

            int col = 0;
            int row = 0;
            if (GridColumns != 1)
            {
                col = (Children.Count - 1) % GridColumns;
            }
            if (GridRows != 1)
            {
                row = (Children.Count - 1) / GridColumns;
            }

            if (_minColWidth < newElement.Width)
                _minColWidth = newElement.Width;
            if (_minRowHeight < newElement.Height)
                _minRowHeight = newElement.Height;

            //
            // Find out what the space is to the left of and above this element
            //
            float _spaceToLeft = 0;
            float _spaceAbove = 0;
            for(int c = col; c > 0; c--)
            {
                UIElement el = Children[row * GridColumns + (c - 1)];
                if (GridColumns != UI_AUTO)
                {
                    _spaceToLeft += el.MarginLeft + el.MarginRight + el.Width;
                }
                else
                {
                    _spaceToLeft += el.MarginLeft + el.MarginRight + el.Width;
                }
            }
            for (int r = row; r > 0; r--)
            {
                UIElement el = Children[r * GridColumns + (col - 1)];
                if (GridRows != UI_AUTO)
                {
                    _spaceAbove += el.MarginTop + el.MarginBottom + RowHeight;
                }
                else
                {
                    _spaceAbove += el.MarginTop + el.MarginBottom + el.Height;
                }
            }

            newElement.Position = newElement.OriginalPosition += Position + new Vector2((int)(_spaceToLeft + newElement.MarginLeft), (int)(_spaceAbove + newElement.MarginTop));

            switch (_dragAndDropType)
            {
                case DragAndDropType.DropElement:
                    newElement.OnLeftClicked = OnLeftClickHandler;
                    newElement.OnLeftReleased = OnLeftReleasedHandler;
                    this.OnLeftReleased += OnLeftReleasedHandler;
                    break;
                case DragAndDropType.InsertElement:
                    newElement.OnLeftClicked = OnLeftClickHandler;
                    newElement.OnLeftReleased = OnLeftReleasedHandler_Insert;
                    break;
                case DragAndDropType.SwapElement:
                    newElement.OnLeftClicked = OnLeftClickHandler;
                    newElement.OnLeftReleased = OnLeftReleasedHandler_Swap;
                    break;
                default: //Do nothing
                    break;
            }
        }

        public override void ClearElements()
        {
            base.ClearElements();
            _minColWidth = 0;
            _minRowHeight = 0;
        }

        public override void RemoveElement(UIElement existingElement)
        {
            int index = Children.IndexOf(existingElement);

            for (int i = Children.Count - 1; i > index; i--)
            {
                //First move the child i back to where i - 1 is
                Children[i].Position = Children[i - 1].Position;
                Children[i].OriginalPosition = Children[i - 1].OriginalPosition;

                //Next, replace i with i - 1
                Children[i] = Children[i - 1];
            }
            Children.RemoveAt(Children.Count - 1);
        }



        /// <summary>
        /// When the left mouse button is released while hovering over sender
        /// </summary>
        /// <param name="sender"></param>
        protected static void OnLeftReleasedHandler_Swap(object sender)
        {
            if (UI.ItemHeld != null)
            {
                UIElement e = (sender as UIElement).GetDraggable();
                //They swap places only if they are different
                if (e != null)
                {
                    if (e == UI.ItemHeld)
                    {
                        UI.ItemHeld.IsHeld = false;
                        UI.ItemHeld = null;
                    }
                    else
                        (e.Parent as UIContainer).SwapChildren(e, UI.ItemHeld);
                }
            }
        }

        /// <summary>
        /// When the left mouse button is released while hovering over sender
        /// </summary>
        /// <param name="sender"></param>
        protected static void OnLeftReleasedHandler_Insert(object sender)
        {
            if (UI.ItemHeld != null)
            {
                UIElement e = (sender as UIElement).GetDraggable();
                //They swap places only if they are different
                if (e != null && e != UI.ItemHeld)
                {
                    (UI.ItemHeld.Parent as UIGrid).SwapAndPushForward(e, UI.ItemHeld);
                    UI.ItemHeld.IsHeld = false;
                    UI.ItemHeld = null;
                }
            }
        }

        private void SwapAndPushForward(UIElement FirstElementToPushForward, UIElement ElementToMoveBack)
        {
            int i1 = Children.IndexOf(FirstElementToPushForward);
            int i2 = Children.IndexOf(ElementToMoveBack);

            if (i1 == -1 || i2 == -1)
            {
                //
                // At least one of the child elements don't exist in this container
                // so check higher up
                //
                if (this.Parent != null)
                    (this.Parent as UIGrid).SwapAndPushForward(FirstElementToPushForward.Parent, ElementToMoveBack.Parent);
            }
            else
            {
                UIElement temp = ElementToMoveBack;
                Vector2 tempPos = FirstElementToPushForward.Position;
                Vector2 tempOrigPos = FirstElementToPushForward.OriginalPosition;

                for (int i = i2 - 1; i >= i1; i--)
                {
                    //First move the child i forward to where i+1 is
                    Children[i].Position = Children[i + 1].Position;
                    Children[i].OriginalPosition = Children[i + 1].OriginalPosition;

                    //Next, replace i + 1 with i
                    Children[i + 1] = Children[i];
                }

                Children[i2].Position = tempPos;
                Children[i2].OriginalPosition = tempOrigPos;
                Children[i1] = temp;
            }
        }
    }
}
