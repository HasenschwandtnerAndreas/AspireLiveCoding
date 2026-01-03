using System.Net.Http.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient("orders", client => client.BaseAddress = new Uri("http://apiservice"));
builder.Services.AddHostedService<OrderProcessor>();

var host = builder.Build();

await host.RunAsync();

record Order(Guid Id, string Item, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

sealed class OrderProcessor : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrderProcessor> _logger;

    public OrderProcessor(IHttpClientFactory httpClientFactory, ILogger<OrderProcessor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        var client = _httpClientFactory.CreateClient("orders");

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var response = await client.PostAsync("/orders/next", content: null, stoppingToken);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var order = await response.Content.ReadFromJsonAsync<Order>(cancellationToken: stoppingToken);
                if (order is null)
                {
                    continue;
                }

                _logger.LogInformation("Processing order {OrderId} for {Item}", order.Id, order.Item);

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                await client.PostAsync($"/orders/{order.Id}/complete", content: null, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Order processing loop failed; retrying.");
            }
        }
    }
}
