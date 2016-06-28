using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HESOYAM_Production.App
{
    public class Particle : IComparable
    {
        public Vector3 position;
        public bool isAlive;
        protected TimeSpan creationTime;
        protected TimeSpan lifespan;
        public TimeSpan removeTime;
        protected Vector3 translation;
        protected float maxSpeed;
        protected float acceleration;
        public float alpha;
        public float distanceToCamera;

        public Particle(Vector3 position, TimeSpan creationTime, Random random)
        {
            this.position = position;
            this.creationTime = creationTime;
            lifespan = new TimeSpan(0, 0, 0, 0, random.Next(100, 500));
            removeTime = creationTime + lifespan;
            isAlive = true;
            maxSpeed = 5.0f;
            alpha = 1.0f;
            acceleration = ((float)random.NextDouble() * 0.1f) + 0.85f;
            translation = new Vector3((float)random.NextDouble() * maxSpeed, (float)random.NextDouble() * maxSpeed, (float)random.NextDouble() * maxSpeed);
        }

        public void update(TimeSpan totalGameTime)
        {
            if(removeTime < totalGameTime)
            {
                isAlive = false;
                return;
            }
            position += translation;
            translation *= acceleration;
            alpha = (float) ((removeTime - totalGameTime).TotalMilliseconds / lifespan.TotalMilliseconds);
        }

        public int CompareTo(object obj)
        {
            if(obj == null) return 1;
            Particle otherParticle = obj as Particle;
            return otherParticle.distanceToCamera.CompareTo(this.distanceToCamera);
        }
    }
}
