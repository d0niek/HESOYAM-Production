using App;
using App.Collisions;
using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace App.Models
{

    abstract class Opponent : Character, IInteractiveObject
    {
        public float speed;
        public float detectionDistance;
        protected Vector3 nextTarget;
        protected bool isChasing;
        protected TimeSpan lastAttack;
        protected TimeSpan attackDelay;
        protected TimeSpan lastShoot;
        protected TimeSpan shootDelay;
        protected Character attackedCharacter;

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
            attackDelay = new TimeSpan(0, 0, 0, 0, 867);
            lastShoot = TimeSpan.Zero;
            shootDelay = new TimeSpan(0, 0, 1);
        }

        protected Vector3 checkSensors(Collider collider, Vector3 vector)
        {
            if (this.colliders["right"].CollidesWith(collider)) {
                this.colliders["right"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z > 0 ? 0 : vector.Z);
            }

            if (this.colliders["left"].CollidesWith(collider)) {
                this.colliders["left"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z < 0 ? 0 : vector.Z);
            }

            if (this.colliders["front"].CollidesWith(collider)) {
                this.colliders["front"].drawColor = Color.OrangeRed;
                vector.X = (vector.X > 0 ? 0 : vector.X);
            }

            if (this.colliders["back"].CollidesWith(collider)) {
                this.colliders["back"].drawColor = Color.OrangeRed;
                vector.X = (vector.X < 0 ? 0 : vector.X);
            }

            return vector;
        }

        protected void moveInDirection(Vector3 direction)
        {
            direction = Vector3.Multiply(direction, speed);
            Move(direction.X, direction.Y, direction.Z);
        }

        public void trigger(Teammate teammate)
        {
            attackedCharacter = teammate;
        }

        #region IInteractiveOptions implementation

        public String[] GetOptionsToInteract()
        {
            String[] options = { "Attack" };
            return options;
        }

        #endregion
    }
}
