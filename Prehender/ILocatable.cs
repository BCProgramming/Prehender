using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Prehender
{
    interface ILocatable
    {
        Vector3 Location { get; set; }
    }
}
