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

    public enum SpellStage
    {
        Stage1 = 0,
        Stage2 = 2,
        Stage3 = 4,
        Stage4 = 6
    }


    public class SpellManager
    {

        //Fields:
        private Dictionary<string, byte> spells;
        private Dictionary<string, Dictionary<SpellStage, Buttons>> inputSchemes;
        private float timer;
        private float spellTimer;
        private bool isSpellActive;
        private Buttons prevGPPress;

        /// <summary>
        /// whenever a new spell is cast, invoke this event passing in the associated spell enum
        /// </summary>
        public event OnSpellCast PlayerCastSpell;

        //Properties:

        //Constructors:
        public SpellManager()
        {
            timer = 0f;
            isSpellActive = false;
            spells = new Dictionary<string, byte>();
            inputSchemes = new Dictionary<string, Dictionary<SpellStage, Buttons>>();
            StreamReader reader = null!;

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
                    spells[splitData[0]] = 0;
                    inputSchemes[splitData[0]] = new Dictionary<SpellStage, Buttons>();

                    //populating the schemes
                    inputSchemes[splitData[0]][SpellStage.Stage1] = (Buttons)int.Parse(splitData[1]);
                    inputSchemes[splitData[0]][SpellStage.Stage2] = (Buttons)int.Parse(splitData[2]);
                    inputSchemes[splitData[0]][SpellStage.Stage3] = (Buttons)int.Parse(splitData[3]);
                    inputSchemes[splitData[0]][SpellStage.Stage4] = (Buttons)int.Parse(splitData[4]);
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
        public void Update(GamePadState gpState, GamePadState prevGPState)
        {
            if (!isSpellActive)
            {
                for (int i = 0; i < spells.Count; i++)
                {
                    Spell spell = (Spell)i;

                    //calling the SpellPolling method on all spells
                    if (SpellPolling(
                        spell.ToString(),
                        gpState,
                        prevGPState))
                    {
                        if (PlayerCastSpell != null)
                        {
                            PlayerCastSpell(spell);
                            isSpellActive = true;
                            spellTimer = 10;
                        }
                    }
                }
            }
            else
            {
                spellTimer -= Globals.DeltaTime;

                if (spellTimer < 0)
                {
                    spellTimer = 0;
                    isSpellActive = false;
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
            //if the passed in key is within the Dictionary
            if (spells.ContainsKey(spellName))
            {
                //getting the int index
                int indexCheck = FindFirstFalseIndex(spells[spellName]);

                //if the spell is already completed
                if (indexCheck > 6)
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
                    int changeIndices = (int)current;

                    //updating the 2 next indices of the byte
                    spells[spellName] = PackData(
                        PackData(spells[spellName], changeIndices, true), 
                        changeIndices + 1, 
                        true);

                    //saving the last key to be pressed
                    prevGPPress = inputSchemes[spellName][current];
                }
                else if (gpState != prevGPState)
                {
                    //if the matching input was not selected than reset the byte
                    spells[spellName] = 0;

                    //set the controller vibration
                    GamePad.SetVibration(PlayerIndex.One, 0.1f, 0.1f);

                    //and set the timer for the vibration to end
                    timer = 1;
                }
            }

            //returns whether or not the spell is completed
            return spells[spellName] == 0b11111111;
        }

        #region Bitpacking methods
        /// <summary>
        /// PROBLEM IN THIS METHOD::: FIX HERE FIRST
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int FindFirstFalseIndex(byte data)
        {
            //declaring a LCV
            int counter = 0;

            //looping through the 8 indices of a byte
            for (byte i = 0b10000000; i > 0; i >>= 1)
            {
                //if an index does not equal true
                if ((data & i) == 0)
                {
                    //break out of the loop
                    break;
                }

                //otherwise, increment the counter
                counter++;
            }

            //return the result of the counter
            return counter;
        }
        /// <summary>
        /// Checks true/false values held within bits
        /// </summary>
        /// <param name="data">Byte containing data</param>
        /// <param name="index">Index of bit being checked in byte</param>
        /// <returns>the value held at that index</returns>
        private bool ReadData(byte data, int index)
        {
            //checking index bounds
            if (index >= 0 && index <= 7)
            {
                //getting the index being checked in byte form
                byte indexChecked = (byte)(1 << index);

                //looping through all indices of a byte
                for (byte i = (byte)1 << 7; i > 0; i >>= 1)
                {
                    //if the current iteration matches the index we are attempting to check
                    if ((i & indexChecked) == indexChecked)
                    {
                        //check the data against i and return
                        // true or false accordingly
                        if ((data & i) == i)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            //default result
            return false;
        }

        /// <summary>
        /// Inserts true/false data into a bit
        /// </summary>
        /// <param name="data">Byte containing true/false data</param>
        /// <param name="index">Index of bit being changed</param>
        /// <param name="value">value of index being changed</param>
        /// <returns>The new Byte of data</returns>
        private byte PackData(byte data, int index, bool value)
        {
            //default value
            byte newData = 0;

            //checking to make sure index is in bounds
            if (index >= 0 && index <= 7)
            {
                //placing a 1 at the location being altered
                byte changes = (byte)(1 << index);

                //if the value is being changed to 0 then filp everything and change
                //operator to AND
                if (!value)
                {
                    changes = (byte)~changes;
                    newData = (byte)(data & changes);
                }
                else
                {
                    //changing the passed in data specifically with OR
                    newData = (byte)(data | changes);
                }
                //returning the new data
                return newData;
            }
            else
            {
                return newData;
            }
        }

        /// <summary>
        /// FOR BYTES WITH ONLY 1 TRUE VALUE
        /// </summary>
        /// <param name="data">byte being searched</param>
        /// <returns>the index of the single true value</returns>
        private int IndexAtTrue(byte data)
        {
            int index = 7;

            for (byte i = 1 << 7; i > 0; i >>= 1)
            {
                if ((data & i) == data)
                {
                    return index;
                }

                index--;
            }

            //default fail value
            return -1;
        }

        /// <summary>
        /// Checks if all data in the byte is true
        /// </summary>
        /// <param name="data">byte being checked</param>
        /// <returns>whether or not all values are true or not</returns>
        private bool IfAllTrue(byte data)
        {
            int trueCounter = 0;

            //loops 8 times for the length of a byte
            for (ushort i = 0; i < 8; i++)
            {
                //checks at each index if it is true
                if (ReadData(data, i))
                {
                    //if is true, up the counter
                    trueCounter++;
                }
            }

            //return whether or not the counter is 8
            return trueCounter >= 8;
        }
        #endregion

    }
}
