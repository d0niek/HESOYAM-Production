using App;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HESOYAM_Production.App
{
    public class Particles : GameObject
    {
        private List<Particle> particleList;
        private Random random;

        public Particles(Engine game) : base(game, "", null)
        {
            particleList = new List<Particle>();
            game.AddComponent(this);
            random = new Random();
        }

        public void Draw()
        {
            Matrix invertY = Matrix.CreateScale(1, -1, 1);

            BasicEffect effect = new BasicEffect(GraphicsDevice);
            effect.View = game.Camera.ViewMatrix;
            effect.Projection = game.Camera.ProjectionMatrix;
            effect.TextureEnabled = true;
            effect.Texture = game.Textures["particle"];

            VertexPositionTexture[] verts = new VertexPositionTexture[4];
            verts[0].TextureCoordinate = new Vector2(0.0f, 1.0f);
            verts[1].TextureCoordinate = new Vector2(0.0f, 0.0f);
            verts[2].TextureCoordinate = new Vector2(1.0f, 1.0f);
            verts[3].TextureCoordinate = new Vector2(1.0f, 0.0f);

            particleList.Sort();

            foreach(Particle i in particleList)
            {
                effect.Alpha = i.alpha;
                Vector3 direction = Vector3.Subtract(game.Camera.position, i.position);
                i.distanceToCamera = direction.Length();
                direction.Normalize();
                Vector3 t1 = new Vector3(1f, 1f, (-direction.X - direction.Y) / direction.Z);
                t1.Normalize();
                Vector3 t2 = Vector3.Cross(direction, t1);
                t2.Normalize();
                t1 = Vector3.Multiply(t1, i.halfSize);
                t2 = Vector3.Multiply(t2, i.halfSize);

                verts[0].Position = Vector3.Add(i.position, -t1);
                verts[1].Position = Vector3.Add(i.position, t2);
                verts[2].Position = Vector3.Add(i.position, -t2);
                verts[3].Position = Vector3.Add(i.position, t1);

                foreach(EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleStrip,
                        verts,
                        0,
                        2
                    );
                }
            }
        }

        public void addParticle(Vector3 position, Particle customParticle, TimeSpan creationTime)
        {
            particleList.Add(new Particle(position, creationTime, customParticle));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            for(int i = particleList.Count - 1; i >= 0; i--)
            {
                if(game.PlayMode)
                {
                    if(particleList[i].isAlive) particleList[i].update(gameTime.TotalGameTime);
                    else particleList.RemoveAt(i);
                }
                else
                {
                    particleList[i].removeTime += gameTime.ElapsedGameTime;
                }
            }
        }

        public Particle addParticle(Vector3 position, TimeSpan creationTime)
        {
            Particle newParticle = new Particle(position, creationTime, random);
            particleList.Add(newParticle);
            return newParticle;
        }
    }
}
