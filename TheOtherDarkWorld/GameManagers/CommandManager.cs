using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheOtherDarkWorld
{
    public class CommandManager
    {
        private static readonly Vector2 COMMAND_POSITION_DEFAULT = new Vector2(10, UI.ScreenY - 50);
        private const int inputRepeatDelay = 20;
        private static TextSprite CommandInput;
        private static UIContainer CommandInputContainer;
        public static bool IsActive;

        static CommandManager()
        {
            CommandInputContainer = new UIContainer(Textures.TextInputBar, COMMAND_POSITION_DEFAULT, CentreVertical: true, IsActive: false);
            CommandInput = new TextSprite("", 1, Color.Black, MarginLeft: 3);
            CommandInputContainer.AddElement(CommandInput);
            UI.HUDElements.AddElement(CommandInputContainer);
        }

        public static void Parse(string command)
        {
            int currentPos = 0;

            switch (NextToken(ref currentPos, command))
            {
                case "start":
                    start();
                    break;
                case "give":
                    give(NextToken(ref currentPos, command), NextToken(ref currentPos, command));
                    break;
                case "set":
                    set(NextToken(ref currentPos, command));
                    break;
                case "help":
                    help(NextToken(ref currentPos, command));
                    break;
            }
        }

        private static void start()
        {
            StateManager.StartLevel();
        }

        private const string STR_AMOUNT_DEFAULT = "1";
        private static void give(string str_type, string str_amount)
        {
            if (str_amount == "") //2nd parameter is optional
                str_amount = STR_AMOUNT_DEFAULT;

            int type, amount;
            if (int.TryParse(str_type, out type) &&
                int.TryParse(str_amount, out amount))
            {
                if (StateManager.State == GameState.InGame)
                {
                    if (GameData.GameItems[type] != null)
                    {
                        if (StateManager.CurrentPlayer.PickUpItem(new GameObjects.Item(type, amount)))
                        {
                            UI.QueueMessage("Player got " + amount + " of " + GameData.GameItems[type].Name);
                        }
                    }
                    else
                        UI.QueueMessage("Item type " + type + " is not defined");    
                }
                else
                {
                    UI.QueueMessage("There is no player to give the item to!");
                }
            }
            else
            {
                if (str_type == "")
                {
                    UI.QueueMessage("Please specify the type!");
                    UI.QueueMessage("Type \"help give\" for info");
                }
            }
        }

        private static void set(string setting)
        {
            switch (setting)
            {
                case "fullbright":
                    GameObjects.GameObject.FULL_BRIGHT = !GameObjects.GameObject.FULL_BRIGHT;
                    break;
                case "fullvision":
                    GameObjects.GameObject.FULL_VISION = !GameObjects.GameObject.FULL_VISION;
                    break;
                default:
                    UI.QueueMessage("There is no setting: " + setting);
                    UI.QueueMessage("Type \"help set\" for info");
                    break;
            }
        }

        private static void help(string parameter)
        {
            switch (parameter)
            {
                case "start":
                    UI.QueueMessage("usage: start [seed]");
                    break;
                case "give":
                    UI.QueueMessage("usage: give <type> [amount]");
                    break;
                case "set":
                    UI.QueueMessage("usage: set <parameter>");
                    break;
                default:
                    UI.QueueMessage("Commands: start, give, set, help");
                    break;
            }
        }

        private static string NextToken(ref int currentPosition, string input)
        {
            StringBuilder result = new StringBuilder();

            //
            //Firstly, skip all the white space
            //
            while (currentPosition != input.Length
                && (input[currentPosition] == ' ' || input[currentPosition] == '\t'))
            {
                currentPosition++;
            }
            
            //
            //Next, get all the characters until the next white space is found
            //
            while (currentPosition != input.Length
                && input[currentPosition] != ' ' && input[currentPosition] != '\t')
            {
                result.Append(input[currentPosition++]);
            }

            return result.ToString();
        }

        public static void Update()
        {
            if (InputManager.JustPressed(Keys.Enter))
            {
                if (IsActive)
                {
                    Parse(CommandInput.Text);
                    CommandInput.Text = "";
                }
                IsActive = !IsActive;
                CommandInputContainer.IsActive = IsActive;
            }

            if (IsActive)
            {
                Keys[] keys = InputManager.keyboardState[0].GetPressedKeys();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (InputManager.JustPressed(keys[i]))
                    {
                        string input = keys[i].ToString();
                        if (input.Length < 2) //Only accept single characters and not things like "LeftShift"
                        {
                            if (InputManager.keyboardState[0].IsKeyDown(Keys.LeftShift) || InputManager.keyboardState[0].IsKeyDown(Keys.RightShift))
                            {
                                CommandInput.Text += input.ToUpper();
                            }
                            else
                            {
                                CommandInput.Text += input.ToLower();
                            }
                        }
                        else if (input.Length == 2) //Check for number keys
                        {
                            if (input[0] == 'D')
                                CommandInput.Text += input[1];
                        }
                        else //Check for special characters
                        {
                            if (keys[i] == Keys.Space)
                            {
                                CommandInput.Text += ' ';
                            }
                        }
                        if (keys[i] == Keys.Back)
                        {
                            if (CommandInput.Text.Length > 0)
                                CommandInput.Text = CommandInput.Text.Remove(CommandInput.Text.Length - 1);
                        }
                    }
                }
            }
        }
    }
}

