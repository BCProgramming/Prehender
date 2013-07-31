using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Prehender
{
    public class ParticleBatch : GameObject 
    {
        private List<Particle> _Particles = new List<Particle>();
        public List<Particle> Particles { get { return _Particles; } set { _Particles = value; } } 
        public override void Render(GameRunningState gameState)
        {
            Particle.RenderPoints(_Particles);
        }
        public Particle AddParticle(Vector3 Location, float Speed)
        {
            //create a random direction first.
            Vector3 RandomDirection = new Vector3((float) (PrehenderGame.rgen.NextDouble() - 0.5),
                (float) (PrehenderGame.rgen.NextDouble() - 0.5),
                (float) (PrehenderGame.rgen.NextDouble() - 0.5));

            RandomDirection.Normalize();
            return AddParticle(Location, RandomDirection * Speed);


        }
        public Particle AddParticle(Vector3 Location, Vector3 Velocity)
        {
            var part = new Particle(Location, Velocity);
            part.TextureKey = "squarepart";
            _Particles.Add(part);
            return part;
        }
        public override bool Update(GameRunningState gameState)
        {
            List<Particle> removeelements = new List<Particle>();
            Particle.UpdatePoints(_Particles, gameState, removeelements.Add);

            foreach (var iterate in removeelements)
            {
                _Particles.Remove(iterate);
            }


            

            return false;
        }
    }
}
