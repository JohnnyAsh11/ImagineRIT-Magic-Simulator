using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Amoeba
{

    public enum Tiles
    {
        Empty,
        Collidable
    }

    /// <summary>
    /// Loads in a level created with a CSV file
    /// </summary>
    public class TileManager
    {

        //Fields:
        private string filepath;
        private Tile[,] tiles;
        private int height;
        private int width;

        //properties:


        //Constructors:
        public TileManager(string filepath)
        {
            this.filepath = filepath;
            tiles = null;
            this.height = 0;
            this.width = 0;

            LoadLevel();
        }

        //Methods:
        private void LoadLevel()
        {
            StreamReader reader = null!;
            string[] splitData;
            string data;
            int row = 0;

            try
            {
                //positional variables for the tiles
                int screenHeight = Globals.Graphics.GraphicsDevice.Viewport.Height;
                int screenWidth = Globals.Graphics.GraphicsDevice.Viewport.Width;
                int tileWidth;
                int tileHeight;
                int tileX = 0;
                int tileY = 0;

                //instantiating the StreamReader
                reader = new StreamReader(filepath);

                //getting the level height / width
                splitData = reader.ReadLine().Split(',');

                //index 1 is the width
                width = int.Parse(splitData[1]);

                //index 2 is the height
                height = int.Parse(splitData[2]);

                //instantiating the 2D array of tiles
                tiles = new Tile[height, width];

                //defining the width/height of tiles
                tileWidth = screenWidth / width;
                tileHeight = screenHeight / height;

                //reading in the data
                while ((data = reader.ReadLine()!) != null)
                {
                    //spliting the data
                    splitData = data.Split(',');

                    //looping through the columns
                    for (int col = 0; col < width; col++)
                    {
                        Tile tile = null;
                        Vectangle position = new Vectangle(
                            tileX,
                            tileY,
                            tileWidth,
                            tileHeight);

                        if (splitData[col] == "e")
                        {
                            tile = new Tile(Tiles.Empty, position);
                        }
                        else if (splitData[col] == "c")
                        {
                            tile = new Tile(Tiles.Collidable, position);
                        }
                        else if (splitData[col] == "E")
                        {
                            //create an enemy at the location
                            //this tile will likely also be empty behind the scenes
                            tile = new Tile(Tiles.Empty, position);
                        }

                        //giving the array a reference to the tile
                        tiles[row, col] = tile;

                        //moving to the next X position
                        tileX += tileWidth;
                    }

                    //moving to the next Y position
                    tileY += tileHeight;

                    //reseting the tile X location
                    tileX = 0;

                    //incrementing the LCV
                    row++;
                }

            }
            catch (Exception e)
            {
                //doing something meaningful with the exception
                Debug.Print(e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Publicly accessible method to set the filepath field to a new file and instantiate a new level
        /// </summary>
        /// <param name="filepath">filepath to the next level</param>
        public void NextLevel(string filepath)
        {
            this.filepath = filepath;
            this.LoadLevel();
        }


        public void Draw()
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    tiles[row, col].Draw();
                }
            }
        }
    }
}
