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

            // 3. Multiple threads enqueue, one thread dequeues
            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue (1 thread dequeues)", i,
                    (id, cqb) => {
                        if (id == cqb.NumThreads - 1) {
                            // Dequeuer thread
                            int k;

                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                                cqb.Queue.TryDequeue(out k);
                            }
                        } else {
                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / (cqb.NumThreads - 1); ++j) {
                                cqb.Queue.Enqueue(100);
                            }
                        }
                    });

                qb.Run();
            });

            // 4. Half of threads enqueues, half of threads dequeues.
            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("ConcurrentQueue (half dequeues)", i,
                    (id, cqb) => {
                        if (id <= cqb.NumThreads / 2) {
                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / (cqb.NumThreads / 2); ++j) {
                                cqb.Queue.Enqueue(100);
                            }
                        } else {
                            // Dequeuer thread
                            int k;

                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                                cqb.Queue.TryDequeue(out k);
                            }
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

            // 3. Multiple threads enqueue, one thread dequeues
            Benchmarker.RunMany(threads, i => {
                var qb = new MultiTailQueueBenchmarker<int>("MultiTailQueue (1 thread dequeues)", i,
                    (id, cqb) => {
                        if (id == cqb.NumThreads - 1) {
                            // Dequeuer thread
                            int k;

                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                                cqb.Queue.TryDequeue(out k);
                            }
                        } else {
                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / (cqb.NumThreads - 1); ++j) {
                                cqb.Queue.Enqueue(100);
                            }
                        }
                    });

                qb.Run();
            });

                        // 4. Half of threads enqueues, half of threads dequeues.
            Benchmarker.RunMany(threads, i => {
                var qb = new ConcurrentQueueBenchmarker<int>("MultiTailQueue (half dequeues)", i,
                    (id, cqb) => {
                        if (id <= cqb.NumThreads / 2) {
                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD / (cqb.NumThreads / 2); ++j) {
                                cqb.Queue.Enqueue(100);
                            }
                        } else {
                            // Dequeuer thread
                            int k;

                            for (int j = 0; j < QUEUE_ACTIONS_PER_THREAD; ++j) {
                                cqb.Queue.TryDequeue(out k);
                            }
                        }
                    });

                qb.Run();
            });
        }
    }
}
