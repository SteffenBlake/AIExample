using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITest.Interfaces
{
    public interface INeuralLayer
    {
        IList<INeuron> Neurons { get; set; }

        INeuralLayer Parent { get; set; }

        INeuralLayer Child { get; set; }

        void Pulse();

        void Mutate(decimal variability);
        
    }
}
