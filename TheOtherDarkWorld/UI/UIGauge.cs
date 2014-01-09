using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class UIGauge : UIContainer
    {
        private float Limit
        {
            get {return GetLimitFunction();}
        }
        private float Current
        {
            get { return GetValueFunction();}
        }

        private UIGaugeSegment GaugeSegment;
        private Func<int> GetValueFunction;
        private Func<int> GetLimitFunction;

        public UIGauge(Color Colour, Color HighlightColour, Color ColourMin, Color ColourMax,
            Func<int> GetValueFunction, Func<int> GetLimitFunction,
            Texture2D Texture = null, Texture2D SegmentTexture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None)
            : base(Colour, HighlightColour, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType)
        {
            this.GetValueFunction = GetValueFunction;
            this.GetLimitFunction = GetLimitFunction;
            GaugeSegment = new UIGaugeSegment(ColourMin, ColourMax, SegmentTexture, MarginLeft: 1, MarginTop: 1);
            AddElement(GaugeSegment);
        }

        public UIGauge(Func<int> GetValueFunction, Func<int> GetLimitFunction,
            Texture2D Texture = null, Texture2D SegmentTexture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None)
            : base(Color.White, Color.White, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth, CentreHorizontal, CentreVertical, DragAndDropType)
        {
            this.GetValueFunction = GetValueFunction;
            this.GetLimitFunction = GetLimitFunction;
            GaugeSegment = new UIGaugeSegment(Color.Black, Color.White, SegmentTexture, MarginLeft: 1, MarginTop: 1);
            AddElement(GaugeSegment);
        }

        public override void Update()
        {
            GaugeSegment.SrcRect = new Rectangle(0, 0, ((int)((Current / Limit) * Width) - 2), Height - 2);
            base.Update();
        }

        private class UIGaugeSegment : UIElement
        {
            public UIGaugeSegment(Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
                : base(Color.White, Color.White, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
            {
            }

            public UIGaugeSegment(Color ColourMin, Color ColourMax, Texture2D Texture = null, Vector2 Position = new Vector2(), Rectangle? SrcRect = null,
            int Width = UI_AUTO, int Height = UI_AUTO,
            bool IsActive = true, bool IsDraggable = false, CursorType CursorType = UI.CURSOR_DEFAULT,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT,
            bool CentreHorizontal = false, bool CentreVertical = false,
            DragAndDropType DragAndDropType = DragAndDropType.None,
            object[] DataBinding = null)
                : base(ColourMin, ColourMax, Texture, Position, SrcRect, Width, Height, IsActive, IsDraggable, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
            {
            }

            public override Color Colour
            {
                get { return Color.Lerp(_colour, _highlightColour, (Parent as UIGauge).Current / (Parent as UIGauge).Limit); }
            }
        }
    }
}
