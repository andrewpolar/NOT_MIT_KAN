using System;
using System.Collections.Generic;
using System.Text;

namespace NOT_MIT_KAN
{
    class Formula2
    {
        const double pi = 3.141592653589793;
        private Random _rnd = new Random();
        double _xmin = -1.0;
        double _xmax = 1.0;    

        private double Function(double x1, double x2, double x3, double x4)
        {
            //example from https://kindxiaoming.github.io/pykan/Examples/Example_2_deep_formula.html
            //torch.exp((torch.sin(torch.pi*(x[:,[0]]**2+x[:,[1]]**2))+torch.sin(torch.pi*(x[:,[2]]**2+x[:,[3]]**2)))/2)
            return Math.Exp((Math.Sin(pi * (x1 * x1 + x2 * x2)) + Math.Sin(pi * (x3 * x3 + x4 * x4)))/2.0);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        { 
            List<double[]> input = new List<double[]>();
            List<double> target = new List<double>();

            for (int i = 0; i < N; ++i)
            {
                double[] arg = GetInput();
                input.Add(arg);
                target.Add(GetTarget(arg));
            }

            return (input, target);
        }

        public double[] GetInput()
        {
            double arg1 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg2 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg3 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            double arg4 = _rnd.Next(10, 1000) / 1000.0 * (_xmax - _xmin) + _xmin;
            return new double[] { arg1, arg2, arg3, arg4 };
        }

        public double GetTarget(double[] input)
        {
            return Function(input[0], input[1], input[2], input[3]);
        }
    }

    class Formula1
    {
        private Random _rnd = new Random();
        double _min = -1.0;
        double _max = 1.0;
        const double pi = 3.141592653589793;

        private double Function(double x, double y)
        {
            return Math.Exp(Math.Sin(pi * x) + y * y);
        }

        public (List<double[]> input, List<double> target) GenerateData(int N)
        {
            List<double[]> input = new List<double[]>();
            List<double> target = new List<double>();

            for (int i = 0; i < N; ++i)
            {
                double[] arg = GetInput();
                input.Add(arg);
                target.Add(Function(arg[0], arg[1]));
            }

            return (input, target);
        }

        public double[] GetInput()
        {
            double arg1 = _rnd.Next(10, 1000) / 1000.0 * (_max - _min) + _min;
            double arg2 = _rnd.Next(10, 1000) / 1000.0 * (_max - _min) + _min;
            return new double[] { arg1, arg2 };
        }

        public double GetTarget(double[] input)
        {
            return Function(input[0], input[1]);
        }
    }
}

