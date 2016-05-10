using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using App;
using System.IO;

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
        public bool playMode;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            this.playMode = true;
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
            this.graphics.IsFullScreen = false;
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

            this.inputState = new InputState(this.GraphicsDevice);

            Vector3 cameraMove = new Vector3(-1500.0f, 2000.0f, 1500.0f);

            this.player = new Player(this, "Player", new Vector3(-1500.0f, 0.0f, 5000.0f));
            this.camera = new Camera(this, "Kamera", Vector3.Add(this.player.position, cameraMove));

            this.player.cameraAngle = (float) (Math.Atan2(cameraMove.X, cameraMove.Z));

            this.player.AddChild(this.camera);
            this.camera.lookAtParent = this.player;

            //TODO: use this.Content to load your game content here
            string parentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            Model wall = Content.Load<Model>("Models/sciana");
            Model door = Content.Load<Model>("Models/drzwi");
            Model window = Content.Load<Model>("Models/okno");
            Scene scene = new Scene(
                              this,
                              "Scene01",
                              parentDir + "/Content/Map/walls32x32.bmp",
                              wall,
                              door,
                              window
                          );

            Model wheelchair = Content.Load<Model>("Models/wozek");
            GameObject testObjects = new GameObject(this, "ObjectName_", wheelchair);
            Components.Add(testObjects);

            player.AddChild(testObjects);
            testObjects.position = this.player.position;
        }

        public void AddComponent(IGameComponent item)
        {
            Components.Add(item);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.inputState.Update();

            if (this.inputState.IsSpace(PlayerIndex.One)) {
                this.playMode = !this.playMode;
            }

            if (this.playMode) {
                this.camera.position = this.camera.playModePosition;
                this.player.update(gameTime, this.inputState);
            }

            this.camera.update(this.inputState);

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
