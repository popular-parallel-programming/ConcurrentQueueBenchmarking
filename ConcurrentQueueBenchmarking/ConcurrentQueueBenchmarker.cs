using System;
using System.Collections.Concurrent;

namespace ConcurrentQueueBenchmarking
{
    public sealed class ConcurrentQueueBenchmarker<T> : Benchmarker where T : new()
    {
        private readonly ConcurrentQueue<T> cq = new ConcurrentQueue<T>();
        private readonly Action<ConcurrentQueueBenchmarker<T>> setup;
        private readonly Action<int, ConcurrentQueueBenchmarker<T>> runner;

        public ConcurrentQueue<T> Queue
        {
            get { return cq; }
        }

        public ConcurrentQueueBenchmarker(string name, int threads,
                                          Action<int, ConcurrentQueueBenchmarker<T>> runner) : base(name, threads)
        {
            this.runner = runner;
        }

        public ConcurrentQueueBenchmarker(string name, int threads,
                                          Action<ConcurrentQueueBenchmarker<T>> setup,
                                          Action<int, ConcurrentQueueBenchmarker<T>> runner) : base(name, threads)
        {
            this.setup  = setup;
            this.runner = runner;
        }

        public override void Setup()
        {
            base.Setup();

            T t;
            while (cq.TryDequeue(out t)) {}

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
