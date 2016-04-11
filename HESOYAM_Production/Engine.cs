using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using App.Render;
using App;

namespace HESOYAM_Production
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //TODO: to remove
        Model myModel;
        Object3D[] testObjects = new Object3D[6];

        public Camera camera;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            this.camera = new Camera(this, new Vector3(0.0f, 50.0f, 5000.0f));

            //TODO: use this.Content to load your game content here
            myModel = Content.Load<Model>("Cube");

            for (int i = 0; i < 6; i++) {
                testObjects[i] = new Object3D(this, myModel, new Vector3(2 * i, 5 + i, 0));
                Components.Add(testObjects[i]);
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
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

            this.camera.update();

            //TODO: to remove
            for (int i = 0; i < 6; i++) {
                testObjects[i].Rotate(
                    GameTimeFloat(gameTime) * MathHelper.ToRadians(0.01f),
                    GameTimeFloat(gameTime) * MathHelper.ToRadians(0.1f),
                    0
                );
            }

            testObjects[0].Move(2, 0, 0);
            testObjects[1].Move(-2, 0, 0);
            testObjects[2].Move(0, 2, 0);
            testObjects[3].Move(0, -2, 0);
            testObjects[4].Move(0, 0, 2);
            testObjects[5].Move(0, 0, -2);

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
