// Create the distributed application builder that wires up resources and services.
var builder = DistributedApplication.CreateBuilder(args);

// Add a Postgres container resource named "postgres".
var postgres = builder.AddPostgres("postgres")
    // Persist data in a named volume so it survives restarts.
    .WithDataVolume("postgres-data");
// Define a logical database named "cardb" within the Postgres resource.
var carDb = postgres.AddDatabase("cardb");

// Add a Redis container resource for caching/demo purposes.
var redis = builder.AddRedis("redis");
// Add a RabbitMQ container resource for messaging/demo purposes.
var rabbitMq = builder.AddRabbitMQ("rabbitmq");
// Add a Seq container resource for centralized logging/tracing demos.
var seq = builder.AddSeq("seq");

// Add a Keycloak container resource for identity provider demos.
var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak:24.0.5")
    // Configure admin credentials for local development.
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    // Expose the Keycloak HTTP endpoint on port 8080.
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    // Start Keycloak in dev mode for quick setup.
    .WithArgs("start-dev");

// Add the Car API project to the app host.
var carApi = builder.AddProject<Projects.AspireLiveCoding_CarApi>("carapi")
    // Inject the Postgres database connection string into the Car API.
    .WithReference(carDb)
    // Inject Redis connection details into the Car API.
    .WithReference(redis)
    // Inject RabbitMQ connection details into the Car API.
    .WithReference(rabbitMq)
    // Point the Car API at the Seq instance for logs/traces.
    .WithReference(seq);
// Add the Car Worker project and connect it to the Car API.
builder.AddProject<Projects.AspireLiveCoding_CarWorker>("carworker")
    // Allow the worker to resolve the Car API via service discovery.
    .WithReference(carApi);

// Build the distributed application model and run it.
builder.Build().Run();
