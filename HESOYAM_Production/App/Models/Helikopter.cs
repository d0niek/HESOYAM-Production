using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models
{
	class Helikopter : GameObject
	{
		public Helikopter(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
        }

		public override void Draw(GameTime gameTime)
        {
			try {
				DrawModelWithEffect(model);
				//DrawModel(model);
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}
}
