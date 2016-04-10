using System;
using Microsoft.Xna.Framework;

namespace App
{
    public interface IGameElement
    {
        Vector3 position { get; set;}
        Vector3 rotation { get; set;}

        void Move(float x, float y, float z);
        void Rotate(float x, float y, float z);
        bool Collision(IGameElement collider);
    }
}

