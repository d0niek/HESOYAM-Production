using App;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace App.Models
{
    class MeleeOpponent : Opponent
    {        
        public MeleeOpponent(

            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            {
                
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(!game.PlayMode || game.Player.IsDead())
            {
                return;
            }

            Vector3 playerDelta = Vector3.Subtract(game.Player.position, position);
            Vector3 attackedCharacterDelta = Vector3.Zero;
            if(attackedCharacter != null) attackedCharacterDelta = Vector3.Subtract(attackedCharacter.position, position);
            if(IsAttacking)
            {
                this.rotateInDirection(attackedCharacterDelta, true);
                OnAttack();

                if(!this.colliders["main"].CollidesWith(attackedCharacter.colliders["main"])) IsAttacking = false;
                if((gameTime.TotalGameTime - lastAttack) > attackDelay || attackedCharacter.IsDead())
                {
                    IsAttacking = false;
                    attackedCharacter.ReduceLife(12f);
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
                attackedCharacter = game.Player;
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

            if(attackedCharacter != null && attackedCharacter.colliders.ContainsKey("main") ? this.colliders["main"].CollidesWith(attackedCharacter.colliders["main"]) : false)
            {
                if(lastAttack + attackDelay < gameTime.TotalGameTime && !game.Player.IsDead())
                {
                    if(!IsAttacking) IsAttacking = true;
                    OnAttack();
                    lastAttack = gameTime.TotalGameTime;
                }
                nextTarget = position;
                if(!IsAttacking) OnIdle();
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

            foreach(IGameObject teammate in game.Scene.children["Teammates"].children.Values)
            {
                if(teammate.colliders.ContainsKey("main"))
                    targetDelta = checkSensors(teammate.colliders["main"], targetDelta);
            }

            targetDelta = checkSensors(game.Scene.Player.colliders["main"], targetDelta);
            float targetDistance = targetDelta.Length();
            targetDelta.Normalize();

            rotateInDirection(targetDelta, true);

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
    }

}
