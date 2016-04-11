using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;

namespace App.Render
{

    public class Object3D : DrawableGameComponent, IGameElement, IGameObject
    {
        private Engine game;
        private Model model;

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public string name { get; set; }

        public IGameObject parent { get; set; }

        public Dictionary<string, IGameObject> children { get; set; }

        public List<Collider> colliders { get; set; }

        public Object3D(Engine game, Model model, Vector3 p = default(Vector3), Vector3 r = default(Vector3)) : base(game)
        {
            this.game = game;
            this.model = model;
            this.position = p;
            this.rotation = r;
            this.children = new Dictionary<string, IGameObject>();
            this.colliders = new List<Collider>();
        }

        public void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);
        }

        public void Rotate(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            rotation = Vector3.Add(delta, rotation);
        }

        public bool Collision(IGameElement collider)
        {
            return false;
        }

        public void AddChild(IGameObject component)
        {
            component.parent = this;
            children.Add(component.name, component);
        }

        public void AddCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        public IGameObject RemoveChild(IGameObject child)
        {
            return RemoveChild(child.name);
        }

        public IGameObject RemoveChild(string childName)
        {
            IGameObject child = children[
                                    childName];
            children.Remove(childName);
            return child;
        }

        public Collider RemoveCollider(Collider collider)
        {
            colliders.Remove(collider);
            return collider;
        }

        public override void Draw(GameTime gameTime)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //TODO: zwrócić pozycję kamery
            Vector3 cameraPosition = this.game.camera.position;

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                    * Matrix.CreateRotationY(rotation.Y)
                    * Matrix.CreateRotationX(rotation.X)
                    * Matrix.CreateRotationZ(rotation.Z)
                    * Matrix.CreateTranslation(position);
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), 
                        game.Graphics().GraphicsDevice.Viewport.AspectRatio, 
                        1.0f, 
                        10000.0f
                    );
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
