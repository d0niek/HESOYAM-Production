using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace App.Models
{

    public class Cupboard : GameObject, IInteractiveObject
    {
        String item;

        public Cupboard(
            Engine game,
            string name,
            Model model,
            String item,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.item = item;
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode) {
                return;
            }

            if (item != "") {
                CheckCollisionWithPlayer();
            }
        }

        private void CheckCollisionWithPlayer()
        {
            if (IsCollisionWithPlayer() && IsMouseOverObject()) {
                OnMouseLeftButtonClick(PickupItem);
            }
        }

        private bool IsCollisionWithPlayer()
        {
            return colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]);
        }

        private void PickupItem()
        {
            game.Player.addItemToBag(item);
            game.Player.checkIfFirstAidKit();
            MoveCupboardFromInteractiveObjectToOther();
            item = "";
        }

        private void MoveCupboardFromInteractiveObjectToOther()
        {
            game.Scene.children["Interactive"].RemoveChild(this);
            game.Scene.children["Others"].AddChild(this);
            this.Hover = false;
        }

        #region IInteractiveOptions implementation

        public List<String> GetOptionsToInteract()
        {
            List<String> options = new List<String>();
            options.Add("Take content");

            return options;
        }

        #endregion
    }
}
