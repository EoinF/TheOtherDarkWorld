using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class MessageBox : Button
    {
        private const int LIFETIME_DEFAULT = 300;
        private int lifetime;
        private int FontSize;
        private Color FontColour;
        
        public MessageBox(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            bool CentreHorizontal = true, bool CentreVertical = true,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            string Text = "", int FontSize = 2)
            : base(Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType, Text, FontSize)
        {
            this.FontColour = Color.White;
            this.FontSize = FontSize;
            this.OnPressed = new Action<object>(x => {lifetime = 1;});
        }

        public MessageBox(Color Colour, Color TextColour, Color HighlightColour, Color ClickColour, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
           int Width = UI_AUTO, int Height = UI_AUTO,
           bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
           float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
           float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
           bool CentreHorizontal = true, bool CentreVertical = true,
           DragAndDropType DragAndDropType = DragAndDropType.None,
           string Text = "", int FontSize = 2)
            : base(Colour, TextColour, HighlightColour, ClickColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType, Text, FontSize)
        {
            this.FontColour = TextColour;
            this.FontSize = FontSize;
            this.OnPressed = new Action<object>(x => {lifetime = 1;});
        }

        public override void Update()
        {
            lifetime--;
            if (lifetime <= 0)
            {
                IsActive = false;
            }
            base.Update();
        }

        public void SetMessage(string Text, int lifetime = LIFETIME_DEFAULT)
        {
            ClearElements();

            AddElement(new TextSprite(Text, FontSize , FontColour, this._cursorType));
            this.lifetime = lifetime;
            this.IsActive = true;
        }
    }
}
