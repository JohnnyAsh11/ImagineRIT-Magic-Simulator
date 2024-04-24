using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Used to determine what spell the player is casting
    /// </summary>
    public enum Spell
    {
        Yashmi,     //Protection Spell
        Hushumi,    //Basic Attack Spell
        Jasica,     //Healing Spell
        Yuruq,      //Burning Spell
        Hiduun      //Nuetral roaming state
    }

    /// <summary>
    /// Player controlled class that serves as the focus point for Seekers
    /// </summary>
    public class Host : PhysicsAgent
    {
        //Fields:
        private List<PhysicsAgent> seekers;
        private const int numOfSeekers = 500;
        private Spell currentSpell;

        //Constructors:
        /// <summary>
        /// Default constructor for the Host class
        /// </summary>
        public Host()
            : base()
        {
            //setting the Host to the roaming state
            currentSpell = Spell.Hiduun;

            //creating the seekers
            seekers = new List<PhysicsAgent>();

            //Randomizing the position of the Hosts
            position.X = rng.Next(0, Globals.Graphics.GraphicsDevice.Viewport.Width + 1);
            position.Y = rng.Next(0, Globals.Graphics.GraphicsDevice.Viewport.Height + 1);

            GenerateSeekers(numOfSeekers);

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
                newSeeker = new Seeker(position.X, position.Y);
                
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
            //checking what the current spell is
            switch (currentSpell)
            {
                case Spell.Hiduun:

                    //adding he physics steering algorithms necessary for the Hiduun state
                    totalForce += Wander(1, 5) * 1.5f;

                    totalForce += KeepInBounds();

                    break;
                case Spell.Yashmi:

                    //seeking the center of the screen
                    totalForce += Seek(new Vector2(
                        Globals.SeekerCenter.X,
                        Globals.SeekerCenter.Y)) * 2.0f;

                    totalForce += KeepInBounds();

                    break;
                case Spell.Hushumi:

                    //looping through the Host's Seekers
                    foreach (PhysicsAgent agent in seekers)
                    {
                        agent.Y = position.Y;

                        //Moving the seekers to the right like darts
                        //agent.X += (Globals.SeekerCenter.X / 50);
                    }

                    totalForce += KeepInBounds();

                    break;
                case Spell.Jasica:

                    //looping through all of the seekers
                    foreach (PhysicsAgent agent in seekers)
                    {
                        //calculating a 1D distance
                        float distance = agent.X - position.X;

                        //if greater than the 0
                        if (distance > 0)
                        {
                            //add
                            agent.Y += distance * 0.2f; 
                        }
                        else
                        {
                            //otherwise, subtract
                            agent.Y -= distance * 0.2f;
                        }
                    }

                    totalForce += Seek(new Vector2(Globals.SeekerCenter.X, Globals.SeekerCenter.Y)) * 0.05f;

                    break;
                case Spell.Yuruq:

                    //looping through the seekers
                    foreach (PhysicsAgent agent in seekers)
                    {
                        agent.Y += rng.Next(-10, 11);
                        agent.X += rng.Next(-10, 11);
                    }

                    //The Hamrakytes still wander under this spell
                    totalForce += Wander(1, 5) * 1.75f;
                    totalForce += KeepInBounds();

                    break;
            }
        }

        /// <summary>
        /// Updates the seekers found within the Host
        /// </summary>
        public void UpdateSeekers()
        {
            //for every Seeker
            foreach (PhysicsAgent agent in seekers)
            {
                //update it
                agent.Update();
            }
        }

        /// <summary>
        /// Host's implementation of per frame render method
        /// </summary>
        public override void Draw()
        {
            //draws the seekers
            DrawSeekers();

            //debug drawing
            //Globals.SB.Draw(
            //    Globals.GameTextures["Pixel"],
            //    position.ToRectangle,
            //    Color.White);
        }

        /// <summary>
        /// private helper method to render the seekers
        /// </summary>
        private void DrawSeekers()
        {
            //drawing all of the seekers
            foreach (PhysicsAgent agent in seekers)
            {
                agent.Draw();
            }
        }

        /// <summary>
        /// Gives a copy of the Host's position for events
        /// </summary>
        /// <returns>The Host's Vector2 position</returns>
        private Vector2 GiveLocation()
        {
            return position.Position;
        }

        /// <summary>
        /// Sets the value for the current spell
        /// </summary>
        /// <param name="_currentSpell">the new current spell</param>
        public void SetCurrentSpell(Spell _currentSpell)
        {
            currentSpell = _currentSpell;
        }
    }
}
