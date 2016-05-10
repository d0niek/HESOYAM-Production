using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using HESOYAM_Production;
using App.Collisions;

namespace App
{

    public class Player : GameObject
    {
        public float cameraAngle { get; set; }

        public Player(
            Engine game,
            string name,
            Vector3 position = default(Vector3),
            Vector3 rotaion = default(Vector3)
        ) : base(game, name, position, rotaion)
        {
        }

        public void update(GameTime gameTime, InputState input)
        {
            float angle = this.getAngleFromMouse(input);

            this.Rotate(0, angle, 0);

            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y + cameraAngle);
            PlayerIndex playerIndex;
            Vector3 vector = Vector3.Zero;

            if (input.IsKeyPressed(Keys.W, PlayerIndex.One, out playerIndex)) {
                vector.Z = -10;
            }

            if (input.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex)) {
                vector.Z = 10;
            }

            if (input.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex)) {
                vector.X = -10;
            }

            if (input.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex)) {
                vector.X = 10;
            }

            vector = Vector3.Transform(vector, rotationMatrixY);
            this.Move(vector.X, 0, vector.Z);
        }

        public new void Move(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            position = Vector3.Add(delta, position);

            foreach (IGameElement child in children.Values) {
                child.Move(x, y, z);
            }

            foreach (Collider collider in colliders) {
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

        private float getAngleFromMouse(InputState input)
        {
            float angle = this.rotation.Y;

            if (input.Mouse.isInGameWindow()) {
                Vector2 mousePos = new Vector2(input.Mouse.CurrentMouseState.X, input.Mouse.CurrentMouseState.Y);

                mousePos.X -= this.Game.GraphicsDevice.Viewport.Width / 2;
                mousePos.Y -= this.Game.GraphicsDevice.Viewport.Height / 2;

                angle = (float) (Math.Atan2(mousePos.X, mousePos.Y)) + this.cameraAngle;
            }

            return angle;
        }
    }
}
