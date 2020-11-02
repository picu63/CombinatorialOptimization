using System;
using ConsoleManager;
using GraphManager.Implementations;
using GraphManager.Interfaces;
using Program;

namespace OptymalizacjaKombinatorycznaZad1
{
    class Program
    {
        static void Main(string[] args)
        {
            IConsoleManager consoleManager = new ConsoleManager<IGraph>(
        new DataReader().ReadDataFromFile,
        (graph, logger) => new GraphSolver().FindAllSolutions(graph, logger),
        (solutions, logger) => new GraphSolver().GetTheBestSolution(solutions, logger)
    //(graph, logger) => new GraphSolver().FindMaximumVerticesSolutionWithLogger(graph, logger)
    );

            consoleManager.RunProgram(args);
        }
    }
}
