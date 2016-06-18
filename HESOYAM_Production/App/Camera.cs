using Microsoft.Xna.Framework;
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
            this.Zoom();

            if (!this.game.PlayMode) {
                if (game.InputState.Mouse.isInGameWindow()) {
                    this.moveCamera();
                }
            } else {
                this.CameraLookAt = this.LookAtParent != null ? this.LookAtParent.position : Vector3.Zero;
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

            this.Move(vector.X, 0, vector.Z);
        }

        private void Zoom()
        {
            MouseState mouseState = new MouseState();

            if (game.InputState.IsNewMouseScrollUp(out mouseState)) {
                this.Move(10, -10, -10);
            } else if (game.InputState.IsNewMouseScrollDown(out mouseState)) {
                this.Move(-10, 10, 10);
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
