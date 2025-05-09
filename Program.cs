using BlazorApptest2.Components;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using System.Diagnostics.Metrics;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

var meter = new Meter("BlazorTestApp.Metrics", "1.0");
builder.Services.AddSingleton<InstrumentationService>();


var otel = builder.Services.AddOpenTelemetry();


Log.Logger = new LoggerConfiguration()
        .WriteTo.OpenTelemetry(
            endpoint: "http://localhost:4317",
            protocol: Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc
        )
        .CreateLogger();


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => 
		resource.AddService(serviceName: "SampleNetApp"))
        
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://localhost:4317");

            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        }));
        otel.WithMetrics(metrics => metrics
         // Metrics provider from OpenTelemetry
    .AddAspNetCoreInstrumentation()
    
    // Metrics provides by ASP.NET Core in .NET 8
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    // Metrics provided by System.Net libraries
    .AddMeter("System.Net.Http")
    .AddMeter("System.Net.NameResolution")
    .AddMeter("BlazorTestApp.Metrics")


    .AddMeter(InstrumentationService.MeterName)

    .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://localhost:4317");

            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        }));
        builder.Logging.AddOpenTelemetry(logging => {
           logging.AddOtlpExporter(options =>
    {
            options.Endpoint = new Uri("http://localhost:4317");

            options.Protocol = OtlpExportProtocol.Grpc;
    });
    logging.AddOtlpExporter();
});
    
    
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
