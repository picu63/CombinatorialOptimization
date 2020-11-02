using GraphManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphManager.Implementations
{
    public class Graph : IGraph
    {
        private string[][] _edges;
        private string[] _vertices;

        #region properties

        public string Name { get; }
        public string Type { get; }
        public string[] Vertices => _vertices;
        public string[][] Edges => _edges;

        #endregion

        #region constructor

        public Graph(string name, string type, string[] vertices, string[][] edges)
        {
            Name = name;
            Type = type;
            _vertices = vertices;
            _edges = edges;
        }

        #endregion

        #region public methods

        public override string ToString()
        {
            var result = new StringBuilder();
            
            result.AppendLine($"Graph name: {Name}");
            result.AppendLine($"Graph type: {Type}");
            result.AppendLine();
            result.AppendLine($"Vertices:\n{{ {string.Join(", ", Vertices)} }}");
            result.AppendLine();
            result.AppendLine($"{Vertices.Length} in total.");
            result.AppendLine();
            result.AppendLine($"Edges:\n{{ {string.Join(",  ", EdgesToStringArray(Edges) )} }}");
            result.AppendLine();
            result.AppendLine($"{Edges.Length} in total.");
            result.AppendLine();

            return result.ToString();
        }

        public object Clone()
        {
            return new Graph(this.Name, this.Type, this.Vertices, this.Edges);
        }

        public void ReplaceVertices(string[] newVertices)
        {
            _vertices = newVertices;
        }

        public void ReplaceEdges(string[][] newEdges)
        {
            _edges = newEdges;
        }

        #endregion

        #region private auxiliary methods

        private IEnumerable<string> EdgesToStringArray(string[][] edges)
        {
            for (int i = 0; i < edges.GetLength(0); i++)
            {
                yield return EdgeToString(edges[i][0], edges[i][1]);
            }
        }

        private string EdgeToString(string[] edge)
        {
            return $"{{{edge[0]} -> {edge[1]}}}";
        }

        private string EdgeToString(string v1, string v2)
        {
            return EdgeToString(new[] { v1, v2 });
        }

        #endregion

    }
}
