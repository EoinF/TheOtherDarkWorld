using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class Button
    {
        public static List<Button> ButtonList { get; set; }

        public Vector2 Position { get; private set; }
        public Rectangle Rect { get; private set; }
        private byte _sfxCounter;
        public bool WasMouseClicking { get; set; }
        public TextSprite Text { get; set; }

        private int _texture;
        public Texture2D Texture { get { return Textures.MenuTextures[_texture]; } }


        public Button(Vector2 Position, TextSprite Text, int texture)
        {
            this.Position = Position;
            this.Text = Text;
            this._texture = texture;
        }

        private bool IsMouseOver()
        {
            return (InputManager.MousePositionP.X > Rect.Left
                && InputManager.MousePositionP.X < Rect.Right
                && InputManager.MousePositionP.Y > Rect.Top
                && InputManager.MousePositionP.Y < Rect.Bottom);
        }

        public void Activate()
        {

        }

        public void Update()
        {
            if (WasMouseClicking) //If the mouse was already clicking on the button
            {
                if (InputManager.JustLeftReleased)//and then the mouse was released
                    this.Activate(); //Then the button was activated
                _sfxCounter++;
            }
            else //If the button wasn't already being clicked
            {
                if (InputManager.JustLeftClicked)
                    WasMouseClicking = IsMouseOver();
                else
                    WasMouseClicking = false;
            }

            //If the mouse isn't being clicked now, then switch WasMouseClicking to false
            if (InputManager.JustLeftReleased)
                WasMouseClicking = false;

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }


    }
}
