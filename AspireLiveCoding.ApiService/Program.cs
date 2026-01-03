using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

var store = new OrderStore();

app.MapPost("/orders", (CreateOrderRequest request) => Results.Ok(store.Create(request.Item)));
app.MapGet("/orders", () => Results.Ok(store.List()));
app.MapGet("/orders/{id:guid}", (Guid id) => store.TryGet(id, out var order)
    ? Results.Ok(order)
    : Results.NotFound());
app.MapPost("/orders/next", () => store.TryClaimNext(out var order)
    ? Results.Ok(order)
    : Results.NoContent());
app.MapPost("/orders/{id:guid}/complete", (Guid id) => store.TryComplete(id, out var order)
    ? Results.Ok(order)
    : Results.NotFound());

app.MapDefaultEndpoints();

app.Run();

record CreateOrderRequest(string Item);

record Order(Guid Id, string Item, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

sealed class OrderStore
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();
    private readonly ConcurrentQueue<Guid> _pendingQueue = new();

    public Order Create(string item)
    {
        var order = new Order(Guid.NewGuid(), item, "pending", DateTimeOffset.UtcNow, null);
        _orders[order.Id] = order;
        _pendingQueue.Enqueue(order.Id);
        return order;
    }

    public IReadOnlyCollection<Order> List() => _orders.Values.OrderBy(o => o.CreatedAt).ToArray();

    public bool TryGet(Guid id, out Order order) => _orders.TryGetValue(id, out order!);

    public bool TryClaimNext(out Order order)
    {
        while (_pendingQueue.TryDequeue(out var id))
        {
            if (_orders.TryGetValue(id, out var existing) && existing.Status == "pending")
            {
                order = existing with { Status = "processing", UpdatedAt = DateTimeOffset.UtcNow };
                _orders[id] = order;
                return true;
            }
        }

        order = default!;
        return false;
    }

    public bool TryComplete(Guid id, out Order order)
    {
        if (_orders.TryGetValue(id, out var existing))
        {
            order = existing with { Status = "done", UpdatedAt = DateTimeOffset.UtcNow };
            _orders[id] = order;
            return true;
        }

        order = default!;
        return false;
    }
}
