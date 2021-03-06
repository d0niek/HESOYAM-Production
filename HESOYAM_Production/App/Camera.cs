﻿using Microsoft.Xna.Framework;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Input;

namespace App
{

    public class Camera : GameObject
    {
        Vector3 playModePosition;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        public Vector3 PlayModePosition {
            get { return playModePosition; }
            private set { }
        }

        public Vector3 CameraLookAt {
            get;
            set;
        }

        public IGameElement LookAtParent {
            get;
            set;
        }

        public Matrix ViewMatrix { 
            get { return viewMatrix; } 
            private set { }
        }

        public Matrix ProjectionMatrix { 
            get { return projectionMatrix; } 
            private set { }
        }

        public Camera(
            Engine game,
            string name,
            Vector3 position = default(Vector3),
            Vector3 rotaion = default(Vector3)
        ) : base(game, name, position, rotaion)
        {
            this.LookAtParent = null;
            this.viewMatrix = Matrix.Identity;
            this.projectionMatrix = Matrix.Identity;
            this.playModePosition = this.position;
        }

        public override void Update(GameTime gameTime)
        {
            if (game.TimeToInteract) {
                return;
            }

            this.Zoom();

            if (!this.game.PlayMode) {
                if (game.InputState.Mouse.isInGameWindow()) {
                    this.moveCamera();
                }
            } else {
                //TODO obrót wokół głowy porządniej?
                this.CameraLookAt = this.LookAtParent != null ? 
                    new Vector3(
                    this.LookAtParent.position.X,
                    this.LookAtParent.position.Y + 160f,
                    this.LookAtParent.position.Z
                ) : Vector3.Zero;
                this.playModePosition = this.position;
            }

            this.viewMatrix = Matrix.CreateLookAt(this.position, this.CameraLookAt, Vector3.Up);
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                this.GraphicsDevice.Viewport.AspectRatio,
                1.0f,
                10000.0f
            );
        }

        private void moveCamera()
        {
            Vector3 vector = Vector3.Zero;
            PlayerIndex playerIndex;

            if (game.InputState.Mouse.isCloseToTopLeftCorner()) {
                vector.Z = -20;
            } else if (game.InputState.Mouse.isCloseToTopRightCorner()) {
                vector.X = 20;
            } else if (game.InputState.Mouse.isCloseToBottomLeftCorner()) {
                vector.X = -20;
            } else if (game.InputState.Mouse.isCloseToBottomRightCorner()) {
                vector.Z = 20;
            } else if (game.InputState.Mouse.isCloseToLeftBorder()) {
                vector.Z = -20;
                vector.X = -20;
            } else if (game.InputState.Mouse.isCloseToRightBorder()) {
                vector.Z = 20;
                vector.X = 20;
            } else if (game.InputState.Mouse.isCloseToTopBorder()) {
                vector.Z = -20;
                vector.X = 20;
            } else if (game.InputState.Mouse.isCloseToBottomBorder()) {
                vector.Z = 20;
                vector.X = -20;
            }

            if (game.InputState.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex)) {
                vector.Z -= 20;
                vector.X -= 20;
            }
            if (game.InputState.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex)) {
                vector.Z += 20;
                vector.X += 20;
            }
            if (game.InputState.IsKeyPressed(Keys.W, PlayerIndex.One, out playerIndex)) {
                vector.Z -= 20;
                vector.X += 20;
            }
            if (game.InputState.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex)) {
                vector.Z += 20;
                vector.X -= 20;
            }

            this.Move(vector.X, 0, vector.Z);
        }

        private void Zoom()
        {
            MouseState mouseState = new MouseState();

            if (game.InputState.IsNewMouseScrollUp(out mouseState) && this.position.Y > 190f) {
                this.Move(20, -20, -20);
            } else if (game.InputState.IsNewMouseScrollDown(out mouseState) && this.position.Y < 1300f) {
                this.Move(-20, 20, 20);
            }
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 newLookAt = this.CameraLookAt - this.position;

            base.Move(x, y, z);

            this.CameraLookAt = Vector3.Add(this.position, newLookAt);
        }
    }
}
