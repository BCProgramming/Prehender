using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prehender
{
    public interface IUpdatable
    {
        /// <summary>
        /// return true to remove this item.
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        bool Update(GameRunningState gameState);
    }
}
