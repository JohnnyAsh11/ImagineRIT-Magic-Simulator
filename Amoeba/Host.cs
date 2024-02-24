using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            futurePosition += (speed * direction);

            //factoring collisions into the future position
            futurePosition = CollisionDetection(futurePosition);

            if (!isGrounded)
            {
                futurePosition.Y -= Globals.Gravity;
            }

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
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                position.ToRectangle,
                Color.Blue);

            Globals.SB.DrawString(
                Globals.DebugFont,
                $"{numOfSeekers}",
                new Vector2(50, 50),
                Color.Purple);

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
                if (seekers[i].Update())
                {
                    seekers.Remove(seekers[i]);
                    continue;
                }
                seekers[i].Target = this.position.Position;
            }
        }

        /// <summary>
        /// Calls the render method for all seekers in the list
        /// </summary>
        private void DrawSeekers()
        {
            foreach (Seeker seeker in seekers)
            {
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
                        futurePosition.Y -= speed;
                        direction.Y = 0;
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
                numOfSeekers++;
                seekers.Add(new Seeker(rng));
            }
        }

    }
}
