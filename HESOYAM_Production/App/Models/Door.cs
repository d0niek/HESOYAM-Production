using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;

namespace App.Models
{

    class Door : Wall
    {
        bool isLock;
        bool isOpen;

        public bool IsOpen {
            get { return isOpen; }
            private set { }
        }

        public Door(
            Engine game,
            string name,
            Model model,
            Model transparentModel,
            bool isLock,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, transparentModel, position, rotation, scale)
        {
            this.isLock = isLock;
            isOpen = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode) {
                return;
            }

            if (IsCollisionWithPlayer() && IsMouseOverObject()) {
                OnMouseLeftButtonClick(TryToOpenDoor);
            }
        }

        private bool IsCollisionWithPlayer()
        {
            return colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]);
        }

        private void TryToOpenDoor()
        {
            if (isLock) {
                TryToUnlockDoor();
            } else {
                isOpen = !isOpen;
            }
        }

        private void TryToUnlockDoor()
        {
            Console.WriteLine("Need key");
        }
    }
}
