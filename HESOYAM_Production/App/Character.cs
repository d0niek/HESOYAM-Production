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
        private bool isInteracting;
        private bool isFinishedInteracting;

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

        public bool IsInteracting
        {
            get { return isInteracting; }
            set { isInteracting = value; }
        }

        public bool IsFinishedInteracting
        {
            get { return isFinishedInteracting; }
            set { isFinishedInteracting = value; }
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
            isInteracting = false;
            isFinishedInteracting = false;

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

        protected void OnMove2()
        {
            this.PlayClip("bron_bieg_przod").Looping = true;
        }

        protected void OnIdle()
        {
            this.PlayClip("postawa").Looping = true;
        }

        protected void OnIdle2()
        {
            this.PlayClip("celowanie").Looping = true;
        }

        protected void OnAttack()
        {
            this.PlayClip("cios_piesc").Looping = true;
        }

        protected void OnInteraction()
        {
            if(!IsInteracting)
            { IsInteracting = true; }            
            AnimationPlayer opponent = this.PlayClip("interakcja");
            opponent.Looping = false;
            if (opponent.Position >= (opponent.Duration)/3.0f)
            {
                IsInteracting = false;
                IsFinishedInteracting = true;
            }              

        }

        protected void OnTeammateIdle()
        {
            this.PlayClip("bujanie").Looping = true;
        }

        #endregion
    }
}
