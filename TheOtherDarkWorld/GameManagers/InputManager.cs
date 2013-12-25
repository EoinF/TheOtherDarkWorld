using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public static partial class InputManager
    {
        public static KeyboardState[] keyboardState = new KeyboardState[15];
        public const int DoubleClickResponsiveness = 15; //Note: This must be less than or equal to the number of mouse states that are stored
        private static MouseState[] mouseState = new MouseState[15];

        public static Point MousePositionP
        {
            get { return new Point(InputManager.mouseState[0].X, InputManager.mouseState[0].Y); }
        }
        public static Vector2 MousePositionV
        {
            get { return new Vector2(InputManager.mouseState[0].X, InputManager.mouseState[0].Y); }
        }

        /// <summary>
        /// This is true AFTER the user has CLICKED
        /// </summary>
        public static bool JustLeftReleased;
        /// <summary>
        /// This is true at the very moment that the user is CLICKING
        /// </summary>
        public static bool JustLeftClicked;

        /// <summary>
        /// This is true at the very moment that the user is CLICKING
        /// </summary>
        public static bool JustRightClicked;

        public static bool DoubleLeftClicked;
        /// <summary>
        /// True if the left mouse button is being held down
        /// </summary>
        public static bool LeftClicking;
        /// <summary>
        /// True if the right mouse button is being held down
        /// </summary>
        public static bool RightClicking;

        /// <summary>
        /// A character value is passed in, to find out if a key was pressed
        /// </summary>
        public static bool[] keysPressed;


        public static void Update()
        {
            UpdateStates();

            if (keyboardState[0].IsKeyDown(Keys.Enter))
            {
                StateManager.StartLevel();

                if (UI.Kills > UI.HighScore)
                    UI.HighScore = UI.Kills;
                UI.Kills = 0;

                StateManager.State = 1;
            }
        }

        /// <summary>
        /// If the key was just pressed this frame only and it wasnt being held down since last frame
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool JustPressed(Keys key)
        {
            return keyboardState[0].IsKeyDown(key) && keyboardState[1].IsKeyUp(key);
        }

        /// <summary>
        /// If the key was just released this frame and was being held down since last frame
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool JustReleased(Keys key)
        {
            return keyboardState[1].IsKeyDown(key) && keyboardState[0].IsKeyUp(key);
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

            JustLeftReleased = mouseState[0].LeftButton == ButtonState.Released && mouseState[1].LeftButton == ButtonState.Pressed;
            JustLeftClicked = mouseState[0].LeftButton == ButtonState.Pressed && mouseState[1].LeftButton == ButtonState.Released;
            
            JustRightClicked = mouseState[0].RightButton == ButtonState.Pressed && mouseState[1].RightButton == ButtonState.Released;
            
            DoubleLeftClicked = CheckDoubleLeftClick();

            LeftClicking = (mouseState[0].LeftButton == ButtonState.Pressed);

            RightClicking = (mouseState[0].RightButton == ButtonState.Pressed);
                


            //
            //Next, perform actions based on what state the game is in
            //

            if (StateManager.State == 0) //Main Menu
            {

            }
            else if (StateManager.State == 1) //In Game
            {
                //InventoryInput();
            }
            else if (StateManager.State == 2) //Pause Menu
            {

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
            int count = 0; //Counts the number of changes in mouse state

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
