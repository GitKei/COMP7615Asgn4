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

namespace P1Pong
{
    /// <summary>
    /// Steve Stanisic - Question 5
    /// Implement commands in the Pong console.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        // XNA related variables
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _sb;
        private SpriteFont _font;

        Pong _pong;
        Color _bg;
        int _scrWidth, _scrHeight;
        Console _cs;

        /// <summary>
        /// Game constructor, set up graphics device and Content folder.
        /// </summary>
        public Game()
        {
            _gdm = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialization of non-graphic components. All we need this for is setting the
        /// window title.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Question 5";
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
            _bg = Color.Black;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Fairly self explanatory, handle gamepad input.
        /// </summary>
        private void HandleGamePad()
        {
            GamePadState ps1 = GamePad.GetState(PlayerIndex.One);
            GamePadState ps2 = GamePad.GetState(PlayerIndex.Two);

            if (ps1.IsConnected)
            {
                if (ps1.Buttons.Back == ButtonState.Pressed)
                    Exit();
            }

            // Allow player 1 to start the ball moving
            if (ps1.Buttons.A == ButtonState.Pressed)
            {
                _pong.StartGame();
            }

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

            // Allow player 1 to start the ball moving
            if (kbState.IsKeyDown(Keys.Space))
                _pong.StartGame();

            // Player 1 paddle movement
            if (kbState.IsKeyDown(Keys.Up))
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.UP);
            if (kbState.IsKeyDown(Keys.Down))
                _pong.MovePaddle(Defs.Player.P1, Pong.Direction.DOWN); ;

            // Player 2 paddle movement
            if (kbState.IsKeyDown(Keys.W))
                _pong.MovePaddle(Defs.Player.P2, Pong.Direction.UP);
            if (kbState.IsKeyDown(Keys.S))
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
            if (_cs._consoleActive)
                _cs.Draw(_scrWidth, _scrHeight, _sb);
    
            _sb.End();

            base.Draw(gameTime);
        }
    }
}
