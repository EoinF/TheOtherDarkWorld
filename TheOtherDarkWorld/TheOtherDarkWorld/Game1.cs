using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TheOtherDarkWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }



        protected override void LoadContent()
        {
            Textures t = new Textures(Content);
            Textures.LoadTextures();
            Window.Title = "The Other Dark World";
            graphics.PreferredBackBufferWidth = UI.ScreenX = 800;
            graphics.PreferredBackBufferHeight = UI.ScreenY = 600;
            graphics.ApplyChanges();

            GameData.LoadGameData("GameContent.xml");

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        protected override void Initialize()
        {
            base.Initialize();
            StateManager.State = 1;
        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            InputManager.Update();

            if (Level.CurrentLevel != null)
                Level.CurrentLevel.Update();

            if (Player.PlayerList != null)
                Player.PlayerList[0].Update();

            if (Projectile.ProjectileList != null)
                for (int i = 0; i < Projectile.ProjectileList.Count; i++)
                {
                    if (Projectile.ProjectileList[i].Update()) //returns true if the projectile was destroyed
                    {
                        Projectile.ProjectileList.RemoveAt(i);
                        i--;
                    }
                }
            UI.Update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            if (Level.CurrentLevel != null)
            {
                Level.CurrentLevel.Draw(spriteBatch);
                Player.DrawAll(spriteBatch);
                Projectile.DrawAll(spriteBatch);
                if (UI.Tooltip != null)
                    UI.Tooltip.Draw(spriteBatch);
                UI.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
