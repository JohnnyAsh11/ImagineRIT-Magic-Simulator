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
        public Vector2 Target
        {
            get { return  targetPosition; }
            set { targetPosition = value; }
        }

        public float Speed 
        { 
            set { speed = value; } 
        }

        //Constructors:
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
        public void Update()
        {
            this.acceleration = Vector2.Zero;

            //targetPosition.X += rng.Next(-2, 3);
            //targetPosition.Y += rng.Next(-2, 3);

            this.ApplyForce(this.Seek(targetPosition) * 40);

            velocity = acceleration * (1.0f / 60.0f);

            position += velocity;


            if (position.Y > windowHeight)
            {
                position.Y = windowHeight;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
            }

            if (position.X > windowWidth)
            {
                position.X = windowWidth;
            }
            else if (position.X < 0)
            {
                position.X = 0;
            }
        }

        public void Draw()
        {
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                position.ToRectangle,
                Color.White);
        }

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
        /// <param name="force">Vector2 force being applied to the PhysObj</param>
        private void ApplyForce(Vector2 force)
        {
            acceleration += force / mass;
        }

    }
}
