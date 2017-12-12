# ConcurrentQueueBenchmarking

A small framework for benchmarking different queue implementations. Currently we
benchmark `ConcurrentQueue` and `MultiTailQueue` in different scenarios:

* Threads enqueue a bunch of items
* Threads dequeue a bunch of items
* Threads enqueue a bunch of items while on thread continuously dequeues items
