using System.Collections;

namespace DisruptorInCalc.Domain;

internal interface IMetricsService
{
    void AddMetrics(string data);
    IEnumerable ExtractMetrics();
    bool HasAny();
}