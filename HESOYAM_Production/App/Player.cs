using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        public void update(InputState input)
        {
            Matrix rotationMatrixY = Matrix.CreateRotationY(this.rotation.Y);
            PlayerIndex playerIndex;
            Vector3 vector = Vector3.Zero;

            if (input.IsKeyPressed(Keys.W, PlayerIndex.One, out playerIndex)) {
                vector.Z = 10;
                vector = Vector3.Transform(vector, rotationMatrixY);
            }

            if (input.IsKeyPressed(Keys.S, PlayerIndex.One, out playerIndex)) {
                vector.Z = -10;
                vector = Vector3.Transform(vector, rotationMatrixY);
            }

            if (input.IsKeyPressed(Keys.A, PlayerIndex.One, out playerIndex)) {
                vector.X = 10;
                vector = Vector3.Transform(vector, rotationMatrixY);
            }

            if (input.IsKeyPressed(Keys.D, PlayerIndex.One, out playerIndex)) {
                vector.X = -10;
                vector = Vector3.Transform(vector, rotationMatrixY);
            }
            
            this.Move(vector.X, 0, vector.Z);
        }
    }
}

