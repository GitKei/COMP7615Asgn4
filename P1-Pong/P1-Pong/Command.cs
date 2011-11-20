using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace P1Pong
{
    /// <summary>
    /// Basic class to hold Command types.
    /// </summary>
    class Command
    {
        public enum Type { BG, CHEAT, QUIT }
        public Color _color;
        public Type _type;
        public Defs.Player _player;

        /// <summary>
        /// Constructor; initializes command type.
        /// </summary>
        /// <param name="t"></param>
        public Command(Type t)
        {
            _type = t;
        }
    }
}
