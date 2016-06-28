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

    public class Player : Character
    {
        float cameraAngle;
        bool isAttacking;
        List<string> bag;
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
            isAttacking = false;
            bag = new List<string>();

            Vector3 newPosition = position;
            Vector3 newSize = new Vector3(35f, 190f, 35f);
            newPosition.Y += 95f;

            AddCollider("hitbox", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition.Y += 30;
            newSize = new Vector3(100f, 250f, 100f);
            AddCollider("main", new Collider(game, newPosition, newSize, Vector3.Zero));

            newPosition = position;
            newSize = new Vector3(5.0f, 10.0f, 40.0f);

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

            AddCollidersToGame();

            lastAttack = TimeSpan.Zero;
            attackDelay = new TimeSpan(0, 0, 0, 0, 870);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            

            if (IsDead()) {
                OnDead();
                return;
            }
            if (isAttacking) {
                OnAttack();
                return;
            }
            
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

            if (this.IsDead()) {
                this.OnDead();
            } else {
                if (vector != Vector3.Zero) {
                    OnMove(vector);
                } else {
                    OnIdle();
                }
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
                vector.Z -= 10;
            }

            if (game.InputState.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex)) {
                vector.Z += 10;
            }

            if (game.InputState.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex)) {
                vector.X -= 10;
            }

            if (game.InputState.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex)) {
                vector.X += 10;
            }

            return vector;
        }

        protected void OnMove(Vector3 moveVector)
        {
            AnimatedObject playerModel = (AnimatedObject) children["playerModel"];
            float rotationY = (float) Math.Atan2(moveVector.X, moveVector.Z);
            rotationY = (float) ((GetAngleFromMouse() - rotationY) % (Math.PI * 2) + (Math.PI / 4));

            if (rotationY < 0) {
                rotationY += (float) (Math.PI * 2);
            }


            if (rotationY >= (1.75 * Math.PI) || rotationY < Math.PI / 4) {
                playerModel.PlayClip("bieg_przod").Looping = true;
            } else if (rotationY <= (1.75 * Math.PI) && rotationY >= (1.25 * Math.PI)) {
                playerModel.PlayClip("bieg_lewo").Looping = true;
            } else if (rotationY <= (0.75 * Math.PI) && rotationY > Math.PI / 4) {
                playerModel.PlayClip("bieg_prawo").Looping = true;
            } else {
                playerModel.PlayClip("bieg_tyl").Looping = true;
            }
        }

        new protected void OnIdle()
        {
            AnimatedObject playerModel = (AnimatedObject) children["playerModel"];
            playerModel.PlayClip("postawa").Looping = true;
        }

        new protected void OnDead()
        {
            AnimatedObject playerModel = (AnimatedObject) children["playerModel"];
            playerModel.PlayClip("smierc").Looping = false;
        }

        new protected void OnAttack()
        {
            if (!isAttacking)
                isAttacking = true;
            AnimatedObject playerModel = (AnimatedObject) children["playerModel"];
            AnimationPlayer player = playerModel.PlayClip("cios_piesc");
            player.Looping = false;
            System.Console.WriteLine("czas animacji " + player.Duration);
            if (player.Position >= player.Duration)
                isAttacking = false;
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
            String[] objectsListInTheScene = { "Walls", "Interactive", "Windows", "Others", "Teammates" };

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
            String[] doors = { "Doors", "ExitDoors" };

            foreach (String door in doors) {
                foreach (Door d in game.Scene.children[door].children.Values) {
                    if (!d.IsOpen) {
                        foreach (Collider collider in d.colliders.Values) {
                            vector = CheckSensors(collider, vector);
                        }
                    }
                }
            }

            return vector;
        }

        private Vector3 CheckCollisionWithOpponents(Vector3 vector, GameTime gameTime)
        {
            //List<String> opponentsToRemove = new List<String>();
            foreach (Opponent opponent in game.Scene.children["Opponents"].children.Values) {
                if (opponent.colliders.ContainsKey("main"))
                    vector = CheckSensors(opponent.colliders["main"], vector);

                if (IsCollisionWithOpponent(opponent) && opponent.IsMouseOverObject()) {
                    OnMouseLeftButtonPressed(() => AttackOpponent(opponent, gameTime));
 
                }
            }

            //RemoveOpponentsFromScene(opponentsToRemove);

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

        private bool IsCollisionWithOpponent(Opponent opponent)
        {
            if (opponent.colliders.ContainsKey("main") && game.Scene.Player.colliders.ContainsKey("main")) {
                return game.Scene.Player.colliders["main"].CollidesWith(opponent.colliders["main"]);
            } else
                return false;
        }

        private void AttackOpponent(Opponent opponent, GameTime gameTime)
        {
            rotateInDirection(Vector3.Subtract(opponent.position, this.position));
            if (lastAttack + attackDelay < gameTime.TotalGameTime) {
                OnAttack();
                opponent.ReduceLife(34f);
                lastAttack = gameTime.TotalGameTime;
            }
        }

        private void RemoveOpponentsFromScene(List<string> opponentsToRemove)
        {
            foreach (String opponentToRemove in opponentsToRemove) {
                IGameComponent opponent = game.Scene.children["Opponents"].RemoveChild(opponentToRemove) as IGameComponent;
                game.Components.Remove(opponent);
            }
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

        void rotateInDirection(Vector3 direction)
        {
            float rotationY = (float)Math.Atan2(direction.X, direction.Z);

            if(Math.Abs(this.rotation.Y - rotationY) > 0.01f)
            {
                this.Rotate(0, rotationY, 0);
            }
        }

        public void addItemToBag(String item)
        {
            bag.Add(item);
            game.Hud.Message = "You have obtained a \"" + item + "\"";
        }

        public bool hasItemInBag(String item)
        {
            return bag.Contains(item);
        }

        public void checkIfFirstAidKit()
        {
            if (hasItemInBag("first aid kit")) {   
                IncreaseLife(50f);
                bag.Remove("first aid kit");
            }
        }
    }
}
