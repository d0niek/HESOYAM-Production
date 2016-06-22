using HESOYAM_Production;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace App
{

    public class Projectile : GameObject
    {
        public Vector3 direction;
        public float speed;

        public Projectile(
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
            DrawModel(model);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
