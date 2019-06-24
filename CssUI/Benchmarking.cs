using xLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KeyPic.Benchmarking
{

    public struct Benchmark_Info
    {
        /// <summary>
        /// Name of the execution method
        /// </summary>
        public string Name;
        /// <summary>
        /// Average execution time
        /// </summary>
        public double Average;
        /// <summary>
        /// Shortest execution time
        /// </summary>
        public double Low;
        /// <summary>
        /// Longest execution time
        /// </summary>
        public double High;
        /// <summary>
        /// Sum of all execution times
        /// </summary>
        public double Total;
        /// <summary>
        /// Number of executions
        /// </summary>
        public int Count;

        public override string ToString()
        {
            return string.Format("[{0}]x{1} ({2:F4}s Total) Average: {3:F4}s | Low: {4:F4}s | High: {5:F4}", Name, Count, Total, Average, Low, High);
        }
    }
    /// <summary>
    /// Makes gathering benchmark times en mass and computing their average, etc.
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Stores a list of timing operation names, for use in addressing history.
        /// </summary>
        static Dictionary<Guid, string> Names = new Dictionary<Guid, string>();
        /// <summary>
        /// Stores a list of the ongoing timers
        /// </summary>
        static Dictionary<Guid, Stopwatch> Timers = new Dictionary<Guid, Stopwatch>();
        /// <summary>
        /// History of elapsed time values in seconds
        /// </summary>
        static Dictionary<string, List<double>> History = new Dictionary<string, List<double>>();

        public static Guid Start(string Name)
        {
            Guid ID = Guid.NewGuid();
            Stopwatch sw = new Stopwatch();
            Timers.Add(ID, sw);
            Names.Add(ID, Name);

            sw.Start();
            return ID;
        }

        public static void Stop(Guid ID)
        {
            Stopwatch sw;
            if (!Timers.TryGetValue(ID, out sw)) return;

            if (!sw.IsRunning) throw new InvalidOperationException("Benchmarking timer wasnt running!!!");
            sw.Stop();
            string Name;
            if (!Names.TryGetValue(ID, out Name)) return;
            if (!History.ContainsKey(Name)) History.Add(Name, new List<double>(0));

            History[Name].Add(sw.Elapsed.TotalSeconds);
            Timers.Remove(ID);
            Names.Remove(ID);
        }

        public static Benchmark_Info Get(string Name)
        {
            var hist = History[Name];

            double sum = 0;
            double? low = null, high = null;
            foreach (double n in hist)
            {
                sum += n;
                if (!low.HasValue) low = n;
                if (!high.HasValue) high = n;

                if (n < low) low = n;
                if (n > high) high = n;
            }
            double avg = (sum / (double)hist.Count);

            return new Benchmark_Info() { Name = Name, Average = avg, Count = hist.Count, High = high.Value, Low = low.Value, Total = sum };
        }

        public static void Print_All()
        {
            foreach (var kv in History)
            {
                Log.Success(Get(kv.Key).ToString());
            }
        }
    }
}
