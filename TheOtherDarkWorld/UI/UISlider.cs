using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class UISlider : UIContainer
    {
        public Action<object> OnSliderChanged;
        UIElement SliderPiece { get { return Children[0]; } }
        public int SliderPosition { get; private set; }


        public override int Height
        {
            get
            {
                if (_height == UI_AUTO) //Autosize
                {
                    int biggestHeight = 0;
                    if (Texture != null)
                        biggestHeight = Texture.Width;
                    if (SliderPiece != null)
                    {
                        biggestHeight = SliderPiece.Height;
                    }
                    return biggestHeight;
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

        public UISlider(Texture2D SliderPieceTexture, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT)
            : base(Texture, Position, SrcRect, Width, Height, IsActive, false, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, true, true, DragAndDropType.DropElement)
        {
            var sliderPiece = new UISliderPiece(SliderPieceTexture, Color.White, Color.White, IsDraggable: true, CursorType: CursorType.Cursor);
            AddElement(sliderPiece);

            SliderPosition = 0;
            OnSliderChanged += UpdateSliderPosition;
        }

        public UISlider(Color Colour, Color HighlightColour, Texture2D SliderPieceTexture, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
           int Width = UI_AUTO, int Height = UI_AUTO,
           bool IsActive = true, CursorType CursorType = UI.CURSOR_DEFAULT,
           float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
           float opacity = UI_INHERIT, float layerDepth = UI_INHERIT)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, false, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, true, true, DragAndDropType.DropElement)
        {
            var sliderPiece = new UISliderPiece(SliderPieceTexture, Colour, HighlightColour, IsDraggable: true, CursorType: CursorType.Cursor);
            AddElement(sliderPiece);

            SliderPosition = 0;
            OnSliderChanged += UpdateSliderPosition;
        }

        private void UpdateSliderPosition(object sender)
        {
            SliderPosition = (int)(((SliderPiece.Position.X - this.Position.X) / this.Width) * 100);
        }


        private class UISliderPiece : UIElement
        {
            public UISliderPiece(Texture2D Texture, Color Colour, Color HighlightColour, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, CursorType CursorType = UI.CURSOR_DEFAULT,
            bool IsDraggable = true,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT)
                : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity)
            {
            }

            public override void Update()
            {
                base.Update();
                if (this.IsHeld)
                {
                    if (Position.X < Parent.Position.X)
                        Position = new Vector2(Parent.Position.X, Position.Y);
                    else if (Position.X > Parent.Position.X + Parent.Width - this.Width)
                        Position = new Vector2(Parent.Position.X + Parent.Width - this.Width, Position.Y);

                    if (Position.Y < Parent.Position.Y)
                        Position = new Vector2(this.Position.X, Parent.Position.Y);
                    else if (Position.Y > Parent.Position.Y + Parent.Height - this.Height)
                        Position = new Vector2(this.Position.X, Parent.Position.Y + Parent.Height - this.Height);
                    (Parent as UISlider).OnSliderChanged(this.Parent);
                }
            }


        }
    }
}
