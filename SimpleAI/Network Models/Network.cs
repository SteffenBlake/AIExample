using System.Collections.Generic;

namespace SimpleAI.Network_Models
{
    public interface INetwork
    {
        IList<ILayer> Layers { get; set; }
    }

    public class Network
    {
        public IList<ILayer> Layers { get; set; }

        public Network()
        {
            Layers = new List<ILayer>();
        }
    }
}
