using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        /// <summary>
        /// Default constructor for the Host class
        /// </summary>
        public Host()
            : base()
        {
            //creating the seekers
            seekers = new List<PhysicsAgent>();
            GenerateSeekers(500);

            //apending the Seeker update method onto the PhysicsAgent update method
            this.OnPhysicsUpdate += UpdateSeekers;
        }

        //Methods:
        /// <summary>
        /// Generates an amount of seekers equal to the amount parameter
        /// </summary>
        /// <param name="amount">the amount of seekers being generated</param>
        private void GenerateSeekers(int amount)
        {
            Seeker newSeeker = null;

            for (uint i = 0; i < amount; i++)
            {
                //creating the seeker
                newSeeker = new Seeker();
                
                //subscribing the GiveLocation event to the Seeker's OnSeekHost event
                newSeeker.OnSeekHost += this.GiveLocation;

                //adding the seeker to the list
                seekers.Add(newSeeker);
            }
        }

        /// <summary>
        /// Overriding to contain the specific
        /// </summary>
        public override void CalcSteeringForces()
        {
            totalForce += Wander(1, 5) * 1.5f;
            totalForce += KeepInBounds();
        }

        public void UpdateSeekers()
        {
            foreach (PhysicsAgent agent in seekers)
            {
                agent.Update();
            }
        }

        public override void Draw()
        {
            DrawSeekers();
        }

        private void DrawSeekers()
        {
            //drawing all of the seekers
            foreach (PhysicsAgent agent in seekers)
            {
                agent.Draw();
            }
        }

        private Vector2 GiveLocation()
        {
            return position.Position;
        }
    }
}
