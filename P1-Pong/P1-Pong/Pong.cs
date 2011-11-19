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
    public class Pong : Microsoft.Xna.Framework.Game
    {
        // XNA related variables
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _sb;
        private SpriteFont _font;

        // Game related variables
        private Paddle _paddle1, _paddle2;
        private Ball _ball;
        private int _scoreP1 = 0;
        private int _scoreP2 = 0;
        Random _rand = new Random();
        bool _gameActive = false;
        Color _bg;
        int _scrWidth, _scrHeight;
        Console _cs;

        // Physics related variables
        private World _world;
        private Body _top, _bot;

        /// <summary>
        /// Game constructor, set up graphics device and Content folder.
        /// </summary>
        public Pong()
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
            _world = new World(Vector2.Zero); // 0 Gravity
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

            // Set up game objets
            Texture2D tex = Content.Load<Texture2D>("Rectangle");
            Vector2 position = new Vector2(tex.Width / 2f, _scrHeight / 2f);
            _paddle1 = new Paddle(tex, position, _world);

            position = new Vector2(_scrWidth - tex.Width / 2f, _scrHeight / 2f);
            _paddle2 = new Paddle(tex, position, _world);

            tex = Content.Load<Texture2D>("Circle");
            position = new Vector2(_scrWidth / 2f, _scrHeight / 2f);
            _ball = new Ball(tex, position, _world);

            // Set up top and bottom walls
            Vector2 startPos = new Vector2(-10, 0) / Defs.MtrInPix;
            Vector2 endPos = new Vector2(_scrWidth + 10, 0) / Defs.MtrInPix;

            _top = BodyFactory.CreateEdge(_world, startPos, endPos);
            _top.IsStatic = true;
            _top.Restitution = 0.8f;
            _top.Friction = 0f;

            startPos = new Vector2(-10, _scrHeight) / Defs.MtrInPix;
            endPos = new Vector2(_scrWidth + 10, _scrHeight) / Defs.MtrInPix;

            _bot = BodyFactory.CreateEdge(_world, startPos, endPos);
            _bot.IsStatic = true;
            _bot.Restitution = 0.8f;
            _bot.Friction = 0f;

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

            // Update our physics model
            _world.Step(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            Vector2 ballPos = _ball.body.Position * Defs.MtrInPix;

            // Give the ball a minimum velocity to help with vertical stalemate
            if (_gameActive)
            {
                Vector2 velocity = _ball.body.LinearVelocity;
                if (-1f < velocity.X && velocity.X < 1)
                {
                    velocity.X *= 2f;
                    velocity.Y = 0;
                    _ball.body.ApplyLinearImpulse(velocity);
                }
            }
            
            // Someone has scored
            if (ballPos.X < 0 || ballPos.X > _scrWidth)
            {
                // Tally score
                if (ballPos.X < 0)
                    ++_scoreP2;
                else if (ballPos.X > _scrWidth)
                    ++_scoreP1;

                // Reset Objects
                _ball.Reset(new Vector2(_scrWidth, _scrHeight) / 2f);
                float width = _paddle1.tex.Width / 2f;
                _paddle1.Reset(new Vector2(width, _scrHeight / 2f));
                _paddle2.Reset(new Vector2(_scrWidth - width, _scrHeight / 2f));

                _gameActive = false;
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

            if (ps1.IsConnected)
            {
                if (ps1.Buttons.Back == ButtonState.Pressed)
                    Exit();
            }

            // Allow player 1 to start the ball moving
            if (!_gameActive && ps1.Buttons.A == ButtonState.Pressed)
            {
                _gameActive = true;
                Vector2 mag = Vector2.Zero;

                float dir = (float)_rand.NextDouble();
                if (dir < 0.5)
                    mag.X -= (float)_rand.NextDouble() + 2;
                else
                    mag.X += (float)_rand.NextDouble() + 2;

                dir = (float)_rand.NextDouble();
                if (dir < 0.5)
                    mag.Y -= (float)_rand.NextDouble();
                else
                    mag.Y += (float)_rand.NextDouble();

                _ball.body.ApplyLinearImpulse(mag);
            }

            // Player 1 paddle movement
            if (ps1.ThumbSticks.Left.Y > 0f)
                _paddle1.body.ApplyForce(new Vector2(0, -20000));
            if (ps1.ThumbSticks.Left.Y < 0f)
                _paddle1.body.ApplyForce(new Vector2(0, 20000));

            // Player 2 paddle movement
            if (ps2.ThumbSticks.Left.Y > 0f)
                _paddle2.body.ApplyForce(new Vector2(0, -20000));
            if (ps2.ThumbSticks.Left.Y < 0f)
                _paddle2.body.ApplyForce(new Vector2(0, 20000));
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
                if (c._player == 1)
                    _scoreP1 += 100;
                if (c._player == 2)
                    _scoreP2 += 100;
            }

            // Allow player 1 to start the ball moving
            if (!_gameActive && kbState.IsKeyDown(Keys.Space))
            {
                _gameActive = true;
                Vector2 mag = Vector2.Zero;

                float dir = (float)_rand.NextDouble();
                if (dir < 0.5)
                    mag.X -= (float)_rand.NextDouble() + 2;
                else
                    mag.X += (float)_rand.NextDouble() + 2;

                dir = (float)_rand.NextDouble();
                if (dir < 0.5)
                    mag.Y -= (float)_rand.NextDouble();
                else
                    mag.Y += (float)_rand.NextDouble();

                _ball.body.ApplyLinearImpulse(mag);
            }

            // Player 1 paddle movement
            if (kbState.IsKeyDown(Keys.Up))
                _paddle1.body.ApplyForce(new Vector2(0, -20000));
            if (kbState.IsKeyDown(Keys.Down))
                _paddle1.body.ApplyForce(new Vector2(0, 20000));

            // Player 2 paddle movement
            if (kbState.IsKeyDown(Keys.W))
                _paddle2.body.ApplyForce(new Vector2(0, -20000));
            if (kbState.IsKeyDown(Keys.S))
                _paddle2.body.ApplyForce(new Vector2(0, 20000));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bg);

            _sb.Begin();

            // Draw game objects
            _paddle1.Draw(_sb);
            _paddle2.Draw(_sb);
            _ball.Draw(_sb);

            // Draw scoreboard
            _sb.DrawString(_font, "Player 1: " + _scoreP1.ToString(), new Vector2(_scrWidth / 2 - 300, 0), Color.White);
            _sb.DrawString(_font, "Player 2: " + _scoreP2.ToString(), new Vector2(_scrWidth / 2 + 200, 0), Color.White);

            // Draw console
            if (_cs._consoleActive)
                _cs.Draw(_scrWidth, _scrHeight, _sb);
    
            _sb.End();

            base.Draw(gameTime);
        }
    }
}
