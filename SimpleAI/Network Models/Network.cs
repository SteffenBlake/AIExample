using SimpleAI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI.Models
{
    public interface INetwork
    {
        IList<ILayer> Layers { get; set; }
    }

    public class Network
    {
        private IList<ILayer> _Layers;
        public IList<ILayer> Layers {
            get
            {
                return _Layers;
            }
            set
            {
                _Layers = value;
            }
        }

        public Network()
        {
            _Layers = new List<ILayer>();
        }
    }
}
