using App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HESOYAM_Production.App
{
    class Emitter : GameObject
    {
        public TimeSpan lastEmitTime;
        public TimeSpan emitDelay;
        public int amountPerRelase;
        public Particle customParticle;
        public Texture2D customTexture;

        public Emitter(Engine game, Vector3 position) : base(game, "", null)
        {
            this.position = position;
            lastEmitTime = TimeSpan.Zero;
            emitDelay = new TimeSpan(0, 0, 0, 0, 10);
            amountPerRelase = 20;
            game.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(game.PlayMode)
            {
                if(lastEmitTime + emitDelay < gameTime.TotalGameTime)
                {
                    for(int i = 0; i < amountPerRelase; i++)
                    {
                        Particle newParticle;
                        if(customParticle == null)
                        {
                            newParticle = game.Particles.addParticle(position, gameTime.TotalGameTime);
                        }
                        else
                        {
                            newParticle = game.Particles.addParticle(position, customParticle, gameTime.TotalGameTime);
                        }
                        if(customTexture != null)
                            newParticle.customTexture = customTexture;
                    }
                    lastEmitTime = gameTime.TotalGameTime; 
                }
            }
            else
            {
                lastEmitTime += gameTime.ElapsedGameTime;
            }
        }
    }
}
