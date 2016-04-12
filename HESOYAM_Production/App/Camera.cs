using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace App
{

    public class Camera : GameObject
    {
        public Matrix ViewMatrix { get; set; }

        public Matrix ProjectionMatrix { get; set; }

        public Camera(
            Game game,
            Vector3 position = default(Vector3),
            Vector3 rotaion = default(Vector3)
        ) : base(game, position, rotaion)
        {
            this.ViewMatrix = Matrix.Identity;
            this.ProjectionMatrix = Matrix.Identity;
        }

        public void update(float aspectRatio)
        {
            Vector3 cameraLookAt = Vector3.Zero;

            this.ViewMatrix = Matrix.CreateLookAt(this.position, cameraLookAt, Vector3.Up);
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio,
                1.0f,
                10000.0f
            );
        }
    }
}
