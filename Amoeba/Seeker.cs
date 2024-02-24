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

        //Properties:
        /// <summary>
        /// get/set access to the Vector2 target position
        /// </summary>
        public Vector2 Target
        {
            get { return  targetPosition; }
            set { targetPosition = value; }
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
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the Seeker class
        /// </summary>
        public void Update()
        {
            const int seekScale = 40;

            //reseting the acceleration
            this.acceleration = Vector2.Zero;

            //applying a seek force to the center of the Host
            this.ApplyForce(this.Seek(targetPosition) * seekScale);

            //calculating the velocity
            velocity = acceleration * Globals.DeltaTime;

            //adding the velocity to the Vectangle position
            position += velocity;

            //calculating the screen bounds
            //  top and bottom bounds
            if (position.Y > windowHeight)
            {
                position.Y = windowHeight;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
            }

            //left and right bounds
            if (position.X > windowWidth)
            {
                position.X = windowWidth;
            }
            else if (position.X < 0)
            {
                position.X = 0;
            }
        }

        /// <summary>
        /// per frame render method for the Seeker class
        /// </summary>
        public void Draw()
        {
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                position.ToRectangle,
                Color.White);
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
