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
        private bool _startedClickHere;

        public Button(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            string Text = "")
            : base(Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, true, true, DragAndDropType)
        {
            AddElement(new TextSprite(Text, 2, Color.White, CursorType));

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

        public Button(Color Colour, Color HighlightColour, Color ClickColour, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
           int Width = UI_AUTO, int Height = UI_AUTO,
           bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
           float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
           float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
           bool CentreHorizontal = false, bool CentreVertical = false,
           DragAndDropType DragAndDropType = DragAndDropType.None,
           string Text = "")
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, true, true, DragAndDropType)
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
                AddElement(new TextSprite(Text, 2, Colour, 0, CursorType: CursorType));
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
