using System;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App.Collisions
{

    public class Collider : DrawableGameComponent, IGameElement
    {
        public IGameObject parent { get; set; }
        private BoundingBox box;
        private Engine game;

        // TODO: Is this even needed?
        // If so, implement getters and setters to translate from BoundingBox.
        // Otherwise consider making this class non IGameElement.
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 scale { get; set; }

        private void Setup(Engine game, Vector3 min, Vector3 max)
        {
            box = new BoundingBox(min, max);
            this.game = game;
        }

        public Collider(Engine game, Vector3 position, Vector3 rotation, Vector3 scale) : base(game)
        {
            Vector3 min = Vector3.Subtract(position, Vector3.Multiply(scale, 0.5f));
            Vector3 max = Vector3.Add(position, Vector3.Multiply(scale, 0.5f));
            Setup(game, min, max);
        }

        public Collider(Engine game, Vector3 min, Vector3 max) : base(game)
        {
            Setup(game, min, max);
        }

        public void Move(float x, float y, float z)
        {
            box.Max = Vector3.Add(box.Max, new Vector3(x, y, z));
            box.Min = Vector3.Add(box.Min, new Vector3(x, y, z));
        }

        public void Rotate(float x, float y, float z)
        {
            // Probably not necessary and also impossible to implement using BoundingBoxes
        }

        public void SetRotation(float x, float y, float z)
        {
            // Probably not necessary and also impossible to implement using BoundingBoxes
        }

        public void Scale(float x, float y, float z)
        {
            // TODO
        }

        public void RotateAroundParent(float x, float y, float z)
        {
            // Probably not necessary and also impossible to implement using BoundingBoxes
        }

        public bool CollidesWith(IGameElement collider)
        {
            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            if(Program.debugMode)
            {
                BasicEffect effect = new BasicEffect(GraphicsDevice);
                effect.LightingEnabled = false;
                effect.View = this.game.camera.ViewMatrix;
                effect.Projection = this.game.camera.ProjectionMatrix;
                effect.CurrentTechnique.Passes[0].Apply();
                Vector3[] boxVertices = box.GetCorners();
                var drawVertices = new[]
                {
                    new VertexPositionColor(boxVertices[0], Color.White),
                    new VertexPositionColor(boxVertices[1], Color.White),
                    new VertexPositionColor(boxVertices[1], Color.White),
                    new VertexPositionColor(boxVertices[2], Color.White),
                    new VertexPositionColor(boxVertices[2], Color.White),
                    new VertexPositionColor(boxVertices[3], Color.White)
                };
                short[] lineListIndices = new short[(6 * 2) - 2];
                for(int i = 0; i < 6 - 1; i++)
                {
                    lineListIndices[i * 2] = (short)(i);
                    lineListIndices[(i * 2) + 1] = (short)(i + 1);
                }

                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, drawVertices, 0, 6, lineListIndices, 0, 3);
            }
        }

        public bool Collision(IGameElement collider)
        {
            throw new NotImplementedException();
        }
    }
}
