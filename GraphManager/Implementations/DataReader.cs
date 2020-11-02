using GraphManager.Extensions;
using GraphManager.Interfaces;
using GraphManager.Models;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GraphManager.Implementations
{
    public class DataReader : IDataReader
    {
        private static readonly bool _notOk = false;
        private static readonly bool _ok = true;

        private static readonly string _vertexNameRegexPattern = @"[A-Z]";

        private static readonly string[] _leftOperators = new[] { "<-", "<" };
        private static readonly string[] _rightOperators = new[] { "->", ">" };
        private static readonly string[] _bidirectionalOperators = new[] { "--", "-" };

        private static readonly string[] _operators;

        static DataReader()
        {
            _operators =
                _leftOperators
                .Add(_rightOperators)
                .Add(_bidirectionalOperators)
                .OrderByDescending(x => x.Count())
                .ToArray();
        }

        public IGraph ReadDataFromFile(string path)
        {
            ReadToString(path, out string content);

            return ReadDataFromString(content);
        }

        public IGraph ReadDataFromString(string content)
        {            
            content = NormalizeContent(content);

            if (CheckInputSyntax(content).IsWrong(out string message))
                throw new Exception($"Error while parsing the graph data input: {message}");

            var type = TakeGraphType(content);

            var name = TakeGraphName(content);

            var vertices = TakeVertices(content);

            var edges = TakeEdges(content);

            CheckIfEdgesContainAdditionalVertices(edges, vertices,
                out bool areThereAdditionalVerticesInEdges,
                out IEnumerable<string> verticesFromEdges);

            AddVerticesFromEdgesIfNecessary(areThereAdditionalVerticesInEdges, ref vertices, verticesFromEdges);

            return new Graph(name, type, vertices, edges);
        }

        #region private and protected auxiliary methods

        protected void AddVerticesFromEdgesIfNecessary(bool areThereAdditionalVerticesInEdges, ref string[] vertices, IEnumerable<string> verticesFromEdges)
        {
            if (areThereAdditionalVerticesInEdges.No())
                return;

            var vertexList = vertices.ToList();

            vertices = vertexList
                .Union(verticesFromEdges)
                .OrderBy(x => x)
                .ToArray();
        }

        private void CheckIfEdgesContainAdditionalVertices(string[][] edges, IEnumerable<string> vertices, out bool areThereAdditionalVerticesInEdges, out IEnumerable<string> verticesFromEdges)
        {
            verticesFromEdges = TakeVerticesFromEdges(edges);

            areThereAdditionalVerticesInEdges = vertices.ContainsAny(verticesFromEdges);            
        }

        private IEnumerable<string> TakeVerticesFromEdges(string[][] edges)
        {
            var res = new List<string>();

            foreach (var edge in edges)
            {
                foreach (var vertex in edge)
                {
                    if (res.Contains(vertex).NoItDoesnt())
                        res.Add(vertex);
                }                
            }

            return res.OrderBy(x => x);
        }

        protected string NormalizeContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return "";

            var result = content
                .Trim()
                .RemoveEndOfLineCharacters()
                .RemoveDoubleSpaces();

            return result;
        }

        private GraphAnalysisResult CheckInputSyntax(string? content)
        {
            if (content == null) 
                return new GraphAnalysisResult(
                    _notOk, 
                    "Graph data input is null.");

            if (content.TakeFirstPart().IsNot("digraph", caseSensitive: false)) 
                return new GraphAnalysisResult(
                    _notOk, 
                    $"The first word of graph data input is not digraph but {content.TakeFirstPart()}.");
                        
            if (content.TakeSecondPart().IsNot(new Regex(_vertexNameRegexPattern))) 
                return new GraphAnalysisResult(
                    _notOk, 
                    $"The second word of graph data input ({content.TakeSecondPart()}) doesn't match {_vertexNameRegexPattern} regex pattern.");

            if (content.TakeThirdPart().IsNotBracketed(Brackets.Curly)) 
                return new GraphAnalysisResult(
                    _notOk, 
                    $"The third word of graph data input ({content.TakeThirdPart().RemoveEndOfLineCharacters().RemoveDoubleSpaces()}) is not brackted.");
            
            return new GraphAnalysisResult(_ok);
        }

        protected string[][]TakeEdges(string content)
        {
            var body = TakeBody(content);

            var elements = body.Split(';');

            elements = RemoveEmptyElement(elements).ToArray();

            elements = elements.Where(x => x.ContainsAny(_operators).YesItDoes()).ToArray();

            var result = new string[elements.Count()][];

            for (int i = 0; i < elements.Count(); i++)
            {
                var element = elements.ElementAt(i);

                GetElementVertices(element, 
                    out string v1, 
                    out string v2);

                result[i] = new[] { v1, v2 };
            }

            return result;
        }

        private void GetElementVertices(string element, out string v1, out string v2)
        {
            var elementItems = element.Split(_operators, StringSplitOptions.RemoveEmptyEntries);

            if (elementItems.Count() < 2)
                throw new ArgumentException($"The element ({element}) of edge input doesn't contain all vertices!");

            var tempV1 = NormalizeContent(elementItems[0]);
            var tempV2 = NormalizeContent(elementItems[1]);

            switch (element)
            {
                case var temp when element.ContainsAny(_leftOperators): // v1 <- < v2
                    v1 = tempV2;
                    v2 = tempV1;
                    break;
                case var temp when element.ContainsAny(_rightOperators): // v1 -> > v2
                default:
                    v1 = tempV1;
                    v2 = tempV2;
                    break;
            }
        }

        private string FindOperator(string item)
        {
            foreach (var @operator in _operators)
            {
                if (item.Contains(@operator))
                    return @operator;
            }

            return string.Empty;
        }

        protected string[] TakeVertices(string content)
        {
            var body = TakeBody(content);

            var elements = body.Split(';');

            elements = RemoveEmptyElement(elements).ToArray();

            return elements
                .Where(x => x.ContainsAny(_operators).NoItDoesnt())
                .ToArray();
        }

        private string TakeBody(string content)
        {
            return content
                .TakeThirdPart()
                .SkipFirstLetter()
                .SkipLastLetter()
                .Trim();
        }

        private IEnumerable<string> RemoveEmptyElement(IEnumerable<string> elements)
        {
            foreach (var item in elements)
            {
                if (string.IsNullOrEmpty(item).No())
                    yield return item;
            }
        }

        private string TakeGraphName(string content)
        {
            return content
                .TakeSecondPart();
        }

        private string TakeGraphType(string content)
        {
            return content
                .TakeFirstPart()
                .ToLower();
        }

        private void ReadToString(string path, out string content)
        {
            if (File.Exists(path).YesItDoes())
                content = File
                    .ReadAllText(path);
            else
                throw new FileLoadException($"The file {path} doesn't exist.");
        }

        #endregion

        #region GraphAnalysisResult class

        private class GraphAnalysisResult
        {
            private bool _graphIsOk;

            public bool IsWrong(out string message) 
            { 
                message = this.Message; 
                
                return !_graphIsOk; 
            }
            public bool IsOk() => _graphIsOk;

            public string Message { get; }

            public GraphAnalysisResult(bool graphIsOk, string message = "")
            {
                _graphIsOk = graphIsOk;
                Message = message;
            }
        }

        #endregion

    }
}
