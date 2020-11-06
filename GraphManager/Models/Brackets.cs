using System;
using System.Collections.Generic;
using System.Text;

namespace GraphManager.Models
{
    public class Brackets
    {
        public char Opening { get; }
        public char Closing { get; }

        public static Brackets Round  { get; } = new Brackets('(', ')');
        public static Brackets Curly  { get; } = new Brackets('{', '}');
        public static Brackets Square { get; } = new Brackets('[', ']');
        public static Brackets Angle  { get; } = new Brackets('⟨', '⟩');

        public Brackets(char opening, char closing)
        {
            Opening = opening;
            Closing = closing;
        }
    }
}
