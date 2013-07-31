using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prehender
{
    public class NextUpdateCall
    {
        public delegate bool DeferredRoutine(PrehenderGame gameObject);
        private DeferredRoutine _Routine = null;
        public NextUpdateCall(DeferredRoutine callMethod)
        {
            _Routine = callMethod;
        }
        public NextUpdateCall(Action a)
        {
            _Routine = (b) => { a(); return true; };
        }
        public bool Invoke(PrehenderGame mGame)
        {
            return _Routine(mGame);
        }

    }
}
