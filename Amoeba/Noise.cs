using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PerlinGenerator
{
    /// <summary>
    /// Perlin Noise class for wave like psuedo-random generation
    /// </summary>
    public class Noise
    {

        //Fields:
        private int[] permutations;
        private Random generator;
        private double x;
        private double y;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// Default constructor for the Perlin Noise generator
        /// </summary>
        public Noise()
        {
            int[] halfPermutations = new int[255];
            int counter = 0;
            this.generator = new Random();

            //setting the initial values of the permutation table
            for (int i = 0; i < 255; i++)
            {
                halfPermutations[i] = i;
            }

            //shuffling the values within the array
            for (int i = halfPermutations.Length - 1; i > 0; i--)
            {
                //calculating a random index
                int index = (int)(generator.NextDouble() * (i - 1));

                //swapping the values with this current index and the random index
                halfPermutations[i] = halfPermutations[index];
                halfPermutations[index] = halfPermutations[i];
            }

            //initializing the real permutation table
            this.permutations = new int[halfPermutations.Length * 2];

            //copying the values to the real permutation table
            for (int i = 0; i < permutations.Length; i++)
            {
                //checking if the ocunter has gone out of range
                if (counter >= 255)
                {
                    counter = 0;
                }

                //assigning the permutation table values
                permutations[i] = halfPermutations[counter];

                //incrementing the counter
                counter++;
            }

            //assigning values to the x and y coordinates
            this.x = generator.NextDouble() * 10;
            this.y = generator.NextDouble() * 10;
        }

        //Methods:
        /// <summary>
        /// Performs the necessary calculations to find the y value in a fade algebraic function
        /// </summary>
        /// <param name="x">the x value in the function calculation</param>
        /// <returns>The y value or coordinate at the given x value of the algebraic function</returns>
        private float Fade(float x)
        {
            //Performing the necessary fade function calculations
            return ((6 * x - 15) * x + 10) * x * x * x;
        }

        /// <summary>
        /// Identifies a directional vector for the Perlin Noise calculation
        /// </summary>
        /// <param name="permutationValue">Current value in the permutation table</param>
        /// <returns>a direction vector</returns>
        private Vector2 ConstantVector(int permutationValue)
        {
            int randomDirection = permutationValue & 3;

            if (randomDirection == 0)
            {
                return new Vector2(1.0f, 1.0f);
            }
            else if (randomDirection == 1)
            {
                return new Vector2(-1.0f, 1.0f);
            }
            else if (randomDirection == 2)
            {
                return new Vector2(-1.0f, -1.0f);
            }
            else
            {
                return new Vector2(1.0f, -1.0f);
            }
        }

        /// <summary>
        /// Performs Linear Interpolation on 2 locational values
        /// </summary>
        /// <param name="distanceBetween">0-1 percent distance from one location to the next</param>
        /// <param name="location1">the first location having the value between found</param>
        /// <param name="location2">the second location having the value between found</param>
        /// <returns>the Linear Interpolation between the given locations</returns>
        private double Lerp(double distanceBetween, double location1, double location2)
        {
            return location1 + distanceBetween * (location2 - location1);
        }

        /// <summary>
        /// Calculated the next random value with the Perlin Noise algorithm
        /// </summary>
        /// <returns>the next value</returns>
        public double NextNoise()
        {
            //X and Y byte positions
            float X = (int)(Math.Floor(x)) & 255;
            float Y = (int)(Math.Floor(y)) & 255;

            //the difference of the x and rounded x
            float xf = (float)(x - Math.Floor(x));

            //the difference of the y and rounded y
            float yf = (float)(y - Math.Floor(y));

            //calculating the dot product between the selected corners in the Permutation table
            //  and corners of the local area
            double dotTopRight = Vector2.Dot(
                new Vector2(xf - 1.0f, yf - 1.0f),
                ConstantVector(permutations[(int)(permutations[(int)(X + 1)] + Y + 1)]));
            double dotTopLeft = Vector2.Dot(
                new Vector2(xf, yf - 1.0f),
                ConstantVector(permutations[(int)(permutations[(int)(X)] + Y + 1)]));

            double dotBottomRight = Vector2.Dot(
                new Vector2(xf - 1.0f, yf),
                ConstantVector(permutations[(int)(permutations[(int)(X + 1)] + Y)]));
            double dotBottomLeft = Vector2.Dot(
                new Vector2(xf, yf),
                ConstantVector(permutations[(int)(permutations[(int)(X)] + Y)]));

            //Calculates the fade value 
            float u = Fade(xf);
            float v = Fade(yf);

            //incrementing the containing x and y values
            x += 0.1f;
            y += 0.1f;

            return Lerp(
                u,
                Lerp(v, dotBottomLeft, dotTopLeft) * 20,
                Lerp(v, dotBottomRight, dotTopRight) * 20);
        }
    }
}
