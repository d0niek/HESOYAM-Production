using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using App.Collisions;

namespace App
{

    public class GameObject : GameComponent, IGameElement, IGameObject
    {
        private Game game;

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public string name { get; set; }

        public IGameObject parent { get; set; }

        public Dictionary<string, IGameObject> children { get; set; }

        public List<Collider> colliders { get; set; }

        public GameObject(Game game) : base(game)
        {
            this.game = game;
            this.position = Vector3.Zero;
            this.rotation = Vector3.Zero;
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
            IGameObject child = children[childName];
            children.Remove(childName);
            return child;
        }

        public Collider RemoveCollider(Collider collider)
        {
            colliders.Remove(collider);
            return collider;
        }
    }
}
