using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITest.Interfaces
{
    public class Neuron : INeuron
    {
        public IList<INeuron> Parents { get; set; }

        public IList<INeuron> Children { get; set; }

        public decimal Signal { get; set; }

        public decimal Weight { get; set; }

        public decimal Value { get; set; }

        public void Pulse()
        {

        }
            

        public void Mutate(decimal variability)
        {

        }
    }
}
