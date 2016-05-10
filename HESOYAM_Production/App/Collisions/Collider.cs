using System;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production.App.Collisions;

namespace App.Collisions
{
    public class Collider : DrawableGameComponent, IGameElement
    {
        VertexPositionColor[] verts = new VertexPositionColor[8];
        static short[] indices = new short[]
        {
        0, 1,
        1, 2,
        2, 3,
        3, 0,
        0, 4,
        1, 5,
        2, 6,
        3, 7,
        4, 5,
        5, 6,
        6, 7,
        7, 4,
        };
        BasicEffect effect;

        public IGameObject parent { get; set; }
        public Color drawColor = Color.GreenYellow;
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

        public Collider(Engine game, Vector3 position, Vector3 size, Vector3 rotation) : base(game)
        {
            Vector3 min = Vector3.Subtract(position, Vector3.Multiply(size, 0.5f));
            Vector3 max = Vector3.Add(position, Vector3.Multiply(size, 0.5f));
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
                BoundingBoxRenderer.Render(box, GraphicsDevice, this.game.camera.ViewMatrix, this.game.camera.ProjectionMatrix, drawColor);
            }
        }

        public bool CollidesWith(Collider other)
        {
            return box.Intersects(other.box);
        }
    }
}
