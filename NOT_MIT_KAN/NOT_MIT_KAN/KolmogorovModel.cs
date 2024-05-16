using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN
{
    class KolmogorovModel
    {
        private int _points_in_interior;
        private int _points_in_exterior;
        private double _muRoot;
        private double _muLeaves;
        private int _nLeaves; 
 
        public List<double[]> _inputs = new List<double[]>();
        public List<double> _target = new List<double>();
        private double[] _xmin = null;
        private double[] _xmax = null;
        public double _targetMin;
        public double _targetMax;
        int[] _interior_structure = null;
        int[] _exterior_structure = null;

        private List<Urysohn> _ulist = new List<Urysohn>();
        private Urysohn _bigU = null;
        private Random _rnd = new Random();

        public KolmogorovModel(int points_in_inner, int points_in_outer, int leaves, double mu_inner, double mu_outer)
        {
            _points_in_interior = points_in_inner;
            _points_in_exterior = points_in_outer;
            _nLeaves = leaves;
            _muLeaves = mu_inner;
            _muRoot = mu_outer;
        }

        public void Initialize(List<double[]> inputs, List<double> target)
        {
            _inputs = inputs;
            _target = target;

            if (inputs.Count != target.Count)
            {
                Console.WriteLine("Invalid training data");
                Environment.ExitCode = 0;
            }

            FindMinMax();

            int number_of_inputs = _inputs[0].Length;
            if (_nLeaves < 0)
            {
                _nLeaves = number_of_inputs * 2 + 1;
            }
            _interior_structure = new int[number_of_inputs];
            for (int i = 0; i < number_of_inputs; i++)
            {
                _interior_structure[i] = _points_in_interior;
            }
            _exterior_structure = new int[_nLeaves];
            for (int i = 0; i < _nLeaves; i++)
            {
                _exterior_structure[i] = _points_in_exterior;
            }

            GenerateInitialOperators();
        }

        private void FindMinMax()
        {
            int size = _inputs[0].Length;
            _xmin = new double[size];
            _xmax = new double[size];

            for (int i = 0; i < size; ++i)
            {
                _xmin[i] = double.MaxValue;
                _xmax[i] = double.MinValue;
            }

            for (int i = 0; i < _inputs.Count; ++i)
            {
                for (int j = 0; j < _inputs[i].Length; ++j)
                {
                    if (_inputs[i][j] < _xmin[j]) _xmin[j] = _inputs[i][j];
                    if (_inputs[i][j] > _xmax[j]) _xmax[j] = _inputs[i][j];
                }

            }

            _targetMin = double.MaxValue;
            _targetMax = double.MinValue;
            for (int j = 0; j < _target.Count; ++j)
            {
                if (_target[j] < _targetMin) _targetMin = _target[j];
                if (_target[j] > _targetMax) _targetMax = _target[j];
            }
        }

        public void GenerateInitialOperators()
        {
            _ulist.Clear();
            int points = _inputs[0].Length;
            for (int counter = 0; counter < _nLeaves; ++counter)
            {
                Urysohn uc = new Urysohn(_xmin, _xmax, _targetMin, _targetMax, _interior_structure);
                _ulist.Add(uc);
            }

            if (null != _bigU)
            {
                _bigU.Clear();
                _bigU = null;
            }

            double[] min = new double[_nLeaves];
            double[] max = new double[_nLeaves];
            for (int i = 0; i < _nLeaves; ++i)
            {
                min[i] = _targetMin;
                max[i] = _targetMax;
            }

            _bigU = new Urysohn(min, max, _targetMin, _targetMax, _exterior_structure);
        }

        private double[] GetVector(double[] data)
        {
            int size = _ulist.Count;
            double[] vector = new double[size];
            for (int i = 0; i < size; ++i)
            {
                vector[i] = _ulist[i].GetU(data);
            }
            return vector;
        }

        public void BuildRepresentation(int nEpochs)
        {
            for (int step = 0; step < nEpochs; ++step)
            {
                double error = 0.0;
                for (int i = 0; i < _inputs.Count; ++i)
                {
                    double[] v = GetVector(_inputs[i]);
                    double model = _bigU.GetU(v);
                    double diff = _target[i] - model;
                    error += diff * diff;

                    for (int k = 0; k < _ulist.Count; ++k)
                    {
                        if (v[k] > _targetMin && v[k] < _targetMax)
                        {
                            double derrivative = _bigU.GetDerrivative(k, v[k]);
                            _ulist[k].Update(diff * derrivative / v.Length, _inputs[i], _muLeaves);
                        }
                    }
                    _bigU.Update(diff, v, _muRoot);
                }
                error /= _inputs.Count;
                error = Math.Sqrt(error);
                //error /= (_targetMax - _targetMin);
                if (0 == step % 25)
                {
                    Console.WriteLine("Training step {0}, RMSE {1:0.0000}", step, error);
                }
            }
        }

        public double ComputeOutput(double[] inputs)
        {
            double[] v = GetVector(inputs);
            double output = _bigU.GetU(v);
            return output;
        }
    }
}