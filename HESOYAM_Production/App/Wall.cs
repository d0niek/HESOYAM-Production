using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App
{

    public class Wall : GameObject
    {
        protected Model modelCut;

        public Texture2D TextureNormal {
            private get;
            set;
        }

        public Texture2D TextureCut {
            private get;
            set;
        }

        public Wall(
            Engine game,
            string name,
            Model model,
            Model modelCut,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.modelCut = modelCut;
        }

        public override void Draw(GameTime gameTime)
        {
            if (isWallCoversCameraLookAt()) {
                setTexture(TextureCut);
                DrawModel(modelCut);
            } else {
                setTexture(TextureNormal);
                DrawModel(model);
            }
        }

        private bool isWallCoversCameraLookAt()
        {
            const int distance = 400;
            Vector3 cameraLookAtPosition = game.Camera.CameraLookAt;

            bool onLeft = position.X <= cameraLookAtPosition.X + 100 && position.X > cameraLookAtPosition.X - distance;
            bool frontOf = position.Z >= cameraLookAtPosition.Z - 50 && position.Z < cameraLookAtPosition.Z + distance;

            return onLeft && frontOf;
        }
    }
}
