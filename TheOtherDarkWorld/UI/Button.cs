using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class Button : UIContainer
    {
        private Color _clickColour;
        private Color _originalColour;
        public Action<object> OnPressed;
        protected bool _startedClickHere;

        public Button(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = true, bool CentreVertical = true,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            string Text = "", int FontSize = 2)
            : base(Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType)
        {
            AddElement(new TextSprite(Text, FontSize, Color.White, CursorType));

            this.OnPressed = new Action<object>(x => { });

            this.OnLeftClicked += (x) =>
            {
                _startedClickHere = true;
            };
            this.OnLeftReleased += (x) =>
            {
                if (_startedClickHere)
                {
                    this.OnPressed(this);
                }
            };
        }

        public Button(Color Colour, Color TextColour, Color HighlightColour, Color ClickColour, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
           int Width = UI_AUTO, int Height = UI_AUTO,
           bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
           float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
           float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
           bool CentreHorizontal = true, bool CentreVertical = true,
           DragAndDropType DragAndDropType = DragAndDropType.None,
           string Text = "", int FontSize = 2)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType)
        {
            _originalColour = HighlightColour;
            _clickColour = ClickColour;

            this.OnPressed = new Action<object>(x => { });

            this.OnLeftClicked += (x) =>
            {
                _startedClickHere = true;
                (x as Button)._highlightColour = _clickColour;
            };
            this.OnLeftReleased += (x) =>
            {
                if (_startedClickHere)
                {
                    this.OnPressed(this);
                }
            };

            if (Text == null)
                System.Diagnostics.Debugger.Break();
            else
                AddElement(new TextSprite(Text, FontSize, TextColour, 0, CursorType: CursorType));
        }


        public override void Update()
        {
            base.Update();
            if (IsActive)
            {
                if (!InputManager.LeftClicking)
                {
                    _highlightColour = _originalColour;
                    _startedClickHere = false;
                }
            }
        }
    }
}
