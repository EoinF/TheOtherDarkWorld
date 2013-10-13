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
using TheOtherDarkWorld.GameObjects;

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

            //Fix the screen width so that it doesnt include the side panel
            UI.ScreenX -= Textures.SidePanel.Width;

            Window.Title = "The Other Dark World";

            GameData.LoadGameData("GameContent.xml");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            InputManager.Update();

            if (StateManager.State == 1)
            {
                Level.CurrentLevel.Update();

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
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Level.CurrentLevel.Players != null && Level.CurrentLevel.Players[Level.CurrentLevel.PlayerIndex].IsBlinded)
                GraphicsDevice.Clear(Color.White);
            else
                GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            if (StateManager.State == 0)
            {
                string prompt = "Press Enter to Begin...";
                spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(graphics.PreferredBackBufferWidth / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White);

            }
            if (StateManager.State == 1)
            {
                string prompt = "Press Enter to Retry...";
                if (!Level.CurrentLevel.Players[0].IsAlive)
                    spriteBatch.DrawString(Textures.Fonts[2], prompt, new Vector2(UI.ScreenX / 2, UI.ScreenY / 2) - (Textures.Fonts[2].MeasureString(prompt) / 2f), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                DrawGameObjects();
                DrawHUDObjects();
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameObjects()
        {
            Level.CurrentLevel.Draw(spriteBatch);
            Projectile.DrawAll(spriteBatch);
            //Level.CurrentLevel.DrawPlayers(spriteBatch);
        }

        private void DrawHUDObjects()
        {
            UI.DrawTooltip(spriteBatch);
            UI.Draw(spriteBatch);
            UI.DrawHUD(spriteBatch, Level.CurrentLevel.Players[Level.CurrentLevel.PlayerIndex]);
        }
    }
}
