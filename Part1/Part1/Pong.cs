using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;

namespace Part1
{
    class Pong
    {
        public enum Direction { UP, DOWN };

        // Externally accessible
        public int _scoreP1 = 0;
        public int _scoreP2 = 0;

        // Game related variables
        private Paddle _paddle1, _paddle2;
        private Ball _ball;
        Random _rand = new Random();
        bool _gameActive = false;

        int _width, _height;

        // Assets
        Song _background;

        // Physics related variables
        private World _world;
        private Body _top, _bot;

        public Pong(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void Init(ContentManager content)
        {
            SoundEffect bounce = content.Load<SoundEffect>("bounce");
            _background = content.Load<Song>("background");
            MediaPlayer.IsRepeating = true;

            _world = new World(Vector2.Zero); // 0 Gravity

            // Set up game objets
            Texture2D tex = content.Load<Texture2D>("Rectangle");
            Vector2 position = new Vector2(tex.Width / 2f, _height / 2f);
            _paddle1 = new Paddle(tex, position, _world);

            position = new Vector2(_width - tex.Width / 2f, _height / 2f);
            _paddle2 = new Paddle(tex, position, _world);

            tex = content.Load<Texture2D>("Circle");
            position = new Vector2(_width / 2f, _height / 2f);
            _ball = new Ball(tex, position, _world, bounce);

            // Set up top and bottom walls
            Vector2 startPos = new Vector2(-10, 0) / Defs.MtrInPix;
            Vector2 endPos = new Vector2(_width + 10, 0) / Defs.MtrInPix;

            _top = BodyFactory.CreateEdge(_world, startPos, endPos);
            _top.IsStatic = true;
            _top.Restitution = 0.8f;
            _top.Friction = 0f;

            startPos = new Vector2(-10, _height) / Defs.MtrInPix;
            endPos = new Vector2(_width + 10, _height) / Defs.MtrInPix;

            _bot = BodyFactory.CreateEdge(_world, startPos, endPos);
            _bot.IsStatic = true;
            _bot.Restitution = 0.8f;
            _bot.Friction = 0f;
        }

        public void MovePaddle(Defs.Player p, Direction d)
        {
            Vector2 force;
            
            if (d == Direction.UP)
                force = new Vector2(0, -20000);
            else
                force = new Vector2(0, 20000);

            if (p == Defs.Player.P1)
                _paddle1.body.ApplyForce(force);
            else
                _paddle2.body.ApplyForce(force);
        }

        public void StartGame()
        {
            if (!_gameActive)
            {
                _gameActive = !_gameActive;
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
                MediaPlayer.Play(_background);
            }
        }

        public void AddScore(Defs.Player p)
        {
            if (p == Defs.Player.P1)
                _scoreP1 += 100;
            if (p == Defs.Player.P2)
                _scoreP2 += 100;
        }

        public void Step(GameTime gameTime)
        {
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
            if (ballPos.X < 0 || ballPos.X > _width)
            {
                // Tally score
                if (ballPos.X < 0)
                    ++_scoreP2;
                else if (ballPos.X > _width)
                    ++_scoreP1;

                // Reset Objects
                _ball.Reset(new Vector2(_width, _height) / 2f);
                _paddle1.Reset(new Vector2(_paddle1.tex.Width / 2f, _height / 2f));
                _paddle2.Reset(new Vector2(_width - _paddle1.tex.Width / 2f, _height / 2f));

                _gameActive = false;
                MediaPlayer.Stop();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw game objects
            _paddle1.Draw(sb);
            _paddle2.Draw(sb);
            _ball.Draw(sb);
        }
    }
}
