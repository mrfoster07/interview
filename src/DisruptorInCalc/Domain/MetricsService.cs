using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor.Dsl;
using DisruptorInCalc.Domain.DTOs;
using DisruptorInCalc.Domain.Handlers;

namespace DisruptorInCalc.Domain
{
    internal class MetricsService : IMetricsService, IDisposable
    {
        private readonly ConcurrentQueue<string> metricsQueue;
        private readonly int timeOutMilliseconds;

        private readonly Task metricBitTask;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly CancellationToken metricBitToken;
        private const int MetricBitDelay = 1000;

        private Action<IMetricsService> onMetricsCollected;
        private object onMetricsCollectedSyncObj = new object();
        private object startSyncObj = new object();

        private MetricsService(int timeOutMilliseconds)
        {
            this.metricsQueue = new ConcurrentQueue<string>();
            this.timeOutMilliseconds = timeOutMilliseconds;
            this.metricBitToken = cancellationTokenSource.Token;
            this.metricBitTask = createMetricBitTask();
        }

        public static MetricsService BuildService(int timeOutMilliseconds)
        {
            return new MetricsService(timeOutMilliseconds);
        }

        public MetricsService RunOnMetricsCollected(Action<IMetricsService> action)
        {
            lock (onMetricsCollectedSyncObj)
            {
                this.onMetricsCollected += action;
            }

            return this;
        }

        public MetricsService Start()
        {
            lock (startSyncObj)
            {
                if (metricBitTask.Status == TaskStatus.Created)
                {
                    metricBitTask.Start();
                }
            }

            return this;
        }

        public void AddMetrics(string data)
        {
            metricsQueue.Enqueue(data);
        }

        public IEnumerable ExtractMetrics()
        {
            while (metricsQueue.TryDequeue(out var localValue)) yield return localValue;
        }

        public bool HasAny()
        {
            return !metricsQueue.IsEmpty;
        }

        private Task createMetricBitTask()
        {
            return new Task(async () =>
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    do
                    {
                        if (stopwatch.ElapsedMilliseconds > timeOutMilliseconds)
                        {
                            stopwatch.Restart();
                            try
                            {
                                onMetricsCollected.Invoke(this);
                            } catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }

                        await Task.Delay(MetricBitDelay, metricBitToken);
                    } while (!metricBitToken.IsCancellationRequested);
                } catch (OperationCanceledException)
                {
                    Console.WriteLine("MetricBit task is canceled");
                }
            }, metricBitToken);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}