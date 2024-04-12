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
        private SpriteFont simFont;

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize()
        {
            base.Initialize();

            simState = SimState.Menu;
        }

        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            //loading the font
            simFont = Content.Load<SpriteFont>("BungeeSpice");

            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            Globals.GameTextures = gameTextures;

            hosts = new List<Host>();

            for (uint i = 0; i < 10; i++)
            {
                hosts.Add(new Host());
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            Globals.GameTime = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                kbState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (simState)
            {
                case SimState.Menu:

                    //polling for input to a state change
                    if (SingleKeyPress(kbState, Keys.Enter))
                    {
                        simState = SimState.Simulation;
                    }

                    break;
                case SimState.Simulation:

                    //Updates for the simulation state
                    foreach (Host host in hosts)
                    {
                        host.Update();
                    }

                    //polling for input into a state change
                    if (SingleKeyPress(kbState, Keys.Enter))
                    {
                        simState = SimState.Pause;
                    }

                    break;
                case SimState.Pause:

                    //polling for input
                    if (SingleKeyPress(kbState, Keys.Enter))
                    {
                        //if enter was pressed, return to the sim
                        simState = SimState.Simulation;
                    }
                    else if (SingleKeyPress(kbState, Keys.Space))
                    {
                        //if space was pressed, return to the main menu
                        simState = SimState.Menu;
                    }

                    break;

            }

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
                        "Hilael Blood Magic Simulator", 
                        Vector2.Zero, 
                        Color.Blue, 
                        0.0f, 
                        Vector2.Zero, 
                        1.0f,
                        SpriteEffects.None,
                        0.0f);

                    break;
                case SimState.Simulation:

                    foreach (Host host in hosts)
                    {
                        host.Draw();
                    }

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

        public void OnResize(Object sender, EventArgs e)
        {
            if ((Globals.Graphics.PreferredBackBufferWidth != Globals.Graphics.GraphicsDevice.Viewport.Width) ||
                (Globals.Graphics.PreferredBackBufferHeight != Globals.Graphics.GraphicsDevice.Viewport.Height))
            {
                Globals.Graphics.PreferredBackBufferWidth = Globals.Graphics.GraphicsDevice.Viewport.Width;
                Globals.Graphics.PreferredBackBufferHeight = Globals.Graphics.GraphicsDevice.Viewport.Height;
                Globals.Graphics.ApplyChanges();
            }
        }
    }
}