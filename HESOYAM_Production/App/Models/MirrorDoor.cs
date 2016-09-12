using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HESOYAM_Production;
using System;
using System.Collections.Generic;

namespace App.Models
{

    class MirrorDoor : Door
    {
        public MirrorDoor(
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
            this.isLock = isLock;
            isOpen = false;
        }

		public override void Draw(GameTime gameTime)
        {
            if (isSegmentCoversCameraLookAt()) {
                setTexture(TextureCut);
                DrawModelWithEffect(modelCut);
            } else {
                setTexture(TextureNormal);
                DrawModelWithEffect(model);
            }
        }

		public override void OpenDoor()
        {
            isOpen = !isOpen;
            String modelDoor = isOpen ? "lustrzane_drzwi_otwarte" : "lustrzane_drzwi";

            model = game.Models[modelDoor];
            modelCut = game.Models[modelDoor + "_przyciete"];
        }

		protected override void OpenOrCloseDoor()
        {
            isOpen = !isOpen;
            String modelDoor = isOpen ? "lustrzane_drzwi_otwarte" : "lustrzane_drzwi";
            model = game.Models[modelDoor];
            modelCut = game.Models[modelDoor + "_przyciete"];
        }
	}
}
