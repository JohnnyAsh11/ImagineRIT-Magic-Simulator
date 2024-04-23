using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MakhaNata_Magic
{
    public delegate void OnSpellCast(Spell castSpell);

    /// <summary>
    /// Tracks the current input stage of a spell
    /// </summary>
    public enum SpellStage
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4
    }

    /// <summary>
    /// Manages all of the input polling for the actual spell casting in the simulation
    /// </summary>
    public class SpellManager
    {
        //Fields:
        private Dictionary<string, byte> spells;
        private Dictionary<string, Dictionary<SpellStage, Buttons>> inputSchemes;
        private float timer;
        private float spellTimer;
        private bool isSpellActive;
        private Buttons prevGPPress;
        private List<Buttons> buttons;
        private string activeSpell;

        /// <summary>
        /// whenever a new spell is cast, invoke this event passing in the associated spell enum
        /// </summary>
        public event OnSpellCast PlayerCastSpell;

        //Properties: - NONE -

        //Constructors:
        /// <summary>
        /// Default constructor for the SpellManager class
        /// </summary>
        public SpellManager()
        {
            timer = 0f;
            isSpellActive = false;
            spells = new Dictionary<string, byte>();
            inputSchemes = new Dictionary<string, Dictionary<SpellStage, Buttons>>();
            StreamReader reader = null!;

            //initializing the list of Buttons with the enumeration values
            buttons = new List<Buttons>
            {
                Buttons.A,
                Buttons.X,
                Buttons.Y,
                Buttons.B,
                Buttons.DPadLeft,
                Buttons.DPadRight,
                Buttons.DPadUp,
                Buttons.DPadDown,
                Buttons.LeftThumbstickDown,
                Buttons.LeftThumbstickRight,
                Buttons.LeftThumbstickLeft,
                Buttons.LeftThumbstickUp,
                Buttons.RightThumbstickDown,
                Buttons.RightThumbstickRight,
                Buttons.RightThumbstickLeft,
                Buttons.RightThumbstickUp,
                Buttons.Back,
                Buttons.BigButton,
                Buttons.LeftShoulder,
                Buttons.RightShoulder,
                Buttons.LeftStick,
                Buttons.RightStick,
                Buttons.LeftTrigger,
                Buttons.RightTrigger
            };

            try
            {
                string rawData;
                string[] splitData;
                reader = new StreamReader("../../../InputSchemes.txt");

                //so long as there is data being loaded
                while ((rawData = reader.ReadLine()) != null)
                {
                    //split the data by it's seperation character
                    splitData = rawData.Split('|');

                    //Setting the dictionary values
                    if (!spells.ContainsKey(splitData[0]))
                    {
                        spells[splitData[0]] = 0b10000000;
                    }

                    if (!inputSchemes.ContainsKey(splitData[0]))
                    {
                        inputSchemes[splitData[0]] = new Dictionary<SpellStage, Buttons>();

                        //populating the schemes
                        inputSchemes[splitData[0]][SpellStage.Stage1] = (Buttons)int.Parse(splitData[1]);
                        inputSchemes[splitData[0]][SpellStage.Stage2] = (Buttons)int.Parse(splitData[2]);
                        inputSchemes[splitData[0]][SpellStage.Stage3] = (Buttons)int.Parse(splitData[3]);
                        inputSchemes[splitData[0]][SpellStage.Stage4] = (Buttons)int.Parse(splitData[4]);
                    }
                }
            }
            catch (Exception e)
            {
                //printing out the Exception's message to the debug window
                Debug.Write(e.Message);
            }
            finally
            {
                //if the reader is not null
                if (reader != null)
                {
                    //close the file stream
                    reader.Close();
                }
            }
        }

        //Methods:
        /// <summary>
        /// Per frame logic update method for the SpellManager class
        /// </summary>
        /// <param name="gpState">Current state of the controls input</param>
        /// <param name="prevGPState">Previous state of the controls input</param>
        /// <param name="spell">The player selected spell</param>
        public void Update(GamePadState gpState, GamePadState prevGPState, Spell spell)
        {
            //making sure that there is no spell already active
            if (!isSpellActive)
            {
                //calling the Spell Poll on the selected Spell
                if (SpellPolling(
                    spell.ToString(),
                    gpState,
                    prevGPState))
                {
                    //if the PlayerCastSpell event is not null
                    if (PlayerCastSpell != null)
                    {
                        //save the cast spell
                        activeSpell = spell.ToString();

                        //invoke it 
                        PlayerCastSpell(spell);

                        //set the Active Spell bool to true
                        isSpellActive = true;

                        //set the spell duration timer
                        spellTimer = 10;
                    }
                }
            }
            else
            {
                //decrementing the spellTimer by delta time
                spellTimer -= Globals.DeltaTime;

                //if delta time is goes under or is 0
                if (spellTimer < 0)
                {
                    //set it to zero
                    spellTimer = 0;

                    //set the active spell to false
                    isSpellActive = false;

                    //reseting the spell's byte
                    spells[activeSpell] = 0b10000000;

                    //invoking the casting event on the base state
                    PlayerCastSpell(Spell.Hiduun);
                }
            }

            //If the timer is greater than zero
            if (timer > 0)
            {
                //decrement it by DeltaTime
                timer -= Globals.DeltaTime;

                //if the timer goes under or at 0
                if (timer <= 0)
                {
                    //set it to zero
                    timer = 0;

                    //turn off controller vibration
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                }
            }
        }

        /// <summary>
        /// Polls the spells for their needed input and resets 
        /// their input string if conditions are not met
        /// </summary>
        /// <param name="spellName">The name of the spell being polled for</param>
        /// <param name="gpState">The current GamePad state</param>
        /// <param name="prevGPState">The previous GamePad state</param>
        /// <returns>Whether or not the spell was completed for casting</returns>
        private bool SpellPolling(string spellName, GamePadState gpState, GamePadState prevGPState)
        {
            if (spellName != "Hiduun")
            {
                //if the passed in key is within the Dictionary
                if (spells.ContainsKey(spellName))
                {
                    //getting the int index
                    int indexCheck = IndexAtTrue(spells[spellName]);

                    //if the spell is already completed
                    if (indexCheck >= 4)
                    {
                        //then return true
                        return true;
                    }

                    //Otherwise cast the int index to the SpellStage
                    SpellStage current = (SpellStage)indexCheck;

                    //Polling for the input needed from that spell's input scheme
                    if (gpState.IsButtonDown(inputSchemes[spellName][current]))
                    {
                        //if that input is pressed, update the associated
                        //  indices of that spell's byte
                        spells[spellName] >>= 1;

                        //saving the last key to be pressed
                        prevGPPress = inputSchemes[spellName][current];
                    }
                    else
                    {
                        foreach (Buttons button in buttons)
                        {
                            if (gpState.IsButtonDown(button) &&
                                button != prevGPPress &&
                                button != inputSchemes[spellName][current])
                            {
                                //if the matching input was not selected than reset the byte
                                spells[spellName] = 0b10000000;

                                //set the controller vibration
                                GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);

                                //and set the timer for the vibration to end
                                timer = 1;
                            }
                        }
                    }
                }

                //returns whether or not the spell is completed
                return spells[spellName] == 0b00001000;
            }
            else { return false; }
        }

        /// <summary>
        /// FOR BYTES WITH ONLY 1 TRUE VALUE
        /// </summary>
        /// <param name="data">byte being searched</param>
        /// <returns>the index of the single true value</returns>
        private int IndexAtTrue(byte data)
        {
            int index = 0;

            for (byte i = 1 << 7; i > 0; i >>= 1)
            {
                if ((data & i) == data)
                {
                    return index;
                }

                index++;
            }

            //default fail value
            return -1;
        }

    }
}
