using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;

namespace App
{

    public class GameObject : DrawableGameComponent, IGameElement, IGameObject
    {
        bool hover;
        bool active;
        protected Engine game;
        protected Model model;
        protected Texture2D texture;

        public string name { get; set; }

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public Vector3 scale { get; set; }

        public IGameObject parent { get; set; }

        public Dictionary<string, IGameObject> children { get; set; }

        public Dictionary<String, Collider> colliders { get; set; }

        public GameObject(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game)
        {
            this.hover = false;
            this.game = game;
            this.name = name;
            this.model = model;
            this.texture = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale ?? Vector3.One;
            this.children = new Dictionary<string, IGameObject>();
            this.colliders = new Dictionary<String, Collider>();
        }

        public GameObject(
            Engine game,
            string name,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game)
        {
            this.game = game;
            this.name = name;
            this.model = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale ?? Vector3.One;
            this.children = new Dictionary<string, IGameObject>();
            this.colliders = new Dictionary<String, Collider>();
        }

        public void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);

            foreach (IGameElement child in children.Values) {
                child.Move(x, y, z);
            }

            foreach (Collider collider in colliders.Values) {
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

            foreach (Collider collider in colliders.Values) {
                collider.RotateAroundParent(x, y, z);
            }
        }

        public void RotateAroundParent(float x, float y, float z)
        {
            IGameElement par = this.parent as IGameElement;
            float s = (float) Math.Sin(-y);
            float c = (float) Math.Cos(y);

            // translate point back to origin:
            this.position = this.position - par.position;

            // rotate point
            float xnew = this.position.X * c - this.position.Z * s;
            float znew = this.position.X * s + this.position.Z * c;
            float ynew = this.position.Y;

            s = (float) Math.Sin(-x);
            c = (float) Math.Cos(x);

            // rotate point
            znew = znew * c - this.position.Y * s;
            ynew = znew * s + this.position.Y * c;

            s = (float) Math.Sin(-z);
            c = (float) Math.Cos(z);

            // rotate point
            ynew = ynew * c - xnew * s;
            xnew = ynew * s + xnew * c;

            // translate point back:
            this.position = new Vector3(xnew, ynew, znew) + par.position;

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

        public void AddChildrenToGame(bool recursively, bool withColliders)
        {
            if (withColliders) {
                foreach (Collider collider in this.colliders.Values) {
                    game.AddComponent(collider);
                }
            }
            foreach (IGameComponent child in this.children.Values) {
                this.game.AddComponent(child);

                if (recursively) {
                    IGameObject ch = child as IGameObject;
                    ch.AddChildrenToGame(true, withColliders);
                }
            }
        }

        public void AddCollidersToGame()
        {
            foreach (Collider collider in this.colliders.Values) {
                game.AddComponent(collider);
            }
        }

        public void AddCollider(String name, Collider collider)
        {
            colliders.Add(name, collider);
        }

        public IGameObject RemoveChild(IGameObject child)
        {
            return RemoveChild(child.name);
        }

        public IGameObject RemoveChild(string childName)
        {
            IGameObject child = children[childName];
            children.Remove(childName);
            return child;
        }

        public Collider RemoveCollider(String name)
        {
            Collider backup = colliders[name];
            colliders.Remove(name);
            return backup;
        }

        public bool IsMouseOverObject()
        {
            Matrix world = Matrix.CreateTranslation(this.position);

            for (int index = 0; index < model.Meshes.Count; index++) {
                BoundingSphere sphere = model.Meshes[index].BoundingSphere;
                sphere = sphere.Transform(world);
                float? distance = IntersectDistance(sphere);

                if (distance != null) {
                    return true;
                }
            }

            return false;
        }

        private float? IntersectDistance(BoundingSphere sphere)
        {
            Ray mouseRay = CalculateRay();
            return mouseRay.Intersects(sphere);
        }

        private Ray CalculateRay()
        {
            Vector2 mouseLocation = new Vector2(
                                        this.game.InputState.Mouse.CurrentMouseState.X,
                                        this.game.InputState.Mouse.CurrentMouseState.Y
                                    );

            Vector3 nearPoint = this.game.GraphicsDevice.Viewport.Unproject(
                                    new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f),
                                    this.game.Camera.ProjectionMatrix,
                                    this.game.Camera.ViewMatrix,
                                    Matrix.Identity
                                );

            Vector3 farPoint = this.game.GraphicsDevice.Viewport.Unproject(
                                   new Vector3(mouseLocation.X, mouseLocation.Y, 1.0f),
                                   this.game.Camera.ProjectionMatrix,
                                   this.game.Camera.ViewMatrix,
                                   Matrix.Identity
                               );

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.model != null) {
                this.DrawModel(this.model);
            }
        }

        protected void DrawModel(Model model)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes) {
                // This is where the mesh orientation is set, as well
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f,0.7f); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(1, -0.5f, -1);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f); // with green highlights
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    effect.EmissiveColor = new Vector3(0, 0, 0);

                    effect.World = transforms[mesh.ParentBone.Index]
                    * Matrix.CreateRotationY(this.rotation.Y)
                    * Matrix.CreateRotationX(this.rotation.X)
                    * Matrix.CreateRotationZ(this.rotation.Z)
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateTranslation(this.position);
                    effect.View = this.game.Camera.ViewMatrix;
                    effect.Projection = this.game.Camera.ProjectionMatrix;

                    // Tmp effect to highlight object under mouse
                    if (active) {
                        effect.AmbientLightColor = new Vector3(0, 0, 255);
                    } else if (hover) {
                        effect.AmbientLightColor = new Vector3(0, 255, 0);
                    }

                    this.DrawTexture(effect);
                }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        protected void DrawTexture(BasicEffect effect)
        {
            if (this.texture != null) {
                effect.TextureEnabled = true;
                effect.Texture = this.texture;
            }
        }

        public void setHover(bool hover)
        {
            this.hover = hover;
        }

        public void setActive(bool active)
        {
            this.active = active;
        }

        public void setTexture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
