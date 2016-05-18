using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App
{

    public class Wall : GameObject
    {
        private Model transparentModel;

        public Wall(
            Engine game,
            string name,
            Model model,
            Model transparentModel,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null) : base(game, name, model, position, rotation, scale)
        {
            this.transparentModel = transparentModel;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.isWallCoversCameraLookAt()) {
                //this.DrawModel(this.transparentModel);
            } else {
                this.DrawModel(this.model);
            }
        }

        private bool isWallCoversCameraLookAt()
        {
            const int distance = 400;
            Vector3 cameraLookAtPosition = this.game.camera.cameraLookAt;

            bool onLeft = this.position.X <= cameraLookAtPosition.X + 100 && this.position.X > cameraLookAtPosition.X - distance;
            bool frontOf = this.position.Z >= cameraLookAtPosition.Z - 50 && this.position.Z < cameraLookAtPosition.Z + distance;

            return onLeft && frontOf;
        }
    }
}
