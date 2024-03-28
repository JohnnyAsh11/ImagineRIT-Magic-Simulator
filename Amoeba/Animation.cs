using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amoeba
{
    public class Animation
    {

        //Fields:
        private Rectangle srcRect;
        private int frameWidth;
        private int frameHeight;
        private int timeBetweenFrames;
        private string asset;
        private float deltaTime;
        private float timer;
        private int numOfFrames;
        private int maxXFrame;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the animation class
        /// </summary>
        /// <param name="frameWidth">The width between frames of the animation</param>
        /// <param name="frameHeight">The height between frames of the animation</param>
        /// <param name="timeBetweenFrames">Amount of time in between frames</param>
        /// <param name="asset">spritesheet name in GameTextures Dictionary</param>
        /// <param name="numOfFrames">spritesheet name in GameTextures Dictionary</param>
        public Animation(int frameWidth, int frameHeight, int timeBetweenFrames, string asset, int numOfFrames)
        {
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.timeBetweenFrames = timeBetweenFrames;
            this.asset = asset;
            this.numOfFrames = numOfFrames;
            //this.maxXFrame = ;

            //creating a field for deltaTime
            this.deltaTime = Globals.DeltaTime;
        }

        //Methods:
        /// <summary>
        /// Render method for the animations passed into the Animation class
        /// </summary>
        /// <param name="position">Rectangle position for the animation rendering</param>
        public void Animate(Rectangle position)
        {
            Globals.SB.Draw(
                Globals.GameTextures[asset],
                position,
                srcRect,
                Color.White,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.0f);

            timer += deltaTime;
            //0.05 is 12 frames per second
            if (timer == 0.05f)
            {
                //calculating the max X source
                int max = frameWidth * numOfFrames;
                srcRect.X += frameWidth;
                timer = 0;

                //checking if the max is reached
                if (srcRect.X >= max)
                {
                    //reseting the X frame
                    srcRect.X = 0;
                }
            }
        }

    }
}
