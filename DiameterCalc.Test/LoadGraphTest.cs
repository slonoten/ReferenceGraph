namespace DiameterCalc.Test
{
    using System;
    using Xunit;

    using DiameterCalc;
    
    public class LoadGraphTest
    {
        [Fact]
        public void TestLoadSimple()
        {
            var graph = GraphInfo.Load("../../../Data/simple.json");
            Assert.Equal(3, graph.Nodes.Count);
            Assert.Single(graph.Edges);
        }

        [Fact]
        public void TestLoadFullyConnected()
        {
            var graph = GraphInfo.Load("../../../Data/fully_connected.json");
            Assert.Equal(10, graph.Nodes.Count);
            Assert.Equal(10, graph.Edges.Count);
        }

        [Fact]
        public void TestLoadRing()
        {
            var graph = GraphInfo.Load("../../../Data/ring.json");
            Assert.Equal(10, graph.Nodes.Count);
            Assert.Equal(10, graph.Edges.Count);
        }
    }
}
