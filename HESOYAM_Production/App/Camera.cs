using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
                    GameTimeFloat(gameTime) * MathHelper.ToRadians(mousePosition.X) / 10.0f,
                    GameTimeFloat(gameTime) * MathHelper.ToRadians(mousePosition.Y) / 10.0f,
                    0
                );
            }

            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y);
            Vector3 transformedReference = Vector3.Transform(this.cameraReference, rotationMatrixY);

            PlayerIndex pi;
            Vector3 v = Vector3.Zero;

            if (input.IsUp(PlayerIndex.One) || input.IsKeyPressed(Keys.Up, PlayerIndex.One, out pi)) {
                v.Z = 10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsDown(PlayerIndex.One) || input.IsKeyPressed(Keys.Down, PlayerIndex.One, out pi)) {
                v.Z = -10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsLeft(PlayerIndex.One) || input.IsKeyPressed(Keys.Left, PlayerIndex.One, out pi)) {
                v.X = 10;
                v = Vector3.Transform(v, rotationMatrixY);
            } else if (input.IsRight(PlayerIndex.One) || input.IsKeyPressed(Keys.Right, PlayerIndex.One, out pi)) {
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
