using System;
using ConsoleSolver;
using GraphManager.Implementations;
using GraphManager.Interfaces;
using Program;

namespace OptymalizacjaKombinatorycznaZad1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            IConsoleManager consoleManager = new ConsoleSolver<IGraph>("Optymalizacja kombinatoryczna ZAD1",
                    new DataReader().ReadDataFromFile,
        (graph, logger) => new GraphSolver().FindAllSolutionsAsync(graph, logger).Result,
        (solutions, logger) => new GraphSolver().GetTheBestSolution(solutions, logger)
            );

            consoleManager.RunProgram(args);
        }
    }
}
