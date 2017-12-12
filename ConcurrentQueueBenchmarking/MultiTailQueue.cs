using System;
using System.Threading;
using System.Collections.Concurrent;

namespace ConcurrentQueueBenchmarking
{
    /// <summary>
    ///   A queue that avoids sychronization between multiple
    ///   enqueueing threads.  Assumes that there is only one thread
    ///   dequeueing.
    /// </summary>
    public class MultiTailQueue<T>
    {
        private readonly LongAdder size;
        private readonly ConcurrentQueue<T>[] queues;

        public MultiTailQueue() : this(5)
        {}

        public MultiTailQueue(int size)
        {
            int queues = 1 << size;
            this.size = new LongAdder();
            this.queues = new ConcurrentQueue<T>[queues];
            for (int i = 0; i < queues; ++i)
                this.queues[i] = new ConcurrentQueue<T>();
        }

        private int CurrentStripe {
            get => Thread.CurrentThread.GetHashCode() & (queues.Length - 1);
        }

        public bool TryDequeue(out T t)
        {
            while (size.Value > 0) {
                for (int c = 0, i = CurrentStripe;
                     c < queues.Length;
                     i = (i + 1) & (queues.Length - 1), ++c) {
                    if (queues[i].TryDequeue(out t)) {
                        size.Decrement();
                        return true;
                    }
                }
            }
            t = default(T);
            return false;
        }

        public void Enqueue(T t)
        {
            size.Increment();
            queues[CurrentStripe].Enqueue(t);
        }

        public int Count {
            get => (int)size.Value;
        }

        public bool IsEmpty {
            get => size.Value == 0;
        }

        public void Clear()
        {
            T t;
            foreach (ConcurrentQueue<T> q in queues)
                while (q.TryDequeue(out t));
            size.Reset();
        }
    }
}
