using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;

namespace App.Models
{

    class InteractiveObject : GameObject
    {
        string itemName;
        string description;

        public InteractiveObject(
            Engine game,
            string name,
            Model model,
            string itemName,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.itemName = itemName;
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode) {
                return;
            }

            if (colliders["main"].CollidesWith(game.Scene.Player.colliders["main"])) {
                if (itemName != null) {
                    game.Player.addItemToCollection(itemName);
                    itemName = null;
                }
            }
        }
    }
}
