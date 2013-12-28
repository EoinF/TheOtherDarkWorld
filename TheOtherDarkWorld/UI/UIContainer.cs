using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class UIContainer : UIElement
    {
        protected object[] DataBinding { get; set; }

        private bool CentreHorizontal, CentreVertical;
        protected List<UIElement> Children;

        public UIElement this[int index]
        {
            get {
                if (index < Children.Count)
                    return Children[index];
                else return null;
            }
        }

        public override int Width
        {
            get
            {
                if (_width == UI_AUTO) //Autosize
                {
                    if (Texture != null)
                        return Texture.Width;
                    else
                    {
                        if (Children.Count > 0)
                            return Children[0].Width;
                        else
                            return 0;
                    }
                }
                else if (_width == UI_INHERIT)
                {
                    if (Parent != null)
                    {
                        if (Parent is UIGrid)
                        {
                            var par = (Parent as UIGrid);
                            if (par.GridColumns != UI_AUTO)
                                return par.ColWidth;
                            else
                                return par.Width;
                        }
                        else if (Parent is UIContainer)
                            return (Parent as UIContainer).Width;
                    }
                    return 0;
                }
                else
                    return _width;
            }
        }
        public override int Height
        {
            get
            {
                if (_height == UI_AUTO) //Autosize
                {
                    if (Texture != null)
                        return Texture.Height;
                    else
                    {
                        if (Children.Count > 0)
                            return Children[0].Height;
                        else
                            return 0;
                    }
                }
                else if (_height == UI_INHERIT)
                {
                    if (Parent != null)
                    {
                        if (Parent is UIGrid)
                        {
                            var par = (Parent as UIGrid);
                            if (par.GridRows != UI_AUTO)
                                return par.RowHeight;
                            else
                                return par.Height;
                        }
                        else if (Parent is UIContainer)
                            return (Parent as UIContainer).Height;
                    }
                    return 0;
                }
                else
                    return _height;
            }
        }

        public override Vector2 Position
        {
            set
            {
                foreach (UIElement child in Children)
                {
                    child.Position += value - this.Position; //Change the position of the child relative to the container
                }
                this._position = value;
            }
        }
        public override Vector2 OriginalPosition
        {
            set
            {
                foreach (UIElement child in Children)
                {
                    child.OriginalPosition += value - this.OriginalPosition; //Change the position of the child relative to the container
                }
                this._originalPosition = value;
            }
        }

        protected DragAndDropType _dragAndDropType { get; set; }


        public UIContainer(Color Colour, Color HighlightColour,
            Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
        {
            Children = new List<UIElement>();
            this.CentreHorizontal = CentreHorizontal;
            this.CentreVertical = CentreVertical;

            this._dragAndDropType = DragAndDropType;
            if (_dragAndDropType == DragAndDropType.DropElement)
            {
                this.OnLeftReleased = OnLeftReleasedHandler;
            }

            this.DataBinding = DataBinding;
            if (this.DataBinding == null)
                this.DataBinding = new object[0];
        }

        public UIContainer(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
            : base(Color.White, Color.White, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
        {
            Children = new List<UIElement>();
            this.CentreHorizontal = CentreHorizontal;
            this.CentreVertical = CentreVertical;

            this._dragAndDropType = DragAndDropType;
            if (_dragAndDropType == DragAndDropType.DropElement)
            {
                this.OnLeftReleased = OnLeftReleasedHandler;
            }

            this.DataBinding = DataBinding;
            if (this.DataBinding == null)
                this.DataBinding = new object[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="newElement"></param>
        public virtual void AddElement(UIElement newElement)
        {
            newElement.Parent = this;
            Children.Add(newElement);
            int p = this.Height;

            newElement.Position = newElement.OriginalPosition += this.Position + new Vector2(newElement.MarginLeft, newElement.MarginTop) + new Vector2(CentreHorizontal ? (this.Width - newElement.Width) / 2 : 0, CentreVertical ? (this.Height - newElement.Height) / 2 : 0);

            if (_dragAndDropType == DragAndDropType.DropElement)
            {
                newElement.OnLeftClicked = OnLeftClickHandler;
                newElement.OnLeftReleased = OnLeftReleasedHandler;
            }
        }

        public virtual void RemoveElement(UIElement existingElement)
        {
            //Undo the changes made when it was added
            if (existingElement != null)
            {
                existingElement.Position = existingElement.OriginalPosition -= this.Position + new Vector2(CentreHorizontal ? (this.Width - existingElement.Width) / 2 : 0, CentreVertical ? (this.Height - existingElement.Height) / 2 : 0);

                Children.Remove(existingElement);
            }
        }

        public virtual void ClearElements()
        {
            while (Children.Count > 0)            
            {
                RemoveElement(Children[0]);
            }
        }

        
        /// <summary>
        /// When the left mouse button is released while hovering over sender
        /// </summary>
        /// <param name="sender"></param>
        protected static void OnLeftReleasedHandler(object sender)
        {
            if (UI.ItemHeld != null)
            {
                if (sender != UI.ItemHeld.Parent)
                {
                    UI.ItemHeld.Parent.OnLeftReleased(UI.ItemHeld.Parent);
                }
                else
                {
                    //
                    // Check if the item is being dropped outside the container it belongs to
                    // Don't allow it to be dropped outside
                    //
                    Vector2 boundaryEnforcedPosition = UI.ItemHeld.Position;
                    if (boundaryEnforcedPosition.X < UI.ItemHeld.Parent.Position.X)
                        boundaryEnforcedPosition.X = UI.ItemHeld.Parent.Position.X;
                    else if (boundaryEnforcedPosition.X > UI.ItemHeld.Parent.Position.X + UI.ItemHeld.Parent.Width - UI.ItemHeld.Width)
                        boundaryEnforcedPosition.X = UI.ItemHeld.Parent.Position.X + UI.ItemHeld.Parent.Width - UI.ItemHeld.Width;

                    if (boundaryEnforcedPosition.Y < UI.ItemHeld.Parent.Position.Y)
                        boundaryEnforcedPosition.Y = UI.ItemHeld.Parent.Position.Y;
                    else if (boundaryEnforcedPosition.Y > UI.ItemHeld.Parent.Position.Y + UI.ItemHeld.Parent.Height - UI.ItemHeld.Height)
                        boundaryEnforcedPosition.Y = UI.ItemHeld.Parent.Position.Y + UI.ItemHeld.Parent.Height - UI.ItemHeld.Height;

                    UI.ItemHeld.OriginalPosition = boundaryEnforcedPosition;
                    UI.ItemHeld.Position = boundaryEnforcedPosition;
                    UI.ItemHeld.IsHeld = false;
                    UI.ItemHeld = null;
                }
            }
        }

        protected static void OnLeftClickHandler(object sender)
        {
            if (UI.ItemHeld == null)
            {
                UIElement el = (sender as UIElement).GetDraggable();
                //The user is clicking and dragging an item
                if (el != null)
                {
                    UI.DragOffset = el.Position - InputManager.MousePositionV;
                    UI.ItemHeld = el;
                    el.IsHeld = true;
                }
            }
        }
        
        public virtual void SwapChildren(UIElement e1, UIElement e2)
        {
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
                if (e1.SwapWith(e2)) //If they actually swap
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

        public override UIElement GetDraggable()
        {
            UIElement el = base.GetDraggable();
            if (el != null)
                return el;

            foreach (UIElement child in Children)
            {
                el = child.GetDraggable();
                if (el != null)
                    return el;
            }

            return null;
        }

        public override void Update()
        {
            if (IsActive)
            {
                base.Update();
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Update();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float extraLayerDepth = 0)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch, extraLayerDepth);
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Draw(spriteBatch, extraLayerDepth + 0.001f);
                }
            }
        }
    }
}
