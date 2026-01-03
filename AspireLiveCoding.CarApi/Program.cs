using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

var store = new CarStore();

app.MapPost("/cars", (CreateCarRequest request) => Results.Ok(store.Create(request.Make, request.Model)));
app.MapGet("/cars", () => Results.Ok(store.List()));
app.MapGet("/cars/{id:guid}", (Guid id) => store.TryGet(id, out var car)
    ? Results.Ok(car)
    : Results.NotFound());
app.MapPost("/cars/{id:guid}/service", (Guid id) => store.TryService(id, out var car)
    ? Results.Ok(car)
    : Results.NotFound());

app.MapDefaultEndpoints();

app.Run();

record CreateCarRequest(string Make, string Model);

record Car(Guid Id, string Make, string Model, string Status, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

sealed class CarStore
{
    private readonly ConcurrentDictionary<Guid, Car> _cars = new();

    public Car Create(string make, string model)
    {
        var car = new Car(Guid.NewGuid(), make, model, "new", DateTimeOffset.UtcNow, null);
        _cars[car.Id] = car;
        return car;
    }

    public IReadOnlyCollection<Car> List() => _cars.Values.OrderBy(c => c.CreatedAt).ToArray();

    public bool TryGet(Guid id, out Car car) => _cars.TryGetValue(id, out car!);

    public bool TryService(Guid id, out Car car)
    {
        if (_cars.TryGetValue(id, out var existing))
        {
            car = existing with { Status = "serviced", UpdatedAt = DateTimeOffset.UtcNow };
            _cars[id] = car;
            return true;
        }

        car = default!;
        return false;
    }
}
