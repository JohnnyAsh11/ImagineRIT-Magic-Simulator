using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Handles all the UI input for the player to select spells
    /// </summary>
    public class SpellUI
    {
        //Fields:
        private Texture2D[] hiduunSpellUI;
        private Texture2D[] yashmiSpellUI;
        private Texture2D[] hushumiSpellUI;
        private Texture2D[] jasicaSpellUI;
        private Texture2D[] yuruqSpellUI;

        private Color[] select;
        private Color[] description;
        private Spell chosenSpell;
        private GamePadState prevGPState;

        //Properties:
        /// <summary>
        /// Get property for the player's chosen spell
        /// </summary>
        public Spell ChosenSpell
        {
            get { return chosenSpell; }
        }

        //Constructors:
        /// <summary>
        /// Default constructor for the SpellUI class
        /// </summary>
        public SpellUI()
        {
            //the chosen spell by default is Hiduun
            chosenSpell = Spell.Hiduun;

            select = new Color[] 
            {
                Color.Red,
                Color.White,
                Color.White,
                Color.White,
                Color.White
            };

            description = new Color[] 
            {
                Color.Red,
                Color.DarkOrchid,
                Color.DarkOrchid,
                Color.DarkOrchid,
                Color.DarkOrchid
            };

            //the visual assets for the Yashmi spell
            yashmiSpellUI = new Texture2D[] 
            { 
                Globals.GameTextures["A"], 
                Globals.GameTextures["LeftTrigger"], 
                Globals.GameTextures["RightTrigger"], 
                Globals.GameTextures["Y"] 
            };

            //the visual assets for the Hushumi Spell
            hushumiSpellUI = new Texture2D[]
            {
                Globals.GameTextures["PadUp"],
                Globals.GameTextures["PadRight"],
                Globals.GameTextures["PadRight"],
                Globals.GameTextures["PadUp"]
            };

            //the visual assets for the Jasica Spell
            jasicaSpellUI = new Texture2D[]
            {
                Globals.GameTextures["X"],
                Globals.GameTextures["LeftBumper"],
                Globals.GameTextures["PadUp"],
                Globals.GameTextures["RightBumper"]
            };

            //the visual assets for the yuruq Spell
            yuruqSpellUI = new Texture2D[]
            {
                Globals.GameTextures["B"],
                Globals.GameTextures["A"],
                Globals.GameTextures["X"],
                Globals.GameTextures["B"]
            };

            //the visual assets for the hiduun Spell
            hiduunSpellUI = new Texture2D[]
            {
                Globals.GameTextures["A"]
            };
        }

        //Methods:
        /// <summary>
        /// Callable render method for the SpellUI data
        /// </summary>
        /// <param name="centerScreen">Vector containing the coordinates for the center of the screen</param>
        public void Draw(Vector2 centerScreen)
        {
            //hiduun
            DrawArray(
                "Hiduun", 
                hiduunSpellUI, 
                centerScreen, 
                50, 
                select[0], 
                "Creates new Hamrakytes \nfor you to cast spells",
                description[0]);

            //Yashmi
            DrawArray(
                "Yashmi", 
                yashmiSpellUI, 
                centerScreen, 
                200, 
                select[1], 
                "Uses your hamrakytes \nto defend you",
                description[1]);

            //Hushumi
            DrawArray(
                "Hushumi", 
                hushumiSpellUI, 
                centerScreen, 
                350, 
                select[2], 
                "Converts your hamrakytes \ninto sharp darts",
                description[2]);

            //Jasica
            DrawArray(
                "Jasica", 
                jasicaSpellUI, 
                centerScreen, 
                500, 
                select[3],
                "returns most of your \nhamrakytes to you",
                description[3]);

            //Yuruq
            DrawArray(
                "Yuruq", 
                yuruqSpellUI, 
                centerScreen, 
                650, 
                select[4], 
                "Causes your hamrakytes to \nreach extreme temperatures",
                description[4]);
        }

        /// <summary>
        /// Prints out the array of textures spaced from each other
        /// </summary>
        /// <param name="spellName">string name of the spell</param>
        /// <param name="textures">array of UI textures</param>
        /// <param name="centerScreen">Vector containing data for the center of the screen</param>
        /// <param name="offsetY">the Y offset for printing</param>
        private void DrawArray(string spellName, Texture2D[] textures, Vector2 centerScreen, int offsetY, Color tint, string directions, Color directionTint)
        {
            int offsetX = -350;

            //printing the name of the spell
            Globals.SB.DrawString(
                Globals.SF,
                spellName,
                new Vector2(centerScreen.X - 900, centerScreen.Y + offsetY),
                tint,
                0.0f,
                Vector2.Zero,
                0.6f,
                SpriteEffects.None,
                1f);

            //looping through the textures
            for (int i = 0; i < textures.Length; i++)
            {
                //rendering them
                Globals.SB.Draw(
                    textures[i],
                    new Vector2(centerScreen.X + offsetX, centerScreen.Y + offsetY - 15),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    0.3f,
                    SpriteEffects.None,
                    0.0f);

                //moving the offset over for the next control of the spell
                offsetX += 200;
            }

            Globals.SB.DrawString(
                Globals.SF,
                directions,
                new Vector2(centerScreen.X + 450, centerScreen.Y + offsetY + 30),
                directionTint,
                0.0f,
                Vector2.Zero,
                0.15f,
                SpriteEffects.None,
                0.0f);
        }

        /// <summary>
        /// Per frame logic update method for the SpellUI class
        /// </summary>
        /// <param name="gpState">GamePadState created in the main Update method</param>
        public void Update(GamePadState gpState)
        {
            //if the input is down
            if (gpState.IsButtonDown(Buttons.LeftThumbstickDown) &&
                prevGPState.IsButtonUp(Buttons.LeftThumbstickDown))
            {
                //loop through the array of colors
                for (int i = 0; i < select.Length; i++)
                {
                    //until the red color is found
                    if (select[i] == Color.Red &&
                        (i + 1) < select.Length)
                    {
                        //alter the color values
                        select[i] = Color.White;
                        select[i + 1] = Color.Red;

                        description[i] = Color.DarkOrchid;
                        description[i + 1] = Color.Red;

                        //setting the value of the chosen spell
                        int num = i + 1;
                        switch (num)
                        {
                            case 0:
                                chosenSpell = Spell.Hiduun;
                                break;
                            case 1:
                                chosenSpell = Spell.Yashmi;
                                break;
                            case 2:
                                chosenSpell = Spell.Hushumi;
                                break;
                            case 3:
                                chosenSpell = Spell.Jasica;
                                break;
                            case 4:
                                chosenSpell = Spell.Yuruq;
                                break;
                        }
                        break;
                    }
                }
            }
            //Check if the input is up
            else if (gpState.IsButtonDown(Buttons.LeftThumbstickUp) &&
                     prevGPState.IsButtonUp(Buttons.LeftThumbstickUp))
            {
                //similarly, loop until the red index is found
                for (int i = 0; i < select.Length; i++)
                {
                    //find the red index and also check if the previous index is in range
                    if (select[i] == Color.Red &&
                        (i - 1) >= 0)
                    {
                        //if so alter the color values
                        select[i] = Color.White;
                        select[i - 1] = Color.Red;

                        description[i] = Color.DarkOrchid;
                        description[i - 1] = Color.Red;

                        //setting the value of the chosen spell
                        int num = i - 1;
                        switch (num)
                        {
                            case 0:
                                chosenSpell = Spell.Hiduun;
                                break;
                            case 1:
                                chosenSpell = Spell.Yashmi;
                                break;
                            case 2:
                                chosenSpell = Spell.Hushumi;
                                break;
                            case 3:
                                chosenSpell = Spell.Jasica;
                                break;
                            case 4:
                                chosenSpell = Spell.Yuruq;
                                break;
                        }
                    }
                }

            }

            prevGPState = gpState;
        }
    }
}
