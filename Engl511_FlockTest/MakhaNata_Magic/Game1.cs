using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //changing the window sizing to be our preferred size
            Globals.Graphics.PreferredBackBufferWidth = 800;
            Globals.Graphics.PreferredBackBufferHeight = 800;
            
            //Globals.Graphics.IsFullScreen = true;
            Globals.Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            simState = SimState.Menu;
        }

        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            Globals.GameTextures = gameTextures;

            hosts = new List<Host>();

            for (uint i = 0; i < 1; i++)
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

                    //Updates for the Menu state

                    if (kbState.IsKeyDown(Keys.Space))
                    {
                        simState = SimState.Simulation;
                    }

                    break;
                case SimState.Simulation:

                    //Updates for the simulation state

                    break;
                case SimState.Pause:

                    //updates for the pause state

                    break;

            }

            foreach (Host host in hosts)
            {
                host.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.SB.Begin();
            foreach (Host host in hosts)
            {
                host.Draw();
            }
            Globals.SB.End();

            base.Draw(gameTime);
        }
    }
}