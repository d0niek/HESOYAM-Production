using System;
using Microsoft.Xna.Framework;

namespace App.Collisions
{

    public class Collider : IGameElement
    {
        public IGameObject parent { get; set; }

        public Vector3 position { get; set; }

        public Vector3 rotation { get; set; }

        public Collider()
        {
            
        }

        public void Move(float x, float y, float z)
        {
            
        }

        public void Rotate(float x, float y, float z)
        {

        }

        public void RotateAroundParent(float x, float y, float z)
        {

        }

        public bool Collision(IGameElement collider)
        {
            return false;
        }
    }
}
