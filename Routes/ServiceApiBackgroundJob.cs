using GoBest.Routes;

public class ServiceApiBackgroundJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceApiBackgroundJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var apiService = scope.ServiceProvider.GetRequiredService<ApiService>();

            await apiService.StartHourlyRequestsAsync(stoppingToken);
        }
    }
}
