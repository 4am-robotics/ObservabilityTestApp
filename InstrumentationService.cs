using System.Diagnostics;
using System.Diagnostics.Metrics;

public class InstrumentationService : IDisposable
{
    internal const string ActivitySourceName = "Metric";
    internal const string MeterName = "Metric";
    private readonly Meter meter;

    public InstrumentationService()
    {
        string? version = typeof(InstrumentationService).Assembly.GetName().Version?.ToString();
       this.ActivitySource = new ActivitySource(ActivitySourceName, version);
       this.meter = new Meter(MeterName, version);
       this.clickCounter = this.meter.CreateCounter<long>("clickCounter", description: "The count of the clicks");
       this.clickCounter.Add(1);
    }

    public ActivitySource ActivitySource { get; }

    public Counter<long> clickCounter { get; }

    public void Dispose()
    {
        this.ActivitySource.Dispose();
        this.meter.Dispose();
    }
}
