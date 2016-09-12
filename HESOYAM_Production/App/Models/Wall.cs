using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App.Models
{
    public class Wall : Segment
    {
        public Wall(
            Engine game,
            string name,
            Model model,
            Model modelCut,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, modelCut, position, rotation, scale)
        {
        }
    }
}
