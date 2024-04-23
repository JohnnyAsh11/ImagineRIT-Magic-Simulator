using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Static class of globally useful data
    /// </summary>
    public static class Globals
    {

        //Fields:
        private static SpriteBatch spriteBatch;
        private static GraphicsDeviceManager graphics;
        private static Dictionary<string, Texture2D> gameTextures;
        private static GameTime gameTime;
        private static SpriteFont spriteFont;
        private static Vector2 seekerCenter;

        //Properties:
        /// <summary>
        /// Get/set access to the SpriteBatch reference
        /// </summary>
        public static SpriteBatch SB
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        /// <summary>
        /// Get/set access to a SpriteFont reference
        /// </summary>
        public static SpriteFont SF
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        /// <summary>
        /// Get/set access to the target position for Seekers
        /// </summary>
        public static Vector2 SeekerCenter
        {
            get { return seekerCenter; }
            set { seekerCenter = value; }
        }

        /// <summary>
        /// Get/set access to the graphics manager reference
        /// </summary>
        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        /// <summary>
        /// Get/set access to the Dictionary of game textures
        /// </summary>
        public static Dictionary<string, Texture2D> GameTextures
        {
            get { return gameTextures; }
            set { gameTextures = value; }
        }

        /// <summary>
        /// Get/set access to a reference of GameTime
        /// </summary>
        public static GameTime GameTime
        {
            get { return gameTime; }
            set { gameTime = value; }
        }

        /// <summary>
        /// Gets the value for the time in between frames (1/60)
        /// </summary>
        public static float DeltaTime { get { return 0.0166f; } }

        //Methods: - NONE -
    }
}
