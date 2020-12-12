using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zad4Optymalizacja
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly List<int> _numbersList;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _numbersList = new List<int>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.Write("Proszê podaæ iloœæ liczb: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out var numbersAmount))
                {
                    Console.WriteLine("Nie uda³o siê odczytaæ iloœci liczb.");
                    continue;
                }

                if (numbersAmount <= 2)
                {
                    Console.WriteLine("Iloœæ liczb do podania musi byæ wiêksza od 2.");
                }
                foreach (var i in Enumerable.Range(1, numbersAmount))
                {
                    while (true)
                    {
                        Console.Write($"Proszê podaæ {i} liczbê: ");
                        var inputNumber = Console.ReadLine();
                        if (int.TryParse(inputNumber, out int number))
                        {
                            _numbersList.Add(number);
                            break;
                        }
                        Console.WriteLine("Nie uda³o siê odczytaæ liczby, spróbuj jeszcze raz");
                    }
                }
            }
        }
    }
}
