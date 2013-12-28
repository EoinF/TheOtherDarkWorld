using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class UIElement
    {
        public const int UI_AUTO = -1;
        public const int UI_INHERIT = -2;
        private const int HOVER_TICK_THRESHOLD = 1;
        public const float OPACITY_DEFAULT = 1;

        public UIElement Parent { get; set; }
        protected CursorType _cursorType { get; set; }

        protected Vector2 _originalPosition;
        public virtual Vector2 OriginalPosition
        {
            get { return _originalPosition; }
            set { _originalPosition = value; }
        }

        protected float _opacity;
        public float Opacity
        {
            get
            {
                if (_opacity != UI_INHERIT)
                    return _opacity;
                else if (Parent != null)
                    return Parent.Opacity;
                else
                    return OPACITY_DEFAULT;
            }
            set
            {
                _opacity = value;
            }
        }

        private float _layerDepth;
        public float LayerDepth
        {
            get
            {
                if (_layerDepth == UI_INHERIT)
                {
                    if (Parent != null)
                        return Parent.LayerDepth;
                    else
                        return UI.UIELEMENT_DEPTH_DEFAULT;
                }
                else if (_layerDepth == UI_AUTO)
                    return UI.UIELEMENT_DEPTH_DEFAULT;
                else
                    return _layerDepth;
            }
            set
            {
                _layerDepth = value;
            }
        }

        protected float _marginLeft;
        public virtual float MarginLeft
        {
            get
            {
                if (_marginLeft == UI_AUTO)
                {
                    if (Parent is UIGrid)
                    {
                        var parent = (Parent as UIGrid);
                        if (parent.GridColumns != UI_AUTO)
                        {
                            if (this._width != UI_AUTO)
                                return ((float)(parent.Width - (this.Width * parent.GridColumns)) / (float)parent.GridColumns) / 2f; //Divide by 2 because there is also margin to the right
                            else
                                return ((float)(parent.Width - (parent.ColWidth * parent.GridColumns)) / (float)parent.GridColumns) / 2f; //Divide by 2 because there is also margin to the right
                        }
                        else
                            return 0;
                    }
                    else
                        return 0;
                }
                else
                {
                    return _marginLeft;
                }
            }
            set
            {
                _marginLeft = value;
            }
        }
        protected float _marginRight;
        public virtual float MarginRight
        {
            get
            {
                if (_marginRight == UI_AUTO)
                {
                    if (Parent is UIGrid)
                    {
                        var parent = (Parent as UIGrid);
                        if (parent.GridColumns != UI_AUTO)
                            return ((float)(parent.Width - (parent.ColWidth * parent.GridColumns)) / (float)parent.GridColumns) / 2f; //Divide by 2 because there is also margin to the right
                        else
                            return 0;
                    }
                    else
                        return 0;
                }
                else
                {
                    return _marginRight;
                }
            }
            set
            {
                _marginRight = value;
            }
        }
        protected float _marginTop;
        public virtual float MarginTop
        {
            get
            {
                if (_marginTop == UI_AUTO)
                {
                    if (Parent is UIGrid)
                    {
                        var parent = (Parent as UIGrid);
                        if (parent.GridRows != UI_AUTO)
                        {
                            return ((float)(parent.Height - (parent.RowHeight * parent.GridRows)) / (float)parent.GridRows) / 2f; //Divide by 2 because there is also margin below
                        }
                        else
                            return 0;
                    }
                    else
                        return 0;
                }
                else
                {
                    return _marginTop;
                }
            }
            set
            {
                _marginTop = value;
            }
        }
        protected float _marginBottom;
        public virtual float MarginBottom
        {
            get
            {
                if (_marginBottom == UI_AUTO)
                {
                    if (Parent is UIGrid)
                    {
                        var parent = (Parent as UIGrid);
                        if (parent.GridRows != UI_AUTO)
                        {
                            return ((float)(parent.Height - (parent.RowHeight * parent.GridRows)) / (float)parent.GridRows) / 2f; //Divide by 2 because there is also margin above
                        }
                        else
                            return 0;
                    }
                    else
                        return 0;
                }
                else
                {
                    return _marginBottom;
                }
            }
            set
            {
                _marginBottom = value;
            }
        }
        public virtual Color Colour
        {
            get
            {
                return IsMouseOver? _highlightColour : _colour;
            }
            set
            {
                _colour = value;
                _highlightColour = value;
            }
        }
        protected Color _colour;
        protected Color _highlightColour;

        public bool IsDraggable { get; set; }
        public virtual Rectangle? SrcRect { get; set; }
        protected Texture2D Texture;


        protected int _width;
        public virtual int Width
        {
            get
            {
                if (_width == UI_AUTO) //Autosize
                {
                    if (Texture != null)
                        return Texture.Width;
                    else
                        return 0;
                }
                else if (_width == UI_INHERIT)
                {
                    if (Parent != null)
                    {
                        if (Parent is UIGrid)
                            return (Parent as UIGrid).ColWidth;
                        else if (Parent is UIContainer)
                            return (Parent as UIContainer).Width;
                    }
                    return 0;
                }
                else
                    return _width;
            }
        }
        protected int _height;
        public virtual int Height
        {
            get
            {
                if (_height == UI_AUTO) //Autosize
                {
                    if (Texture != null)
                        return Texture.Height;
                    else
                        return 0;
                }
                else if (_height == UI_INHERIT)
                {
                    if (Parent != null)
                    {
                        if (Parent is UIGrid)
                            return (Parent as UIGrid).RowHeight;
                        else if (Parent is UIContainer)
                            return (Parent as UIContainer).Height;
                    }
                    return 0;
                }
                else
                    return _height;
            }
        }

        protected Vector2 _position;
        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public bool IsActive { get; set; }
        private bool _isHeld;
        public bool IsHeld
        {
            get
            {
                return _isHeld;
            }
            set
            {
                _isHeld = value;
                if (!value) //Not being held anymore
                    Position = OriginalPosition; //So the item is returned to its original position
            }
        }

        public bool WasMouseOver { get; set; }
        public bool IsMouseOver { get; set; }
        private int _hoverTicks;
        public bool IsHovering { get { return _hoverTicks > HOVER_TICK_THRESHOLD; } }

        public Action<object> OnDoubleClick;
        public Action<object> OnLeftClicked;
        public Action<object> OnLeftReleased;
        public Action<object> OnMouseEnter;
        public Action<object> OnMouseLeave;
        public Action<object> OnHover;

        public UIElement(Color Colour = new Color(), Color HighlightColour = new Color(), Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT)
        {
            this._position = this._originalPosition = Position;
            this.Texture = Texture;
            this.SrcRect = SrcRect;
            this._colour = Colour;
            this._highlightColour = HighlightColour;

            this._width = Width;
            this._height = Height;
            this.LayerDepth = layerDepth;

            this.OnDoubleClick = new Action<object>(x => { });
            this.OnLeftClicked = new Action<object>(x => { });
            this.OnLeftReleased = new Action<object>(x => { });
            this.OnMouseEnter = new Action<object>(x => { });
            this.OnMouseLeave = new Action<object>(x => { });
            this.OnHover = new Action<object>(x => { });

            this.IsActive = IsActive;
            this.IsDraggable = IsDraggable;
            this._cursorType = CursorType;

            this.MarginLeft = MarginLeft;
            this.MarginRight = MarginRight;
            this.MarginTop = MarginTop;
            this.MarginBottom = MarginBottom;
            this._opacity = opacity;
        }

        public virtual UIElement GetDraggable()
        {
            if (this.IsDraggable)
                return this;
            else
                return null;
        }

        /// <summary>
        /// Swaps the attributes of this element with another element. 
        /// NB: The parent container handles the swapping of indices in its array of children
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual bool SwapWith(UIElement e)
        {
            Vector2 tempPos = this.Position;
            Vector2 tempOrigPos = this.OriginalPosition;

            this.Position = e.Position;
            this.OriginalPosition = e.OriginalPosition;
            e.Position = tempPos;
            e.OriginalPosition = tempOrigPos;

            return true; //Swapping of positions always successful
        }

        public virtual void Update()
        {
            if (IsActive)
            {
                if (IsHeld)
                    Position = new Vector2(InputManager.MousePositionP.X + UI.DragOffset.X, InputManager.MousePositionP.Y + UI.DragOffset.Y);

                WasMouseOver = IsMouseOver;

                IsMouseOver = InputManager.MousePositionP.X > this.Position.X
                && InputManager.MousePositionP.X < this.Position.X + Width
                && InputManager.MousePositionP.Y > this.Position.Y
                && InputManager.MousePositionP.Y < this.Position.Y + Height;

                if (IsMouseOver)
                {
                    if (!WasMouseOver)
                        OnMouseEnter(this);

                    _hoverTicks++;
                    if (InputManager.DoubleLeftClicked)
                        OnDoubleClick(this);
                    if (InputManager.JustLeftClicked)
                        OnLeftClicked(this);
                    else if (!InputManager.LeftClicking)
                    {
                        if (IsHovering)
                            OnHover(this);
                        UI.CursorMode = _cursorType;
                    }

                    if (InputManager.JustLeftReleased)
                        OnLeftReleased(this);
                }
                else
                {
                    if (WasMouseOver)
                        OnMouseLeave(this);
                    _hoverTicks = 0;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, float extraLayerDepth = 0)
        {
            if (IsActive && Texture != null)
                spriteBatch.Draw(Texture, Position, SrcRect, Colour * Opacity, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth + extraLayerDepth);
        }
    }
}

