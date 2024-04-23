using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    public delegate Vector2 GetLocation();

    /// <summary>
    /// Seeker class where every instance of represents a single pixel of an instance of a Host
    /// </summary>
    public class Seeker : PhysicsAgent
    {
        //Fields:
        private uint width;
        private uint height;
        public event GetLocation OnSeekHost;

        //Constructors:
        /// <summary>
        /// Default constructor for the Seeker class
        /// </summary>
        public Seeker()
            : base()
        {
            width = (uint)Globals.Graphics.GraphicsDevice.Viewport.Width;
            height = (uint)Globals.Graphics.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// Parameterized constructor for the Seeker class
        /// </summary>
        /// <param name="x">specified X position of the Seeker class</param>
        /// <param name="y">specified Y position of the Seeker class</param>
        public Seeker(float x, float y)
            : base()
        {
            position.X = x;
            position.Y = y;

            width = (uint)Globals.Graphics.GraphicsDevice.Viewport.Width;
            height = (uint)Globals.Graphics.GraphicsDevice.Viewport.Height;
        }

        //Methods:
        /// <summary>
        /// Calculates the steering forces for the Physics Agent Update method
        /// </summary>
        public override void CalcSteeringForces()
        {
            KeyboardState kbState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            //setting the color to red
            color = Color.Red;

            //adding the wander force to the totalforce
            totalForce += Wander(2, 2) * 0.3f;

            //so long as the OnSeekHost event has methods subscribed to it
            if (OnSeekHost != null)
            {
                //seek the point that is returned
                totalForce += Seek(OnSeekHost()) * .15f;
            }

            //ScreenWrap();
        }
    }
}
