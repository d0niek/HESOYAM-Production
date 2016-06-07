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
        InputState inputState;
        String rootDir;
        Dictionary<String, Model> models;
        Dictionary<String, Texture2D> textures;
        GraphicsDeviceManager graphics;
        HUD hud;
        SpriteBatch spriteBatch;
        Camera camera;
        Player player;
        Scene scene;
        bool playMode;

        public InputState InputState {
            get { return inputState; }
            private set { }
        }

        public Dictionary<String, Model> Models {
            get { return models; }
            private set { }
        }

        public Dictionary<String, Texture2D> Textures {
            get { return textures; }
            private set { }
        }

        public SpriteBatch SpriteBatch {
            get { return spriteBatch; }
            private set { }
        }

        public Camera Camera {
            get { return camera; }
            private set { }
        }

        public Scene Scene {
            get { return scene; }
            private set { }
        }

        public bool PlayMode {
            get;
            set;
        }

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            playMode = true;
            rootDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            models = new Dictionary<String, Model>();
            textures = new Dictionary<String, Texture2D>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.IsFullScreen = false;
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
            inputState = new InputState(GraphicsDevice);

            inputState = new InputState(this.GraphicsDevice);
            hud = new HUD(this);

            LoadModels();
            LoadTextures();

            scene = new Scene(
                this,
                "Scene_1",
                rootDir + "/Content/Map/scene_1"
            );

            Vector3 cameraMove = new Vector3(-500.0f, 500.0f, 500.0f);
            float cameraAngle = (float) (Math.Atan2(cameraMove.X, cameraMove.Z));

            player = new Player(this, "Player", cameraAngle, scene.Player.position);
            camera = new Camera(this, "Camera", Vector3.Add(player.position, cameraMove));

            camera.LookAtParent = player;

            player.AddChild(camera);
            player.AddChild(scene.Player);
        }

        private void LoadModels()
        {
            String modelsDir = rootDir + "/Content/Models";

            String[] files = Directory.GetFiles(modelsDir);
            foreach (String file in files) {
                String name = file.Remove(0, modelsDir.Length + 1).Replace(".FBX", "");

                LoadModel(name);
            }
        }

        private void LoadModel(String name)
        {
            try {
                Model model = Content.Load<Model>("Models/" + name);

                models.Add(name, model);
            } catch (ContentLoadException e) {
                Console.WriteLine("Model '" + name + "' does not exists in Content.mgcb");
            }
        }

        private void LoadTextures()
        {
            String texturesDir = rootDir + "/Content/Textures";

            String[] files = Directory.GetFiles(texturesDir);
            foreach (String file in files) {
                String name = file.Remove(0, texturesDir.Length + 1).Replace(".png", "");

                LoadTexture(name);
            }
        }

        private void LoadTexture(String name)
        {
            try {
                Texture2D texture = Content.Load<Texture2D>("Textures/" + name);

                textures.Add(name, texture);
            } catch (ContentLoadException e) {
                Console.WriteLine("Texture '" + name + "' does not exists in Content.mgcb");
            }
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
            inputState.Update();
            if (inputState.IsSpace(PlayerIndex.One)) {
                playMode = !playMode;
            }

            PlayerIndex outPlayerIndex;
            if (inputState.IsNewKeyPress(Keys.F5, null, out outPlayerIndex)) {
                Program.debugMode = !Program.debugMode;
            }

            if (playMode) {
                camera.position = camera.PlayModePosition;
                player.update();
            }

            camera.update();

            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            #if !__IOS__ &&  !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            #endif

            // TODO: Add your update logic here
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
                Exit();
            }

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
