namespace DiameterCalc.Test
{
    using System;
    using System.Linq;
    using Xunit;

    using DiameterCalc;

    public class DistanceTest
    {
        [Fact]
        public void TestFullyConnectedEccentricity()
        {
            var graph = GraphInfo.Load("../../../Data/fully_connected.json");
            var (distance, nodes) = Distance.CalcEccentricity(graph.BuildSparseIncedenceMatrix(), 0);
            Assert.Equal(1, distance);
            Assert.Equal(9, nodes.Count);
        }

        [Fact]
        public void TestRingEccentricity()
        {
            var graph = GraphInfo.Load("../../../Data/ring.json");
            var (distance, nodes) = Distance.CalcEccentricity(graph.BuildSparseIncedenceMatrix(), 0);
            Assert.Equal(9, distance);
            Assert.Single(nodes);
        }

        [Fact]
        public void TestFullyConnectedDiameter()
        {
            var graph = GraphInfo.Load("../../../Data/fully_connected.json");
            var (diam, pairs) = Distance.CalcDiameter(graph.BuildSparseIncedenceMatrix());
            Assert.Equal(1, diam);
            Assert.Equal(90, pairs.Count());
        }

        [Fact]
        public void TestRingDiameter()
        {
            var graph = GraphInfo.Load("../../../Data/ring.json");
            var (diam, pairs) = Distance.CalcDiameter(graph.BuildSparseIncedenceMatrix());
            Assert.Equal(9, diam);
            Assert.Equal(10, pairs.Count());
        }
    }
}
