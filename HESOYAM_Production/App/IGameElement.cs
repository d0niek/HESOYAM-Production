using System;
using Microsoft.Xna.Framework;

namespace App
{

    public interface IGameElement
    {
        IGameObject parent { get; set; }

        Vector3 position { get; set; }

        Vector3 rotation { get; set; }

        Vector3 scale { get; set; }

        void Scale(float x, float y, float z);

        void Move(float x, float y, float z);

        void Rotate(float x, float y, float z);

        void RotateAroundParent(float x, float y, float z);

        bool Collision(IGameElement collider);
    }
}
