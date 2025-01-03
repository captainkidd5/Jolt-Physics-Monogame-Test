using JoltPhysicsSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace JoltMonogameTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Camera Camera { get; private set; }

        public static JoltSim JoltSim;

        public static Random Random = new Random();

        private static KeyboardState _oldKeyBoardState;
        private static KeyboardState _newKeyBoardState;

        public static RayDrawer RayDrawer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Camera = new Camera();
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            JoltSim = new Tests();
            JoltSim.Initialize(GraphicsDevice);

            RayDrawer = new RayDrawer();
            RayDrawer.Initialize(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyBoardState = _newKeyBoardState;
            _newKeyBoardState = Keyboard.GetState();

            Camera.Update(gameTime);



            JoltSim.Update(gameTime);
            base.Update(gameTime);

        }

        public static bool IsKeyPressed(Keys key)
        {
            if (_newKeyBoardState.IsKeyDown(key))
                return true;

            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            JoltSim.Draw();
            base.Draw(gameTime);
        }
    }

}
