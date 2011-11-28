using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Part3
{
    class Menu
    {
        public bool _menuActive = false;
        GraphicsDevice _gd;
        SpriteFont _sf;

        List<string> _highScores;
        KeyboardState _oldKeys;
        GamePadState _oldPad;

        /// <summary>
        /// Constructor; initializes the console.
        /// </summary>
        /// <param name="gd">The graphics device that the console will be drawn to.</param>
        /// <param name="sf">The text font to use in the console.</param>
        public Menu(GraphicsDevice gd, SpriteFont sf)
        {
            _gd = gd;
            _sf = sf;
            _highScores = new List<string>();
        }

        /// <summary>
        /// Call this method to read the scores out of the specified stream.
        /// </summary>
        /// <param name="scores">The stream to parse.</param>
        /// <returns>A list of the scores read.</returns>
        public List<string> ParseScores(Stream scores)
        {
            List<string> result = new List<string>();

            StreamReader sr = new StreamReader(scores);
            String input;

            while ((input = sr.ReadLine()) != null)
            {
                result.Add(input);
            }

            sr.Close();

            return result;
        }

        /// <summary>
        /// Call this method to update the High Score list of the menue.
        /// </summary>
        /// <param name="scores">The stream to parse.</param>
        public void SetScores(Stream scores)
        {
            using (StreamReader sr = new StreamReader(scores))
            {
                String input;

                while ((input = sr.ReadLine()) != null)
                {
                    _highScores.Add(input);
                }

                sr.Close();
            }
        }

        /// <summary>
        /// Call this method when input is received to have the console react to the user's commands.
        /// </summary>
        /// <param name="kbs">The current keyboard state.</param>
        /// <returns>The Command that corresponds to the user's input.</returns>
        public void CheckInput(KeyboardState kbs)
        {
            // Handle console input
            if (_menuActive)
            {
                if (kbs.IsKeyDown(Keys.M) && _oldKeys.IsKeyUp(Keys.M))
                    _menuActive = false;
            }
            else
            {
                // Should we bring the menu up?
                if (kbs.IsKeyDown(Keys.M) && _oldKeys.IsKeyUp(Keys.M))
                    _menuActive = true;
            }

            _oldKeys = kbs;;
        }

        /// <summary>
        /// Call this method when input is received to have the console react to the user's commands.
        /// </summary>
        /// <param name="kbs">The current keyboard state.</param>
        /// <returns>The Command that corresponds to the user's input.</returns>
        public void CheckInput(GamePadState gs)
        {
            if (_menuActive)
            {
                if (gs.IsButtonDown(Buttons.Y) && _oldPad.IsButtonUp(Buttons.Y))
                    _menuActive = false;
            }
            else
            {
                // Should we bring the menu up?
                if (gs.IsButtonDown(Buttons.Y) && _oldPad.IsButtonUp(Buttons.Y))
                    _menuActive = true;
            }

            _oldPad = gs;
        }

        /// <summary>
        /// Draws the console to the screen.
        /// </summary>
        /// <param name="scrWidth">The current viewport width.</param>
        /// <param name="scrHeight">The current viewport height.</param>
        /// <param name="sb">The spritebatch to add draw commands to.</param>
        public void Draw(int scrWidth, int scrHeight, SpriteBatch sb)
        {
            if (!_menuActive)
                return;

            Rectangle cRect = new Rectangle(10, scrHeight / 2 - 90, scrWidth - 20, scrHeight / 2 + 80);
            Texture2D tex = new Texture2D(_gd, cRect.Width, cRect.Height);
            int texSize = tex.Width * tex.Height;
            Color[] buff = new Color[texSize];

            // Make the menu grey-ish.
            for (int i = 0; i < texSize; ++i)
            {
                buff[i].R = 80;
                buff[i].G = 80;
                buff[i].B = 80;
                buff[i].A = 255;
            }
            tex.SetData(buff);

            // Make the console semi-transparent.
            Color menuColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            sb.Draw(tex, cRect, menuColor);

            // Draw the high scores
            int j = 0;
            foreach (string s in _highScores)
            {
                sb.DrawString(_sf, _highScores[j], new Vector2(10f, j * 25 + cRect.Y), new Color(0.7f, 0.7f, 0.7f, 0.7f));
                ++j;
            }
        }
    }
}