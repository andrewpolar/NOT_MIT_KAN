//This is identification of Kolmogorov-Arnold representation.
//Developed by Andrew Polar and Mike Poluektov.
//Published:
//https://www.sciencedirect.com/science/article/abs/pii/S0016003220301149
//https://www.sciencedirect.com/science/article/abs/pii/S0952197620303742
//https://arxiv.org/abs/2305.08194

//Two toy datasets are taken from MIT benchmark:
//https://kindxiaoming.github.io/pykan/
//Accuracy and perfomance for them are about the same as for pykan.

using System;
using System.Collections.Generic;

namespace NOT_MIT_KAN
{
    class Program
    {
        static void Main(string[] args)
        {
            Formula1 f1 = new Formula1();
            (List<double[]> input, List<double> target) = f1.GenerateData(1000);

            DateTime start = DateTime.Now;
            KolmogorovModel km = new KolmogorovModel(6, 8, 5, 0.05, 0.05);
            km.Initialize(input, target);
            km.BuildRepresentation(600);
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            double time = duration.Minutes * 60.0 + duration.Seconds + duration.Milliseconds / 1000.0;
            Console.WriteLine("Time for building representation {0:####.00} seconds", time);

            double error = 0.0;
            int NTests = 100;
            for (int i = 0; i < NTests; ++i)
            {
                double[] test_input = f1.GetInput();
                double test_target = f1.GetTarget(test_input);
                double model = km.ComputeOutput(test_input);
                error += (test_target - model) * (test_target - model);
            }
            error /= NTests;
            error = Math.Sqrt(error);
            Console.WriteLine("\nRMSE for unseen data {0:0.0000}", error);

            //Formula2 f2 = new Formula2();
            //(List<double[]> input, List<double> target) = f2.GenerateData(3000);

            //DateTime start = DateTime.Now;
            //KolmogorovModel km = new KolmogorovModel(6, 8, 9, 0.05, 0.05);
            //km.Initialize(input, target);
            //km.BuildRepresentation(300);
            //DateTime end = DateTime.Now;
            //TimeSpan duration = end - start;
            //double time = duration.Minutes * 60.0 + duration.Seconds + duration.Milliseconds / 1000.0;
            //Console.WriteLine("Time for building representation {0:####.00} seconds", time);

            //double error = 0.0;
            //int NTests = 100;
            //for (int i = 0; i < NTests; ++i)
            //{
            //    double[] test_input = f2.GetInput();
            //    double test_target = f2.GetTarget(test_input);
            //    double model = km.ComputeOutput(test_input);
            //    error += (test_target - model) * (test_target - model);
            //}
            //error /= NTests;
            //error = Math.Sqrt(error);
            //Console.WriteLine("\nRMSE for unseen data {0:0.0000}", error);
        }
    }
}
