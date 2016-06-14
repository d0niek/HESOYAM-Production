using System;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App.Models
{

    public class Lamp : GameObject
    {
        public Lamp(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.emisiveColor = new Vector3(0.3f,0.3f,0.2f);
        }
    }
}

