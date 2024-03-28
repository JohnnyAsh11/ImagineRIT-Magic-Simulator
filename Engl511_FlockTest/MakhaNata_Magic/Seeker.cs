using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    public class Seeker : PhysicsAgent
    {

        //Fields:
        private float timer;

        //Properties:

        //Constructors:
        public Seeker()
            : base()
        {
            timer = 0;
        }

        //Methods:
        public override void CalcSteeringForces()
        {
            MouseState mState = Mouse.GetState();

            float distance = Vector2.DistanceSquared(position.Position, mState.Position.ToVector2());

            //150 squared is 22500, so were checking distance against the squared range
            if (distance < 22500)
            {
                color = Color.Blue;
                timer = 0;

                totalForce += Seek(mState.Position.ToVector2());
            }
            else
            {
                timer += Globals.GameTime.TotalGameTime.Milliseconds;

                if (timer < 4000)
                {
                    color = Color.Red;

                    totalForce += Wander(2, 2) * 0.1f;
                    //totalForce += Flock(SeekerManager.Instance.Seekers);
                    //totalForce += KeepInBounds();
                }
                else if (timer >= 4)
                {
                    totalForce += Flock(SeekerManager.Instance.Seekers);
                }
            }

            ScreenWrap();
        }

    }
}
