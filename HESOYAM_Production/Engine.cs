using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using App;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using HESOYAM_Production.App;

namespace HESOYAM_Production
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        InputState inputState;
        String rootDir;
        Dictionary<String, SpriteFont> fonts;
        Dictionary<String, Model> models;
        Dictionary<String, Texture2D> textures;
        Dictionary<String, Effect> shaders;
        GraphicsDeviceManager graphics;
        HUD hud;
        Particles particles;
        Camera camera;
        Player player;
        Scene scene;
        RenderTarget2D renderTarget;

        public SpriteBatch spriteBatch;

        public InputState InputState {
            get { return inputState; }
            private set { }
        }

        public Dictionary<String, SpriteFont> Fonts {
            get { return fonts; }
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

        public Dictionary<String, Effect> Shaders
        {
            get { return shaders; }
            private set { }
        }

        public Camera Camera {
            get { return camera; }
            private set { }
        }

        public Player Player {
            get { return player; }
            private set { }
        }

        public HUD Hud {
            get { return hud; }
            private set { }
        }

        public Particles Particles
        {
            get { return particles; }
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

        public bool TimeToInteract {
            get;
            set;
        }

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            PlayMode = true;
            rootDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            fonts = new Dictionary<String, SpriteFont>();
            models = new Dictionary<String, Model>();
            textures = new Dictionary<String, Texture2D>();
            shaders = new Dictionary<String, Effect>();
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
            graphics.IsFullScreen = true;            
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.ApplyChanges();
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

            LoadFonts();
            LoadModels("Models", models);
            LoadModels("Animation", models);
            LoadTextures();
            LoadShaders();
            Skybox skybox = new Skybox(this, "Textures/Sunset", Content);
            Components.Add(skybox);

            scene = new Scene(
                this,
                "Scene_4",
                rootDir + "/Content/Map/scene_4"
            );
            hud = new HUD(this);
            particles = new Particles(this);

            Vector3 cameraMove = new Vector3(-500.0f, 500.0f, 500.0f);
            float cameraAngle = (float) (Math.Atan2(cameraMove.X, cameraMove.Z));

            player = new Player(this, "Player", cameraAngle, scene.Player.position);
            camera = new Camera(this, "Camera", Vector3.Add(player.position, cameraMove));

            camera.LookAtParent = player;

            player.AddChild(camera);
            player.children.Add("playerModel", scene.Player);

            Components.Add(player);
            Components.Add(camera);

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        private void LoadFonts()
        {
            String modelsDir = rootDir + "/Content/Fonts";

            String[] files = Directory.GetFiles(modelsDir);
            foreach (String file in files) {
                String name = file.Remove(0, modelsDir.Length + 1).Replace(".spritefont", "");

                LoadFont(name);
            }
        }

        private void LoadFont(String name)
        {
            try {
                SpriteFont font = Content.Load<SpriteFont>("Fonts/" + name);

                fonts.Add(name, font);
            } catch (ContentLoadException) {
                Console.WriteLine("Font '" + name + "' does not exists in Content.mgcb");
            }
        }

        public void LoadModels(String dirName, Dictionary<String, Model> models)
        {
            String modelsDir = rootDir + "/Content/" + dirName;
            String[] files = Directory.GetFiles(modelsDir);

            foreach (String file in files) {
                String name = file.Remove(0, modelsDir.Length + 1)
                    .Replace(".FBX", "")
                    .Replace(".fbx", "")
                    .Replace(".x", "")
                    .Replace(".X", "");
                Model model = LoadModel(dirName, name);
                
                if (model != null) {
                    models.Add(name, model);
                }
            }
        }

        private Model LoadModel(String dirName, String name)
        {
            try {
                Model model = Content.Load<Model>(dirName + "/" + name);

                return model;
            } catch (ContentLoadException) {
                Console.WriteLine("Model '" + name + "' does not exists in Content.mgcb");

                return null;
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
            } catch (ContentLoadException) {
                Console.WriteLine("Texture '" + name + "' does not exists in Content.mgcb");
            }
        }

        private void LoadShaders()
        {
            String shadersDir = rootDir + "/Content/Shaders";

            String[] files = Directory.GetFiles(shadersDir);
            foreach(String file in files)
            {
                String name = file.Remove(0, shadersDir.Length + 1).Replace(".fx", "");

                LoadShader(name);
            }
        }

        private void LoadShader(String name)
        {
            try
            {
                Effect shader = Content.Load<Effect>("Shaders/" + name);

                shaders.Add(name, shader);
            }
            catch(ContentLoadException)
            {
                Console.WriteLine("Shader '" + name + "' does not exists in Content.mgcb");
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
                TogglePlayMode();
                OnPlayMode();
            }

            ToggleDebugMode();

            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            #if !__IOS__ &&  !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                OnEscButton();
            #endif

            // TODO: Add your update logic here
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
                OnEscButton();
            }

            base.Update(gameTime);
        }

        public void TogglePlayMode()
        {
            PlayMode = !PlayMode;
        }

        private void OnPlayMode()
        {
            if (PlayMode) {
                camera.position = camera.PlayModePosition;

                ResetHUDInPauseMode();
            }
        }

        private void ToggleDebugMode()
        {
            PlayerIndex outPlayerIndex;
            if (inputState.IsNewKeyPress(Keys.F5, null, out outPlayerIndex)) {
                Program.debugMode = !Program.debugMode;
            }
        }

        private void OnEscButton()
        {
            if (PlayMode) {
                Exit();
            } else {
                ResetHUDInPauseMode();
            }
        }

        private void ResetHUDInPauseMode()
        {
            hud.ResetSelectedTeammate();
            hud.ResetObjectToInteract();

            TimeToInteract = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            shaders["Trip"].Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds / 100);
            spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            //spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, shaders["Trip"]);
            //spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, shaders["Greyscale"]);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 480), Color.White);
            this.hud.Draw(gameTime);
            spriteBatch.End();
        }

        static float GameTimeFloat(GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
