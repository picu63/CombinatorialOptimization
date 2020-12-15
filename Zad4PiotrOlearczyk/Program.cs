using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zad4PiotrOlearczyk
{
    class Program
    {
        public static List<int> NumbersList { get; set; } = new List<int>();

        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
            while (true)
            {
                if (!InputAmount(out var numbersAmount)) continue;
                NumbersList = InputNumbers(numbersAmount);
                var stopWatch = Stopwatch.StartNew();
                Console.WriteLine("Rozpoczęcie obliczeń...");
                int solution = GetMaxSolution(NumbersList);
                Console.WriteLine($"Wynik: {solution}");
                Console.WriteLine($"Upłynęło czasu: {stopWatch.ElapsedMilliseconds}");
            }
        }

        private static int GetMaxSolution(List<int> numbersList)
        {
            var totalSum=0;
            while (true)
            {
                if (numbersList.Count == 2)
                {
                    return totalSum;
                }
                var index = FindMaxIndex(numbersList);
                totalSum += numbersList[index - 1] + numbersList[index] + numbersList[index + 1];
                numbersList.RemoveAt(index);
            }
        }

        private static int FindMaxIndex(List<int> numbersList)
        {
            var maxDifferences = new List<int>();
            for (int i = 1; i < numbersList.Count - 1; i++)
            {
                var sumNeighbors = numbersList[i - 1] + numbersList[i + 1];
                var middleNumber = numbersList[i];
                var difference = Math.Abs(sumNeighbors - middleNumber);
                maxDifferences.Add(difference);
            }

            var max = maxDifferences.Max();
            var index = maxDifferences.IndexOf(max)+1;
            return index;
        }

        private static bool InputAmount(out int numbersAmount)
        {
            Console.Write("Proszę podać ilość liczb: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out numbersAmount))
            {
                Console.WriteLine("Nie udało się odczytać ilości liczb.");
                return false;
            }

            if (numbersAmount <= 2)
            {
                Console.WriteLine("Ilość liczb do podania musi być większa od 2.");
                return false;
            }

            return true;
        }

        private static List<int> InputNumbers(int numbersAmount)
        {
            var numbers = new List<int>();
            foreach (var i in Enumerable.Range(1, numbersAmount))
            {
                while (true)
                {
                    Console.Write($"Proszę podać {i} liczbę: ");
                    var inputNumber = Console.ReadLine();
                    if (int.TryParse(inputNumber, out int number))
                    {
                        numbers.Add(number);
                        break;
                    }

                    Console.WriteLine("Nie udało się odczytać liczby, spróbuj jeszcze raz");
                }
            }

            return numbers;
        }

    }
}
