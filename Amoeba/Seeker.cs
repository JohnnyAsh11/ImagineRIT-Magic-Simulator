using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;

namespace Amoeba
{
    /// <summary>
    /// Physical object build to follow instantiations of the Host class 
    /// </summary>
    public class Seeker
    {

        //Fields:
        private Vector2 targetPosition;
        private Vectangle position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private float speed;
        private float mass;
        private Random rng;
        private int windowHeight;
        private int windowWidth;
        private int seekScale;
        private bool isFired;
        private int fireTimer;

        //Properties:
        /// <summary>
        /// get/set access to the Vector2 target position
        /// </summary>
        public Vector2 Target
        {
            get { return  targetPosition; }
            set { targetPosition = value; }
        }

        public bool Fired
        {
            get { return isFired; }
            set
            {
                position.Width += 40;
                isFired = value; 
            }
        }

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the Seeker class
        /// </summary>
        /// <param name="rng">Random object used to determine the seeker's speed</param>
        public Seeker(Random rng)
        {
            windowHeight = Globals.Graphics.GraphicsDevice.Viewport.Height;
            windowWidth = Globals.Graphics.GraphicsDevice.Viewport.Width;

            this.rng = rng;
            this.speed = (float)(rng.NextDouble() * 100) + 30;
            this.targetPosition = Vector2.Zero;
            this.position = new Vectangle(
                windowWidth / 2,
                windowHeight / 2,
                2, 
                2);
            this.velocity = new Vector2(1, 1);
            this.mass = 1;
            this.acceleration = Vector2.Zero;
            this.seekScale = 10;
            this.isFired = false;
            this.fireTimer = 60;
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the Seeker class
        /// </summary>
        public bool Update()
        {
            if (!isFired)
            {
                //reseting the acceleration
                this.acceleration = Vector2.Zero;

                if (Vector2.DistanceSquared(position.Position, targetPosition) > 50)
                {
                    seekScale = 40;
                }

                //applying a seek force to the center of the Host
                this.ApplyForce(this.Seek(targetPosition) * seekScale);

                //calculating the velocity
                velocity = acceleration * Globals.DeltaTime;

                //adding the velocity to the Vectangle position
                position += velocity;

                if (position.Y > targetPosition.Y)
                {
                    //this will have the seekers rain down
                    //position.Y -= (position.Y + targetPosition.Y);

                    //this will have all of the seekers form a horizontal line
                    //position.Y -= (position.Y - targetPosition.Y);

                    //magnetic fields effect
                    position.Y += (targetPosition.Y / position.Y);
                }
            }
            else
            {
                //position.Height++;

                position.X += speed;

                //decrement the fire timer
                fireTimer--;
                if (fireTimer == 0)
                {
                    //when the timer reaches 0, return true to remove the object
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// per frame render method for the Seeker class
        /// </summary>
        public void Draw()
        {
            //defining the position for the seekers to be rendered as centered on the host
            Vectangle renderPos = position;
            renderPos.X += 25;
            renderPos.Y += 25;

            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                renderPos.ToRectangle,
                Color.Red);            
        }

        /// <summary>
        /// Calculates a seek steering force to the Host's position
        /// </summary>
        /// <param name="targetPosition">The Host's position</param>
        /// <returns>a seeking force to the location</returns>
        private Vector2 Seek(Vector2 targetPosition)
        {
            Vector2 seekingForce;

            //calculating the desired velocity which will be straight to the target
            Vector2 desiredVelocity = targetPosition - position.Position;
            desiredVelocity = Vector2.Normalize(desiredVelocity) * speed;

            //calculating the proper force required to smoothly travel to the desired velocity
            seekingForce = desiredVelocity - velocity;

            return seekingForce;
        }

        /// <summary>
        /// Calculates the force based off of Newton's Second Law
        /// </summary>
        /// <param name="force">Vector2 force being applied to the seeker's Acceleration vector</param>
        private void ApplyForce(Vector2 force)
        {
            acceleration += force / mass;
        }

    }
}
