using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAI.Models
{
    class DeepThink
    {
        private Random rnd;

        private Network Network = new Network();

        public DeepThink(int Seed, params int[] LayerCounts)
        {
            rnd = new Random(Seed);

            for (int x = 0, xMax = LayerCounts.Count(); x < xMax; x++)
            {
                var newLayer = new Layer();
                for (int y=0, yMax = LayerCounts[x]; y < yMax; y++)
                {
                    newLayer.Neurons.Add(new Neuron());
                }
                Network.Layers.Add(newLayer);
            }
        }

        public void Train(double[] Inputs, double[] ExpectedOutputs)
        {
            var OutputLayer = Network.Layers.Last();

            if (ExpectedOutputs.Count() > OutputLayer.Neurons.Count())
            {
                throw new Exception("Training Outputs exceed number of Neurons in Output Layer.");
            }
            else if (ExpectedOutputs.Count() > OutputLayer.Neurons.Count())
            {
                throw new Exception("Neurons in Output Layer exceed Training Outputs.");
            }

            for (int n = 0, MaxVal = ExpectedOutputs.Count(); n < MaxVal; n++)
            {
                OutputLayer.Neurons[n].ExpectedOut = ExpectedOutputs[n];
            }

            Run(Inputs);
        }

        public double[] Run(double[] Inputs)
        {
            var InputLayer = Network.Layers.First();
            var OutputLayer = Network.Layers.Last();
            if (Inputs.Count() > InputLayer.Neurons.Count())
            {
                throw new Exception("Training Inputs exceed number of Neurons in Input Layer.");
            }
            else if (Inputs.Count() > InputLayer.Neurons.Count())
            {
                throw new Exception("Neurons in Input Layer exceed Training Inputs.");
            }

            for (int n = 0, MaxVal = Inputs.Count(); n < MaxVal; n++)
            {
                InputLayer.Neurons[n].Input = Inputs[n];
            }

            PropogateForward(Network);
            PropogateBackward(Network);
            var outpouts = OutputLayer.Neurons.Select(n => n.Value).ToArray();
            Reset(Network);

            return outpouts;
        }

        public void Bind(Network Network)
        {
            for (int n = 1, Max = Network.Layers.Count; n < Max; n++)
            {
                var ParentLayer = Network.Layers[n - 1];
                var ChildLayer = Network.Layers[n];
                Bind(ParentLayer, ChildLayer);
            }
        }

        private void Bind(ILayer ParentLayer, ILayer ChildLayer)
        {
            foreach(INeuron ParentNeuron in ParentLayer.Neurons)
            {
                foreach (INeuron ChildNeuron in ChildLayer.Neurons)
                {
                    Bind(ParentNeuron, ChildNeuron);
                }
            }
        }

        private void Bind(INeuron Parent, INeuron Child)
        {
            Parent.Children.Add(Child);
            Child.Parents.Add(Parent, rnd.NextDouble());
        }

        //Propogate Layer by Layer
        public void PropogateForward(Network Network)
        {
            foreach (ILayer Layer in Network.Layers)
            {
                PropogateForward(Layer);
            }
        }

        private void PropogateForward(ILayer Layer)
        {
            foreach (INeuron Neuron in Layer.Neurons)
            {
                PropogateForward(Neuron);
            }
        }

        /// <summary>
        /// Sets the Value of a Neuron based on its Inputs
        /// </summary>
        /// <param name="Neuron"></param>
        private void PropogateForward(INeuron Neuron)
        {
            //Input will not be null if this is a root Neuron
            if (Neuron.Input.HasValue)
            {
                //First layer Neurons just have a value of their input
                Neuron.Value = Neuron.Input.Value;
            }
            else
            {
                // This is a Neuron in the second or deeper Layer. 
                // Neuron's value = sum of its parents values multiplied by weight of respective connection
                Neuron.Input = Neuron.Parents.Sum(n => n.Key.Value * n.Value);

                //Sigmoid function to make it a value between 0 and 1
                Neuron.Value = Sigmoid(Neuron.Input ?? 0);
            }
        }

        public void PropogateBackward(Network Network)
        {
            foreach (ILayer Layer in Network.Layers)
            {
                PropogateBackward(Layer);
            }
        }

        private void PropogateBackward(ILayer Layer)
        {
            foreach (INeuron Neuron in Layer.Neurons)
            {
                PropogateBackward(Neuron);
            }
        }

        /// <summary>
        /// Sets the new Weights of a Neuron based on its Error
        /// </summary>
        /// <param name="Neuron"></param>
        private void PropogateBackward(INeuron Neuron)
        {
            if (Neuron.ExpectedOut.HasValue)
            {
                //Delta value of output layer
                Neuron.ExpectedOut -= Neuron.Value;
            }
            else
            {
                //Sum up Children neuron Errors multiplied by respective Weight linking to them
                Neuron.ExpectedOut = Neuron.Children.Sum(n => n.Error * n.Parents[Neuron]);
            }
            //Derivative of Sigmoid Function
            Neuron.Error = Neuron.ExpectedOut.Value * (1 - Neuron.Value) * Neuron.Value;

            var NewParents = new Dictionary<INeuron, double>();
            foreach(KeyValuePair<INeuron, double> Weight in Neuron.Parents)
            {
                var Parent = Weight.Key;
                NewParents[Parent] = Weight.Value + Weight.Value * Parent.Input.Value;
            }
        }

        public void Reset(Network Network)
        {
            foreach (ILayer Layer in Network.Layers)
            {
                Reset(Layer);
            }
        }

        private void Reset(ILayer Layer)
        {
            foreach (INeuron Neuron in Layer.Neurons)
            {
                Reset(Neuron);
            }
        }

        private void Reset(INeuron Neuron)
        {
            Neuron.Input = null;
            Neuron.ExpectedOut = null;
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
