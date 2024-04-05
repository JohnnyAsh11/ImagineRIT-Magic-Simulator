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
        public Host()
            : base()
        {
            seekers = new List<PhysicsAgent>();

            GenerateSeekers(500);

            //apending the Seeker update method onto the PhysicsAgent update method
            this.OnPhysicsUpdate += UpdateSeekers;
        }

        //Methods:
        private void GenerateSeekers(int amount)
        {
            Seeker newSeeker = null;

            for (uint i = 0; i < amount; i++)
            {
                newSeeker = new Seeker();

                newSeeker.OnSeekHost += this.GiveLocation;

                seekers.Add(newSeeker);
            }
        }

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
