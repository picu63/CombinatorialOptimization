using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Serilog;

namespace ConsoleSolver
{
    public class ConsoleSolver<TData> : IConsoleManager
    {
        private readonly Func<string, TData> _dataReader;
        private readonly Func<TData, ILogger, IEnumerable<TData>> _solutionsFinder;
        private readonly Func<IEnumerable<TData>, ILogger, TData> _bestSolutionFinder;

        int _separatorLength = 35;

        private readonly ILogger _log;
        public string Description { get; }

        public ConsoleSolver(string description,
            Func<string, TData> dataReader,
            Func<TData, ILogger, IEnumerable<TData>> solutionsFinder,
            Func<IEnumerable<TData>, ILogger, TData> bestSolutionFinder)
        {
            this.Description = description;
            _dataReader = dataReader;
            _solutionsFinder = solutionsFinder;
            _bestSolutionFinder = bestSolutionFinder;

            _log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();
        }

        public void RunProgram(string[] pathToFile)
        {
            _log.Information($"{new string('=', _separatorLength)}\nRozpoczęcie działania programu: {DateTime.Now.ToShortTimeString()}.");

            CheckArgs(pathToFile);

            _log.Information("Sprawdzanie podanego argumentu...");
            var path = GetPathFromArgs(pathToFile);

            _log.Information($"Ładowanie zawartości z pliku: {path}...");
            var inputData = _dataReader.Invoke(path);

            var started = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            _log.Information($"Uruchamianie programu: {this.Description}...");
            var solutions = _solutionsFinder.Invoke(inputData, _log);

            var bestSolution = _bestSolutionFinder(solutions, _log);

            var elapsed = stopwatch.Elapsed;

            _log.Information($"Generowanie rozwiązań...");
            var consoleResult = GenerateLog(started, elapsed, solutions, bestSolution);

            _log.Information($"\n{consoleResult}"); 

            _log.Information($"Czas zakończenia pracy programu: {DateTime.Now.ToShortTimeString()}.");

            Console.WriteLine("Naciśnij dowolny klawisz aby zakończyć...");

            Console.ReadKey();

        }

        private void WriteToConsoleWithColorChange(TimeSpan elapsed, string consoleResult)
        {
            var consoleDefaultColor = Console.ForegroundColor;

            if (elapsed.TotalSeconds > 10)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(consoleResult);

            Console.ForegroundColor = consoleDefaultColor;
        }

        private string GenerateLog(DateTime started, TimeSpan elapsed, IEnumerable<TData> solutions, TData bestSolution)
        {
            var logContent = new StringBuilder();
            var content = "";

            logContent.AppendLine($"{new string('_', _separatorLength)}");

            //header
            content = FormatHeader("Znalezione rozwiązania:");
            logContent.AppendLine($"{content}");

            //all solutions
            for (int i = 0; i < solutions.Count(); i++)
            {
                content = FormatSubheader($"Nr. rozwiązania: {i + 1}");
                logContent.AppendLine(content);
                logContent.AppendLine(solutions.ElementAt(i).ToString());
            }


            //summary
            content = FormatHeader("PODSUMOWANIE:");
            logContent.AppendLine($"{content}");

            logContent.AppendLine($"Czas startu algorytmu: {started.ToShortDateString()} {started.ToShortTimeString()}");
            logContent.AppendLine();
            logContent.AppendLine($"Czas wykonania operacji: {FormatTimeSpan(elapsed)}");
            logContent.AppendLine();
            logContent.AppendLine($"Znaleziono {solutions.Count()} rozwiązań.");
            logContent.AppendLine();
            //best solutions
            content = FormatHeader("NAJLEPSZE ROZWIĄZANIE:");
            logContent.AppendLine($"{content }");
            logContent.AppendLine(bestSolution.ToString());

            return logContent.ToString();
        }

        private string FormatHeader(string content)
        {
            var res = new StringBuilder();

            res.AppendLine(new string('_', _separatorLength));
            res.AppendLine();
            res.AppendLine(content);


            return res.ToString();
        }

        private string FormatSubheader(string content)
        {
            var res = new StringBuilder();

            res.AppendLine(new string('_', _separatorLength));
            res.AppendLine();
            res.AppendLine(content);

            return res.ToString();
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            var twoDigitFormat = "{0:00}";
            var threeDigitFormat = "{0:000}";

            var h = string.Format(twoDigitFormat, timeSpan.Hours);
            var m = string.Format(twoDigitFormat, timeSpan.Minutes);
            var s = string.Format(twoDigitFormat, timeSpan.Seconds);
            var ms = string.Format(threeDigitFormat, timeSpan.Milliseconds);

            return $"{h}:{m}:{s}.{ms}";
        }

        private string GetPathFromArgs(string[] args)
        {
            var path = args.FirstOrDefault();

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The file path is null or empty!");

            if (!File.Exists(path))
                throw new FileNotFoundException($"For the given path >>{path}<< no file was found.");

            return path;
        }

        private void CheckArgs(string[] args)
        {
            if (args == null && !args.Any())
                throw new ArgumentException("No argument was given!");
        }
    }
}
