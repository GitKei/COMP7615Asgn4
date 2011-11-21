using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Part3
{
    /// <summary>
    /// Class to encapsulate console functionality.
    /// </summary>
    class Console
    {
        public bool _consoleActive = false;
        List<string> _history;
        string _commandLine;
        GraphicsDevice _gd;
        SpriteFont _sf;

        KeyboardState _oldKeys;

        /// <summary>
        /// Constructor; initializes the console.
        /// </summary>
        /// <param name="gd">The graphics device that the console will be drawn to.</param>
        /// <param name="sf">The text font to use in the console.</param>
        public Console(GraphicsDevice gd, SpriteFont sf)
        {
            _history = new List<string>();
            _commandLine = "pong> ";
            _gd = gd;
            _sf = sf;
        }

        /// <summary>
        /// Call this method when input is received to have the console react to the user's commands.
        /// </summary>
        /// <param name="kbs">The current keyboard state.</param>
        /// <returns>The Command that corresponds to the user's input.</returns>
        public Command CheckInput(KeyboardState kbs)
        {
            Command result = null;

            // Handle console input
            if (_consoleActive)
            {
                foreach (Keys k in kbs.GetPressedKeys())
                {
                    if (_oldKeys.IsKeyUp(k))
                    {
                        switch (k.ToString())
                        {
                            case "Escape": // Close the console
                                _consoleActive = false;
                                break;
                            case "Back": // Backspace
                                if (_commandLine.Length > 6)
                                    _commandLine = _commandLine.Substring(0, _commandLine.Length - 1);
                                break;
                            case "Enter": // Execute command
                                // Only allow 4 history lines
                                string command = _commandLine.Substring(6, _commandLine.Length - 6).ToLower();
                                if (_history.Count == 8)
                                    _history.RemoveRange(0, 2);

                                _history.Add(_commandLine);
                                switch (command)
                                {
                                    case "bg blue": // Change background to blue
                                        _history.Add("Command executed.");
                                        result = new Command(Command.Type.BG);
                                        result._color = new Color(0f, 0f, 0.5f);
                                        break;
                                    case "bg black": // Change background to black
                                        _history.Add("Command executed.");
                                        result = new Command(Command.Type.BG);
                                        result._color = new Color(0f, 0f, 0f);
                                        break;
                                    case "bg green": // Change background to green
                                        _history.Add("Command executed.");
                                        result = new Command(Command.Type.BG);
                                        result._color = new Color(0f, 0.5f, 0f);
                                        break;
                                    case "cheat p1": // Add 100 to p1's score
                                        _history.Add("Command executed.");
                                        result = new Command(Command.Type.CHEAT);
                                        result._player = Defs.Player.P1;
                                        break;
                                    case "cheat p2": // Add 100 to p2's score
                                        _history.Add("Command executed.");
                                        result = new Command(Command.Type.CHEAT);
                                        result._player = Defs.Player.P2;
                                        break;
                                    default: // No idea what the user wants
                                        _history.Add("Bad command.");
                                        break;
                                }
                                _commandLine = "pong> ";
                                break;
                            case "Space": // Space character
                                _commandLine += " ";
                                break;
                            case "D0": // Numeric entry is rather awkward
                                _commandLine += "0";
                                break;
                            case "D1":
                                _commandLine += "1";
                                break;
                            case "D2":
                                _commandLine += "2";
                                break;
                            case "D3":
                                _commandLine += "3";
                                break;
                            case "D4":
                                _commandLine += "4";
                                break;
                            case "D5":
                                _commandLine += "5";
                                break;
                            case "D6":
                                _commandLine += "6";
                                break;
                            case "D7":
                                _commandLine += "7";
                                break;
                            case "D8":
                                _commandLine += "8";
                                break;
                            case "D9":
                                _commandLine += "9";
                                break;
                            default:
                                _commandLine += k.ToString().ToLower();
                                break;
                        }
                    }
                }
            }
            else
            {
                // Should we bring the console up?
                if (kbs.IsKeyDown(Keys.C) && _oldKeys.IsKeyUp(Keys.C))
                {
                    _consoleActive = true;
                    _oldKeys = kbs;
                    return result;
                }

                // Does the user want to quit?
                if (kbs.IsKeyDown(Keys.Escape) && _oldKeys.IsKeyUp(Keys.Escape))
                    return new Command(Command.Type.QUIT);
            }

            _oldKeys = kbs;

            return result;
        }

        /// <summary>
        /// Draws the console to the screen.
        /// </summary>
        /// <param name="scrWidth">The current viewport width.</param>
        /// <param name="scrHeight">The current viewport height.</param>
        /// <param name="sb">The spritebatch to add draw commands to.</param>
        public void Draw(int scrWidth, int scrHeight, SpriteBatch sb)
        {
            Rectangle cRect = new Rectangle(10, scrHeight / 2 + 10, scrWidth - 20, scrHeight / 2 - 20);
            Texture2D tex = new Texture2D(_gd, cRect.Width, cRect.Height);
            int texSize = tex.Width * tex.Height;
            Color[] buff = new Color[texSize];

            // Make the console grey-ish.
            for (int i = 0; i < texSize; ++i)
            {
                buff[i].R = 80;
                buff[i].G = 80;
                buff[i].B = 80;
                buff[i].A = 255;
            }
            tex.SetData(buff);

            // Make the console semi-transparent.
            Color consColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            sb.Draw(tex, cRect, consColor);

            // Draw the current command and commands from the history.
            int j = 0;
            foreach (string s in _history)
            {
                sb.DrawString(_sf, _history[j], new Vector2(10f, j * 25 + cRect.Y), consColor);
                ++j;
            }

            sb.DrawString(_sf, _commandLine, new Vector2(10f, 200 + cRect.Y), new Color(0.7f, 0.7f, 0.7f, 0.7f));
        }
    }
}