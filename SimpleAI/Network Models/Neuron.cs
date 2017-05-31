using System.Collections.Generic;

namespace SimpleAI.Network_Models
{
    public interface INeuron
    {
        IDictionary<INeuron, double> Parents { get; set; }

        List<INeuron> Children { get; set; }

        double? Input { get; set; }

        double Value { get; set; }

        double? ExpectedOut { get; set; }

        double Error { get; set; }
    }

    public class Neuron : INeuron
    {
        public double? Input { get; set; }

        public IDictionary<INeuron, double> Parents { get; set; }

        public List<INeuron> Children { get; set; }

        public double Value { get; set; }

        public double? ExpectedOut { get; set; }

        public double Error { get; set; }


        public Neuron()
        {
            Parents = new Dictionary<INeuron, double>();
            Children = new List<INeuron>();
            Value = 0;
            Input = null;
            ExpectedOut = null;
            Error = 0;
        }

    }
}
