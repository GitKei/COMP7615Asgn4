using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

namespace Part3
{
    /// <summary>
    /// Basic shape class, contains a texture and physics body.
    /// </summary>
    public class Shape
    {
        /// <summary>
        /// Constructor; sets the texture.
        /// </summary>
        /// <param name="texture">The texture that will be applied to the shape.</param>
        public Shape(Texture2D texture)
        {
            tex = texture;
        }

        public Texture2D tex;
        public Body body;

        /// <summary>
        /// Subclasses must override this.
        /// </summary>
        /// <param name="pos">Position to reset to.</param>
        public virtual void Reset(Vector2 pos) { }
        /// <summary>
        /// Subclasses must override this.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to add draw commands to.</param>
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
