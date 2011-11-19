using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace P1Pong
{
    /// <summary>
    /// This class represents a game paddle.
    /// </summary>
    public class Paddle : Shape
    {
        /// <summary>
        /// Constructor; initializes the paddles screen position and underlying physics representation.
        /// </summary>
        /// <param name="texture">The texture to apply to the paddle.</param>
        /// <param name="position">The center based position of the paddle.</param>
        /// <param name="world">The world to add the paddle to.</param>
        public Paddle(Texture2D texture, Vector2 position, World world)
            : base(texture)
        {
            float width = tex.Width;
            width /= Defs.MtrInPix;

            float height = tex.Height;
            height /= Defs.MtrInPix;

            Vector2 pos = position / Defs.MtrInPix;

            body = BodyFactory.CreateRectangle(world, width, height, 10000f, pos);
            body.BodyType = BodyType.Dynamic;
            body.Friction = 0.5f;
            body.Restitution = 0.5f;
        }

        /// <summary>
        /// Resets the paddle's position and inertia.
        /// </summary>
        /// <param name="pos">The position to reset to.</param>
        public override void Reset(Vector2 pos)
        {
            pos /= Defs.MtrInPix;
            body.ResetDynamics();
            body.Rotation = 0;
            body.Position = pos;
        }

        /// <summary>
        /// Draws the paddle to the screen.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to add the draw commands to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = body.Position * Defs.MtrInPix;

            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
            spriteBatch.Draw(tex, pos, null, Color.White, body.Rotation, origin, 1f, SpriteEffects.None, 0);
            Color[] data = new Color[3000];
            tex.GetData<Color>(data);
        }
    }
}
