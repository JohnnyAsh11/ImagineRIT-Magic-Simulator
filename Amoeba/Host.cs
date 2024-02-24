using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading.Tasks;

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
        private Random rng;

        private double wanderTimer;

        private int windowHeight;
        private int windowWidth;

        //Properties:

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
                windowHeight / 2,
                windowWidth / 2, 
                50,
                50);
            this.rng = new Random();

            wanderTimer = 0;

            InstantiateSeekers(numOfSeekers);
        }

        //Methods:
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
            foreach (Seeker follower in seekers)
            {
                follower.Target = this.position.Position;
                follower.Update();
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
        /// Per frame logic update method for the Host class
        /// </summary>
        public void Update()
        {
            //polling for key input
            KeyboardState kbState = Keyboard.GetState();

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

            //direction = Vector2.Normalize(direction);
            position += (speed * direction);
            //position.Y -= Globals.Gravity;

            //keeping the host within the bounds of the game window
            //   top and bottom bounds checks
            if (position.Y > windowHeight)
            {
                position.Y = windowHeight;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
            }

            //Left and right bounds checks
            if (position.X > windowWidth)
            {
                position.X = windowWidth;
            }
            else if (position.X < 0)
            {
                position.X = 0;
            }

            //updating the seekers
            UpdateSeekers();
        }

        /// <summary>
        /// Render method for the Host class
        /// </summary>
        public void Draw()
        {
            Vectangle renderPos = position;
            renderPos.X -= position.Width / 2;
            renderPos.Y -= position.Height / 2;

            DrawSeekers();
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                renderPos.ToRectangle,
                Color.Blue);

        }
    }
}
