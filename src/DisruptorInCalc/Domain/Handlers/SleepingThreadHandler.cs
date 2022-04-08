using Disruptor;
using DisruptorInCalc.Domain.DTOs;

namespace DisruptorInCalc.Domain.Handlers
{
    internal class SleepingThreadHandler : IWorkHandler<CalculationRequestEventDto>
    {
        public void OnEvent(CalculationRequestEventDto evt)
        {
            Thread.Sleep(2000);
        }
    }
}