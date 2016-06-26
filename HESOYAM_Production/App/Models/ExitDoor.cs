using System;
using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App.Models
{

    class ExitDoor : Door
    {
        public ExitDoor(
            Engine game,
            string name,
            Model model,
            Model transparentModel,
            bool isLock,
            Vector3 position = default(Vector3),
            Vector3 rotation = default(Vector3),
            Vector3? scale = null
        ) : base(game, name, model, transparentModel, isLock, position, rotation, scale)
        {
        }

        protected override void OpenOrCloseDoor()
        {
            isOpen = !isOpen;
            String modelDoor = isOpen ? "drzwi_duze_otwarte" : "drzwi_duze";

            model = game.Models[modelDoor];
            modelCut = game.Models[modelDoor + "_przyciete"];
        }
    }
}
