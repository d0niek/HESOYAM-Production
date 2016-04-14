using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;



namespace App.Render
{

    public class Object3D : DrawableGameComponent, IGameElement, IGameObject
    {
        private Engine game;
        private Model model;

        public string name { get; set; }

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public Vector3 scale { get; set; }

        public IGameObject parent { get; set; }

        public Dictionary<string, IGameObject> children { get; set; }

        public List<Collider> colliders { get; set; }

        public Object3D(
            Engine game, 
            Model model, 
            string name,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null) : base(game)
        {
            this.game = game;
            this.model = model;
            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale ?? Vector3.One;
            this.children = new Dictionary<string, IGameObject>();
            this.colliders = new List<Collider>();
        }

        public void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);

            foreach (IGameElement child in children.Values) {
                child.Move(x, y, z);
            }

            foreach (Collider collider in colliders) {
                collider.Move(x, y, z);
            }
        }

        public void Rotate(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            rotation = Vector3.Add(delta, rotation);

            foreach (IGameElement child in children.Values) {
                child.RotateAroundParent(x, y, z);
            }

            foreach (Collider collider in colliders) {
                collider.RotateAroundParent(x, y, z);
            }
        }

        public void RotateAroundParent(float x, float y, float z)
        {
            IGameElement par = this.parent as IGameElement;
            this.position = Vector3.Transform(
                par.position - this.position, 
                Matrix.CreateRotationX(x));

            this.position = Vector3.Transform(
                par.position - this.position, 
                Matrix.CreateRotationY(y));

            this.position = Vector3.Transform(
                par.position - this.position, 
                Matrix.CreateRotationZ(z));

            this.Rotate(x, y, z);
        }

        public void SetRotation(float x, float y, float z)
        {
            foreach (IGameElement child in children.Values) {
                child.RotateAroundParent(-this.rotation.X, -this.rotation.Y, -this.rotation.Z);
                child.RotateAroundParent(x, y, z);
            }

            this.rotation = new Vector3(x, y, z);
        }

        public void Scale(float x, float y, float z)
        {
            this.scale = new Vector3(x, y, z);

            foreach (IGameElement child in children.Values) {
                child.Scale(x, y, z);
            }
        }

        public bool Collision(IGameElement collider)
        {
            return false;
        }

        public void AddChild(IGameObject component)
        {
            IGameElement com = component as IGameElement;
            com.parent = this;
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

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                    * Matrix.CreateRotationY(this.rotation.Y)
                    * Matrix.CreateRotationX(this.rotation.X)
                    * Matrix.CreateRotationZ(this.rotation.Z)
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateTranslation(this.position);
                    effect.View = this.game.camera.ViewMatrix;
                    effect.Projection = this.game.camera.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
