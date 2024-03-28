using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MakhaNata_Magic
{
    public class Game1 : Game
    {

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            SeekerManager.Instance.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.SB.Begin();
            SeekerManager.Instance.Draw();
            Globals.SB.End();

            base.Draw(gameTime);
        }
    }
}