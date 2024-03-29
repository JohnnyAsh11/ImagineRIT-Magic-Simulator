using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    public class Seeker : PhysicsAgent
    {

        //Fields:
        private uint width;
        private uint height;

        //Properties:

        //Constructors:
        public Seeker()
            : base()
        {
            width = (uint)Globals.Graphics.GraphicsDevice.Viewport.Width;
            height = (uint)Globals.Graphics.GraphicsDevice.Viewport.Height;
        }

        //Methods:
        public override void CalcSteeringForces()
        {
            KeyboardState kbState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            float distance = Vector2.DistanceSquared(position.Position, mState.Position.ToVector2());

            if (kbState.IsKeyDown(Keys.W))
            {
                height -= 25;
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                width += 25;
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                width -= 25;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                height += 2s5;
            }


            //position.X += rng.Next(-2, 3);
            //position.Y += rng.Next(-2, 3);

            //150 squared is 22500, so were checking distance against the squared range
            if (distance < 22500)
            {
                //setting the color to blue
                color = Color.Blue;

                //Adding the seek force towards the mouse to the total force
                totalForce += Seek(mState.Position.ToVector2());
            }
            else
            {
                //setting the color to red
                color = Color.Red;

                //adding the wander force to the totalforce
                totalForce += Wander(2, 2) * 0.3f;
            }

            //seeking the center of the screen
            totalForce += Seek(new Vector2(width / 2, height / 2)) * .15f;

            //allowing the pixel boids to wrap
            ScreenWrap();
        }

    }
}
