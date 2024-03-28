
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakhaNata_Magic
{
    public abstract class GameObject
    {

        //Fields:
        protected Texture2D asset;
        protected Vectangle position;
        protected Color color;

        //Properties:
        /// <summary>
        /// Get/set access to the Position Vectangle
        /// </summary>
        public Vectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the GameObject class
        /// </summary>
        /// <param name="asset">Texture asset of the object</param>
        /// <param name="position">Vectangle position of the object</param>
        public GameObject(Texture2D asset, Vectangle position)
        {
            this.position = position;
            this.asset = asset;
            this.color = Color.White;
        }

        //Methods:
        /// <summary>
        /// Per frame logic update method for GameObjects
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// base Draw method for GameObjects
        /// </summary>
        public virtual void Draw()
        {
            Globals.SB.Draw(
                asset,
                position.ToRectangle,
                color);
        }
    }
}
