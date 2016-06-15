using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

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

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode) {
                return;
            }

            if (hasItem) {
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
            Console.WriteLine("Picked");
            MoveCupboardFromInteractiveObjectToOther();
            hasItem = false;
        }

        private void MoveCupboardFromInteractiveObjectToOther()
        {
            game.Scene.children["Interactive"].RemoveChild(this);
            game.Scene.children["Others"].AddChild(this);
            this.Hover = false;
        }
    }
}
