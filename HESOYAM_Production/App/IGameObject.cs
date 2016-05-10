using App.Collisions;
using System;
using System.Collections.Generic;

namespace App
{

    public interface IGameObject
    {
        string name { get; set; }

        Dictionary<string, IGameObject> children { get; set; }

        Dictionary<String, Collider> colliders { get; set; }

        void AddChild(IGameObject component);

        IGameObject RemoveChild(IGameObject child);

        IGameObject RemoveChild(string childName);

        void AddChildrenToGame(bool recursively, bool withColliders);

        void AddCollidersToGame();

        void AddCollider(String name, Collider colider);

        Collider RemoveCollider(String name);
    }
}
