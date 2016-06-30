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
        public TimeSpan creationTime;
        public TimeSpan lifespan;
        public TimeSpan removeTime;
        public Vector3 translation;
        public float maxSpeed;
        public float acceleration;
        public float alpha;
        public float distanceToCamera;
        public float halfSize;
        public bool decay;

        public Particle(Vector3 position, TimeSpan creationTime, Random random)
        {
            this.position = position;
            this.creationTime = creationTime;
            lifespan = new TimeSpan(0, 0, 0, 0, random.Next(100, 500));
            removeTime = creationTime + lifespan;
            isAlive = true;
            maxSpeed = 5.0f;
            halfSize = ((float)random.NextDouble() * 4f) + 3f;
            alpha = 1.0f;
            decay = true;
            acceleration = ((float)random.NextDouble() * 0.1f) + 0.85f;
            translation = new Vector3((float)random.NextDouble() * maxSpeed, (float)random.NextDouble() * maxSpeed, (float)random.NextDouble() * maxSpeed);
        }

        public Particle(Vector3 position, TimeSpan creationTime, Particle definingParticle)
        {
            this.position = position;
            this.creationTime = creationTime;
            lifespan = definingParticle.lifespan;
            removeTime = creationTime + lifespan;
            isAlive = true;
            maxSpeed = definingParticle.maxSpeed;
            halfSize = definingParticle.halfSize;
            alpha = 1.0f;
            decay = definingParticle.decay;
            acceleration = definingParticle.acceleration;
            translation = definingParticle.translation;
        }

        public Particle()
        {
            this.position = Vector3.Zero;
            this.isAlive = true;
            this.creationTime = TimeSpan.Zero;
            this.lifespan = new TimeSpan(0, 0, 1);
            this.removeTime = creationTime + lifespan;
            this.translation = Vector3.Zero;
            this.maxSpeed = 5.0f;
            this.acceleration = 0.9f;
            this.alpha = 1.0f;
            this.decay = true;
            this.halfSize = 10.0f;
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
            if(decay)
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
