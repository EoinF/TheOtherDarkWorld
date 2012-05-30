using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheOtherDarkWorld
{
    public static partial class InputManager
    {
        public static KeyboardState[] keyboardState = new KeyboardState[15];
        public const int DoubleClickResponsiveness = 15; //Note: This must be less than or equal to the number of mouse states that are stored
        public static MouseState[] mouseState = new MouseState[15];
        public static bool MousePreviouslyReleased
        {
            get
            {
                return mouseState[1].LeftButton == ButtonState.Released
                    || mouseState[2].LeftButton == ButtonState.Released
                    || mouseState[3].LeftButton == ButtonState.Released;
            }
        }
        public static bool JustLeftClicked;
        public static bool JustLeftClicking;
        public static bool DoubleLeftClicked;
        public static bool CursorMode;

        public static void Update()
        {
            UpdateStates();
            CheckPlayerInput();

            if (keyboardState[0].IsKeyDown(Keys.Enter))
                StartLevel();
        }


        private static void CheckPlayerInput()
        {
            if (Level.CurrentLevel == null)
                return;

            if (keyboardState[0].IsKeyDown(Keys.W))
            {
                Player.PlayerList[0].Velocity -= new Vector2(0, 2);
            }
            if (keyboardState[0].IsKeyDown(Keys.S))
            {
                Player.PlayerList[0].Velocity += new Vector2(0, 2);
            }
            if (keyboardState[0].IsKeyDown(Keys.A))
            {
                Player.PlayerList[0].Velocity -= new Vector2(2, 0);
            }
            if (keyboardState[0].IsKeyDown(Keys.D))
            {
                Player.PlayerList[0].Velocity += new Vector2(2, 0);
            }

            if (keyboardState[0].IsKeyUp(Keys.Left) && keyboardState[1].IsKeyDown(Keys.Left))
            {
                Level.GenerateLevel(Level.LevelType.Hallways, -1, Level.CurrentLevel.Width, Level.CurrentLevel.Height);
            }
            if (keyboardState[0].IsKeyUp(Keys.Right) && keyboardState[1].IsKeyDown(Keys.Right))
            {
                Level.GenerateLevel(Level.LevelType.Hallways, 1, Level.CurrentLevel.Width, Level.CurrentLevel.Height);
            }


            if (keyboardState[0].IsKeyDown(Keys.R))
            {
                if (Player.PlayerList[0].Inventory[0].GetType() == typeof(Weapon))
                {
                    Player.PlayerList[0].Reload();
                }
            }


            if (mouseState[0].LeftButton == ButtonState.Pressed && !CursorMode)
                Player.PlayerList[0].Activate_Primary();
            if (mouseState[0].RightButton == ButtonState.Pressed && !CursorMode)
                Player.PlayerList[0].Activate_Secondary();

        }

        public static void StartLevel()
        {
            Random rand = new Random();
            Level.GenerateLevel(Level.LevelType.Hallways, rand.Next(), 400, 200);

            Player.PlayerList = new Player[1] { new Player(new Vector2(170, 200), 6, 2, new Weapon(0), new Weapon(1), Color.LightBlue, Vector2.Zero, 0) };
            Player.PlayerList[0].Inventory[2] = new Item(100, 15);
            Player.PlayerList[0].Inventory[3] = new Item(101, -1);
            Projectile.ProjectileList = new List<Projectile>();
            
            Vector2 offset = new Vector2(800 - Textures.SidePanel.Width, 0);

            UI.HUDText = new List<TextSprite>();
            UI.HUDText.Add(new TextSprite("Items", 1, Color.Violet, -1, offset + new Vector2(14, 145)));
            UI.HUDText.Add(new TextSprite("Health", 1, Color.Violet, -1, offset + new Vector2(11, 10)));


            UI.Inventory = new List<UIElement>();
            UI.Inventory.Add(new UIElement(offset + new Vector2(10, 100), 0, 0));
            UI.Inventory.Add(new UIElement(offset + new Vector2(35, 100), 0, 0));

            //Starts at 2, because it skips over the two equipped items
            for (int i = 0; i < Player.PlayerList[0].Inventory.Length - 2; i++)
            {
                UI.Inventory.Add(new UIElement(offset + new Vector2(7 + (i % 2) * 30, 160 + (int)(i / 2) * 40), 0, 0));
            }
        }

        private static void UpdateStates()
        {
            for (int i = mouseState.Length - 1; i > 0; i--)
            {
                mouseState[i] = mouseState[i - 1];
                keyboardState[i] = keyboardState[i - 1];
            }
            mouseState[0] = Mouse.GetState();
            keyboardState[0] = Keyboard.GetState();

            //Next, update all the status booleans. This means it's only done once per frame

            JustLeftClicked = mouseState[0].LeftButton == ButtonState.Released && mouseState[1].LeftButton == ButtonState.Pressed;
            JustLeftClicking = mouseState[0].LeftButton == ButtonState.Pressed && mouseState[1].LeftButton == ButtonState.Released;
            DoubleLeftClicked = CheckDoubleLeftClick();

            //Only allow the mode to change so that an item isn't activated
            //when the mouse goes into a non cursor mode area of the screen
            if (mouseState[0].LeftButton == ButtonState.Released)
                CursorMode = false;

            //
            //Next, perform actions based on what state the game is in
            //
            if (StateManager.State == 0) //Main Menu
            {

            }
            else if (StateManager.State == 1) //In Game
            {
                if (mouseState[0].X > (800 - Textures.SidePanel.Width))
                {
                    //Only allow the mode to change so that an item isn't activated
                    //when the mouse goes into a non cursor mode area of the screen
                    if (mouseState[0].LeftButton == ButtonState.Released)
                    {
                        CursorMode = true;
                    }
                }
                InventoryInput();
            }
            else if (StateManager.State == 2) //Pause Menu
            {
                CursorMode = true;
            }

        }

        /// <summary>
        /// Checks if there's a gap between two clicks.
        /// The clicks have to have a mouseReleased period before them
        /// </summary>
        /// <Returns></Returns>
        private static bool CheckDoubleLeftClick()
        {
            bool WaitingForClick = true;
            int count = 0; //Counts the number of clicks

            for (int i = 0; i < DoubleClickResponsiveness; i++)
            {
                if (WaitingForClick && mouseState[i].LeftButton == ButtonState.Pressed)
                {
                    count++;
                    WaitingForClick = false;
                }
                if (!WaitingForClick && mouseState[i].LeftButton == ButtonState.Released)
                {
                    count++;
                    WaitingForClick = true;
                }
            }

            return (count >= 4);
            //i.e. There was a click, then a rest after the previous click, then the previous click, and a rest before that click too.
        }

    }
}
