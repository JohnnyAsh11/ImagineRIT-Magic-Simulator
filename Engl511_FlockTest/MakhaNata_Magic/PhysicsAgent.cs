using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Acts as an equivalent to the GameObject class but applies physics properties to the objects
    /// </summary>
    public abstract class PhysicsAgent : GameObject
    {
        //Fields:
        protected float maxSpeed;
        protected float mass;

        protected Vector2 acceleration;
        protected Vector2 velocity;
        protected Vector2 totalForce;
        private Vector2 startingPosition;
        private Vector2 cohesionPoint;

        private int timer;
        private float wanderAngle;

        protected Random rng;

        //Properties:
        /// <summary>
        /// Get/set access to the PhysicsAgent's Velocity
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //Constructors:
        /// <summary>
        /// Default constructor for PhysicsAgents
        /// </summary>
        public PhysicsAgent()
            : base(Globals.GameTextures["Pixel"], new Vectangle(200, 200, 2, 2))
        {
            mass = 1;
            maxSpeed = 5;
            acceleration = Vector2.Zero;
            velocity = Vector2.Zero;
            totalForce = Vector2.Zero;
            rng = new Random();

            startingPosition = new Vector2(
                position.X,
                position.Y);

            position.X += rng.Next(-100, 101);
            position.Y += rng.Next(-100, 101);
        }


        //Methods:
        /// <summary>
        /// PhysicsAgent's Update method equivalent
        /// </summary>
        public abstract void CalcSteeringForces();

        /// <summary>
        /// Standard Update method for all Physics Agents 
        /// </summary>
        public override void Update()
        {
            //reset the Acceleration every frame
            acceleration = Vector2.Zero;

            //reseting the totalForce
            totalForce = Vector2.Zero;

            //calling CalcSteeringForces to perform whatever steer methods this Agent
            //  would be using
            this.CalcSteeringForces();

            //applying the calculated steering force
            this.ApplyForce(totalForce);

            //applying the acceleration to the velocity
            velocity += acceleration;

            //applying the velocity to the position
            position.Position += velocity * 5;
        }

        /// <summary>
        /// Applies a force to the PhysicsAgent's acceleration
        /// </summary>
        /// <param name="force">Force being applied to the Agent</param>
        public void ApplyForce(Vector2 force)
        {
            acceleration += force / mass;
        }

        #region Steering Methods

        /// <summary>
        /// Creates an 800x800 box that the PhysicsAgent will remain within
        /// </summary>
        /// <returns>A Vector2 force to keep the Agent within bounds</returns>
        protected Vector2 KeepInBounds()
        {
            Vector2 vecPosition = this.position.Position;
            int width = Globals.Graphics.GraphicsDevice.Viewport.Width;
            int height = Globals.Graphics.GraphicsDevice.Viewport.Height;


            //checks performed to tell whether the PhysicsAgent is out of bounds
            if (vecPosition.Y >= height - 100 ||
                vecPosition.Y <= height - height + 100 ||
                vecPosition.X >= width - 100 ||
                vecPosition.X <= width - width + 100)
            {
                //if it is then seek the Starting position of the PhysicsAgent
                return Seek(startingPosition);
            }

            //return Vector(0, 0) if the object is within bounds
            return Vector2.Zero;
        }

        /// <summary>
        /// Checks the physicsAgent against the bounds of the screen to wrap it around
        /// </summary>
        protected void ScreenWrap()
        {
            //setting some variables on the stack to reduce property calls
            Vector2 position = this.position.Position;
            int width = Globals.Graphics.GraphicsDevice.Viewport.Width;
            int height = Globals.Graphics.GraphicsDevice.Viewport.Height;

            //checking the X wrapping
            if (position.X > width)
            {
                this.position.X = 0;
            }            
            else if (position.X < 0)
            {
                this.position.X = width;
            }

            //checking the Y wrapping
            if (position.Y > height)
            {
                this.position.Y = 0;
            }
            else if (position.Y < 0)
            {
                this.position.Y = height;
            }
        }

        /// <summary>
        /// The Seek Force calculation method
        /// </summary>
        /// <param name="targetPosition">The position of the object being seeked</param>
        /// <returns>An approiate force to seek that object</returns>
        protected Vector2 Seek(Vector2 targetPosition)
        {
            //declaring the variable that will hold our seeking force
            Vector2 seekingForce;

            //calculating the vector that would point to the desired location
            Vector2 desiredVelocity = targetPosition - position.Position;

            //normalizing that and multiplying it buy the maxSpeed of this physics agent
            desiredVelocity = Vector2.Normalize(desiredVelocity) * maxSpeed;

            //performing desiredVelocity - velocity to retrieve a vector force that will
            // smoothly track closer and closer to the desired velocity every frame that 
            // this method is called
            seekingForce = desiredVelocity - velocity;

            return seekingForce;
        }

        /// <summary>
        /// Algorithm for AI PhysicsAgent wandering
        /// </summary>
        /// <param name="time">The time being used for future position calculations</param>
        /// <param name="radius">Radius of the circle used for the Wander Algorithm</param>
        /// <returns>The Seek force being applied to acceleration for wandering</returns>
        protected Vector2 Wander(float time, float radius)
        {
            //finding the location of the projected wander circle
            Vector2 futurePosition = CalcFuturePosition(time);

            timer--; ;
            if (timer <= 0)
            {
                wanderAngle = (float)(rng.NextDouble() * (Math.PI * 2));
                timer = 30;
            }

            //getting a random point on the circle
            //float randAngle = (float)(rng.NextDouble() * (Math.PI * 2));

            //calculating the x and y position of the point on the circle
            float x = futurePosition.X + (float)(Math.Cos(wanderAngle)) * radius;
            float y = futurePosition.Y + (float)(Math.Sin(wanderAngle)) * radius;

            //seeking the point found
            return Seek(new Vector2(x, y));
        }

        /// <summary>
        /// Calculated the future position based on the current velocity and a number of iterations
        /// </summary>
        /// <param name="time">Amount of time passed</param>
        /// <returns>The future position of the PhysicsAgent</returns>
        protected Vector2 CalcFuturePosition(float time)
        {
            Vector2 futurePosition;

            //getting the future position by getting the current position and
            // adding velecity to it multplied by time
            futurePosition = position.Position + (velocity * time);

            return futurePosition;
        }
        
        /// <summary>
        /// Calculates a Flee steering force away from a target position
        /// </summary>
        /// <param name="targetPosition">Position being fleed from</param>
        /// <returns>The correct force to travel away from the target position</returns>
        protected Vector2 Flee(Vector2 targetPosition)
        {
            Vector2 seekingForce;

            //calcualting the opposite desired velocity to that of seek
            Vector2 desiredVelocity = position.Position - targetPosition;
            desiredVelocity = Vector2.Normalize(desiredVelocity) * maxSpeed;

            //calculating the proper force required to smoothly travel to the desired velocity
            seekingForce = desiredVelocity - velocity;

            return seekingForce;
        }

        /// <summary>
        /// calculates a seek force to the center of the flock
        /// </summary>
        /// <returns>The Vector3 force to the center of the flock</returns>
        private Vector2 Cohesion(List<PhysicsAgent> collection)
        {
            float maxDistance = 250f;
            int inRangeAgents = 0;
            Vector2 cohesionForce = Vector2.Zero;

            //summing all of the positions of the fish in the flock
            foreach (PhysicsAgent agent in collection)
            {
                //calculate the distance between this agents position to the other
                // agent positions
                float distance = Vector2.Distance(
                    position.Position,
                    agent.Position.Position);

                //if the distance is less than the max distance
                if (distance < maxDistance)
                {
                    //then sum the positions
                    cohesionForce += agent.Position.Position;

                    //increment the number of agents in range
                    inRangeAgents++;
                }
            }

            //if there are agents in range
            if (inRangeAgents > 0)
            {
                //divide the sum of all positions by the number of flocking fish
                // to get the average position
                cohesionForce /= inRangeAgents;

                //setting to cohesion point for gizmos
                cohesionPoint = cohesionForce;

                //seek that average position
                return Seek(cohesionForce);
            }
            else
            {
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// Determines the average direction of in range agents and 
        /// calculates a force to them to move together
        /// </summary>
        /// <returns>a force that moves together with other aligned agents</returns>
        private Vector2 Alignment(List<PhysicsAgent> collection)
        {
            float maxDistance = 250f;
            int inRangeAgents = 0;
            Vector2 alignDirection = Vector2.Zero;

            //summing all of the normalized velocities
            foreach (PhysicsAgent agent in collection)
            {

                //calculating the distance between this agent and the other agents
                float distance = Vector2.Distance(
                    Position.Position,
                    agent.Position.Position);

                if (distance == 0 || agent.Velocity == Vector2.Zero) continue;

                //if the agents are in range of each other
                if (distance < maxDistance)
                {
                    //sum the velocity of this agent with the summation of the other velocities
                    alignDirection += Vector2.Normalize(agent.Velocity);

                    //increment the in range agents counter
                    inRangeAgents++;
                }
            }

            //if there were agents found in range
            if (inRangeAgents > 0)
            {
                //divide the sum of all positions by number of flocking fish
                // in range to get the average velocity
                alignDirection /= inRangeAgents;

                //normalizing that velocity to get the direction and scaling it by the
                // maxSpeed
                alignDirection *= maxSpeed;

                //Calculating the steering force
                return alignDirection - this.velocity;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// Calculates a force that keeps Agents from going over eachother
        /// </summary>
        /// <returns>the Force that prevents overlapping</returns>
        private Vector2 Separate(List<PhysicsAgent> collection)
        {
            //foreach agent found within the 'too close' distance, add to this vector
            Vector2 separateForce = Vector2.Zero;

            foreach (PhysicsAgent agent in collection)
            {
                float distance;

                distance = Vector2.Distance(position.Position, agent.Position.Position);

                if (Double.Epsilon < distance)
                {
                    separateForce += Flee(agent.Position.Position) * (1 / distance);
                }

            }

            return separateForce;
        }

        /// <summary>
        /// Flocking steering algorithm that works with any collection of PhysicsAgents
        /// </summary>
        /// <param name="collection">The list of PhysicsAgents flocking</param>
        /// <returns>The flocking force for an individual of the flock</returns>
        protected Vector2 Flock(List<PhysicsAgent> collection)
        {
            Vector2 flockForce = Vector2.Zero;

            flockForce += Cohesion(collection) * .4f;
            flockForce += Alignment(collection) * .2f;
            flockForce += Separate(collection) * .4f;

            return flockForce;
        }

        #endregion
    }
}
