using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenTK.Graphics.OpenGL;

namespace App
{

    public class Wall : GameObject
    {
        private Model transparentModel;
        private Texture2D textureNormal;
        private Texture2D textureCut;

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
                this.setTexture(this.textureCut);
                this.DrawModel(this.transparentModel);
            } else {
                this.setTexture(this.textureNormal);
                this.DrawModel(this.model);
            }
        }

        public void setTextureNormal(Texture2D textureNormal)
        {
            this.textureNormal = textureNormal;
        }

        public void setTextureCut(Texture2D textureCut)
        {
            this.textureCut = textureCut;
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
