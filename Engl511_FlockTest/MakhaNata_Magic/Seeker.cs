using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhaNata_Magic
{
    public class Seeker : PhysicsAgent
    {

        //Fields:

        //Properties:

        //Constructors:
        public Seeker()
            : base()
        {
            
        }

        //Methods:
        public override void CalcSteeringForces()
        {
            totalForce += Wander(2, 2) * 0.1f;
            totalForce += Flock(SeekerManager.Instance.Seekers);
            //totalForce += KeepInBounds();
            ScreenWrap();
        }

    }
}
