using GraphManager.Extensions;
using GraphManager.Implementations;
using GraphManager.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program
{
    public class GraphSolver : IGraphSolver
    {
        ILogger _log;

        public IEnumerable<IGraph> FindAllSolutions(IGraph inputGraph)
        {
            return FindAllSolutionsWithLogger(inputGraph, null);
        }

        public IEnumerable<IGraph> FindAllSolutionsWithLogger(IGraph inputGraph, ILogger logger)
        {
            var workingVertices = new List<string>(inputGraph.Vertices);

            var solitaireVertices = GetUnattachedVertices(inputGraph);

            workingVertices = workingVertices.RemoveValues(solitaireVertices).ToList();

            var solutions = new List<IGraph>();

            foreach (var vertex in workingVertices)
            {
                logger?.Information($"Wyszukiwanie rozwiązania dla wierzchołka: {vertex}");

                var solution = GetSolution(vertex, inputGraph.Edges).ToList();

                solution = solution.Add(solitaireVertices).ToList();

                solution = solution.SortStringAsInt().ToList();

                if (CheckIfSolutionAlreadyExist(solution, solutions).Yes())
                    continue;

                logger?.Information($"Znaleziono rozwiązanie {solution.Count()} vertices.");

                solutions.Add(new Graph("", "", solution.ToArray(), Array.Empty<string[]>()));
            }
           
            return solutions;
        }

        private bool CheckIfSolutionAlreadyExist(List<string> vertices, List<IGraph> solutions)
        {
            foreach (var graph in solutions)
            {
                var foundVertices = graph.Vertices;

                if (foundVertices.Length != vertices.Count)
                    continue;

                if (foundVertices.SequenceEqual(vertices))
                    return true;
            }

            return false;
        }

        private bool SolutionsContainVertex(List<IGraph> solutions, string vertex)
        {
            foreach (var graph in solutions)
            {
                if (graph.Vertices.Contains(vertex))
                    return true;
            }

            return false;
        }

        protected IEnumerable<string> GetSolution(string vertex, string[][] edges)
        {
            var solution = new List<string>(new[] { vertex });

            var edgeBuffer = new List<string[]>(edges);
            var excludedVertices = new List<string>();
            var excludedEdges = new List<string[]>();
            var searchingEdges = new List<string[]>();

            while (edgeBuffer.Count > 0)
            {
                //z bufora krawędzi weź krawędzie do wykluczenia (tj. takie, któe zawierają wierzchołki rozwiązania)
                excludedEdges = excludedEdges.Add(TakeEdgesContainingVertices(removeFromEdges: ref edgeBuffer, containingVertices: solution)).ToList();

                if (edgeBuffer.Any().No())
                    break; //żadne wierzchołki już się nie znajdą, bo z pustego bufora nic nie wejdzie do listy krawędzi przeszukiwanych, więc już teraz można przerwać

                //z krawędzi wykluczonych weź wierzchołki nienależące do rozwiązania i dodaj do listy wierzchołków wykluczonych
                excludedVertices = excludedVertices.Add(TakeVerticesFromEdges(browsedEdges: excludedEdges, exceptFor: solution)).ToList();

                //z bufora krawędzi weź krawędzie zawierające wierzchołki wykluczone i dodaj je do krawędzi przeszukiwanych
                searchingEdges = searchingEdges.Add(TakeEdgesContainingVertices(removeFromEdges: ref edgeBuffer, containingVertices: excludedVertices)).ToList();

                //do rozwiązania dodaj wierzchołki z krawędzi przeszukiwanych, z pominięciem wierzchołków wykluczonych
                solution = solution.Add(TakeVerticesFromEdges(browsedEdges: searchingEdges, exceptFor: excludedVertices)).ToList();

                if (edgeBuffer.Any().No())
                    break; //szkoda kilku milisekund na czyszczenie listy krawędzi przeszukiwanych i sprawdzanie warunku kolejnej iteracji

                //wyczyść kolejkę krawędzi do przejrzenia
                searchingEdges.Clear();

            }

            return solution;
        }

        protected IEnumerable<string> TakeVerticesFromEdges(List<string[]> browsedEdges, List<string> exceptFor)
        {
            var result = new List<string>();

            foreach (var edge in browsedEdges)
            {
                var vertex1 = edge[0];
                var vertex2 = edge[1];

                if (exceptFor.Contains(vertex1).NoItDoesnt())
                    result.Add(vertex1);

                if (exceptFor.Contains(vertex2).NoItDoesnt())
                    result.Add(vertex2);
            }

            result.Sort();

            return result.Distinct();
        }

        protected IEnumerable<string[]> TakeEdgesContainingVertices(ref List<string[]> removeFromEdges, List<string> containingVertices)
        {
            var newEdgesList = new List<string[]>();
            var result = new List<string[]>();

            foreach (var edge in removeFromEdges)
            {
                if (containingVertices.Contains(edge[0]) || containingVertices.Contains(edge[1]))
                    result.Add(edge);
                else
                    newEdgesList.Add(edge);
            }               

            removeFromEdges = newEdgesList;

            return result;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
        }

        protected void RemoveEdgesContaingStartingVertex(string startingVertex, ref IGraph graph)
        {
            var resultEdges = new List<string[]>(graph.Edges);

            foreach (var edge in graph.Edges)
            {
                if (edge.Contains(startingVertex))
                {
                    resultEdges.Remove(edge);
                }
            }

            graph.ReplaceEdges(resultEdges.ToArray());

        }

        protected IEnumerable<string> GetUnattachedVertices(IGraph inputGraph)
        {
            if (inputGraph.Vertices == null && inputGraph.Vertices.Any().No())
                return Array.Empty<string>();

            var result = inputGraph.Vertices.ToList();

            foreach (var edge in inputGraph.Edges)
            {
                if (result.Contains(edge[0]))
                    result.Remove(edge[0]);

                if (result.Contains(edge[1]))
                    result.Remove(edge[1]);
            }

            return result;
        }

        public IGraph FindMaximumVerticesSolutionWithLogger(IGraph inputGraph, ILogger logger)
        {
            var solutions = FindAllSolutionsWithLogger(inputGraph, logger);

            logger?.Information($"Found {solutions.Count()} solutions in total.");
            logger?.Information($"Getting the best solution...");

            return GetTheBestSolution(solutions, logger);
        }

        public IGraph FindMaximumVerticesSolution(IGraph inputGraph)
        {
            return FindMaximumVerticesSolutionWithLogger(inputGraph, null);
        }

        public IGraph GetTheBestSolution(IEnumerable<IGraph> solutions, ILogger logger)
        {
            var numberOfVertices = solutions.Select(x => x.Vertices.Length);

            var maxNumberOfVertices = numberOfVertices.Max();
            var minNumberOfVertices = numberOfVertices.Min();

            logger?.Information($"Found solutions contain from {minNumberOfVertices} up to {maxNumberOfVertices} vertices.");

            var bestSolution = solutions
                .Where(x => x.Vertices.Length == maxNumberOfVertices)
                .FirstOrDefault();

            if (bestSolution == null)
                throw new Exception("There is no best solution for the given graph.");

            logger?.Information("Best solution found.");

            return bestSolution;
        }



        
    }
}
