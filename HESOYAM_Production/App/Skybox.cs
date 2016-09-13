using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public class Skybox : DrawableGameComponent
    {
        private Model skyBox;
        private TextureCube skyBoxTexture;
        private Effect skyBoxEffect;
        private float size = 5000f;
        private Engine game;

        public Skybox(Engine game, string skyboxTexture, ContentManager Content)
            : base(game)
        {
            this.game = game;
            skyBox = Content.Load<Model>("Models/cube");
            skyBoxTexture = Content.Load<TextureCube>(skyboxTexture);
            skyBoxEffect = Content.Load<Effect>("Shaders/Skybox");
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        float margin = -7500f;
                        Vector3 skyboxPos = new Vector3(-1000 + size, size / 2, margin + size) + this.game.Camera.position;
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size)
                            * Matrix.CreateRotationY(0.78f)
                            * Matrix.CreateTranslation(skyboxPos));
                        part.Effect.Parameters["View"].SetValue(this.game.Camera.ViewMatrix);
                        part.Effect.Parameters["Projection"].SetValue(this.game.Camera.ProjectionMatrix);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(skyboxPos);
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
