using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TheOtherDarkWorld
{
    public class TextSprite : UIElement
    {
        private int _spritefont;
        public override int Height { get { return (int)(Lines * Spritefont.MeasureString("ABCabc").Y) + 1; } }
        public string Text;
        public int LineWidth;
        public int Lines {get; set;}

        /// <summary>
        /// Creates an object that displays text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Font">Tiny = 0, Small = 1, Medium = 2, Large = 3</param>
        /// <param name="colour"></param>
        /// <param name="LineWidth"></param>
        /// <param name="position"></param>
        public TextSprite(string text, int FontSize, Color Colour, int LineWidth, Vector2 Position = new Vector2(),
            CursorType CursorType = UI.CURSOR_DEFAULT, bool IsActive = true,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT)
            :base(null, Position, Colour, Colour, null, LineWidth, UI_AUTO, IsActive, false, CursorType, MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
        {
            this._spritefont = FontSize;
            this.LineWidth = LineWidth;
            this.Lines = 0;

            if (LineWidth > 0)
            {
                int index = 0;
                string[] Words = text.Split(' ');
                Text = "";
                int LongestLine = 0;

                while (index < Words.Length)
                {
                    StringBuilder current = new StringBuilder();

                    float nextTokenLength = Spritefont.MeasureString(Words[index]).X;

                    current.Append(Words[index++]);
                    float currentLineLength = nextTokenLength;

                    if (index < Words.Length)
                    {
                        nextTokenLength = Spritefont.MeasureString(Words[index] + ' ').X;

                        //
                        //Keep appending words until they don't fit on the line anymore
                        //
                        while (currentLineLength + nextTokenLength < LineWidth)
                        {
                            currentLineLength += nextTokenLength;

                            current.Append(" ");
                            current.Append(Words[index++]);

                            if (index < Words.Length)
                            {
                                nextTokenLength = Spritefont.MeasureString(Words[index] + ' ').X;
                            }
                            else
                                break;
                        }
                    }

                    if (currentLineLength > LongestLine)
                        LongestLine = (int)currentLineLength;

                    Text += current.ToString();
                    if (index >= Words.Length && Spritefont.MeasureString(current).X <= LineWidth)
                    {
                        //If we ran out of words
                    }
                    else
                    {
                        //Only add a new line if there are words still to be added
                        Text += "\n";
                    }
                    Lines++;
                }
                this._width = LongestLine;
            }
            else
            {
                this._width = (int)Spritefont.MeasureString(text).X;
                this.Lines = 1;
                this.Text = text;
            }
        }


        /// <summary>
        /// This constructor is only used when the textsprite is associated with a button. The button will handle its position
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Font">Tiny = 0, Small = 1, Medium = 2, Large = 3 </param>
        /// <param name="colour"></param>
        /// <param name="LineWidth"></param>
        /// <param name="position"></param>
        public TextSprite(string text, int FontSize, Color Colour, 
            CursorType CursorType = UI.CURSOR_DEFAULT, bool IsActive = true,
            float MarginLeft = UI_AUTO, float MarginRight = UI_AUTO, float MarginTop = UI_AUTO, float MarginBottom = UI_AUTO,
            float opacity = UI_INHERIT, float layerDepth = UI_INHERIT)
            :base(null, Vector2.Zero, Colour, Colour, null, UI_AUTO, UI_AUTO, IsActive, false, CursorType,MarginLeft, MarginRight, MarginTop, MarginBottom, opacity, layerDepth)
        {
            this.Text = text;
            this._spritefont = FontSize;
            this._width = (int)Spritefont.MeasureString(text).X;
            this.LineWidth = _width;
            this.Lines = 1;
        }

        public override void Draw(SpriteBatch spriteBatch, float extraLayerDepth = 0)
        {
            spriteBatch.DrawString(Spritefont, Text, Position, Colour * Opacity, 0, Vector2.Zero, 1, 0, LayerDepth + extraLayerDepth);
        }

        public SpriteFont Spritefont
        {
            get
            {
                return Textures.Fonts[this._spritefont];
            }
        }
    }
}
