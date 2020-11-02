using System;
using System.Collections.Generic;
using System.Text;

namespace GraphManager.Interfaces
{
    public interface IGraph : ICloneable
    {
        string Name { get; }
        string Type { get; }
        string[] Vertices { get; }
        string[][] Edges { get; }

        string ToString();

        public void ReplaceVertices(string[] newVertices);
        public void ReplaceEdges(string[][] edges);

    }
}
