using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
            logger.Debug("Start odnajdywania wszystkich rozwiązań.");
            //Odnajdywanie wierzchołków które nie mają żadnych krawędzi
            var solutions = new List<IGraph>();
            var verticesWithoutEdges = graph.Vertices.Where(v => !graph.Edges.Any(edge => edge.Contains(v))).ToList();
            var workingVertices = graph.Vertices.RemoveValues(verticesWithoutEdges);
            var minSolution = int.MaxValue;
            foreach (var vertex in workingVertices)
            {
                var solution = GetSolution(vertex, graph.Edges).ToList();

                    solution.Add(verticesWithoutEdges);

                solution.Sort();
                if (CheckIfSolutionAlreadyExist(solution, solutions).Yes())
                    continue;
                solutions.Add(new Graph("", "", solution.ToArray(), Array.Empty<string[]>()));
            }

            return solutions;
        }

        public async Task<IEnumerable<IGraph>> FindAllSolutionsAsync(IGraph graph, ILogger logger)
        {
            logger.Debug("Starting to find all solutions.");
            //Odnajdywanie wierzchołków które nie mają żadnych krawędzi
            var solutions = new ConcurrentBag<IGraph>();
            try
            {
                var verticesWithoutEdges = graph.Vertices.Where(v => !graph.Edges.Any(edge => edge.Contains(v))).ToList();
                var workingVertices = graph.Vertices.RemoveValues(verticesWithoutEdges);
                var tasks = new List<Task>();
                var minSolution = int.MaxValue;
                foreach (var vertex in workingVertices)
                {
                    var task = Task.Run(() =>
                    {
                        var solution = GetSolution(vertex, graph.Edges);

                        if (verticesWithoutEdges.Any())
                        {
                            solution = solution.Add(verticesWithoutEdges);
                        }

                        var sortedSolution = solution.OrderBy(x => x.Length).ThenBy(x => x).ToArray();
                        if (!CheckIfSolutionAlreadyExist(solution, solutions))
                           solutions.Add(new Graph("", "", sortedSolution, Array.Empty<string[]>()));
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);


                //Parallel.ForEach(workingVertices, vertex =>
                //{
                //    var solution = GetSolution(vertex, graph.Edges);

                //    if (verticesWithoutEdges.Any())
                //    {
                //        solution.Add(verticesWithoutEdges);
                //    }

                //    var sortedSolution = solution.OrderBy(x => x.Length).ThenBy(x => x);
                //    if (!CheckIfSolutionAlreadyExist(solution, solutions))
                //        solutions.Add(new Graph("", "", sortedSolution.ToArray(), Array.Empty<string[]>()));
                //});
            }
            catch (AggregateException aggregateException)
            {
                logger.Error("Wystąpił jeden lub więcej błędów.");
                throw;
            }

            logger?.Information($"Liczba znalezionych rozwiązań: {solutions?.Count()}");

            return solutions;
        }

        private bool CheckIfSolutionAlreadyExist(IEnumerable<string> vertices, IEnumerable<IGraph> solutions)
        {
            foreach (var graph in solutions)
            {
                var foundVertices = graph.Vertices;

                if (foundVertices.Length != vertices.Count())
                    continue;

                if (foundVertices.SequenceEqual(vertices))
                    return true;
            }

            return false;
        }

        private IEnumerable<string> GetSolution(string vertex, IEnumerable<string[]> graphEdges)
        {
            var verticesSolution = new HashSet<string>();
            verticesSolution.Add(vertex);

            var excludedVertices = new HashSet<string>(new[] { vertex });
            var excludedEdges = new HashSet<string[]>();
            
            var currentEdges = graphEdges.Where(e => e.Any(edge => edge.Contains(vertex)));
            var edgeBuffer = graphEdges;
            var currentVertices = Enumerable.Empty<string>().Add(new[] { vertex });
            while (edgeBuffer.Any())
            {
                excludedEdges.UnionWith(GetEdgesFromVertices(currentVertices, ref edgeBuffer, ref currentEdges));
                if (!edgeBuffer.Any())
                {
                    break;
                }
                currentVertices = currentEdges.SelectMany(edge => edge).Distinct().RemoveValues(currentVertices);
                excludedVertices.UnionWith(currentVertices);
                excludedEdges.UnionWith(GetEdgesFromVertices(currentVertices, ref edgeBuffer, ref currentEdges));
                currentVertices = currentEdges.SelectMany(edge => edge).Distinct().RemoveValues(currentVertices);
                verticesSolution.UnionWith(currentVertices);
                //verticesSolution.AddRange(currentVertices);
            }

            return verticesSolution.Distinct();
        }

        private IEnumerable<string> GetVerticesFromEdges(ref IEnumerable<string[]> edges, ref IEnumerable<string> withoutVertices)
        {
            return edges.SelectMany(edge => edge).RemoveValues(withoutVertices);
        }

        private IEnumerable<string[]> GetEdgesFromVertices(IEnumerable<string> vertices, ref IEnumerable<string[]> edges, ref IEnumerable<string[]> currentEdges)
        {
            var output = new HashSet<string[]>();
            foreach (var vertex in vertices)
            {
                output.UnionWith(GetEdgesFromVertex(vertex, edges));
            }

            currentEdges = output;
            edges = edges.RemoveValues(output);
            return output;
        }

        private IEnumerable<string[]> GetEdgesFromVertex(string vertex, IEnumerable<string[]> edges)
        {
            return edges.Where(e => e.Contains(vertex));
        }

        public IGraph GetTheBestSolution(IEnumerable<IGraph> solutions, ILogger logger)
        {
            logger?.Information("Wyszukiwanie najlepszego rozwiązania...");
            var selector = new Func<IGraph, int>(graph => graph.Vertices.Length);
            var max = solutions.Max(selector);
            var min = solutions.Min(selector);

            var theBestSolution = solutions.FirstOrDefault(graph => graph.Vertices.Length == min);
            if (theBestSolution.Equals(null))
            {
                throw new ArgumentException("Błąd w znalezieniu najlepszego rozwiązania", nameof(theBestSolution));
            }
            logger?.Information("Znaleziono najlepsze rozwiązanie");
            return theBestSolution;
        }

    }
}
