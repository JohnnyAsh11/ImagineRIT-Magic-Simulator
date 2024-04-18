using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    public delegate Vector2 GetLocation();

    public class Seeker : PhysicsAgent
    {

        //Fields:
        private uint width;
        private uint height;

        public event GetLocation OnSeekHost;

        //Properties: - NONE -

        //Constructors:
        public Seeker()
            : base()
        {
            width = (uint)Globals.Graphics.GraphicsDevice.Viewport.Width;
            height = (uint)Globals.Graphics.GraphicsDevice.Viewport.Height;
        }
        public Seeker(float x, float y)
            : base()
        {
            position.X = x;
            position.Y = y;

            width = (uint)Globals.Graphics.GraphicsDevice.Viewport.Width;
            height = (uint)Globals.Graphics.GraphicsDevice.Viewport.Height;
        }

        //Methods:
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

            //allowing the pixel boids to wrap
            //ScreenWrap();
            //totalForce += KeepInBounds();
        }

    }
}
