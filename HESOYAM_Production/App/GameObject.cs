using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using App.Collisions;

namespace App
{
    public class GameObject : GameComponent, IGameElement, IGameObject
    {
        public Vector3 position { get; set;}
        public Vector3 rotation { get; set;}
        public string name { get; set;}
        private Game game;
        public IGameObject parent { get; set;}
        public List<IGameObject> children { get; set;}
        public List<Collider> colliders { get; set;}

        public GameObject (Game game)
            : base(game)
        {
            this.game = game;
        }

        public void Move (float x, float y, float z)
        {
            
        }

        public void Rotate (float x, float y, float z)
        {

        }

        public bool Collision (IGameElement collider)
        {
            return false;
        }

        public void AddChild(IGameObject component)
        {

        }

        public void AddCollider (Collider collider)
        {

        }

        public IGameObject RemoveChild (IGameObject child)
        {
            return child;
        }

        public IGameObject RemoveChild (string childName)
        {
            return new GameObject(game);
        }

        public Collider RemoveCollider (Collider collider)
        {
            return collider;
        }
    }
}

