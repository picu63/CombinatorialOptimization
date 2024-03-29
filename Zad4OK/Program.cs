﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zad4OK
{
    class Program
    {
        public static List<int> NumbersList { get; set; } = new List<int>();

        static void Main(string[] args)
        {
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

        private static int GetMaxSolution(List<int> numbersList, int removedSum = 0)
        {
            if (numbersList.Count == 3)
            {
                return numbersList[0] + numbersList[1] + numbersList[2] + removedSum;
            }

            List<int> maxValues = new List<int>();

            var rangeForCalculation = Enumerable.Range(1, numbersList.Count - 2).ToList();
            foreach (var i in rangeForCalculation)
            {
                removedSum = numbersList[i - 1] + numbersList[i] + numbersList[i + 1];
                var cloneList = new List<int>(numbersList);
                cloneList.RemoveAt(i);
                maxValues.Add(GetMaxSolution(cloneList, removedSum));
            }

            return maxValues.Select(x => x + removedSum).Max();
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
