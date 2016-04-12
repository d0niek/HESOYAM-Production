using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace App
{

    public class Camera : GameObject
    {
        // Direction the camera points without rotation.
        private Vector3 cameraReference = new Vector3(0, 0, 1);

        public Vector3 TargetOffset { get; set; }

        public Matrix ViewMatrix { get; set; }

        public Matrix ProjectionMatrix { get; set; }

        public Camera(Game game, Vector3 p = default(Vector3), Vector3 r = default(Vector3)) : base(game, p, r)
        {
            this.ViewMatrix = Matrix.Identity;
            this.ProjectionMatrix = Matrix.Identity;
        }

        public void update(InputState input, GameTime gameTime, float aspectRatio)
        {
            if (input.LastMouseState.Position != Point.Zero) {
                Point mousePosition = input.CurrentMouseState.Position - input.LastMouseState.Position;

                this.Rotate(
                    -GameTimeFloat(gameTime) * MathHelper.ToRadians(mousePosition.Y) / 40.0f,
                    -GameTimeFloat(gameTime) * MathHelper.ToRadians(mousePosition.X) / 40.0f,
                    0
                );
            }

            Debug.WriteLine(this.rotation);

            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y);
            Matrix rotationMatrixX = Matrix.CreateRotationX(this.rotation.X);
            Vector3 transformedReference = Vector3.Transform(this.cameraReference, rotationMatrixX * rotationMatrixY);

            PlayerIndex pi;
            Vector3 v = Vector3.Zero;

            if (input.IsKeyPressed(Keys.W, PlayerIndex.One, out pi)) {
                v.Z = 10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsKeyPressed(Keys.S, PlayerIndex.One, out pi)) {
                v.Z = -10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsKeyPressed(Keys.A, PlayerIndex.One, out pi)) {
                v.X = 10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsKeyPressed(Keys.D, PlayerIndex.One, out pi)) {
                v.X = -10;
                v = Vector3.Transform(v, rotationMatrixY);
            }

            this.Move(v.X, 0, v.Z);

            Vector3 cameraLookAt = this.position + transformedReference;

            this.ViewMatrix = Matrix.CreateLookAt(this.position, cameraLookAt, Vector3.Up);
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio,
                1.0f,
                10000.0f
            );
        }

        private float GameTimeFloat(GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
