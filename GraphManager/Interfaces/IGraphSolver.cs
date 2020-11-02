using System;
using System.Collections.Generic;
using System.Text;

namespace GraphManager.Interfaces
{
    public interface IGraphSolver
    {
        IGraph FindMaximumVerticesSolution(IGraph inputGraph);

        IEnumerable<IGraph> FindAllSolutions(IGraph inputGraph);
    }
}
