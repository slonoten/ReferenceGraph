namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;
    using System.Diagnostics;

    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                if (args.Length != 1 || !File.Exists(args[0]))
                {
                    throw new ApplicationException("Using: DiameterCalc.dll <graph_file_name>");
                }     
                
                var graph = GraphInfo.Load(args[0]);

                var timer = Stopwatch.StartNew();

                Console.WriteLine("Calculating graph diameter. Nodes count {0}...", graph.Nodes.Count);

                var (diameter, pairs) = Distance.CalcDiameterParallel(graph.BuildSparseIncedenceMatrix());

                var nodeToNameDict = graph.Nodes.ToDictionary(p => p.Value, p => p.Key);

                Console.WriteLine("Complete in {0}", timer.Elapsed);
                Console.WriteLine();

                Console.WriteLine("Diameter is {0}", diameter);

                foreach(var pair in pairs)
                {
                    Console.WriteLine("{0} - {1}", nodeToNameDict[pair.Item1], nodeToNameDict[pair.Item2]);
                }

                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
