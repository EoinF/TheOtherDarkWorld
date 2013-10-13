using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TheOtherDarkWorld
{
        public class TextSprite
        {
            private int _spritefont;
            public Color Colour;
            public Vector2 Position;
            public string Text;
            public static List<TextSprite> TextList;
            public int LineWidth;

            public TextSprite(string text, int Font, Color colour, int LineWidth, Vector2 position)
            {
                this.Position = position;
                this.Text = text;
                this._spritefont = Font;
                this.Colour = colour;
                this.LineWidth = LineWidth;

                if (LineWidth > 0)
                {
                    int index = 0;
                    string[] Words = Text.Split(' ');
                    Text = ""; 

                    while (index < Words.Length)
                    {
                        StringBuilder current = new StringBuilder();

                        while (Spritefont.MeasureString(current).X < LineWidth && index < Words.Length)
                        {
                            current.Append(Words[index++]);
                            current.Append(" ");
                        }

                        if (index >= Words.Length && Spritefont.MeasureString(current).X <= LineWidth)
                        {
                            //If we ran out of words
                            Text += current.ToString();
                        }
                        else
                        {
                            //Takes all the words except the last one, because it didn't fit on the line with the last word included
                            index--;
                            Text += current.ToString().Substring(0, current.Length - (Words[index].Length + 1)); //1 extra needed to remove the space

                            //Only add a new line if there are words still to be added
                            Text += "\n";
                        }
                    }                    
                }
            }

            //This constructor is used when the textsprite is associated with a button. The button will handle its position
            public TextSprite(string text, int FontSize, Color colour, int LineWidth)
            {
                this.Position = Vector2.Zero;
                this.Text = text;
                this._spritefont = FontSize;
                this.Colour = colour;
                this.LineWidth = LineWidth;
            }

            public virtual void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(Spritefont, Text, Position, Colour, 0f, Vector2.Zero, 1f, 0, 0.9f);
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
