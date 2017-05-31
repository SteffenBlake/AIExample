using System.Collections.Generic;

namespace SimpleAI.Network_Models
{
    public interface ILayer
    {
        IList<INeuron> Neurons { get; set; }
    }

    public class Layer : ILayer
    {
        public IList<INeuron> Neurons { get; set; }

        public Layer()
        {
            Neurons = new List<INeuron>();
        }
    }
}
