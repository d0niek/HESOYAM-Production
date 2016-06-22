using HESOYAM_Production;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using App.Animation;

namespace App
{

    public class Character : AnimatedObject, ICharacter
    {
        float life;
        float maxLife;
        bool isAttacking;

        public float Life {
            get { return life; }
            private set { }
        }

        public float MaxLife {
            get { return maxLife; }
            private set { }
        }

        public bool IsAttacking {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

        public Character(
            Engine game,
            String name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            maxLife = 100f;
            life = maxLife;
            isAttacking = false;
        }

        public Character(
            Engine game,
            String name,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, position, rotation, scale)
        {
            maxLife = 100f;
            life = maxLife;
        }

        #region ICharacter implementation

        public void ReduceLife(float reduceBy)
        {
            life -= reduceBy;
            if (life < 0) {
                life = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void IncreaseLife(float increaseBy)
        {
            life += increaseBy;
            if (life > maxLife) {
                life = maxLife;
            }
        }

        public bool IsDead()
        {
            return life <= 0;
        }

        protected void OnDead()
        {
            this.PlayClip("smierc").Looping = false;
            this.colliders.Clear();
        }

        protected void OnMove()
        {
            this.PlayClip("bieg_przod").Looping = true;
        }

        protected void OnIdle()
        {
            this.PlayClip("postawa").Looping = true;
        }

        protected void OnAttack()
        {

            this.PlayClip("cios_piesc").Looping = false;
        }

        #endregion
    }
}
