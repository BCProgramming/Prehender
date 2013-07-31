using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prehender
{
    public abstract class GameObject : IRenderable,IUpdatable

    {
        public abstract void Render(GameRunningState gameState);
        //return true to remove this element.
        public abstract bool Update(GameRunningState gameState);
    }
}
