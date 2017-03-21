using SimpleAI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI
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

        private double? _Input;

        public double? Input
        {
            get
            {
                return _Input;
            }
            set
            {
                _Input = value;
            }
        }

        private IDictionary<INeuron, double> _Parents;
        public IDictionary<INeuron, double> Parents
        {
            get
            {
                return _Parents;
            }
            set
            {
                _Parents = value;
            }
        }

        private List<INeuron> _Children;
        public List<INeuron> Children
        {
            get
            {
                return _Children;
            }
            set
            {
                _Children = value;
            }
        }

        private double _Value;
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        private double? _ExpectedOut;
        public double? ExpectedOut
        {
            get
            {
                return _ExpectedOut;
            }
            set
            {
                _ExpectedOut = value;
            }
        }

        private double _Error;
        public double Error
        {
            get
            {
                return _Error;
            }
            set
            {
                _Error = value;
            }
        }


        public Neuron()
        {
            _Parents = new Dictionary<INeuron, double>();
            _Children = new List<INeuron>();
            _Value = 0;
            _Input = null;
            _ExpectedOut = null;
            _Error = 0;
        }

    }
}
