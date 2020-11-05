using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphManager.Extensions;
using GraphManager.Implementations;
using GraphManager.Interfaces;
using Serilog;

namespace Program
{
    public class GraphSolver
    {
        public IEnumerable<IGraph> FindAllSolutions(IGraph graph, ILogger logger)
        {
            logger.Debug("Starting to find all solutions.");
            //Odnajdywanie wierzchołków które nie mają żadnych krawędzi
            var solutions = new List<IGraph>();
            var verticesWithoutEdges = graph.Vertices.Where(v => !graph.Edges.Any(edge => edge.Contains(v)));
            var workingVertices = graph.Vertices.RemoveValues(verticesWithoutEdges);
            foreach (var vertex in workingVertices)
            {
                var solution = GetSolution(vertex, graph.Edges);
            }
            return new List<Graph>();
        }

        private List<string> GetSolution(string vertex, IEnumerable<string[]> graphEdges)
        {
            var verticesSolution = new List<string>();

            var excludedVertices = new HashSet<string>(new []{vertex});
            var excludedEdges = new HashSet<string[]>();
            var currentEdges = graphEdges.Where(e => e.Any(edge => edge.Contains(vertex)));
            var currentVertices = Enumerable.Empty<string>().Add(new[] {vertex});
            while (true)
            {
                excludedEdges.UnionWith(GetEdgesFromVertices(currentVertices, ref currentEdges));
                currentVertices = currentEdges.SelectMany(edge => edge).Distinct().RemoveValues(currentVertices);
                excludedVertices.UnionWith(currentVertices);
                excludedEdges.UnionWith(GetEdgesFromVertices(currentVertices, ref currentEdges));
            }

        }

        private IEnumerable<string> GetVerticesFromEdges(ref IEnumerable<string[]> edges, ref IEnumerable<string> withoutVertices)
        {
            return edges.SelectMany(edge => edge).RemoveValues(withoutVertices);
        }

        private IEnumerable<string[]> GetEdgesFromVertices(IEnumerable<string> vertices, ref IEnumerable<string[]> edges)
        {
            var output = new HashSet<string[]>();
            foreach (var vertex in vertices)
            {
                output.UnionWith(GetEdgesFromVertex(vertex, edges));
            }

            edges = output;
            return output;
        }

        private IEnumerable<string[]> GetEdgesFromVertex(string vertex, IEnumerable<string[]> edges)
        {
            return edges.Where(e => e.Contains(vertex));
        }

        public IGraph GetTheBestSolution(IEnumerable<IGraph> solutions, ILogger logger)
        {
            throw new NotImplementedException();
        }

    }
}
