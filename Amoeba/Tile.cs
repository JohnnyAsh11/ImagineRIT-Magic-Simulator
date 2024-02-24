using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amoeba
{
    /// <summary>
    /// Individual tiles in the world
    /// </summary>
    public class Tile
    {

        //fields:
        private Tiles tileType;
        private Vectangle position;
        private Color color;

        //properties:

        //Constructors:
        /// <summary>
        /// Parameterized constructor for the Tiles of the game
        /// </summary>
        /// <param name="tileType">The tile's type</param>
        /// <param name="position">The location that the tile is and it's size data</param>
        public Tile(Tiles tileType, Vectangle position)
        {
            this.tileType = tileType;
            this.position = position;

            if (tileType == Tiles.Empty)
            {
                this.color = Color.Black;
            }
            else if (tileType == Tiles.Collidable)
            {
                this.color = Color.Blue;
            }
            else if (tileType == Tiles.Enemy)
            {
                this.color = Color.Red;
            }
        }

        //Methods:
        /// <summary>
        /// Render method for the Tile class
        /// </summary>
        public void Draw()
        {
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                position.ToRectangle,
                color);
        }



    }
}
