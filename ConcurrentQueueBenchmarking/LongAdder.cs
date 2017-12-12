using System;
using System.Threading;

namespace ConcurrentQueueBenchmarking
{
    /// <summary>
    ///   A simple C# variant of Java's LongAdder, inspired by Peter's
    ///   NewLongAdderPadded from the PCPP course.
    /// </summary>
    public class LongAdder
    {
        private const int CACHE_LINE_SIZE = 64 * 8;
        private const int NSTRIPES = 31;
        private const int PADDING = CACHE_LINE_SIZE / sizeof(long);

        private readonly long[] counters;

        public LongAdder()
        {
            counters = new long[NSTRIPES * PADDING];
        }

        /// <summary>
        ///   Add delta to the value of the long adder.
        /// </summary>
        public void Add(long delta)
        {
            int idx = (Thread.CurrentThread.GetHashCode() % NSTRIPES) * PADDING;
            Interlocked.Add(ref counters[idx], delta);
        }

        /// <summary>
        ///   The same as Add(1).
        /// </summary>
        public void Increment()
        {
            Add(1L);
        }

        /// <summary>
        ///   The same as Add(-1).
        /// </summary>
        public void Decrement()
        {
            Add(-1L);
        }

        /// <summary>
        ///   The value of this instance. Since the value is computed
        ///   on demand and without locking, there may not be a "real"
        ///   point in time at which the instance has the resulting
        ///   value.
        /// </summary>
        public long Value
        {
            get {
                long sum = 0;
                // For-loop instead of LINQ Sum() to skip padding values.
                for (int i = 0; i < counters.Length; i += PADDING)
                    sum += Interlocked.Read(ref counters[i]);
                return sum;
            }
        }

        /// <summary>
        ///   Resets the counter to 0. This method is not thread-safe!
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < counters.Length; i += PADDING)
                counters[i] = 0L;
        }
    }
}
