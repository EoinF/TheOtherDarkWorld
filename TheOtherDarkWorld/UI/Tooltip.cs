using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld
{
    public class Tooltip : UIElement
    {
        public TextSprite Text;
        public TextSprite Header;
        public int Timeout;
        public static Color TooltipColour = new Color(1, 1, 1, 0.8f);


        public Tooltip(Vector2 Position, string Header, string Text)
            : base(Position, -1, 1)
        {
            this.Header = new TextSprite(Header, 2, Color.SteelBlue, Textures.UITextures[1].Width - 20, Position + new Vector2(10, 20));
            this.Text = new TextSprite(Text, 1, Color.Violet, Textures.UITextures[1].Width - 20, Position + new Vector2(10, 50));
            this.Timeout = 50;
        }

        public void Update()
        {
            if (!this.IsMouseOver || Timeout < 120)
                Timeout--;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.UITextures[1], Position, null, TooltipColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            Header.Draw(spriteBatch);
            Text.Draw(spriteBatch);
        }
    }
}
