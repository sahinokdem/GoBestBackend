using GoBest.Routes;
using Microsoft.Extensions.Hosting;

public class ServiceApiBackgroundJob : BackgroundService
{
    private readonly ApiService _serviceApiService;

    public ServiceApiBackgroundJob(ApiService serviceApiService)
    {
        _serviceApiService = serviceApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceApiService.StartHourlyRequestsAsync(stoppingToken);
    }
}
