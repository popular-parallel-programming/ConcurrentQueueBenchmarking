using System;

namespace ConcurrentQueueBenchmarking
{
    class Program
    {
        private const int QUEUE_ACTIONS_PER_THREAD = 1000000;

        static void Main(string[] args)
        {
            var threads = new [] { 2, 4, 8, 16, 32 };

            // 1. ConcurrentQueue.Enqueue
            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue.Enqueue", i,
                    (_, cqb) => {
                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                            cqb.Queue.Enqueue(100);
                        }});

                qb.Run();
            });

            // 2. ConcurrentQueue.TryDequeue
            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue.TryDequeue", i,
                    cqb => {
                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                            cqb.Queue.Enqueue(100);
                        }
                    }, (_, cqb) => {
                        int k;

                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / cqb.NumThreads; ++j) {
                            cqb.Queue.TryDequeue(out k);
                        }
                    });

                qb.Run();
            });

            // 1. MultiTailQueue.Enqueue
            Benchmarker.RunMany(threads, i => {
                var qb = new MultiTailQueueBenchmarker<int>("MultiTailQueue.Enqueue", i,
                    (_, mtqb) => {
                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                            mtqb.Queue.Enqueue(100);
                        }});

                qb.Run();
            });

            // 2. MultiTailQueue.TryDequeue
            Benchmarker.RunMany(threads, i => {
                var qb = new MultiTailQueueBenchmarker<int>("MultiTailQueue.TryDequeue", i,
                    mtqb => {
                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                            mtqb.Queue.Enqueue(100);
                        }
                    }, (_, mtqb) => {
                        int k;

                        for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / mtqb.NumThreads; ++j) {
                            mtqb.Queue.TryDequeue(out k);
                        }
                    });

                qb.Run();
            });
        }
    }
}
