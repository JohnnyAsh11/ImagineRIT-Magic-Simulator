using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MakhaNata_Magic
{
    /// <summary>
    /// Tracks the states of the simulation
    /// </summary>
    public enum SimState
    {
        Menu,
        Simulation,
        Pause
    }


    public class Game1 : Game
    {

        private List<Host> hosts;

        private SimState simState;
        private KeyboardState prevKBState; 
        private GamePadState prevGPState;
        private SpriteFont simFont;
        private Vector2 centerScreen;
        private const uint hostCap = 40;
        private byte spellByte;

        private SpellManager spellManager;

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //allowing the user to resize the window and subscribing the callback method for window resizing
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize()
        {
            base.Initialize();

            //setting up some starting values
            simState = SimState.Menu;
            centerScreen = new Vector2((Globals.Graphics.GraphicsDevice.Viewport.Width / 2), 0);
            spellByte = 0;
        }

        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            //loading the font
            simFont = Content.Load<SpriteFont>("BungeeSpice");
            
            //loading textures into the gameTextures dictionary
            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            gameTextures["StartButton"] = Content.Load<Texture2D>("StartImage");
            gameTextures["BackButton"] = Content.Load<Texture2D>("BackImage");

            //giving the dictionary to the Globals class
            Globals.GameTextures = gameTextures;

            //creating the starting hosts for the simulation
            hosts = new List<Host>();
            for (uint i = 0; i < 10; i++)
            {
                hosts.Add(new Host());
            }

            spellManager = new SpellManager();
        }

        protected override void Update(GameTime gameTime)
        {
            //Setting control variable values
            KeyboardState kbState = Keyboard.GetState();
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);
            Globals.GameTime = gameTime;

            switch (simState)
            {
                case SimState.Menu:

                    //polling for input to a state change
                    if (SingleKeyPress(kbState, Keys.Enter) ||
                        SingleControllerPress(gpState, Buttons.A))
                    {
                        simState = SimState.Simulation;
                    }

                    #region controller keybind testing
                    //GamePadState controllerState = GamePad.GetState(PlayerIndex.One);

                    //int moveSpeed = 5;
                    //if (controllerState.IsButtonDown(Buttons.LeftThumbstickUp))
                    //{
                    //    controllerTesting.Y -= moveSpeed;
                    //    GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                    //}
                    //else if (controllerState.IsButtonDown(Buttons.LeftThumbstickDown))
                    //{
                    //    controllerTesting.Y += moveSpeed;
                    //    GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                    //}
                    //else if (controllerState.IsButtonDown(Buttons.LeftThumbstickLeft))
                    //{
                    //    controllerTesting.X -= moveSpeed;
                    //    GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                    //}
                    //else if (controllerState.IsButtonDown(Buttons.LeftThumbstickRight))
                    //{
                    //    controllerTesting.X += moveSpeed;
                    //    GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                    //}
                    //else
                    //{
                    //    GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                    //}
                    #endregion

                    break;
                case SimState.Simulation:

                    spellManager.Update(gpState, prevGPState);

                    //Updates for the simulation state
                    foreach (Host host in hosts)
                    {
                        host.Update();
                    }

                    //polling for input into a state change
                    if (SingleKeyPress(kbState, Keys.Enter) || 
                        SingleControllerPress(gpState, Buttons.Start))
                    {
                        simState = SimState.Pause;
                    }
                    if (SingleKeyPress(kbState, Keys.Space) || 
                        SingleControllerPress(gpState, Buttons.A))
                    {
                        hosts.Add(new Host());

                        //if the hosts go over the cap
                        if (hosts.Count > hostCap)
                        {
                            //remove the first one
                            hosts.RemoveAt(0);
                        }
                    }

                    break;
                case SimState.Pause:

                    //polling for input
                    if (SingleKeyPress(kbState, Keys.Enter) || 
                        SingleControllerPress(gpState, Buttons.B) || 
                        SingleControllerPress(gpState, Buttons.Start))
                    {
                        //if enter was pressed, return to the sim
                        simState = SimState.Simulation;
                    }
                    else if (SingleKeyPress(kbState, Keys.Space) || 
                             SingleControllerPress(gpState, Buttons.Back))
                    {
                        //if space was pressed, return to the main menu
                        Reset();
                        simState = SimState.Menu;
                    }

                    break;

            }

            prevGPState = gpState;
            prevKBState = kbState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.SB.Begin();

            switch (simState)
            {
                case SimState.Menu:

                    Globals.SB.DrawString(
                        simFont,
                        "Blood Magic Simulator",
                        new Vector2(centerScreen.X - 375, 10),
                        Color.Red,
                        0.0f,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                    Globals.SB.DrawString(
                        simFont,
                        "Press A to begin",
                        new Vector2(centerScreen.X - 200, 100),
                        Color.DarkOrchid,
                        0.0f,
                        Vector2.Zero,
                        0.8f,
                        SpriteEffects.None,
                        0.0f);

                    break;
                case SimState.Simulation:

                    foreach (Host host in hosts)
                    {
                        host.Draw();
                    }

                    Globals.SB.Draw(
                        Globals.GameTextures["StartButton"],
                        new Vector2(centerScreen.X - 175, centerScreen.Y - 10),
                        null,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        0.15f,
                        SpriteEffects.None,
                        1f);
                    Globals.SB.DrawString(
                        simFont,
                        "Press     to view spells",
                        new Vector2(centerScreen.X - 350, centerScreen.Y),
                        Color.DarkOrchid,
                        0.0f,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);

                    break;
                case SimState.Pause:



                    break;
            }
            Globals.SB.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks for a single key press from the user
        /// </summary>
        /// <param name="kbState">The current iteration's keyboard state</param>
        /// <param name="key">the key being checked</param>
        /// <returns>Whether or not a single press of the key occurred</returns>
        private bool SingleKeyPress(KeyboardState kbState, Keys key)
        {
            return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks for a single controller press from the user
        /// </summary>
        /// <param name="controllerState">The current iteration's controller state</param>
        /// <param name="button">the button being checked</param>
        /// <returns>Whether or not a single press of the controller occurred</returns>
        private bool SingleControllerPress(GamePadState controllerState, Buttons button)
        {
            return controllerState.IsButtonDown(button) && prevGPState.IsButtonUp(button);
        }

        /// <summary>
        /// Resets the game to its starting Host count and clears the previous hosts
        /// </summary>
        private void Reset()
        {
            hosts.Clear();
            for (uint i = 0; i < 10; i++)
            {
                hosts.Add(new Host());
            }
        }

        /// <summary>
        /// Callback method for when the window is resized
        /// </summary>
        private void OnResize(Object sender, EventArgs e)
        {
            //Applying changes to the viewport if the window has changed dimensions
            if ((Globals.Graphics.PreferredBackBufferWidth != Globals.Graphics.GraphicsDevice.Viewport.Width) ||
                (Globals.Graphics.PreferredBackBufferHeight != Globals.Graphics.GraphicsDevice.Viewport.Height))
            {
                Globals.Graphics.PreferredBackBufferWidth = Globals.Graphics.GraphicsDevice.Viewport.Width;
                Globals.Graphics.PreferredBackBufferHeight = Globals.Graphics.GraphicsDevice.Viewport.Height;
                Globals.Graphics.ApplyChanges();
            }

            //changing the position of the top center
            centerScreen.X = (Globals.Graphics.GraphicsDevice.Viewport.Width / 2);
        }
    }
}