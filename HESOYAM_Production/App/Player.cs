using System;
using Microsoft.Xna.Framework;

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

        public void update()
        {
            
        }
    }
}

