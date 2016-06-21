using App;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace App.Models
{

    class Opponent : Character
    {
        public float speed;
        public float detectionDistance;
        private Vector3 nextTarget;
        private bool isChasing;
        private TimeSpan lastAttack;
        private TimeSpan attackDelay;

        public Opponent(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            Setup();

            Vector3 newPosition = position;
            Vector3 newSize = new Vector3(5.0f, 10.0f, 40.0f);

            newPosition.X += 45;
            AddCollider("front", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.X -= 90;
            AddCollider("back", new Collider(game, newPosition, newSize, Vector3.Zero));

            newSize = new Vector3(40.0f, 10.0f, 5.0f);

            newPosition.X += 45;
            newPosition.Z += 45;
            AddCollider("right", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.Z -= 90;
            AddCollider("left", new Collider(game, newPosition, newSize, Vector3.Zero));
        }

        private void Setup()
        {
            speed = 5.0f;
            detectionDistance = 1000.0f;
            nextTarget = position;
            isChasing = false;
            lastAttack = TimeSpan.Zero;
            attackDelay = new TimeSpan(0, 0, 1);
        }

        private Vector3 checkSensors(Collider collider, Vector3 vector)
        {
            if(this.colliders["right"].CollidesWith(collider))
            {
                this.colliders["right"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z > 0 ? 0 : vector.Z);
            }

            if(this.colliders["left"].CollidesWith(collider))
            {
                this.colliders["left"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z < 0 ? 0 : vector.Z);
            }

            if(this.colliders["front"].CollidesWith(collider))
            {
                this.colliders["front"].drawColor = Color.OrangeRed;
                vector.X = (vector.X > 0 ? 0 : vector.X);
            }

            if(this.colliders["back"].CollidesWith(collider))
            {
                this.colliders["back"].drawColor = Color.OrangeRed;
                vector.X = (vector.X < 0 ? 0 : vector.X);
            }

            return vector;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(!game.PlayMode)
            {
                return;
            }

            if(this.IsDead())
            {
                OnDead();
                return;
            }

            foreach(Collider collider in colliders.Values)
            {
                collider.drawColor = Color.GreenYellow;
            }

            Vector3 playerDelta = Vector3.Subtract(game.Player.position, position);
            float playerDistance = playerDelta.Length();
            playerDelta.Normalize();
            bool playerVisible = isVisible(playerDelta, playerDistance);

            Vector3 playerToNextTargetDelta = Vector3.Subtract(game.Player.position, nextTarget);

            if(playerToNextTargetDelta.Length() > 300f)
            {
                nextTarget = position;
            }

            if(playerVisible && playerDistance < detectionDistance)
            {
                isChasing = true;
            }
            if(this.colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]))
            {
                if(lastAttack + attackDelay < gameTime.TotalGameTime)
                {
                    game.Player.ReduceLife(19f);
                    //System.Console.WriteLine("-19");
                    lastAttack = gameTime.TotalGameTime;
                }
                nextTarget = position;
                OnIdle();
            }
            else if(isChasing)
            {
                if(Math.Abs(nextTarget.X - position.X) < 20f && Math.Abs(nextTarget.Z - position.Z) < 20f)
                {
                    LinkedList<Tuple<int, int>> newPath = game.Scene.movement.getPathToTarget(
                                                              position,
                                                              game.Player.position);
                    if(newPath != null && newPath.Count > 0)
                    {
                        LinkedListNode<Tuple<int, int>> candidateNode = newPath.Last;
                        do
                        {
                            Vector3 candidatePosition = game.Scene.movement.coordsToPosition(candidateNode.Value);
                            Vector3 candidateDelta = Vector3.Subtract(candidatePosition, position);
                            float candidateDistance = candidateDelta.Length();
                            candidateDelta.Normalize();
                            if(isVisible(candidateDelta, candidateDistance))
                            {
                                nextTarget = candidatePosition;
                                break;
                            }
                            candidateNode = candidateNode.Previous;
                            if(candidateNode == null)
                            {
                                nextTarget = position;
                                break;
                            }
                        } while(true);
                    }
                    else
                    {
                        isChasing = false;
                        OnIdle();
                    }
                }
            }

            Vector3 targetDelta = Vector3.Subtract(nextTarget, position);
            if(targetDelta.Length() < 2f)
                return;

            foreach(IGameObject wall in game.Scene.children["Walls"].children.Values)
            {
                foreach(Collider collider in wall.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach(IGameObject interactiveObject in game.Scene.children["Interactive"].children.Values)
            {
                foreach(Collider collider in interactiveObject.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach(IGameObject other in game.Scene.children["Others"].children.Values)
            {
                foreach(Collider collider in other.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach(IGameObject opponent in game.Scene.children["Opponents"].children.Values)
            {
                if(opponent != this && opponent.colliders.ContainsKey("main"))
                    targetDelta = checkSensors(opponent.colliders["main"], targetDelta);
            }

            targetDelta = checkSensors(game.Scene.Player.colliders["main"], targetDelta);
            float targetDistance = targetDelta.Length();
            targetDelta.Normalize();

            rotateInDirection(targetDelta);

            if(targetDelta.Length() > 0f && targetDistance > 10f)
            {
                moveInDirection(targetDelta);
                OnMove();
            }
            else
            {
                OnIdle();
                nextTarget = position;
            }
        }

        private void moveInDirection(Vector3 direction)
        {
            direction = Vector3.Multiply(direction, speed);
            Move(direction.X, direction.Y, direction.Z);

        }

        void rotateInDirection(Vector3 direction)
        {
            float rotationY = (float)Math.Atan2(direction.X, direction.Z);

            if(Math.Abs(this.rotation.Y - rotationY) > 0.01f)
            {
                this.rotation = new Vector3(0, rotationY, 0);
            }
        }
    }
}
