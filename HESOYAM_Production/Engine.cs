using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using App;
using System.IO;
using App.Animation;
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
        GraphicsDeviceManager graphics;
        HUD hud;
        Camera camera;
        public Player player;
        Scene scene;

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
            PlayMode = true;
            rootDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            fonts = new Dictionary<String, SpriteFont>();
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
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent ()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);
            inputState = new InputState (GraphicsDevice);

            LoadFonts();
            LoadModels ("Models",models);
            LoadModels ("Animation",models);
            LoadTextures ();

            scene = new Scene (
                this,
                "Scene_1",
                rootDir + "/Content/Map/scene_1"
            );
            hud = new HUD(this);

            Vector3 cameraMove = new Vector3 (-500.0f, 500.0f, 500.0f);
            float cameraAngle = (float)(Math.Atan2 (cameraMove.X, cameraMove.Z));

            player = new Player (this, "Player", cameraAngle, scene.Player.position);
            camera = new Camera (this, "Camera", Vector3.Add (player.position, cameraMove));

            camera.LookAtParent = player;

            player.AddChild (camera);
            player.children.Add("playerModel",scene.Player);
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
            } catch (ContentLoadException e) {
                Console.WriteLine("Font '" + name + "' does not exists in Content.mgcb");
            }
        }

        public void LoadModels(String dirName, Dictionary<String, Model> models)
        {
            String modelsDir = rootDir + "/Content/" + dirName;
            String[] files = Directory.GetFiles(modelsDir);

            foreach (String file in files) {
                String name = file.Remove(0, modelsDir.Length + 1).Replace(".FBX", "").Replace(".fbx", "");
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
            } catch (ContentLoadException e) {
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
                TogglePlayMode();
            }

            PlayerIndex outPlayerIndex;
            if (inputState.IsNewKeyPress(Keys.F5, null, out outPlayerIndex)) {
                Program.debugMode = !Program.debugMode;
            }

            if (PlayMode) {
                camera.position = camera.PlayModePosition;
                player.update();
                foreach(Opponent opponent in scene.children["Opponents"].children.Values)
                {
                    opponent.update();
                }
                hud.ResetSelectedTeammate();
                hud.ResetObjectToInteract();
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

        public void TogglePlayMode()
        {
            PlayMode = !PlayMode;
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
