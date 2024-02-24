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
        public Tile(Tiles tileType, Vectangle position)
        {
            this.tileType = tileType;
            this.position = position;

            if (tileType == Tiles.Empty)
            {
                this.color = Color.Purple;
            }
            if (tileType == Tiles.Collidable)
            {
                this.color = Color.Yellow;
            }
        }

        //Methods:
        public void Draw()
        {
            Globals.SB.Draw(
                Globals.GameTextures["Pixel"],
                position.ToRectangle,
                color);
        }



    }
}
