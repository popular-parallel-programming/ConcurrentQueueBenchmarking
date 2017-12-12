using System;
using System.Collections.Concurrent;

namespace ConcurrentQueueBenchmarking
{
    public sealed class MultiTailQueueBenchmarker<T> : Benchmarker where T : new()
    {
        private readonly MultiTailQueue<T> mtq = new MultiTailQueue<T>();
        private readonly Action<MultiTailQueueBenchmarker<T>> setup;
        private readonly Action<int, MultiTailQueueBenchmarker<T>> runner;

        public MultiTailQueue<T> Queue
        {
            get { return mtq; }
        }

        public MultiTailQueueBenchmarker(string name, int threads,
                                         Action<int, MultiTailQueueBenchmarker<T>> runner) : base(name, threads)
        {
            this.runner = runner;
        }

        public MultiTailQueueBenchmarker(string name, int threads,
                                         Action<MultiTailQueueBenchmarker<T>> setup,
                                         Action<int, MultiTailQueueBenchmarker<T>> runner) : base(name, threads)
        {
            this.setup  = setup;
            this.runner = runner;
        }

        public override void Setup()
        {
            base.Setup();

            T t;
            while (mtq.TryDequeue(out t)) {}

            if (this.setup != null) {
                this.setup(this);
            }

            for (int i = 0; i < this.NumThreads; ++i) {
                this.AllocThread(i, j => {
                    runner(j, this);
                });
            }
        }
    }
}
