using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MakhaNata_Magic
{
    public class Game1 : Game
    {

        private List<Host> hosts;

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
        }

        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            Globals.GameTextures = gameTextures;

            hosts = new List<Host>();

            for (uint i = 0; i < 20; i++)
            {
                hosts.Add(new Host());
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.GameTime = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
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