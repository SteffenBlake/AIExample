using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI.Models
{
    class DeepThink
    {
        private readonly Random _rnd;

        private readonly Network _network = new Network();

        public DeepThink(int seed, params int[] layerCounts)
        {
            _rnd = new Random(seed);

            for (int x = 0, xMax = layerCounts.Count(); x < xMax; x++)
            {
                var newLayer = new Layer();
                for (int y=0, yMax = layerCounts[x]; y < yMax; y++)
                {
                    newLayer.Neurons.Add(new Neuron());
                }
                _network.Layers.Add(newLayer);
            }

            Bind(_network);
        }

        public void Train(double[] inputs, double[] expectedOutputs)
        {
            var outputLayer = _network.Layers.Last();

            if (expectedOutputs.Count() > outputLayer.Neurons.Count())
            {
                throw new Exception("Training Outputs exceed number of Neurons in Output Layer.");
            }

            if (expectedOutputs.Count() > outputLayer.Neurons.Count())
            {
                throw new Exception("Neurons in Output Layer exceed Training Outputs.");
            }

            for (int n = 0, maxVal = expectedOutputs.Count(); n < maxVal; n++)
            {
                outputLayer.Neurons[n].ExpectedOut = expectedOutputs[n];
            }

            Run(inputs);
        }

        public double[] Run(double[] inputs)
        {
            var inputLayer = _network.Layers.First();
            var outputLayer = _network.Layers.Last();
            if (inputs.Count() > inputLayer.Neurons.Count())
            {
                throw new Exception("Training Inputs exceed number of Neurons in Input Layer.");
            }

            if (inputs.Count() > inputLayer.Neurons.Count())
            {
                throw new Exception("Neurons in Input Layer exceed Training Inputs.");
            }

            for (int n = 0, maxVal = inputs.Count(); n < maxVal; n++)
            {
                inputLayer.Neurons[n].Input = inputs[n];
            }

            PropagateForward(_network);
            var outpouts = outputLayer.Neurons.Select(n => n.Input ?? 0D).ToArray();
            PropagateBackward(_network);
            Reset(_network);

            return outpouts;
        }

        public void Bind(Network network)
        {
            for (int n = 1, max = network.Layers.Count; n < max; n++)
            {
                var parentLayer = network.Layers[n - 1];
                var childLayer = network.Layers[n];
                Bind(parentLayer, childLayer);
            }
        }

        private void Bind(ILayer parentLayer, ILayer childLayer)
        {
            foreach(var parentNeuron in parentLayer.Neurons)
            {
                foreach (var childNeuron in childLayer.Neurons)
                {
                    Bind(parentNeuron, childNeuron);
                }
            }
        }

        private void Bind(INeuron parent, INeuron child)
        {
            parent.Children.Add(child);
            child.Parents.Add(parent, _rnd.NextDouble());
        }

        //Propagate Layer by Layer
        public void PropagateForward(Network network)
        {
            foreach (var layer in network.Layers)
            {
                PropagateForward(layer);
            }
        }

        private void PropagateForward(ILayer layer)
        {
            foreach (var neuron in layer.Neurons)
            {
                PropagateForward(neuron);
            }
        }

        /// <summary>
        /// Sets the Value of a Neuron based on its Inputs
        /// </summary>
        /// <param name="neuron"></param>
        private void PropagateForward(INeuron neuron)
        {
            //Input will not be null if this is a root Neuron
            if (neuron.Input.HasValue)
            {
                //First layer Neurons just have a value of their input
                neuron.Value = neuron.Input.Value;
            }
            else
            {
                // This is a Neuron in the second or deeper Layer. 
                // Neuron's value = sum of its parents values multiplied by weight of respective connection
                neuron.Input = neuron.Parents.Sum(n => n.Key.Value * n.Value);

                //Sigmoid function to make it a value between 0 and 1
                neuron.Value = Sigmoid(neuron.Input ?? 0);
            }
        }

        public void PropagateBackward(Network network)
        {
            foreach (var layer in network.Layers.Reverse())
            {
                PropagateBackward(layer);
            }
        }

        private void PropagateBackward(ILayer layer)
        {
            foreach (var neuron in layer.Neurons)
            {
                PropagateBackward(neuron);
            }
        }

        /// <summary>
        /// Sets the new Weights of a Neuron based on its Error
        /// </summary>
        /// <param name="neuron"></param>
        private void PropagateBackward(INeuron neuron)
        {
            if (neuron.ExpectedOut.HasValue)
            {
                //Delta value of output layer
                neuron.ExpectedOut -= neuron.Value;
            }
            else
            {
                //Sum up Children neuron Errors multiplied by respective Weight linking to them
                neuron.ExpectedOut = neuron.Children.Sum(n => n.Error * n.Parents[neuron]);
            }
            //Derivative of Sigmoid Function
            neuron.Error = neuron.ExpectedOut.Value * (1 - neuron.Value) * neuron.Value;

            var newParents = new Dictionary<INeuron, double>();
            foreach(KeyValuePair<INeuron, double> Weight in neuron.Parents)
            {
                var parent = Weight.Key;
                newParents[parent] = Weight.Value + Weight.Value * parent.Input.Value;
            }
            neuron.Parents = newParents;
        }

        public void Reset(Network network)
        {
            foreach (ILayer layer in network.Layers)
            {
                Reset(layer);
            }
        }

        private void Reset(ILayer layer)
        {
            foreach (var neuron in layer.Neurons)
            {
                Reset(neuron);
            }
        }

        private void Reset(INeuron neuron)
        {
            neuron.Input = null;
            neuron.ExpectedOut = null;
        }

        /// <summary>
        /// Converts a value (gotten from summing a list of values 0-1) into a value 0-1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double Sigmoid(double input)
        {
            return 1 / (1 + Math.Exp(-input));
        }
    }
}
