using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CssUI
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

    public struct Benchmark_Instance
    {
        public string Name;
        public Stopwatch Timing;
    }
    /// <summary>
    /// Makes gathering benchmark times en mass and computing their average, etc.
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Stores a list of timing operation names, for use in addressing history.
        /// </summary>
        //static Dictionary<Guid, string> Names = new Dictionary<Guid, string>();
        /// <summary>
        /// Stores a list of the ongoing timers
        /// </summary>
        //static Dictionary<Guid, Stopwatch> Timers = new Dictionary<Guid, Stopwatch>();

        /// <summary>
        /// Stores a list of the ongoing benchmarks
        /// </summary>
        static ConcurrentDictionary<Guid, Benchmark_Instance> Active = new ConcurrentDictionary<Guid, Benchmark_Instance>();

        /// <summary>
        /// History of elapsed benchmark time values in seconds
        /// </summary>
        static ConcurrentDictionary<string, List<double>> History = new ConcurrentDictionary<string, List<double>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid Start(string Name)
        {
            Guid ID = Guid.NewGuid();

            var Info = new Benchmark_Instance()
            {
                Name = Name,
                Timing = new Stopwatch()
            };

            Active.TryAdd(ID, Info);

            Info.Timing.Start();
            return ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stop(Guid ID)
        {
            Benchmark_Instance Instance;
            if (!Active.TryGetValue(ID, out Instance))
                throw new KeyNotFoundException("Cannot stop benchmark timer, the specified ID does not exist!");

            string Name = Instance.Name;
            Stopwatch Timer = Instance.Timing;

            // Stop the timer
            if (!Timer.IsRunning) throw new InvalidOperationException("Benchmarking timer wasnt running!!!");
            Timer.Stop();
            // Setup our list of times if it doesnt exist
            if (!History.ContainsKey(Name))
            {
                if (!History.TryAdd(Name, new List<double>(100)))
                    throw new Exception("Unable to add key to benchmark history!");
            }

            History[Name].Add(Timer.Elapsed.TotalSeconds);
            Active.TryRemove(ID, out _);
        }

        public static Benchmark_Info? Get(string Name)
        {
            if (History.TryGetValue(Name, out List<double> hist))
            {
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
            return null;
        }

        public static void Print(string name)
        {
            var info = Get(name);
            if (info.HasValue)
            {
                xLog.Log.Success(info.Value.ToString());
            }
        }

        public static void Print_All()
        {
            foreach (var kv in History)
            {
                xLog.Log.Success(Get(kv.Key).Value.ToString());
            }
        }
    }
}
