using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Player controlled class that serves as the focus point for Seekers
    /// </summary>
    public class Host : PhysicsAgent
    {
        //Fields:
        private List<PhysicsAgent> seekers;

        //Properties:

        //Constructors:
        public Host()
            : base()
        {
            seekers = new List<PhysicsAgent>();

            GenerateSeekers(200);
        }

        //Methods:
        private void GenerateSeekers(int amount)
        {
            for (uint i = 0; i < amount; i++)
            {
                seekers.Add(new Seeker());
            }
        }

        public override void CalcSteeringForces()
        {
            totalForce += Wander(3, 1);
        }

        public override void UpdateSeekers()
        {
            foreach (PhysicsAgent agent in seekers)
            {
                agent.Update();
            }
        }

        private void DrawSeekers()
        {



            //drawing all of the seekers
            foreach (PhysicsAgent agent in seekers)
            {
                agent.Draw();
            }
        }

    }
}
