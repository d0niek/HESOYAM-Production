using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using App.Collisions;
using HESOYAM_Production.App;

namespace App
{

    public class Projectile : GameObject
    {
        public Vector3 direction;
        public float speed;

        public Projectile(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            speed = 5.0f;
            direction = default(Vector3);
            game.AddComponent(this);
        }

        public Projectile(
            Engine game,
            Vector3 position,
            Vector3 direction,
            float speed = 10.0f
        ) : base(game, "defaultProjectile", game.Models["strzykawka"], position, default(Vector3), new Vector3(3f, 3f, 3f))
        {
            this.direction = direction;
            this.speed = speed;
            rotation = new Vector3(-(float)Math.Atan2(direction.X, direction.Z) + (float)Math.PI / 2, 0f, -(float)Math.PI / 2);
            game.AddComponent(this);
            AddCollider("back", new Collider(game, position, new Vector3(10f, 10f, 10f), Vector3.Zero));
            Vector3 front = new Vector3(position.X + (50f * direction.X), position.Y, position.Z + (50f * direction.Z));
            AddCollider("front", new Collider(game, front, new Vector3(10f, 10f, 10f), Vector3.Zero));
            AddCollidersToGame();
            AddChild(new Emitter(game, front));
        }

        public override void Draw(GameTime gameTime)
        {
            DrawModel(model);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(game.PlayMode)
            {
                moveInDirection();
                CheckCollisionsWithObstacles();
                CheckCollisionsWithPlayer();
            }
        }

        private void moveInDirection()
        {
            Vector3 next = Vector3.Multiply(direction, speed);
            Move(next.X, next.Y, next.Z);
        }
        
        private void destroy()
        {
            foreach(Collider colliderToRemove in colliders.Values)
                game.Components.Remove(colliderToRemove);
            foreach(IGameComponent child in children.Values)
                game.Components.Remove(child);
            game.Components.Remove(this);
        }

        private void CheckCollisionsWithObstacles()
        {
            String[] objectsListInTheScene = { "Walls", "Interactive", "Windows", "Others" };

            foreach(String objectsList in objectsListInTheScene)
            {
                foreach(IGameObject obstacle in game.Scene.children[objectsList].children.Values)
                {
                    foreach(Collider otherCollider in obstacle.colliders.Values)
                    {
                        foreach(Collider thisCollider in colliders.Values)
                        {
                            if(thisCollider.CollidesWith(otherCollider))
                            {
                                destroy();
                            }
                        }
                    }
                }
            }
        }

        private void CheckCollisionsWithPlayer()
        {
            foreach(Collider thisCollider in colliders.Values)
            {
                if(thisCollider.CollidesWith(game.Player.colliders["hitbox"]))
                {
                    game.Player.ReduceLife(19f);
                    destroy();
                }
            }
            
        }
}
}
