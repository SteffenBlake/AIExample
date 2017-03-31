using SimpleAI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI.Models
{
    public interface ILayer
    {
        IList<INeuron> Neurons { get; set; }
    }

    public class Layer : ILayer
    {
        private IList<INeuron> _Neurons;
        public IList<INeuron> Neurons {
            get
            {
                return _Neurons;
            }
            set
            {
                _Neurons = value;
            }
        }

        public Layer()
        {
            _Neurons = new List<INeuron>();
        }
    }
}
