namespace DiameterCalc
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;


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

                var (diameter, pairs) = Distance.CalcDiameter(graph.BuildSparseIncedenceMatrix());

                var nodeToNameDict = graph.Nodes.ToDictionary(p => p.Value, p => p.Key);

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
