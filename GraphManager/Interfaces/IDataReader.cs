using System;
using System.Collections.Generic;
using System.Text;

namespace GraphManager.Interfaces
{
    public interface IDataReader
    {
        IGraph ReadDataFromFile(string path);

        IGraph ReadDataFromString(string input);
    }
}
