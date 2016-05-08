using Microsoft.Xna.Framework;
using HESOYAM_Production;

namespace App
{

    public class Camera : GameObject
    {
        private const int borderWidth = 100;

        public Vector3 playModePosition;

        public Vector3 cameraLookAt;

        public IGameElement lookAtParent { get; set; }

        public Matrix ViewMatrix { get; set; }

        public Matrix ProjectionMatrix { get; set; }

        public Camera(
            Engine game,
            string name,
            Vector3 position = default(Vector3),
            Vector3 rotaion = default(Vector3)
        ) : base(game, name, position, rotaion)
        {
            this.lookAtParent = null;
            this.ViewMatrix = Matrix.Identity;
            this.ProjectionMatrix = Matrix.Identity;
            this.playModePosition = this.position;
        }

        public void update(InputState input, float aspectRatio)
        {
            if (!this.game.playMode) {
                if (input.Mouse.isInGameWindow()) {
                    this.moveCamera(input);
                }
            } else {
                this.cameraLookAt = this.lookAtParent != null ? this.lookAtParent.position : Vector3.Zero;
                this.playModePosition = this.position;
            }

            this.ViewMatrix = Matrix.CreateLookAt(this.position, this.cameraLookAt, Vector3.Up);
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio,
                1.0f,
                10000.0f
            );
        }

        private void moveCamera(InputState input)
        {
            Vector3 vector = Vector3.Zero;

            if (input.Mouse.isCloseToTopLeftCorner()) {
                vector.Z = -20;
            } else if (input.Mouse.isCloseToTopRightCorner()) {
                vector.X = 20;
            } else if (input.Mouse.isCloseToBottomLeftCorner()) {
                vector.X = -20;
            } else if (input.Mouse.isCloseToBottomRightCorner()) {
                vector.Z = 20;
            } else if (input.Mouse.isCloseToLeftBorder()) {
                vector.Z = -20;
                vector.X = -20;
            } else if (input.Mouse.isCloseToRightBorder()) {
                vector.Z = 20;
                vector.X = 20;
            } else if (input.Mouse.isCloseToTopBorder()) {
                vector.Z = -20;
                vector.X = 20;
            } else if (input.Mouse.isCloseToBottomBorder()) {
                vector.Z = 20;
                vector.X = -20;
            }

            this.Move(vector.X, 0, vector.Z);
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 newLookAt = this.cameraLookAt - this.position;

            base.Move(x, y, z);

            this.cameraLookAt = Vector3.Add(this.position, newLookAt);
        }
    }
}
