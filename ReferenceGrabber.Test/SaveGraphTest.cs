namespace ReferenceGrabber.Test
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    using ReferenceGraphBuilder;

    public class SaveGraphTest
    {
        [Fact]
        public void TestSave()
        {
            var refs = new[] {"b", "c"};
            var pages = new[] { ("a", (IEnumerable<string>)refs)};

            Program.SaveGraph(pages, "simple.json");
        }
    }
}
