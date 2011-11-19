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
    /// This class represents the game ball.
    /// </summary>
    public class Ball : Shape
    {
        /// <summary>
        /// Construction; initializes the ball's position and underlying physics representation.
        /// </summary>
        /// <param name="texture">The texture to apply to the ball.</param>
        /// <param name="position">The center based position of the ball.</param>
        /// <param name="world">The world in which to place the ball.</param>
        public Ball(Texture2D texture, Vector2 position, World world) : base(texture)
        {
            float radius = tex.Height;
            radius /= 2f;
            radius /= Defs.MtrInPix;

            Vector2 pos = position;
            pos.X /= Defs.MtrInPix;
            pos.Y /= Defs.MtrInPix;

            body = BodyFactory.CreateCircle(world, radius, 1f, pos);
            body.BodyType = BodyType.Dynamic;
            body.Friction = 0.5f;
            body.Restitution = 1.2f;
        }

        /// <summary>
        /// Reset the ball's position and inertia.
        /// </summary>
        /// <param name="pos">Position to reset to.</param>
        public override void Reset(Vector2 pos)
        {
            pos /= Defs.MtrInPix;
            body.ResetDynamics();
            body.Position = pos;
        }

        /// <summary>
        /// Draw the ball onscreen.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to add the draw command to.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = body.Position * Defs.MtrInPix;

            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
            spriteBatch.Draw(tex, pos, null, Color.White, body.Rotation, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
