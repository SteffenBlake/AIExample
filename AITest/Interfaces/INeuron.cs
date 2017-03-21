using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITest.Interfaces
{
    public interface INeuron
    {
        IList<INeuron> Parents { get; set; }

        IList<INeuron> Children { get; set; }

        decimal Signal { get; set; }

        decimal Weight { get; set; }

        decimal Value { get; set; }

        void Pulse();

        void Mutate(decimal variability);
    }
}
