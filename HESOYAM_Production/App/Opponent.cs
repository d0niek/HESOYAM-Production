using App;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Opponent : GameObject
    {
        public float speed;
        public float detectionDistance;

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
            detectionDistance = 500.0f;
        }

        public void update()
        {
            Vector3 playerDelta = Vector3.Subtract(game.player.position, position);
            float playerDistance = playerDelta.Length();
            playerDelta.Normalize();
            bool playerVisible = isVisible(playerDelta, playerDistance);
            Vector3 targetDelta;

            if(playerVisible)
            {
                targetDelta = Vector3.Subtract(game.player.position, position);
            }
            else
            {
                Vector3? aStarTarget = game.Scene.movement.getMovementTarget(position, game.player.position);
                if(aStarTarget == null) return;
                targetDelta = Vector3.Subtract((Vector3)aStarTarget, position);
            }

            targetDelta.Normalize();
            moveInDirection(targetDelta);
        }

        private void moveInDirection(Vector3 direction)
        {
            direction = Vector3.Multiply(direction, speed);
            Move(direction.X, direction.Y, direction.Z);
        }

        private bool isVisible(Vector3 direction, float distance)
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
            System.Console.WriteLine("molte volte");
            return true;
        }
    }
}
