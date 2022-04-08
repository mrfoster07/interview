using System.Diagnostics;
using Disruptor;
using DisruptorInCalc.Domain.DTOs;
using DisruptorInCalc.Domain.Helpers;

namespace DisruptorInCalc.Domain.Handlers
{
    internal class CalculationRequestHandler : IWorkHandler<CalculationRequestEventDto>
    {
        private readonly IMetricsService metricsService;

        public CalculationRequestHandler(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public void OnEvent(CalculationRequestEventDto evt)
        {
            var stopwatch = Stopwatch.StartNew();

            var parsedRequest = ParseHelper.Parse(evt.RawRequest);
            var calculationResult =
                CalculationHelper.RunCalculation(parsedRequest.FuncName, parsedRequest.ArgA, parsedRequest.ArgB);

            Console.WriteLine(
                $"Operation Id: {parsedRequest.Id}, Code: {calculationResult.Code}, Result: {calculationResult.Value}");

            stopwatch.Stop();

            metricsService.AddMetrics(
                $"Operation Id: {parsedRequest.Id}, Duration (ms): {stopwatch.ElapsedMilliseconds}");
        }
    }
}