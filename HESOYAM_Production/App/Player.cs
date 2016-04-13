using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using App.Collisions;

namespace App
{

    public class Player : GameObject
    {
        public Player(
            Game game,
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

            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y);
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

        public void Rotate(float x, float y, float z)
        {
            Vector3 delta = new Vector3(x, y, z);
            rotation = Vector3.Add(delta, rotation);

            foreach (IGameElement child in children.Values) {
                if (!(child is Camera)) {
                    child.Rotate(x, y, z);
                }
            }

            foreach (Collider collider in colliders) {
                collider.RotateAroundParent(x, y, z);
            }
        }

        private float getAngleFromMouse(InputState input)
        {
            float angle = 0.0f;
            Vector2 windowSize = new Vector2(
                                     this.Game.GraphicsDevice.Viewport.Width,
                                     this.Game.GraphicsDevice.Viewport.Height
                                 );
            Vector2 mousePos = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);

            if (mousePos.X >= 0 && mousePos.Y >= 0 && mousePos.X <= windowSize.X && mousePos.Y <= windowSize.Y) {
                Vector2 direction = mousePos - (windowSize / 2);
                angle = (float) (Math.Atan2(direction.Y, direction.X));
            }

            return angle;
        }
    }
}
