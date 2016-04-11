using System;
using Microsoft.Xna.Framework;

namespace App
{

    public class Camera : GameObject
    {
        public Camera(Game game, Vector3 p = default(Vector3), Vector3 r = default(Vector3)) : base(game, p, r)
        {
        }
    }
}
