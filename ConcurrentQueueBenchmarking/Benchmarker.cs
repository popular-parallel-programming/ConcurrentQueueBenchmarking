using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ConcurrentQueueBenchmarking
{
    class Compiler
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        static public T ConsumeValue<T>(T dummy)
        {
            return dummy;
        }
    }

    public class Benchmarker
    {
        public string Name { get; set; } = "";
        public int NumThreads { get; set; } = 1;
        public int Iterations { get; set; } = 10;
        public int Warmups { get; set; } = 3;
        public List<double> Measurements { get; set; } = new List<double>();

        private Thread[] threads;
        private CountdownEvent countdownEvent;

        public double Mean
        {
            get {
                if (this.Iterations > 0) {
                    return this.Measurements.Sum() / this.Iterations;
                } else {
                    return Double.NaN;
                }
            }
        }

        public double Variance
        {
            get {
                if (this.Iterations > 0) {
                    double mean = this.Mean;

                    return this.Measurements
                        .Select(m => Math.Pow(m - mean, 2.0))
                        .Sum() / (this.Iterations - 1.0);
                }

                return Double.NaN;
            }
        }

        public double StandardDeviation
        {
            get { return Math.Sqrt(this.Variance); }
        }

        public Benchmarker()
        {
        }

        public Benchmarker(string name, int threads)
        {
            this.Name = name;
            this.NumThreads = threads;
        }

        public void AllocThread(int i, Action<int> runner)
        {
            this.threads[i] = new Thread(() => {
                this.countdownEvent.Wait();
                runner(i);
            });
        }

        public virtual void Setup()
        {
            if (this.NumThreads > 0) {
                threads = new Thread[this.NumThreads];
            }

            if (countdownEvent == null) {
                countdownEvent = new CountdownEvent(1);
            }
        }

        public virtual int Benchmark()
        {
            if (this.countdownEvent == null) {
                throw new InvalidOperationException("Must call Setup before Benchmark");
            } else {
                this.countdownEvent.Reset();
            }

            foreach (Thread thread in this.threads) {
                thread.Start();
            }

            this.countdownEvent.Signal();
            return 0;
        }

        public virtual void Shutdown()
        {
            foreach (Thread thread in this.threads) {
                thread.Join();
            }
        }

        private void ForceGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void Run()
        {
            #if DEBUG
            Console.WriteLine("WARNING: You are benchmarking in DEBUG mode");
            #endif

            // Force full garbage collection
            this.ForceGarbageCollection();

            var stopWatch = new Stopwatch();

            for (int i = 0; i < this.Warmups; ++i) {
                Compiler.ConsumeValue(i);
            }

            for (int i = 0; i < this.Iterations; ++i) {
                // 1. Run setup code
                this.Setup();

                // 2. Run benchmark and time result
                stopWatch.Restart();
                Compiler.ConsumeValue(this.Benchmark());
                stopWatch.Stop();

                // 3. Shutdown benchmark, record measurement and force garbage collection
                this.Shutdown();
                this.Measurements.Add(stopWatch.Elapsed.TotalMilliseconds);
                this.ForceGarbageCollection();
            }

            Console.WriteLine("{0,-25}\t{1,10} {2,10:0.000} {3,6:0.000} {4,5}",
                this.Name, 
                this.NumThreads,
                this.Mean,
                this.StandardDeviation,
                "ms/op");
        }

        public static void PrintHeader()
        {
            Console.WriteLine("{0,-25}\t{1,10:0.000} {2,10:0.000} {3,6:0.000} {4,5}",
                "Name",
                "Threads",
                "Mean",
                "Stddev",
                "Unit");
        }

        public static void RunMany(IEnumerable<int> threadCounts, Action<int> runner)
        {
            PrintHeader();

            foreach (int i in threadCounts) {
                runner(i);
            }
        }

        public static void RunMany(Benchmarker benchmarker, IEnumerable<int> threadCounts, Action<int> runner)
        {
            PrintHeader();

            foreach (int i in threadCounts) {
                runner(i);
            }
        }
    }
}
