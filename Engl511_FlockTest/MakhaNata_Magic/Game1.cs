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
        private const uint hostCap = 20;
        private byte spellByte;

        private SpellUI uiManager;
        private SpellManager spellManager;

        /// <summary>
        /// the Game1 class's default constructor
        /// </summary>
        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //allowing the user to resize the window and subscribing the callback method for window resizing
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        /// <summary>
        /// MonoGame's Initialize method
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            //setting up some starting values
            simState = SimState.Menu;
            centerScreen = new Vector2((Globals.Graphics.GraphicsDevice.Viewport.Width / 2), 0);
            spellByte = 0;
        }

        /// <summary>
        /// MonoGame's Content load method
        /// </summary>
        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            //loading the font
            simFont = Content.Load<SpriteFont>("BungeeSpice");
            Globals.SF = simFont;
            
            //loading textures into the gameTextures dictionary
            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            gameTextures["StartButton"] = Content.Load<Texture2D>("StartImage");
            gameTextures["BackButton"] = Content.Load<Texture2D>("BackImage");
            gameTextures["PadRight"] = Content.Load<Texture2D>("DPad_Right");
            gameTextures["PadUp"] = Content.Load<Texture2D>("DPad_Up");
            gameTextures["A"] = Content.Load<Texture2D>("A_Button");
            gameTextures["Y"] = Content.Load<Texture2D>("Y_Button");
            gameTextures["RightTrigger"] = Content.Load<Texture2D>("Trigger_Right");
            gameTextures["LeftTrigger"] = Content.Load<Texture2D>("Trigger_Left");
            gameTextures["X"] = Content.Load<Texture2D>("X_Button");
            gameTextures["B"] = Content.Load<Texture2D>("B_Button");
            gameTextures["LeftBumper"] = Content.Load<Texture2D>("Bumper_Left");
            gameTextures["RightBumper"] = Content.Load<Texture2D>("Bumper_Right");

            //giving the dictionary to the Globals class
            Globals.GameTextures = gameTextures;
            
            spellManager = new SpellManager();
            uiManager = new SpellUI();

            //creating the starting hosts for the simulation
            hosts = new List<Host>();
            for (uint i = 0; i < 10; i++)
            {
                Host host = new Host();
                hosts.Add(host);

                //subscribing all Hosts to the spell change event
                spellManager.PlayerCastSpell += host.SetCurrentSpell;
            }
        }

        /// <summary>
        /// MonoGame's Update logic method
        /// </summary>
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

                    break;
                case SimState.Simulation:

                    spellManager.Update(gpState, prevGPState, uiManager.ChosenSpell);

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
                    if (uiManager.ChosenSpell == Spell.Hiduun &&
                        SingleControllerPress(gpState, Buttons.A))
                    {
                        Host host = new Host();
                        hosts.Add(host);

                        //subscribing all Hosts to the spell change event
                        spellManager.PlayerCastSpell += host.SetCurrentSpell;

                        //if the hosts go over the cap
                        if (hosts.Count > hostCap)
                        {
                            //unsubscribing the first host from the spell manager
                            spellManager.PlayerCastSpell -= hosts[0].SetCurrentSpell;

                            //remove the first host from the list
                            hosts.RemoveAt(0);
                        }
                    }

                    break;
                case SimState.Pause:

                    uiManager.Update(gpState);

                    //polling for input
                    if (SingleKeyPress(kbState, Keys.Enter) || 
                        SingleControllerPress(gpState, Buttons.B) || 
                        SingleControllerPress(gpState, Buttons.Start) || 
                        SingleControllerPress(gpState, Buttons.A))
                    {
                        //if enter was pressed, return to the sim
                        simState = SimState.Simulation;
                    }
                    else if (SingleKeyPress(kbState, Keys.Space)/* || 
                             SingleControllerPress(gpState, Buttons.Back)*/)
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

        /// <summary>
        /// MonoGame's Draw method
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.SB.Begin();

            switch (simState)
            {
                case SimState.Menu:

                    //Drawing the menu UI
                    Globals.SB.DrawString(
                        simFont,
                        "Blood Magic Simulator",
                        new Vector2(centerScreen.X - 850, centerScreen.Y),
                        Color.Red,
                        0.0f,
                        Vector2.Zero,
                        0.7f,
                        SpriteEffects.None,
                        0.0f);
                    Globals.SB.DrawString(
                        simFont,
                        "Press A to begin",
                        new Vector2(centerScreen.X - 500, centerScreen.Y + 150),
                        Color.DarkOrchid,
                        0.0f,
                        Vector2.Zero,
                        0.6f,
                        SpriteEffects.None,
                        0.0f);

                    break;
                case SimState.Simulation:

                    //Looping through the hosts
                    foreach (Host host in hosts)
                    {
                        //rendering them to the window
                        host.Draw();
                    }

                    //Drawing the UI elements for the Simulation state
                    Globals.SB.Draw(
                        Globals.GameTextures["StartButton"],
                        new Vector2(centerScreen.X - 175, Globals.Graphics.GraphicsDevice.Viewport.Height - 115),
                        null,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        0.2f,
                        SpriteEffects.None,
                        1f);
                    Globals.SB.DrawString(
                        simFont,
                        "Press     to pick spells",
                        new Vector2(centerScreen.X - 400, Globals.Graphics.GraphicsDevice.Viewport.Height - 100),
                        Color.DarkOrchid,
                        0.0f,
                        Vector2.Zero,
                        0.4f,
                        SpriteEffects.None,
                        0.0f);

                    break;
                case SimState.Pause:

                    //Calling the UI manager's Draw method
                    uiManager.Draw(centerScreen);

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

            Globals.SeekerCenter = new Vector2(
                Globals.Graphics.GraphicsDevice.Viewport.Width / 2,
                Globals.Graphics.GraphicsDevice.Viewport.Height / 2);

            //changing the position of the top center
            centerScreen.X = (Globals.Graphics.GraphicsDevice.Viewport.Width / 2);
        }
    }
}