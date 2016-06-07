using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using App;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace HESOYAM_Production
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        private InputState inputState;
        private String rootDir;
        private Dictionary<String, Model> models;
        private Dictionary<String, Texture2D> textures;
        private GraphicsDeviceManager graphics;
        private HUD hud;

        public SpriteBatch spriteBatch;
        public Camera camera;
        public Player player;
        public Scene scene;
        public bool playMode;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            this.playMode = true;
            this.rootDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            this.models = new Dictionary<String, Model>();
            this.textures = new Dictionary<String, Texture2D>();
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
            this.hud = new HUD(this, this.inputState, this.textures);

            this.LoadModels();
            this.LoadTextures();

            scene = new Scene(
                this,
                "Scene_1",
                this.rootDir + "/Content/Map/scene_1",
                this.models,
                this.textures
            );

            Vector3 cameraMove = new Vector3(-500.0f, 500.0f, 500.0f);

            this.player = new Player(this, "Player", scene.Player.position);
            this.camera = new Camera(this, "Kamera", Vector3.Add(this.player.position, cameraMove));

            this.player.cameraAngle = (float) (Math.Atan2(cameraMove.X, cameraMove.Z));
            this.camera.lookAtParent = this.player;

            this.player.AddChild(this.camera);
            this.player.AddChild(scene.Player);
        }

        private void LoadModels()
        {
            String modelsDir = this.rootDir + "/Content/Models";

            String[] files = Directory.GetFiles(modelsDir);
            foreach (String file in files) {
                String name = file.Remove(0, modelsDir.Length + 1).Replace(".FBX", "");

                this.LoadModel(name);
            }
        }

        private void LoadModel(String name)
        {
            try {
                Model model = Content.Load<Model>("Models/" + name);

                this.models.Add(name, model);
            } catch (ContentLoadException e) {
                Console.WriteLine("Model '" + name + "' does not exists in Content.mgcb");
            }
        }

        private void LoadTextures()
        {
            String texturesDir = this.rootDir + "/Content/Textures";

            String[] files = Directory.GetFiles(texturesDir);
            foreach (String file in files) {
                String name = file.Remove(0, texturesDir.Length + 1).Replace(".png", "");

                this.LoadTexture(name);
            }
        }

        private void LoadTexture(String name)
        {
            Texture2D texture = Content.Load<Texture2D>("Textures/" + name);

            this.textures.Add(name, texture);
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

            PlayerIndex outPlayerIndex;
            if (this.inputState.IsNewKeyPress(Keys.F5, null, out outPlayerIndex)) {
                Program.debugMode = !Program.debugMode;
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

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);

            this.hud.Draw();
        }

        static float GameTimeFloat(GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
