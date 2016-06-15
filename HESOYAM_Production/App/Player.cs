using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using HESOYAM_Production;
using App.Collisions;
using App.Animation;
using System.Collections.Generic;
using App.Models;

namespace App
{

    public class Player : GameObject
    {
        float cameraAngle;
        List<string> pickedUpObjects;
        private TimeSpan lastAttack;
        private TimeSpan attackDelay;

        public Player(
            Engine game,
            string name,
            float cameraAngle,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3)
        ) : base(game, name, position, rotation)
        {
            this.cameraAngle = cameraAngle;
            pickedUpObjects = new List<string>();
            Vector3 newPosition = position;
            Vector3 newSize = new Vector3(5.0f, 10.0f, 40.0f);

            newPosition.X += 50;
            AddCollider("front", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.X -= 100;
            AddCollider("back", new Collider(game, newPosition, newSize, Vector3.Zero));

            newSize = new Vector3(40.0f, 10.0f, 5.0f);

            newPosition.X += 50;
            newPosition.Z += 50;
            AddCollider("right", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.Z -= 100;
            AddCollider("left", new Collider(game, newPosition, newSize, Vector3.Zero));

            AddCollidersToGame();

            lastAttack = TimeSpan.Zero;
            attackDelay = new TimeSpan(0, 0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode) {
                return;
            }

            float angle = GetAngleFromMouse();
            Rotate(0, angle, 0);

            Vector3 vector = MovePlayer();

            ResetCollidersColor();
            vector = CheckCollisionsWithSceneObjects(vector);
            vector = CheckCollisionWithOpponents(vector, gameTime);

            Move(vector.X, 0, vector.Z);
        }

        private float GetAngleFromMouse()
        {
            float angle = rotation.Y;

            if (game.InputState.Mouse.isInGameWindow()) {
                Vector2 mousePos = new Vector2(
                                       game.InputState.Mouse.CurrentMouseState.X, 
                                       game.InputState.Mouse.CurrentMouseState.Y
                                   );

                mousePos.X -= Game.GraphicsDevice.Viewport.Width / 2;
                mousePos.Y -= Game.GraphicsDevice.Viewport.Height / 2;

                angle = (float) (Math.Atan2(mousePos.X, mousePos.Y)) + cameraAngle;
            }

            return angle;
        }

        private Vector3 MovePlayer()
        {
            Vector3 vector = ReadInputAndMovePlayer();

            if (vector != Vector3.Zero) {
                AnimatePlayerMove(true);
            } else {
                AnimatePlayerMove(false);
            }

            Matrix rotationMatrixY = Matrix.CreateRotationY(rotation.Y + cameraAngle);
            vector = Vector3.Transform(vector, rotationMatrixY);

            FixSpeedOfMovingDiagonally(vector);

            return vector;
        }

        private Vector3 ReadInputAndMovePlayer()
        {
            PlayerIndex playerIndex;
            Vector3 vector = Vector3.Zero;

            if (game.InputState.IsKeyPressed(Keys.W, PlayerIndex.One, out playerIndex)) {
                vector.Z = -10;
            }

            if (game.InputState.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex)) {
                vector.Z = 10;
            }

            if (game.InputState.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex)) {
                vector.X = -10;
            }

            if (game.InputState.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex)) {
                vector.X = 10;
            }

            return vector;
        }

        void AnimatePlayerMove(bool isMoveing)
        {
            AnimatedObject playerModel = (AnimatedObject) children["playerModel"];
            AnimationPlayer clipPlayer = playerModel.player;

            if (isMoveing) {
                if (clipPlayer.Clip == playerModel.Clips["postawa"]) {
                    playerModel.PlayClip("bieg_przod").Looping = true;
                }
            } else {
                if (clipPlayer.Clip != playerModel.Clips["postawa"]) {
                    playerModel.PlayClip("postawa").Looping = true;
                }
            }
        }

        private void FixSpeedOfMovingDiagonally(Vector3 vector)
        {
            if (vector.X == 0f) {
                vector.Z /= (float) Math.Sqrt(2);
            } else if (vector.Z == 0f) {
                vector.X /= (float) Math.Sqrt(2);
            }
        }

        private void ResetCollidersColor()
        {
            foreach (Collider collider in colliders.Values) {
                collider.drawColor = Color.GreenYellow;
            }
        }

        private Vector3 CheckCollisionsWithSceneObjects(Vector3 vector)
        {
            String[] objectsListInTheScene = { "Walls", "Interactive", "Windows", "Others" };

            foreach (String objectsList in objectsListInTheScene) {
                vector = CheckCollisionsWithObjects(objectsList, vector);
            }

            vector = CheckCollisionWithDoors(vector);

            return vector;
        }

        private Vector3 CheckCollisionsWithObjects(String objectsList, Vector3 vector)
        {
            foreach (IGameObject objectOnScene in game.Scene.children[objectsList].children.Values) {
                foreach (Collider collider in objectOnScene.colliders.Values) {
                    vector = CheckSensors(collider, vector);
                }
            }

            return vector;
        }

        private Vector3 CheckCollisionWithDoors(Vector3 vector)
        {
            foreach (Door door in game.Scene.children["Doors"].children.Values) {
                if (!door.IsOpen) {
                    foreach (Collider collider in door.colliders.Values) {
                        vector = CheckSensors(collider, vector);
                    }
                }
            }

            return vector;
        }

        private Vector3 CheckCollisionWithOpponents(Vector3 vector, GameTime gameTime)
        {
            foreach (IGameObject opponent in game.Scene.children["Opponents"].children.Values) {
                vector = CheckSensors(opponent.colliders["main"], vector);
                if (game.Scene.Player.colliders["main"].CollidesWith(opponent.colliders["main"])) {
                    if (game.InputState.Mouse.CurrentMouseState.LeftButton.Equals(ButtonState.Pressed)) {
                        if (lastAttack + attackDelay < gameTime.TotalGameTime) {
                            Console.WriteLine("Player attacked");
                            lastAttack = gameTime.TotalGameTime;
                        }
                    }
                }
            }

            return vector;
        }

        private Vector3 CheckSensors(Collider collider, Vector3 vector)
        {
            if (colliders["right"].CollidesWith(collider)) {
                colliders["right"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z > 0 ? 0 : vector.Z);
            }

            if (colliders["left"].CollidesWith(collider)) {
                colliders["left"].drawColor = Color.OrangeRed;
                vector.Z = (vector.Z < 0 ? 0 : vector.Z);
            }

            if (colliders["front"].CollidesWith(collider)) {
                colliders["front"].drawColor = Color.OrangeRed;
                vector.X = (vector.X > 0 ? 0 : vector.X);
            }

            if (colliders["back"].CollidesWith(collider)) {
                colliders["back"].drawColor = Color.OrangeRed;
                vector.X = (vector.X < 0 ? 0 : vector.X);
            }

            return vector;
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);

            foreach (IGameElement child in children.Values) {
                child.Move(x, y, z);
            }

            foreach (Collider collider in colliders.Values) {
                collider.Move(x, y, z);
            }
        }

        public new void Rotate(float x, float y, float z)
        {
            foreach (IGameElement child in children.Values) {
                if (!(child is Camera)) {
                    child.SetRotation(x, y, z);
                }
            }
        }

        public void addItemToCollection(string itemName)
        {
            pickedUpObjects.Add(itemName);
            Console.WriteLine("Podniesiono " + itemName);
        }

        public bool getKeyInfo(string itemName)
        {
            return pickedUpObjects.Contains(itemName);
        }
    }
}
