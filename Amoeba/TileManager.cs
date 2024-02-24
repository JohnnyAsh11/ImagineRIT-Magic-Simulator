using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Amoeba
{
    /// <summary>
    /// Loads in a level created with a CSV file
    /// </summary>
    public class TileManager
    {

        //Fields:
        private string filepath;
        private Tile[,] tiles;

        //properties:


        //Constructors:
        public TileManager(string filepath)
        {
            this.filepath = filepath;
            tiles = null;

        }

        //Methods:
        private void LoadLevel()
        {
            StreamReader reader = null!;

            try
            {
                //instantiating the StreamReader
                reader = new StreamReader(filepath);
                
            }
            catch (Exception e)
            {
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

    }
}
