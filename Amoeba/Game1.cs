using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using PerlinGenerator;

namespace Amoeba
{
    public class Game1 : Game
    {

        private Host player;
        private TileManager tileManager;
        private Noise noiseGenerator;

        private List<Vector2> positions;
        private float xPos;
        private float offset;

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //changing the window sizing to be our preferred size
            Globals.Graphics.PreferredBackBufferWidth = 1000;
            Globals.Graphics.PreferredBackBufferHeight = 1000;

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
            Globals.DebugFont = Content.Load<SpriteFont>("Arial40");

            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();

            gameTextures["Pixel"] = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
            gameTextures["Pixel"].SetData<Color>(new Color[1] { Color.White });

            Globals.GameTextures = gameTextures;

            player = new Host(500);
            //tileManager = new TileManager("../../../TestLevel.csv");
            noiseGenerator = new Noise();
            positions = new List<Vector2>();
            xPos = 0;
            offset = 25;

            //player.GetCollidableTiles += tileManager.GetCollisionTiles;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            Globals.GameTime = gameTime;

            float yPos = (float)(noiseGenerator.NextNoise()) + offset;
            xPos += 1;

            if (xPos > Globals.Graphics.GraphicsDevice.Viewport.Width)
            {
                xPos = 0;
                offset += 25;
            }

            positions.Add(new Vector2(xPos, yPos));

            //player.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.SB.Begin();

            for (int i = 0; i < positions.Count; i++)
            {
                Globals.SB.Draw(
                    Globals.GameTextures["Pixel"],
                    positions[i],
                    Color.White);
            }
            //tileManager.Draw();
            //player.Draw();

            Globals.SB.End();

            base.Draw(gameTime);
        }
    }
}