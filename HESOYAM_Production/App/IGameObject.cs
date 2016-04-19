using System;
using App.Collisions;
using System.Collections.Generic;

namespace App
{

    public interface IGameObject
    {
        string name { get; set; }

        Dictionary<string, IGameObject> children { get; set; }

        List<Collider> colliders { get; set; }

        void AddChild(IGameObject component);

        IGameObject RemoveChild(IGameObject child);

        IGameObject RemoveChild(string childName);

        void AddChildrenToGame(bool recursively);

        void AddCollider(Collider colider);

        Collider RemoveCollider(Collider colider);
    }
}
