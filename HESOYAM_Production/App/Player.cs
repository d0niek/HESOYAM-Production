using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using HESOYAM_Production;
using App.Collisions;
using App.Animation;
using System.Collections.Generic;
using System.Linq;

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

        public void update(GameTime gameTime)
        {   
            float angle = this.getAngleFromMouse();

            this.Rotate(0, angle, 0);

            Vector3 vector = this.movePlayer();

            foreach (Collider collider in colliders.Values) {
                collider.drawColor = Color.GreenYellow;
            }

            foreach (IGameObject wall in game.Scene.children["Walls"].children.Values) {
                foreach (Collider collider in wall.colliders.Values) {
                    vector = checkSensors(collider, vector);
                }
            }
            
            foreach (IGameObject interactiveObject in game.Scene.children["Interactive"].children.Values)
            {
                foreach (Collider collider in interactiveObject.colliders.Values)
                {
                    vector = checkSensors(collider, vector);
                }
            }

            foreach (IGameObject door in game.Scene.children["Doors"].children.Values)
            {
                foreach (Collider collider in door.colliders.Values)
                {
                    vector = checkSensors(collider, vector);
                }
            }

            foreach(IGameObject opponent in game.Scene.children["Opponents"].children.Values)
            {
                vector = checkSensors(opponent.colliders["main"], vector);
                if(game.Scene.Player.colliders["main"].CollidesWith(opponent.colliders["main"]))
                {
                    if(game.InputState.Mouse.CurrentMouseState.LeftButton.Equals(ButtonState.Pressed))
                    {
                        if(lastAttack + attackDelay < gameTime.TotalGameTime)
                        {
                            System.Console.WriteLine("Player attacked");
                            lastAttack = gameTime.TotalGameTime;
                        }

                    }
                }
            }

            this.Move(vector.X, 0, vector.Z);
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
            System.Console.WriteLine("Podniesiono " + itemName);
        }
        public bool getKeyInfo(string itemName)
        {
            if (pickedUpObjects.Contains(itemName)) return true;
            else return false;
        }

        private float getAngleFromMouse()
        {
            float angle = this.rotation.Y;

            if (game.InputState.Mouse.isInGameWindow()) {
                Vector2 mousePos = new Vector2(
                                       game.InputState.Mouse.CurrentMouseState.X, 
                                       game.InputState.Mouse.CurrentMouseState.Y
                                   );

                mousePos.X -= this.Game.GraphicsDevice.Viewport.Width / 2;
                mousePos.Y -= this.Game.GraphicsDevice.Viewport.Height / 2;

                angle = (float) (Math.Atan2(mousePos.X, mousePos.Y)) + this.cameraAngle;
            }

            return angle;
        }

        private Vector3 movePlayer()
        {
            Vector3 vector = this.readInputAndMovePlayer();

            if (vector != Vector3.Zero) {
                animatePlayerMove(true);
            } else {
                animatePlayerMove(false);
            }

            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y + cameraAngle);
            vector = Vector3.Transform(vector, rotationMatrixY);

            this.fixSpeedOfMovingDiagonally(vector);

            return vector;
        }

        void animatePlayerMove(bool isMoveing)
        {
            AnimatedObject playerModel = (AnimatedObject) this.children["playerModel"];
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

        private Vector3 readInputAndMovePlayer()
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

        private void fixSpeedOfMovingDiagonally(Vector3 vector)
        {
            if (vector.X == 0f) {
                vector.Z /= (float) Math.Sqrt(2);
            } else if (vector.Z == 0f) {
                vector.X /= (float) Math.Sqrt(2);
            }
        }
    }
}
