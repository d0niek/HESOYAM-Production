using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App.Models
{

    public class Cupboard : GameObject
    {
        bool hasItem;

        public Cupboard(
            Engine game,
            string name,
            Model model,
            bool hasItem,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.hasItem = hasItem;
        }
    }
}

