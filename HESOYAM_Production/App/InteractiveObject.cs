using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;

namespace App
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
           ) : base(game, name, model, position, rotation)
        {
            this.itemName = itemName;
        }



        public void update()
        {
            if (colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]))
            {
                if(itemName != null)
                {
                    game.player.addItemToCollection(itemName);
                    itemName = null;
                }

            }
        }
    }
}
