using System;
using System.Collections.Generic;
using System.Text;

namespace GraphManager.Extensions
{
    public static class BoolExtensions
    {
        public static bool Yes(this bool value) => value;
        public static bool YesItDoes(this bool value) => value;
        public static bool YesItHas(this bool value) => value;

        public static bool No(this bool value) => !value;
        public static bool NoItDoesnt(this bool value) => !value;
        public static bool NoItHasNot(this bool value) => !value;

    }
}
