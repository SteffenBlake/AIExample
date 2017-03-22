using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleAI.Models
{
    class DeepThink
    {
        private readonly Random _rnd;

        private readonly Network _network = new Network();

        private double _lastError;

        /// <summary>
        /// Gets the Last Error the system had during a training session.
        /// </summary>
        public double LastError
        {
            get
            {
                return _lastError;
            }
        }

        /// <summary>
        /// Instantiates a DeepThink system with a starting seed and neurons in layers
        /// </summary>
        /// <param name="seed">The Starting seed for the system for debugging</param>
        /// <param name="layerCounts">The number of neurons in each layer</param>
        public DeepThink(int seed, params int[] layerCounts)
        {
            _rnd = new Random(seed);
            _lastError = 0D;

            for (int x = 0, xMax = layerCounts.Count(); x < xMax; x++)
            {
                var newLayer = new Layer();
                for (int y = 0, yMax = layerCounts[x]; y < yMax; y++)
                {
                    newLayer.Neurons.Add(new Neuron());
                }
                _network.Layers.Add(newLayer);
            }

            Bind(_network);
        }

        /// <summary>
        /// Trains the network to learn patterns
        /// </summary>
        /// <param name="inputs">Inputs to train with.</param>
        /// <param name="expectedOutputs">Expected values to recieve based on input. Inputting none will grant an error of 0%</param>
        public void Train(double[] inputs, double[] expectedOutputs)
        {
            PropogateForward(inputs, false);

            var lastErrors = PropagateBackward(_network, expectedOutputs);
            _lastError = Math.Sqrt(lastErrors.Sum(v => v*v));

            Reset(_network);
        }

        /// <summary>
        /// Utulizes the neural network to perform logic on the inputs.
        /// </summary>
        /// <param name="inputs">Values to pass into the network</param>
        /// <param name="reset">Whether or not to reset the network after use. Best left on True  if not training.</param>
        /// <returns>The logical output based on training</returns>
        public double[] Run(double[] inputs, bool reset = true)
        {
            return PropogateForward(inputs, reset);
        }

        /// <summary>
        /// Punishes the Network, teaching it to bias away from its last result.
        /// </summary>
        public void Punish()
        {
            var length = _network.Layers.Last().Neurons.Count;
            double[] zeros = new double[length];
            for (int n=0; n<length; n++)
            {
                zeros[n] = 0D;
            }
            PropagateBackward(_network, zeros);
            Reset(_network);
        }

        /// <summary>
        /// Rewards the network for its results, further cementing it.
        /// </summary>
        public void Reward()
        {
            PropagateBackward(_network);
            Reset(_network);
        }

        private double[] PropogateForward(double[] inputs, bool reset = true)
        {
            var inputLayer = _network.Layers.First();

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

            foreach (var layer in _network.Layers)
            {
                PropagateForward(layer);
            }

            var output = _network.Layers.Last().Neurons.Select(n => n.Value).ToArray();

            if (reset) Reset(_network);

            return output;
        }

        private void Bind(Network network)
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
            foreach (var parentNeuron in parentLayer.Neurons)
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
                if (neuron.Input.Value == double.NaN)
                {
                    neuron.Input = double.MaxValue;
                }

                //First layer Neurons just have a value of their input
                neuron.Value = neuron.Input.Value;
            }
            else
            {
                // This is a Neuron in the second or deeper Layer. 
                // Neuron's value = sum of its parents values multiplied by weight of respective connection
                neuron.Input = neuron.Parents.Sum(n => n.Key.Value * n.Value);

                //Sigmoid function to make it a value between 0 and 1
                neuron.Value = Sigmoid(neuron.Input.Value);
            }
        }

        public double[] PropagateBackward(Network network, params double[] expectedOutputs)
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

            foreach (var layer in network.Layers.Reverse())
            {
                PropagateBackward(layer);
            }

            return outputLayer.Neurons.Select(n => n.Error).ToArray();
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
            foreach (KeyValuePair<INeuron, double> Weight in neuron.Parents)
            {
                var parent = Weight.Key;
                newParents[parent] = Weight.Value + neuron.Error * parent.Value;
            }
            neuron.Parents = newParents;
        }

        public void Reset()
        {
            Reset(_network);
        }
        private void Reset(Network network)
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
            try
            {
                var divisor = 1 + Math.Exp(-1 * input);
                if (divisor == 0) return 0D;
                var output = 1 / (divisor);
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return 0D;
            }
        }
    }
}
