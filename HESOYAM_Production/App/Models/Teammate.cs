using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using App.Collisions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace App.Models
{

    public class Teammate : Character
    {
        private float speed;
        private Vector3 nextTarget;
        private Vector3 targetedInteractivePosition;

        public Teammate(
            Engine game,
            String name,
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
            nextTarget = position;
            targetedInteractivePosition = position;
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

        private void onMoveToCommand()
        {
            String[] sceneInteractiveObjectsToLoop = { "Doors", "Interactive", "Opponents" };

            foreach(String interactiveObjectsToLoop in sceneInteractiveObjectsToLoop)
            {
                foreach(GameObject interactive in game.Scene.children[interactiveObjectsToLoop].children.Values)
                {
                    if(interactive.IsMouseOverObject())
                    {
                        nextTarget = position;
                        targetedInteractivePosition = interactive.position;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!game.PlayMode)
            {
                if(active)
                {
                    if(game.InputState.Mouse.CurrentMouseState.RightButton.Equals(ButtonState.Pressed));
                    {
                        onMoveToCommand();
                    }
                }
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

            if(Math.Abs(targetedInteractivePosition.X - position.X) < 100f && Math.Abs(targetedInteractivePosition.Z - position.Z) < 100f)
            {
                nextTarget = position;
                OnIdle();
            }
            else if(Math.Abs(nextTarget.X - position.X) < 20f && Math.Abs(nextTarget.Z - position.Z) < 20f)
            {
                LinkedList<Tuple<int, int>> newPath = game.Scene.movement.getPathToTarget(
                                                          position,
                                                          targetedInteractivePosition);
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
                    } while(candidateNode != null);
                }
                else
                {
                    nextTarget = position;
                    OnIdle();
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

            foreach(IGameObject opponent in game.Scene.children["Opponents"].children.Values)
            {
                if(opponent != this && opponent.colliders.ContainsKey("main"))
                    targetDelta = checkSensors(opponent.colliders["main"], targetDelta);
            }

            targetDelta = checkSensors(game.Scene.Player.colliders["main"], targetDelta);
            targetDelta.Normalize();

            rotateInDirection(targetDelta);

            if(targetDelta.Length() > 0f)
            {
                moveInDirection(targetDelta);
                OnMove();
            }
        }
    }
}
