using App;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HESOYAM_Production.App
{
    class Opponent : GameObject
    {
        public float speed;

        public Opponent(
            Engine game,
            string name,
            float cameraAngle,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3)
        ) : base(game, name, position, rotation)
        {
            Setup();
        }

        public Opponent(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation)
        {
            Setup();
        }

        private void Setup()
        {
            speed = 3.0f;
        }

        public void update()
        {
            Vector3 delta = Vector3.Subtract(game.player.position, position);
            delta.Normalize();
            delta = Vector3.Multiply(delta, speed);
            Move(delta.X, delta.Y, delta.Z);
        }
    }
}
