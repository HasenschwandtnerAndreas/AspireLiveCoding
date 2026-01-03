using System.Net.Http.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient("cars", client => client.BaseAddress = new Uri("http://carapi"));
builder.Services.AddHostedService<CarServiceWorker>();

var host = builder.Build();

await host.RunAsync();

record Car(Guid Id, string Make, string Model, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

sealed class CarServiceWorker : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CarServiceWorker> _logger;

    public CarServiceWorker(IHttpClientFactory httpClientFactory, ILogger<CarServiceWorker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(4));
        var client = _httpClientFactory.CreateClient("cars");

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var cars = await client.GetFromJsonAsync<List<Car>>("/cars", cancellationToken: stoppingToken);
                var next = cars?.FirstOrDefault(c => c.Status == "new");
                if (next is null)
                {
                    continue;
                }

                _logger.LogInformation("Servicing car {CarId} {Make} {Model}", next.Id, next.Make, next.Model);

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                await client.PostAsync($"/cars/{next.Id}/service", content: null, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Car service loop failed; retrying.");
            }
        }
    }
}
