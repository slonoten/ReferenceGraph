namespace ReferenceGrabber.Test
{
    using System;
    using System.Linq;
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

        [Fact]
        public void TestSaveFullyConnected()
        {
            var refs = (IEnumerable<string>)Enumerable.Range('a', 'k'- 'a').Select(c => new string((char)c, 1)).ToArray();
            var pages = refs.Select(c => (c, refs));

            Program.SaveGraph(pages, "fully_connected.json");
        }

        [Fact]
        public void TestSaveRing()
        {
            var pages = Enumerable.Range('a', 'k' - 'a')
                .Select(c => (new string((char)c, 1), (IEnumerable<string>)new[] { new string((char)(c + 1), 1) } ))
                .ToList();
            pages.Add(("k", (IEnumerable<string>)new[] { "a" }));

            Program.SaveGraph(pages, "ring.json");
        }
    }
}
