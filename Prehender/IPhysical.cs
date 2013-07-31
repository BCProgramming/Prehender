using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prehender
{
    public interface IPhysical
    {
        void AddToWorld(Jitter.World Target);
        void RemoveFromWorld(Jitter.World Target);
    }
}
