using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using App.Collisions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using HESOYAM_Production.App;

namespace App.Models
{

    public class Teammate : Character
    {
        private float speed;
        private Vector3 nextTarget;
        private GameObject targetedObject;
        private TimeSpan lastAttack;
        private TimeSpan attackDelay;
        private LinkedList<Tuple<int, int>> newPath;
        //private List<Emitter> emitterPath;

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
            targetedObject = this;
            lastAttack = TimeSpan.Zero;
            attackDelay = new TimeSpan(0, 0, 0, 0, 870);
            //emitterPath = new List<Emitter>();
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

        private void onMoveToCommand()
        {
            String[] sceneInteractiveObjectsToLoop = { "Interactive", "Doors", "Opponents" };

            foreach(String interactiveObjectsToLoop in sceneInteractiveObjectsToLoop)
            {
                foreach(GameObject interactive in game.Scene.children[interactiveObjectsToLoop].children.Values)
                {
                    if(interactive.IsMouseOverObject())
                    {
                        nextTarget = position;
                        targetedObject = interactive;
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
                    if(game.InputState.Mouse.CurrentMouseState.RightButton.Equals(ButtonState.Pressed))
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

            Vector3 targetToNextTargetDelta = Vector3.Subtract(targetedObject.position, nextTarget);

            if(targetToNextTargetDelta.Length() > 300f)
            {
                nextTarget = position;
            }

            if((targetedObject.colliders.ContainsKey("main") && colliders["main"].CollidesWith(targetedObject.colliders["main"]))
                || Math.Abs(targetedObject.position.X - position.X) < 20f && Math.Abs(targetedObject.position.Z - position.Z) < 20f)
            {
                nextTarget = position;
            }
            else if(Math.Abs(nextTarget.X - position.X) < 20f && Math.Abs(nextTarget.Z - position.Z) < 20f)
            {
                newPath = game.Scene.movement.getPathToTarget(
                                                          position,
                                                          targetedObject.position);
                if(newPath != null && newPath.Count > 0)
                {
                    /*foreach(Emitter i in emitterPath)
                    {
                        game.Components.Remove(i);
                    }
                    emitterPath.Clear();*/

                    LinkedListNode<Tuple<int, int>> candidateNode = newPath.Last;
                    bool nextTargetFound = false;

                    do
                    {
                        Vector3 candidatePosition = game.Scene.movement.coordsToPosition(candidateNode.Value);
                        Vector3 candidateDelta = Vector3.Subtract(candidatePosition, position);
                        float candidateDistance = candidateDelta.Length();
                        candidateDelta.Normalize();
                        if(isVisible(candidateDelta, candidateDistance))
                        {
                            if(!nextTargetFound)
                            {
                                nextTarget = candidatePosition;
                                nextTargetFound = true; 
                            }
                        }

                        /*Emitter newEmitter = new Emitter(game, candidatePosition);
                        newEmitter.amountPerRelase = 1;
                        newEmitter.emitDelay = new TimeSpan(0, 0, 0, 0, 300);
                        Particle customParticle = new Particle();
                        customParticle.acceleration = 1.0f;
                        if(candidateNode.Previous != null)
                        {
                            Vector3 translation = Vector3.Subtract(candidatePosition, game.Scene.movement.coordsToPosition(candidateNode.Previous.Value));
                            translation.Normalize();
                            translation = Vector3.Multiply(translation, 1.0f);
                            customParticle.translation = translation;
                        }
                        newEmitter.customParticle = customParticle;
                        emitterPath.Add(newEmitter);*/

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

            foreach(Opponent opponent in game.Scene.children["Opponents"].children.Values)
            {
                if(opponent.colliders.ContainsKey("main"))
                {
                    if(targetedObject == opponent && colliders["main"].CollidesWith(opponent.colliders["main"]))
                    {
                        AttackOpponent(opponent, gameTime);
                        return;
                    }
                    targetDelta = checkSensors(opponent.colliders["main"], targetDelta);
                }
            }

            if(targetDelta.Length() < 2f)
            {
                OnIdle();
                return;
            }

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

            targetDelta = checkSensors(game.Scene.Player.colliders["main"], targetDelta);
            targetDelta.Normalize();

            rotateInDirection(targetDelta, true);

            if(targetDelta.Length() > 0f)
            {
                moveInDirection(targetDelta);
                OnMove();
            }
        }

        private void AttackOpponent(Opponent opponent, GameTime gameTime)
        {
            opponent.trigger(this);
            rotateInDirection(Vector3.Subtract(opponent.position, this.position), true);
            if(lastAttack + attackDelay < gameTime.TotalGameTime)
            {
                OnAttack();
                opponent.ReduceLife(25f);
                lastAttack = gameTime.TotalGameTime;
            }
            if(opponent.IsDead())
                targetedObject = this;
        }
    }
}
