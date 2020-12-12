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
                Console.Write("Prosz� poda� ilo�� liczb: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out var numbersAmount))
                {
                    Console.WriteLine("Nie uda�o si� odczyta� ilo�ci liczb.");
                    continue;
                }

                if (numbersAmount <= 2)
                {
                    Console.WriteLine("Ilo�� liczb do podania musi by� wi�ksza od 2.");
                }
                foreach (var i in Enumerable.Range(1, numbersAmount))
                {
                    while (true)
                    {
                        Console.Write($"Prosz� poda� {i} liczb�: ");
                        var inputNumber = Console.ReadLine();
                        if (int.TryParse(inputNumber, out int number))
                        {
                            _numbersList.Add(number);
                            break;
                        }
                        Console.WriteLine("Nie uda�o si� odczyta� liczby, spr�buj jeszcze raz");
                    }
                }
            }
        }
    }
}
