using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITest.Interfaces
{
    public interface INeuralNet
    {
        IList<INeuralLayer> Net { get; set; }

        void Mutate(decimal variability);

    }
}
