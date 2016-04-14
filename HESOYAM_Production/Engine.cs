using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using App.Render;
using App;
using System.Diagnostics;

namespace HESOYAM_Production
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        private InputState inputState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;
        public Player player;

        //TODO: to remove
        Model myModel;
        Object3D[] testObjects = new Object3D[102];

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.inputState = new InputState();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.player = new Player(this, "Player");
            this.camera = new Camera(this, "Kamera", new Vector3(-1500.0f, 2000.0f, 1500.0f));

            this.player.AddChild(this.camera);
            this.camera.lookAtParent = this.player;

            //TODO: use this.Content to load your game content here
            myModel = Content.Load<Model>("Cube");

            for (int i = 0; i < 102; i++) {
                testObjects[i] = new Object3D(
                    this, myModel, "ObjectName_" + i, new Vector3(300 * (i / 10), -300, 300 * (i % 10))
                );
                Components.Add(testObjects[i]);
            }

            this.player.AddChild(testObjects[100]);
            testObjects[100].AddChild(testObjects[101]);
            testObjects[100].position = this.player.position;
            testObjects[101].position = this.player.position;
            testObjects[101].Move(200f,0,0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.inputState.Update();

            this.player.update(gameTime, this.inputState);
            this.camera.update(GraphicsDevice.Viewport.AspectRatio);

            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            #if !__IOS__ &&  !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            #endif

            // TODO: Add your update logic here
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        static float GameTimeFloat(GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public GraphicsDeviceManager Graphics()
        {
            return this.graphics;
        }
    }
}
