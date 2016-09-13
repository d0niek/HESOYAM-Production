using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace App
{

    abstract public class Segment : GameObject
    {
        protected Model modelCut;

        public Texture2D TextureNormal {
            get;
            set;
        }

        public Texture2D TextureCut {
            get;
            set;
        }

        public Segment(
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
			try {
				if (isSegmentCoversCameraLookAt()) {
					setTexture(TextureCut);
					DrawModelWithEffect(modelCut);
				} else {
					setTexture(TextureNormal);
					DrawModel(model);
				}
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

        protected bool isSegmentCoversCameraLookAt()
        {
            const int distance = 400;
            Vector3 cameraLookAtPosition = game.Camera.CameraLookAt;

            bool onLeft = position.X <= cameraLookAtPosition.X + 100 && position.X > cameraLookAtPosition.X - distance;
            bool frontOf = position.Z >= cameraLookAtPosition.Z - 50 && position.Z < cameraLookAtPosition.Z + distance;

            return onLeft && frontOf;
        }
    }
}
