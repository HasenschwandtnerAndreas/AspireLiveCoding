var builder = DistributedApplication.CreateBuilder(args);

var carApi = builder.AddProject<Projects.AspireLiveCoding_CarApi>("carapi");
builder.AddProject<Projects.AspireLiveCoding_CarWorker>("carworker")
    .WithReference(carApi);

builder.Build().Run();
