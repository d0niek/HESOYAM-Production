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
        private Vector3 nextTarget;
        private bool isChasing;

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
            speed = 5.0f;
            detectionDistance = 500.0f;
            nextTarget = position;
            isChasing = false;
        }

        public void update()
        {
            Vector3 playerDelta = Vector3.Subtract(game.player.position, position);
            float playerDistance = playerDelta.Length();
            playerDelta.Normalize();
            bool playerVisible = isVisible(playerDelta, playerDistance);

            if(playerVisible)
            {
                nextTarget = game.player.position;
                isChasing = true;
            }
            else if(isChasing)
            {
                if(Math.Abs(nextTarget.X - position.X) < 10f && Math.Abs(nextTarget.Z - position.Z) < 10f)
                {
                    LinkedList<Tuple<int, int>> newPath = game.Scene.movement.getPathToTarget(position, game.player.position);
                    if(newPath != null && newPath.Count > 0)
                    {
                        LinkedListNode<Tuple<int, int>> candidateNode = newPath.First;
                        do
                        {
                            Vector3 candidatePosition = game.Scene.movement.coordsToPosition(candidateNode.Value);
                            Vector3 candidateDelta = Vector3.Subtract(candidatePosition, position);
                            float candidateDistance = candidateDelta.Length();
                            candidateDelta.Normalize();
                            if(isVisible(candidateDelta, candidateDistance))
                            {
                                nextTarget = candidatePosition;
                            }
                            else break;
                            candidateNode = candidateNode.Next;
                        }
                        while(candidateNode != null);
                    }
                }
            }

            Vector3 targetDelta = Vector3.Subtract(nextTarget, position);
            if(targetDelta.Length() < 2f) return;
            targetDelta.Normalize();
            if(targetDelta.Length() > 0) moveInDirection(targetDelta);
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
            return true;
        }
    }
}
