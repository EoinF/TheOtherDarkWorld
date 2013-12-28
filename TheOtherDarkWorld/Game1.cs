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
            Textures.LoadTextures(Content, graphics.GraphicsDevice);

            graphics.PreferredBackBufferWidth = UI.ScreenX = 800;
            graphics.PreferredBackBufferHeight = UI.ScreenY = 600;
            graphics.ApplyChanges();

            Window.Title = "The Other Dark World";

            GameData.LoadGameData("GameContent.xml");
            UI.InitializeHUD();

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        protected override void Initialize()
        {
            base.Initialize();
            StateManager.State = 0;
        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            //try
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                InputManager.Update();
                StateManager.Update();
                CommandManager.Update();
                UI.Update();

                base.Update(gameTime);
            }
            try { }
            catch (Exception ex)
            {
                DebugManager.WriteError(ex);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            StateManager.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        
            try { }
            catch (Exception ex)
            {
                DebugManager.WriteError(ex);
            }
        }
    }
}
