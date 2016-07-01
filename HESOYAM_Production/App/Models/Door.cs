using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;
using System.Collections.Generic;

namespace App.Models
{

    class Door : Wall, IInteractiveObject
    {
        public bool isLock;
        protected bool isOpen;

        public bool IsOpen {
            get { return isOpen; }
            set { isOpen = value; }
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

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.PlayMode)
            {
                return;
            }

         //   if (IsCollisionWithPlayer() && IsMouseOverObject())
         //   {
         //       OnMouseLeftButtonClick(TryToOpenDoor);
         //
         //   }
        }

        private bool IsCollisionWithPlayer()
        {
            return colliders["main"].CollidesWith(game.Scene.Player.colliders["main"]);
        }

       
        public void TryToOpenDoor()
        {
            if (isLock) {
                TryToUnlockDoor();
            } else {
                OpenOrCloseDoor();
            }
        }

        public void TryToOpenDoorTeammate()
        {
            if(isLock)
            {
                game.Hud.Message = "Need key to open the door";
            }
            else
            {
                OpenOrCloseDoor();
            }
        }

        public void OpenDoor()
        {
            isOpen = !isOpen;
            String modelDoor = isOpen ? "drzwi_otwarte" : "drzwi";

            model = game.Models[modelDoor];
            modelCut = game.Models[modelDoor + "_przyciete"];
        }

        private void TryToUnlockDoor()
        {
            String message = "Need key to open the door";
            if (game.Player.hasItemInBag("key")) {
                this.OpenOrCloseDoor();
                message = "You unlocked the door";
                isLock = false;
                game.Player.removeItemFromBag("key");
            } else
            if (game.Player.hasItemInBag("key2"))
            {
                this.OpenOrCloseDoor();
                message = "You unlocked the door";
                isLock = false;
                game.Player.removeItemFromBag("key2");
                game.Player.isKey2Droped = true;
            }

            game.Hud.Message = message;
        }

        public void forceUnlcok()
        {
            this.OpenOrCloseDoor();
            game.Hud.Message = "You unlocked the door";
            isLock = false;
        }

        protected virtual void OpenOrCloseDoor()
        {
            isOpen = !isOpen;
            String modelDoor = isOpen ? "drzwi_otwarte" : "drzwi";
            model = game.Models[modelDoor];
            modelCut = game.Models[modelDoor + "_przyciete"];
                                
        }

        #region IInteractiveOptions implementation

        public List<String> GetOptionsToInteract()
        {
            List<String> options = new List<String>();
            options.Add(isOpen ? "Close" : "Open");

            return options;
        }

        public string performAction(string action)
        {
            if(action != null)
            {
                if(action.Equals("Open") || action.Equals("Close"))
                {
                    TryToOpenDoorTeammate();
                }
                if(isLock) return "Locked";
            }
            return null;
        }
        #endregion
    }
}
