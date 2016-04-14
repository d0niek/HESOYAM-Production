using Microsoft.Xna.Framework;
using HESOYAM_Production;

namespace App
{

    public class Camera : GameObject
    {
        public Vector3 cameraLookAt;

        public Vector3 startPosition;

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
            this.startPosition = this.position;
            this.ViewMatrix = Matrix.Identity;
            this.ProjectionMatrix = Matrix.Identity;
        }

        public void update(float aspectRatio)
        {

            if (this.lookAtParent != null) {
                cameraLookAt = this.lookAtParent.position;
            } else {
                cameraLookAt = Vector3.Zero;
            }

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
