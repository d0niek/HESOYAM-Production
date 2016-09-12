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
        public bool active;
        protected Engine game;
        protected Model model;
        protected Texture2D texture;
        protected Vector3 emisiveColor = Vector3.Zero;

        public string name { get; set; }

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public Vector3 scale { get; set; }

        public IGameObject parent { get; set; }

        public Dictionary<string, IGameObject> children { get; set; }

        public Dictionary<String, Collider> colliders { get; set; }

        public bool Hover
        {
            protected get;
            set;
        }

        public GameObject(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game)
        {
            this.Hover = false;
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

            foreach(IGameElement child in children.Values)
            {
                child.Move(x, y, z);
            }

            foreach(Collider collider in colliders.Values)
            {
                collider.Move(x, y, z);
            }
        }

        public void Rotate(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            rotation = Vector3.Add(delta, rotation);

            foreach(IGameElement child in children.Values)
            {
                child.RotateAroundParent(x, y, z);
            }

            foreach(Collider collider in colliders.Values)
            {
                collider.RotateAroundParent(x, y, z);
            }
        }

        public void RotateAroundParent(float x, float y, float z)
        {
            IGameElement par = this.parent as IGameElement;
            float s = (float)Math.Sin(-y);
            float c = (float)Math.Cos(y);

            // translate point back to origin:
            this.position = this.position - par.position;

            // rotate point
            float xnew = this.position.X * c - this.position.Z * s;
            float znew = this.position.X * s + this.position.Z * c;
            float ynew = this.position.Y;

            s = (float)Math.Sin(-x);
            c = (float)Math.Cos(x);

            // rotate point
            znew = znew * c - this.position.Y * s;
            ynew = znew * s + this.position.Y * c;

            s = (float)Math.Sin(-z);
            c = (float)Math.Cos(z);

            // rotate point
            ynew = ynew * c - xnew * s;
            xnew = ynew * s + xnew * c;

            // translate point back:
            this.position = new Vector3(xnew, ynew, znew) + par.position;

            this.Rotate(x, y, z);
        }

        protected void rotateInDirection(Vector3 direction, bool lerp)
        {
            float rotationY;
            if(lerp)
                rotationY = MathHelper.Lerp(this.rotation.Y, (float)Math.Atan2(direction.X, direction.Z), 0.1f);
            else
                rotationY = (float)Math.Atan2(direction.X, direction.Z);

            if(Math.Abs(this.rotation.Y - rotationY) > 0.01f)
            {
                this.rotation = new Vector3(0, rotationY, 0);
            }
        }

        public void SetRotation(float x, float y, float z)
        {
            foreach(IGameElement child in children.Values)
            {
                child.RotateAroundParent(-this.rotation.X, -this.rotation.Y, -this.rotation.Z);
                child.RotateAroundParent(x, y, z);
            }

            this.rotation = new Vector3(x, y, z);
        }

        public void Scale(float x, float y, float z)
        {
            this.scale = new Vector3(x, y, z);

            foreach(IGameElement child in children.Values)
            {
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
            if(withColliders)
            {
                foreach(Collider collider in this.colliders.Values)
                {
                    game.AddComponent(collider);
                }
            }
            foreach(IGameComponent child in this.children.Values)
            {
                this.game.AddComponent(child);

                if(recursively)
                {
                    IGameObject ch = child as IGameObject;
                    ch.AddChildrenToGame(true, withColliders);
                }
            }
        }

        public void AddCollidersToGame()
        {
            foreach(Collider collider in this.colliders.Values)
            {
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
            BoundingBox box;
            if(colliders.ContainsKey("main"))
            {
                box = colliders["main"].box;

                float? distance = IntersectDistance((BoundingBox)box);
                if(distance != null)
                {
                    return true;
                }
            }

            return false;
        }

        private float? IntersectDistance(BoundingBox box)
        {
            Ray mouseRay = CalculateRay();
            return mouseRay.Intersects(box);
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
            if(this.model != null)
            {
                this.DrawModel(this.model);
            }
        }

        protected void DrawModel(Model model)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts)
                {
                    Effect lighting = game.Shaders["Test"];
                    part.Effect = lighting;

                    Matrix world = transforms[mesh.ParentBone.Index]
                    * Matrix.CreateRotationY(this.rotation.Y)
                    * Matrix.CreateRotationX(this.rotation.X)
                    * Matrix.CreateRotationZ(this.rotation.Z)
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateTranslation(this.position);

                    lighting.Parameters["World"].SetValue(world);
                    lighting.Parameters["View"].SetValue(game.Camera.ViewMatrix);
                    lighting.Parameters["Projection"].SetValue(game.Camera.ProjectionMatrix);
                    lighting.Parameters["ModelTexture"].SetValue(texture);
                }
                mesh.Draw();
            }
        }
		
		protected void DrawModelWithEffect(Model model)
		{
			// Draw the model. A model can have multiple meshes, so loop.
			foreach (ModelMesh mesh in model.Meshes) {
				// This is where the mesh orientation is set, as well
				// as our camera and projection.

				foreach (ModelMeshPart part in mesh.MeshParts) {
					part.Effect = this.game.Shaders["alpha"];
					part.Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform
					* Matrix.CreateRotationY(this.rotation.Y)
					* Matrix.CreateRotationX(this.rotation.X)
					* Matrix.CreateRotationZ(this.rotation.Z)
					* Matrix.CreateScale(this.scale)
					* Matrix.CreateTranslation(this.position));
					part.Effect.Parameters["View"].SetValue(this.game.Camera.ViewMatrix);
					part.Effect.Parameters["ViewVector"].SetValue(this.game.Camera.ViewVector);
					part.Effect.Parameters["Projection"].SetValue(this.game.Camera.ProjectionMatrix);
					part.Effect.Parameters["ModelTexture"].SetValue(this.texture);

					Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(part.Effect.Parameters["World"].GetValueMatrix()));
					part.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
				}

				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
		}

        protected void DrawTexture(BasicEffect effect)
        {
            if(this.texture != null)
            {
                //effect.TextureEnabled = true;
                effect.Texture = this.texture;
            }
        }
		
        public void setActive(bool active)
        {
            this.active = active;
        }

        public void setTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        protected void OnMouseLeftButtonClick(Action action)
        {
            game.InputState.Mouse.OnMouseLeftButtonClick(action);
        }

        protected void OnMouseLeftButtonPressed(Action action)
        {
            game.InputState.Mouse.OnMouseLeftButtonPressed(action);
        }

        protected bool isVisible(Vector3 direction, float distance)
        {
            Ray otherRay = new Ray(position, direction);
            foreach(IGameObject wall in game.Scene.children["Walls"].children.Values)
            {
                foreach(Collider collider in wall.colliders.Values)
                {
                    float? rayDistance = otherRay.Intersects(collider.box);
                    if(rayDistance != null && rayDistance < distance)
                    {
                        return false;
                    }
                }
            }
            foreach(IGameObject other in game.Scene.children["Others"].children.Values)
            {
                foreach(Collider collider in other.colliders.Values)
                {
                    float? rayDistance = otherRay.Intersects(collider.box);
                    if(rayDistance != null && rayDistance < distance)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
