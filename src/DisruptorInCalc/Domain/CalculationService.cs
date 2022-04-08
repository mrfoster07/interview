using Disruptor.Dsl;
using DisruptorInCalc.Domain.DTOs;
using DisruptorInCalc.Domain.Handlers;

namespace DisruptorInCalc.Domain
{
    internal class CalculationService
    {
        private const int RingBufferSize = 1024;
        private readonly int timeOutMilliseconds = 8000;
        private readonly Disruptor<CalculationRequestEventDto> _disruptor;
        private readonly IMetricsService _metricsService;

        private CalculationService()
        {
            _metricsService = addCalculationMetrics();
            _disruptor = addDisruptor(_metricsService);
        }

        public static CalculationService BuildService()
        {
            return new CalculationService();
        }

        public void ProcessCalculationRequest(string[] rawRequest)
        {
            if (rawRequest.Length == 0)
            {
                return;
            }

            using var scope = _disruptor.PublishEvents(rawRequest.Length);
            for (var index = 0; index < rawRequest.Length; index++)
            {
                var data = scope.Event(index);
                data.RawRequest = rawRequest[index];
            }
        }

        private MetricsService addCalculationMetrics()
        {
            return MetricsService
                .BuildService(timeOutMilliseconds)
                .RunOnMetricsCollected(m =>
                {
                    if (m.HasAny())
                    {
                        Console.WriteLine("\n- Metrics:");
                        foreach (var metrics in m.ExtractMetrics())
                        {
                            Console.WriteLine(metrics);
                        }
                        Console.WriteLine();
                    }
                })
                .Start();
        }

        private Disruptor<CalculationRequestEventDto> addDisruptor(IMetricsService metricsService)
        {
            var disruptor = new
                Disruptor<CalculationRequestEventDto>(() => new CalculationRequestEventDto(), RingBufferSize);

            disruptor
                .HandleEventsWithWorkerPool(new SleepingThreadHandler())
                .ThenHandleEventsWithWorkerPool(new CalculationRequestHandler(metricsService));

            disruptor.Start();

            return disruptor;
        }
    }
}