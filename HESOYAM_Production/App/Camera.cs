using Microsoft.Xna.Framework;
using HESOYAM_Production;
using System;

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
                if (this.game.isMouseInGameWindow()) {
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
            Vector2 mousePosition = new Vector2(
                                        input.CurrentMouseState.Position.X,
                                        input.CurrentMouseState.Position.Y
                                    );
            Vector3 vector = Vector3.Zero;

            if (this.isMouseCloseToTopLeftCorner(mousePosition)) {
                vector.Z = -20;
            } else if (this.isMouseCloseToTopRightCorner(mousePosition)) {
                vector.X = 20;
            } else if (this.isMouseCloseToBottomLeftCorner(mousePosition)) {
                vector.X = -20;
            } else if (this.isMouseCloseToBottomRightCorner(mousePosition)) {
                vector.Z = 20;
            } else if (this.isMouseCloseToLeftBorder(mousePosition)) {
                vector.Z = -20;
                vector.X = -20;
            } else if (this.isMouseCloseToRightBorder(mousePosition)) {
                vector.Z = 20;
                vector.X = 20;
            } else if (this.isMouseCloseToTopBorder(mousePosition)) {
                vector.Z = -20;
                vector.X = 20;
            } else if (this.isMouseCloseToBottomBorder(mousePosition)) {
                vector.Z = 20;
                vector.X = -20;
            }

            this.Move(vector.X, 0, vector.Z);
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 newLookAt = this.position - this.cameraLookAt;

            base.Move(x, y, z);

            this.cameraLookAt = Vector3.Add(this.position, newLookAt);
        }

        private bool isMouseCloseToLeftBorder(Vector2 mousePosition)
        {
            return mousePosition.X >= 0 && mousePosition.X < borderWidth;
        }

        private bool isMouseCloseToRightBorder(Vector2 mousePosition)
        {
            return mousePosition.X <= this.game.GraphicsDevice.Viewport.Width &&
            mousePosition.X > this.game.GraphicsDevice.Viewport.Width - borderWidth;
        }

        private bool isMouseCloseToTopBorder(Vector2 mousePosition)
        {
            return mousePosition.Y >= 0 && mousePosition.Y < borderWidth;
        }

        private bool isMouseCloseToBottomBorder(Vector2 mousePosition)
        {
            return mousePosition.Y <= this.game.GraphicsDevice.Viewport.Height &&
            mousePosition.Y > this.game.GraphicsDevice.Viewport.Height - borderWidth;
        }

        private bool isMouseCloseToTopLeftCorner(Vector2 mousePosition)
        {
            return this.isMouseCloseToLeftBorder(mousePosition) && this.isMouseCloseToTopBorder(mousePosition);
        }

        private bool isMouseCloseToTopRightCorner(Vector2 mousePosition)
        {
            return this.isMouseCloseToRightBorder(mousePosition) && this.isMouseCloseToTopBorder(mousePosition);
        }

        private bool isMouseCloseToBottomLeftCorner(Vector2 mousePosition)
        {
            return this.isMouseCloseToLeftBorder(mousePosition) && this.isMouseCloseToBottomBorder(mousePosition);
        }

        private bool isMouseCloseToBottomRightCorner(Vector2 mousePosition)
        {
            return this.isMouseCloseToRightBorder(mousePosition) && this.isMouseCloseToBottomBorder(mousePosition);
        }
    }
}
