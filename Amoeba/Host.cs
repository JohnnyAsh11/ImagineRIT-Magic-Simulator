using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Amoeba
{
    /// <summary>
    /// Functionally the player class that serves as the host location for all Seekers that are instantied
    /// </summary>
    public class Host
    {

        //Fields:
        private Vectangle position;
        private Vector2 direction;
        private float speed;
        private KeyboardState prevKBState;
        private List<Seeker> seekers;
        private int numOfSeekers;
        private Random rng;
        private bool isGrounded;

        private Vector2 acceleration;
        private Vector2 velocity;
        private float wanderAngle;
        private float wanderTimer;

        private int windowHeight;
        private int windowWidth;

        public event CollisionList GetCollidableTiles;

        //Properties:
        public int NumOfSeekers
        {
            get { return numOfSeekers; }
            set { numOfSeekers = value; }
        }

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the Host class
        /// </summary>
        /// <param name="numOfSeekers">the Number of seekers that this host will have</param>
        public Host(int numOfSeekers)
        {
            windowHeight = Globals.Graphics.GraphicsDevice.Viewport.Height;
            windowWidth = Globals.Graphics.GraphicsDevice.Viewport.Width;

            direction = Vector2.Zero;
            speed = 5.0f;
            this.seekers = new List<Seeker>();
            this.position = new Vectangle(
                windowWidth / 2,
                windowHeight / 2, 
                50,
                50);
            this.rng = new Random();

            velocity = Vector2.Zero;
            this.wanderTimer = 0;
            wanderAngle = (float)(rng.NextDouble() * (Math.PI * 2));

            this.numOfSeekers = numOfSeekers;
            InstantiateSeekers(this.numOfSeekers);
        }

        //Methods:
        /// <summary>
        /// Per frame logic update method for the Host class
        /// </summary>
        public void Update()
        {
            //polling for key input
            KeyboardState kbState = Keyboard.GetState();
            Vectangle futurePosition = position;
            //isGrounded = false;

            acceleration = Vector2.Zero;

            //checking for up and down movement
            if (kbState.IsKeyDown(Keys.W))
            {
                direction.Y = -1;
            }
            else if (kbState.IsKeyDown(Keys.S))
            {
                direction.Y = 1;
            }
            else
            {
                direction.Y = 0;
            }

            //checking for left and right movement
            if (kbState.IsKeyDown(Keys.A))
            {
                direction.X = -1;
            }
            else if (kbState.IsKeyDown(Keys.D))
            {
                direction.X = 1;
            }
            else
            {
                direction.X = 0;
            }

            AttackInput(kbState);
            ReloadSeekers(kbState);

            //direction = Vector2.Normalize(direction);
            ApplyForce(Wander(2f, 1f));

            velocity += acceleration;

            velocity = Vector2.Clamp(
                velocity, 
                Vector2.Zero, 
                new Vector2(speed, speed));

            //futurePosition += velocity;
            futurePosition += (direction * speed);

            //factoring collisions into the future position
            futurePosition = CollisionDetection(futurePosition);

            //checking if the host is on the ground
            //if (!isGrounded)
            //{
            //    //futurePosition.Y -= Globals.Gravity;
            //}

            position = futurePosition;
            prevKBState = kbState;

            //updating the seekers
            UpdateSeekers();
        }

        /// <summary>
        /// Render method for the Host class
        /// </summary>
        public void Draw()
        {
            DrawSeekers();
            //Globals.SB.Draw(
            //    Globals.GameTextures["Pixel"],
            //    position.ToRectangle,
            //    Color.Red);

            //Globals.SB.DrawString(
            //    Globals.DebugFont,
            //    $"{numOfSeekers}",
            //    new Vector2(50, 50),
            //    Color.Purple);
        }

        /// <summary>
        /// Creates the seeker objects that will follow the Host
        /// </summary>
        /// <param name="numOfSeekers">Total number of seekers that are being added</param>
        private void InstantiateSeekers(int numOfSeekers)
        {
            //instantiates per the specified number
            for (int i = 0; i < numOfSeekers; i++)
            {
                seekers.Add(new Seeker(rng));
            }
        }

        /// <summary>
        /// Update method for the Seekers in the seekers list
        /// </summary>
        private void UpdateSeekers()
        {
            //updates the seeker's target position
            // and their Update logic
            for (int i = seekers.Count - 1; i >= 0; i--)
            {
                //checking if the seeker's time is up
                if (seekers[i].Update())
                {
                    //if it is, remove the reference to it
                    seekers.Remove(seekers[i]);

                    //move to the next iteration
                    continue;
                }

                //setting the target to the host's current position
                seekers[i].Target = this.position.Position;
            }
        }

        /// <summary>
        /// Calls the render method for all seekers in the list
        /// </summary>
        private void DrawSeekers()
        {
            //looping through the list of seekers
            foreach (Seeker seeker in seekers)
            {
                //calls the seeker's individual draw methods
                seeker.Draw();
            }
        }

        /// <summary>
        /// Performs collision checks against the environment
        /// </summary>
        private Vectangle CollisionDetection(Vectangle futurePosition)
        {
            List<Tile> collidables = null;

            //checking the event for null values
            if (GetCollidableTiles != null)
            {
                Vector2 top = new Vector2(position.X + (position.Width / 2), position.Y);
                Vector2 bottom = new Vector2(position.X + (position.Width / 2), position.Y + position.Height);
                Vector2 left = new Vector2(position.X, position.Y + (position.Height / 2));
                Vector2 right = new Vector2(position.X + position.Width, position.Y + (position.Height / 2));

                //invoking the event
                collidables = GetCollidableTiles();

                //looping through the values in the event
                foreach (Tile tile in collidables)
                {
                    //Checking if the tile's position contains any of the Vector2s
                    if (tile.Position.ToRectangle.Contains(top))
                    {
                        futurePosition.Y += speed;
                    }
                    else if (tile.Position.ToRectangle.Contains(left))
                    {
                        futurePosition.X += speed;
                    }
                    else if (tile.Position.ToRectangle.Contains(right))
                    {
                        futurePosition.X -= speed;
                    }
                    else if (tile.Position.ToRectangle.Contains(bottom))
                    {
                        //if the bottom is within the tile's position
                        //perform the same actions
                        futurePosition.Y -= speed;

                        //also stop the direction vector in the Y direction
                        direction.Y = 0;

                        //let the rest of class know that the host is on the ground
                        isGrounded = true;
                        break;
                    }
                }
            }

            return futurePosition;
        }

        /// <summary>
        /// Checks for the input necessary for attacking in game
        /// </summary>
        /// <param name="kbState">the key press polling variable</param>
        private void AttackInput(KeyboardState kbState)
        {
            //checks if the space bar was pressed and if there are still seekers to fire
            if (kbState.IsKeyDown(Keys.Space) &&
                prevKBState.IsKeyUp(Keys.LeftShift) &&
                numOfSeekers > 0)
            {
                //is so, fire a seeker and decrement the number of remaining seekers
                seekers[numOfSeekers - 1].Fired = true;
                numOfSeekers--;
            }
        }

        /// <summary>
        /// Method for the Seeker reload input
        /// </summary>
        /// <param name="kbState">The key press polling variable</param>
        private void ReloadSeekers(KeyboardState kbState)
        {
            //the player cannot have more than 1000 seekers
            if (numOfSeekers > 1000)
            {
                return;
            }
            else if (kbState.IsKeyDown(Keys.LeftShift) &&
                    prevKBState.IsKeyUp(Keys.Space))
            {
                //increase the number of seekers
                numOfSeekers++;

                //add the seekers to the list
                seekers.Add(new Seeker(rng));
            }
        }


        /// <summary>
        /// Calculates a random Wander force to be applied to the Agent 
        /// </summary>
        /// <param name="time">How far ahead the future Position will calculate</param>
        /// <param name="radius">The radius of the Wander Algorithm's circle</param>
        /// <returns>A wander force for the Agent</returns>
        protected Vector2 Wander(float time, float radius)
        {
            //finding the location of the projected wander circle
            Vector2 futurePosition = CalcFuturePosition(time);

            //every 2 seconds, getting a random point on the circle to wander to
            wanderTimer += Globals.DeltaTime;
            if (wanderTimer >= .1)
            {
                wanderAngle = (float)(rng.NextDouble() * (Math.PI * 2));
                wanderTimer = 0;
            }

            //calculating the x and y position of the point on the circle
            float x = (float)(futurePosition.X + Math.Cos(wanderAngle) * radius);
            float y = (float)(futurePosition.Y + Math.Sin(wanderAngle) * radius);

            //seeking the point found
            return Seek(new Vector2(x, y));
        }

        /// <summary>
        /// Calculates the future position of the Agent based on its current
        /// Velocity Vector
        /// </summary>
        /// <param name="time">Amound of frames ahead</param>
        /// <returns>The future position of the Agent</returns>
        protected Vector2 CalcFuturePosition(float time)
        {
            Vector2 futurePosition = Vector2.Zero;

            //getting the future position by getting the current position and
            // adding velecity to it multplied by time
            futurePosition = (position.Position + velocity) * time;

            return futurePosition;
        }

        /// <summary>
        /// The Seek Force calculation method
        /// </summary>
        /// <param name="targetPosition">The position of the object being seeked</param>
        /// <returns>An approiate force to seek that object</returns>
        private Vector2 Seek(Vector2 targetPosition)
        {
            //declaring the variable that will hold our seeking force
            Vector2 seekingForce;

            //calculating the vector that would point to the desired location
            Vector2 desiredVelocity = targetPosition - position.Position;

            //normalizing that and multiplying it buy the maxSpeed of this physics agent
            desiredVelocity = Vector2.Normalize(desiredVelocity) * speed;

            //performing desiredVelocity - velocity to retrieve a vector force that will
            // smoothly track closer and closer to the desired velocity every frame that 
            // this method is called
            seekingForce = desiredVelocity - velocity;

            return seekingForce;
        }


        /// <summary>
        /// Calculates the force based off of Newton's Second Law
        /// </summary>
        /// <param name="force">Vector2 force being applied to the seeker's Acceleration vector</param>
        private void ApplyForce(Vector2 force)
        {
            acceleration += force / 1;
        }

    }
}
