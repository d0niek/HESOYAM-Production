﻿using App;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace App.Models
{
    class RangedOpponent : Opponent
    {
        private float shootDistance;
        bool isShooting;
        
        public RangedOpponent(

            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            {
                shootDistance = 400.0f;
                isShooting = false;

            }
        }

        private void shoot(Vector3 direction, TimeSpan time)
        {
            if (lastShoot + shootDelay < time)
            {
                lastShoot = time;
                new Projectile(game, new Vector3(position.X, position.Y + 120f, position.Z), direction, 15f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!game.PlayMode)
            {
                return;
            }

            Vector3 playerDelta = Vector3.Subtract(game.Player.position, position);
                                
            if (this.IsDead())
            {
                OnDead();
                return;
            }

            foreach (Collider collider in colliders.Values)
            {
                collider.drawColor = Color.GreenYellow;
            }

            //Vector3 playerDelta = Vector3.Subtract(game.Player.position, position);
            float playerDistance = playerDelta.Length();
            playerDelta.Normalize();
            bool playerVisible = isVisible(playerDelta, playerDistance);

            Vector3 playerToNextTargetDelta = Vector3.Subtract(game.Player.position, nextTarget);

            if (playerToNextTargetDelta.Length() > 300f)
            {
                nextTarget = position;
            }

            if (playerVisible && (playerDistance < detectionDistance) && !isShooting)
            {
                isChasing = true;
            }

            if(playerDistance <= shootDistance)
            {
                isShooting = true;
                OnIdle2();
                // brak animacji idle dla broni
            }

            if (isShooting)
            {
                this.rotateInDirection(playerDelta);
                shoot(playerDelta, gameTime.TotalGameTime);
                isShooting = false;
                return;
            }

             if (isChasing)
            {
                
                if (Math.Abs(nextTarget.X - position.X) < 20f && Math.Abs(nextTarget.Z - position.Z) < 20f)
                {
                    LinkedList<Tuple<int, int>> newPath = game.Scene.movement.getPathToTarget(
                                                              position,
                                                              game.Player.position);
                    if (newPath != null && newPath.Count > 0)
                    {
                        LinkedListNode<Tuple<int, int>> candidateNode = newPath.Last;
                        do
                        {
                            Vector3 candidatePosition = game.Scene.movement.coordsToPosition(candidateNode.Value);
                            Vector3 candidateDelta = Vector3.Subtract(candidatePosition, position);
                            float candidateDistance = candidateDelta.Length();
                            candidateDelta.Normalize();
                            if (isVisible(candidateDelta, candidateDistance))
                            {
                                nextTarget = candidatePosition;
                                break;
                            }
                            candidateNode = candidateNode.Previous;
                            if (candidateNode == null)
                            {
                                nextTarget = position;
                                break;
                            }
                        } while (true);
                    }
                    else
                    {
                        isChasing = false;
                        OnIdle();
                    }
                }
            }

            Vector3 targetDelta = Vector3.Subtract(nextTarget, position);
            if (targetDelta.Length() < 2f)
                return;

            foreach (IGameObject wall in game.Scene.children["Walls"].children.Values)
            {
                foreach (Collider collider in wall.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach (IGameObject interactiveObject in game.Scene.children["Interactive"].children.Values)
            {
                foreach (Collider collider in interactiveObject.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach (IGameObject other in game.Scene.children["Others"].children.Values)
            {
                foreach (Collider collider in other.colliders.Values)
                {
                    targetDelta = checkSensors(collider, targetDelta);
                }
            }

            foreach (IGameObject opponent in game.Scene.children["Opponents"].children.Values)
            {
                if (opponent != this && opponent.colliders.ContainsKey("main"))
                    targetDelta = checkSensors(opponent.colliders["main"], targetDelta);
            }

            targetDelta = checkSensors(game.Scene.Player.colliders["main"], targetDelta);
            float targetDistance = targetDelta.Length();
            targetDelta.Normalize();

            rotateInDirection(targetDelta);

            if (targetDelta.Length() > 0f && targetDistance > 10f)
            {
                moveInDirection(targetDelta);
                OnMove2();
            }
            else
            {
                OnIdle();
                nextTarget = position;
            }
        }
    }

}
