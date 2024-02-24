using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Amoeba
{
    /// <summary>
    /// Custom Rectangle struct using only Float values in order to better perform vector based
    /// calculations for GameObject positions
    /// </summary>
    public struct Vectangle
    {
        //Fields:
        private float x;
        private float y;
        private float width;
        private float height;

        //Properties:
        /// <summary>
        /// Essentially casting the Vectangle to a MonoGame Rectangle
        /// </summary>
        public Rectangle ToRectangle
        {
            get
            {
                //returning a MonoGame Rectangle
                return new Rectangle(
                    (int)x,
                    (int)y,
                    (int)width,
                    (int)height);
            }
        }

        /// <summary>
        /// Get/sets the Vector2 coordinates of the Vectangle
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(x, y); }
            set 
            { 
                x = value.X;
                y = value.Y;
            }
        }

        /// <summary>
        /// Get/set for the Height of the Vectangle
        /// </summary>
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Get/set for the Width of the Vectangle
        /// </summary>
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets/sets the X coordinate of the Vectangle
        /// </summary>
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Get/set the Y coordinate of the Vectangle
        /// </summary>
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Static property to get an empty Vectangle
        /// </summary>
        public static Vectangle Empty
        {
            get { return new Vectangle(0, 0, 0, 0); }
        }

        #region PRIVATE PROPERTIES FOR CONTAINS METHOD
        /// <summary>
        /// Calculates the top right of the Vectangle
        /// </summary>
        private float TopRight { get { return x + width; } }
        /// <summary>
        /// Calculates the bottom left of the Vectangle
        /// </summary>
        private float BottomLeft { get { return x + height; } }
        #endregion

        //Constructors:
        /// <summary>
        /// Parameterized constructor containing width/height
        /// </summary>
        /// <param name="x">Top left X position of the Vectangle</param>
        /// <param name="y">Top left Y position of the Vectangle</param>
        /// <param name="width">width of the Vectangle</param>
        /// <param name="height">height of the Vectangle</param>
        public Vectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Parameterized constructor containing default width/height of 100
        /// </summary>
        /// <param name="x">Top left X position</param>
        /// <param name="y">Top left Y position</param>
        public Vectangle(float x, float y)
        {
            this.x = x;
            this.y = y;
            width = 100f;
            height = 100f;
        }

        /// <summary>
        /// Parameterized constructor containing Vector2s instead of individual float values
        /// </summary>
        /// <param name="location">Contains top left X and Y positioning</param>
        /// <param name="size">Contains width/height information</param>
        public Vectangle(Vector2 location, Vector2 size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.X;
            this.height = size.Y;
        }

        //Methods:
        /// <summary>
        /// Checks if the Vectangle contains a point
        /// </summary>
        /// <param name="location">Point being checked</param>
        /// <returns>Whether or not the Point is in the Vectangle</returns>
        public bool Contains(Point location)
        {
            return location.X < this.TopRight && location.X > x &&
                   location.Y < this.BottomLeft && location.Y > y;
        }

        /// <summary>
        /// Checks if the Vectangle contains a Vector2
        /// </summary>
        /// <param name="location">Vector being checked</param>
        /// <returns>Whether or not the Point is in the Vectangle</returns>
        public bool Contains(Vector2 location)
        {
            return location.X < this.TopRight && location.X > x &&
                   location.Y < this.BottomLeft && location.Y > y;
        }

        /// <summary>
        /// Performs an AABB collision check against a MonoGame Rectangle
        /// </summary>
        /// <param name="rect">Rectangle being checked against</param>
        /// <returns>whether or not a collision has occured</returns>
        public bool Intersects(Rectangle rect)
        {
            //Determining whether a collision has occurred
            return rect.X < (this.x + this.width) &&
                   (rect.X + rect.Width) > this.x &&
                   rect.Y < (this.y + this.height) &&
                   (rect.Y + rect.Height) > this.y;
        }

        /// <summary>
        /// Performs an AABB collision check against another Vectangle
        /// </summary>
        /// <param name="vect">Vectangle being checked against</param>
        /// <returns>whether or not a collision has occured</returns>
        public bool Intersects(Vectangle vect)
        {
            //Determining whether a collision has occurred
            return vect.X < (this.x + this.width) &&
                   (vect.X + vect.Width) > this.x &&
                   vect.Y < (this.y + this.height) &&
                   (vect.Y + vect.Height) > this.y;
        }

        //Operator Overloads:
        /// <summary>
        /// Adds the X and Y values of a Vector2 to the X and Y coordinates of the Vectangle
        /// </summary>
        /// <param name="rect">Rectangle having its values changed</param>
        /// <param name="changeInPosition">positional change</param>
        /// <returns>A Vectangle with the positional updates</returns>
        public static Vectangle operator +(Vectangle rect, Vector2 changeInPosition)
        {
            //performing the proper arithmetic to the Position Vector within the Vectangle
            float newX = rect.x + changeInPosition.X;
            float newY = rect.y + changeInPosition.Y;

            //returning the new Vectangle with the new Position Vector
            return new Vectangle(newX, newY, rect.Width, rect.Height);
        }

        /// <summary>
        /// Adds the X and Y values of a Vector2 to the X and Y coordinates of the Vectangle
        /// </summary>
        /// <param name="rect">Rectangle having its values changed</param>
        /// <param name="changeInPosition">positional change</param>
        /// <returns>A Vectangle with the positional updates</returns>
        public static Vectangle operator -(Vectangle rect, Vector2 changeInPosition)
        {
            //performing the proper arithmetic to the Position Vector within the Vectangle
            float newX = rect.x - changeInPosition.X;
            float newY = rect.y - changeInPosition.Y;

            //returning the new Vectangle with the new Position Vector
            return new Vectangle(newX, newY, rect.Width, rect.Height);
        }
    }
}