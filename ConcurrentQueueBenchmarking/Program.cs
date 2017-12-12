using System;

namespace ConcurrentQueueBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var threads = new [] { 2, 4, 8, 16, 32 };

            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue.Enqueue", i,
                    (_, cqb) => {
                        for (int j = 0; j < 100000; ++j) {
                            cqb.Queue.Enqueue(100);
                        }});

                qb.Run();
            });

            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue.TryDequeue", i,
                    cqb => {
                        for (int j = 0; j < 100000; ++j) {
                            cqb.Queue.Enqueue(100);
                        }
                    }, (_, cqb) => {
                        int k;

                        for (int j = 0; j < 100000 / cqb.NumThreads; ++j) {
                            cqb.Queue.TryDequeue(out k);
                        }
                    });

                qb.Run();
            });

            Benchmarker.RunMany(threads, i => {
                var qb = new MultiTailQueueBenchmarker<int>("MultiTailQueue.Enqueue", i,
                    (_, mtqb) => {
                        for (int j = 0; j < 100000; ++j) {
                            mtqb.Queue.Enqueue(100);
                        }});

                qb.Run();
            });

            Benchmarker.RunMany(threads, i => {
                var qb = new MultiTailQueueBenchmarker<int>("MultiTailQueue.TryDequeue", i,
                    mtqb => {
                        for (int j = 0; j < 100000; ++j) {
                            mtqb.Queue.Enqueue(100);
                        }
                    }, (_, mtqb) => {
                        int k;

                        for (int j = 0; j < 100000 / mtqb.NumThreads; ++j) {
                            mtqb.Queue.TryDequeue(out k);
                        }
                    });

                qb.Run();
            });
        }
    }
}
