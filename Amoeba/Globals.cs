using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Amoeba
{
    /// <summary>
    /// Static class of globally useful and accessible data 
    /// </summary>
    public static class Globals
    {

        //Fields:
        private static SpriteBatch sb;
        private static GraphicsDeviceManager graphics;
        private static Dictionary<string, Texture2D> gameTextures;
        private static GameTime gameTime;

        //Properties:
        /// <summary>
        /// get/set access to a SpriteBatch reference
        /// </summary>
        public static SpriteBatch SB
        {
            get { return sb; }
            set { sb = value; }
        }

        /// <summary>
        /// get/set access to a hash table of all textures used in game
        /// </summary>
        public static Dictionary<string, Texture2D> GameTextures
        {
            get { return gameTextures; }
            set { gameTextures = value; }
        }

        /// <summary>
        /// get/set access to the graphics manager class for MonoGame
        /// </summary>
        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public static GameTime GameTime
        {
            get { return gameTime; }
            set { gameTime = value; }
        }

        public static float Gravity
        {
            get { return -2.0f; }
        }

        //Methods: - NONE -

    }
}
