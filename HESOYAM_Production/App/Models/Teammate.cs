using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace App.Models
{

    public class Teammate : Character
    {
        public Teammate(
            Engine game,
            String name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
        }
    }
}
