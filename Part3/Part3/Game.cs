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

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;

namespace Part3
{
    /// <summary>
    /// Santana Mach and Steve Stanisic
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        // XNA related variables
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _sb;
        private SpriteFont _font;

        // Game variables
        Pong _pong;
        Color _bg;
        int _scrWidth, _scrHeight;
        Console _cs;
        Menu _menu;

        // Saving related variables
        StorageDevice _storage;
        IAsyncResult _result;

        /// <summary>
        /// Game constructor, set up graphics device and Content folder.
        /// </summary>
        public Game()
        {
            _gdm = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Initialization of non-graphic components. All we need this for is setting the
        /// window title.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Part 3";
            base.Initialize();
        }

        /// <summary>
        /// Load any game content here. We have a reasonable amount to set up now; we have
        /// our game related setup as well as our 3d world and effects.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _sb = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("SegoeUI");

            // We'll need to know our viewport height throughout
            _scrWidth = GraphicsDevice.Viewport.Width;
            _scrHeight = GraphicsDevice.Viewport.Height;

            _pong = new Pong(_scrWidth, _scrHeight);
            _pong.Init(Content);

            // Set up our console
            _cs = new Console(GraphicsDevice, _font);
            _menu = new Menu(GraphicsDevice, _font);
            _bg = Color.Black;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (_storage == null || !_storage.IsConnected)
                return;

            // Open a storage container.
            IAsyncResult result = _storage.BeginOpenContainer("Pong", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = _storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            Stream stream;
            StreamWriter sw;

            int highScore = (_pong._scoreP1 > _pong._scoreP2 ? _pong._scoreP1 : _pong._scoreP2);

            // Check to see whether the save exists.
            if (container.FileExists(filename))
            {
                stream = container.OpenFile(filename, FileMode.Open);
                List<string> old = _menu.ParseScores(stream);
                stream.Close();

                int numScores = 0;

                container.DeleteFile(filename);
                stream = container.CreateFile(filename);

                sw = new StreamWriter(stream);

                // Parse scores
                foreach (string s in old)
                {
                    char[] delim = { ' ' };
                    string[] sarray = s.Split(delim);
                    int prevHigh = int.Parse(sarray[1]);

                    if (numScores == 7)
                        break;

                    if (highScore > prevHigh)
                    {
                        if (_pong._scoreP1 > _pong._scoreP2)
                            sw.WriteLine("Player1: {0}", _pong._scoreP1.ToString());
                        else
                            sw.WriteLine("Player2: {0}", _pong._scoreP2.ToString());

                        ++numScores;
                        highScore = -1;
                    }

                    if (numScores == 7)
                        break;

                    sw.WriteLine(s);
                    ++numScores;
                }   
            }
            else
            {
                // New file, write high score
                stream = container.CreateFile(filename);
                sw = new StreamWriter(stream);

                if (_pong._scoreP1 > _pong._scoreP2)
                    sw.WriteLine("Player1: {0}", _pong._scoreP1.ToString());
                else
                    sw.WriteLine("Player2: {0}", _pong._scoreP2.ToString());
            }

            sw.Close();

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        private void LoadSaved()
        {
            // Open a storage container.
            IAsyncResult result = _storage.BeginOpenContainer("Pong", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = _storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            _menu.SetScores(stream);

            // Close the file.
            stream.Close();

            // Dispose the container.
            container.Dispose();
        }
        /// <summary>
        /// Update game logic here. This is getting complicated enough that we'll break
        /// input out into separate methods.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle input
            HandleGamePad();
            HandleKeyboard();

            _pong.Step(gameTime);

            if (_storage == null && _result == null)
            {
                // Set the request flag
                if (!Guide.IsVisible)
                {
                    _result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                }
            }

            // If a save is pending, save as soon as the
            // storage device is chosen
            if (_result != null && _result.IsCompleted)
            {
                _storage = StorageDevice.EndShowSelector(_result);
                LoadSaved();
                _result = null;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Fairly self explanatory, handle gamepad input.
        /// </summary>
        private void HandleGamePad()
        {
            GamePadState ps1 = GamePad.GetState(PlayerIndex.One);
            GamePadState ps2 = GamePad.GetState(PlayerIndex.Two);

            if (ps1.IsConnected && ps1.Buttons.Back == ButtonState.Pressed)
                    Exit();

            // Allow player 1 to start the ball moving
            if (ps1.Buttons.A == ButtonState.Pressed)
                _pong.StartGame();

            // Player 1 paddle movement
            if (ps1.ThumbSticks.Left.Y > 0f)
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.UP);
            if (ps1.ThumbSticks.Left.Y < 0f)
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.DOWN);

            // Player 2 paddle movement
            if (ps2.ThumbSticks.Left.Y > 0f)
                _pong.MovePaddle(Defs.Player.P2, Pong.Direction.UP);
            if (ps2.ThumbSticks.Left.Y < 0f)
                _pong.MovePaddle(Defs.Player.P2, Pong.Direction.DOWN);
        }

        /// <summary>
        /// Fairly self explanatory, handle keyboard input.
        /// </summary>
        private void HandleKeyboard()
        {
            KeyboardState kbState = Keyboard.GetState();

            // Handle console input first
            Command c = _cs.CheckInput(kbState);

            // Check for Commands
            if (c != null && c._type == Command.Type.QUIT)
                this.Exit();
            else if (c != null && c._type == Command.Type.BG)
                _bg = c._color;
            else if (c != null && c._type == Command.Type.CHEAT)
            {
                _pong.AddScore(c._player);
            }

            _menu.CheckInput(kbState);

            // Allow player 1 to start the ball moving
            if (kbState.IsKeyDown(Keys.Space))
                _pong.StartGame();

            // Player 1 paddle movement
            if (kbState.IsKeyDown(Keys.W))
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.UP);
            if (kbState.IsKeyDown(Keys.S))
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.DOWN); ;

            // Player 2 paddle movement
            if (kbState.IsKeyDown(Keys.Up))
                _pong.MovePaddle(Defs.Player.P2, Pong.Direction.UP);
            if (kbState.IsKeyDown(Keys.Down))
                _pong.MovePaddle(Defs.Player.P2, Pong.Direction.DOWN);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bg);

            _sb.Begin();

            _pong.Draw(_sb);

            // Draw scoreboard
            _sb.DrawString(_font, "Player 1: " + _pong._scoreP1.ToString(), new Vector2(_scrWidth / 2 - 300, 0), Color.White);
            _sb.DrawString(_font, "Player 2: " + _pong._scoreP2.ToString(), new Vector2(_scrWidth / 2 + 200, 0), Color.White);

            // Draw console
            _cs.Draw(_scrWidth, _scrHeight, _sb);
            _menu.Draw(_scrWidth, _scrHeight, _sb);
    
            _sb.End();

            base.Draw(gameTime);
        }
    }
}
