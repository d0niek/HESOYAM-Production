using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using App.Render;

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
        Object3D testObject;

        public Engine ()
        {
            graphics = new GraphicsDeviceManager (this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize ()
        {
            // TODO: Add your initialization logic here

            base.Initialize ();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent ()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);

            //TODO: use this.Content to load your game content here
            myModel = Content.Load<Model> ("Cube");
            testObject = new Object3D (this, myModel);
            Components.Add (testObject);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update (GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            #if !__IOS__ &&  !__TVOS__
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();
            #endif

            // TODO: Add your update logic here
            // Allows the game to exit
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit ();

            //TODO: to remove
            testObject.Rotate (
                GameTimeFloat(gameTime) * MathHelper.ToRadians (0.01f),
                GameTimeFloat(gameTime) * MathHelper.ToRadians (0.1f),
                0);
            testObject.Move (
                GameTimeFloat(gameTime) * 0.3f, 
                GameTimeFloat(gameTime) * 0.1f,
                GameTimeFloat(gameTime) * 2f);

            base.Update (gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw (GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
            base.Draw (gameTime);
        }

        static float GameTimeFloat (GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public GraphicsDeviceManager Graphics ()
        {
            return this.graphics;
        }
    }
}

