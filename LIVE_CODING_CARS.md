# Aspire Live Coding: Cars (ca. 60 Minuten)

Ziel: Ein kleines Aspire-Beispiel mit Car API + Worker, die per Service Discovery sprechen.
Die API legt Autos an, der Worker "serviced" regelmaessig das naechste Auto.

## Ablauf (Vorschlag)

1) Intro (5 min)
- AppHost + ServiceDefaults erklaeren
- Was zeigt Aspire? Orchestrierung, Service Discovery, Telemetrie, Health

2) Services anlegen (10 min)
- Web API: `AspireLiveCoding.CarApi`
- Worker: `AspireLiveCoding.CarWorker`
- Beide referenzieren `AspireLiveCoding.ServiceDefaults`

3) API implementieren (15 min)
- Minimal API mit Endpunkten:
  - `POST /cars`
  - `GET /cars`
  - `GET /cars/{id}`
  - `POST /cars/{id}/service`
- In-Memory Store mit Dictionary

4) Worker implementieren (10 min)
- BackgroundService mit PeriodicTimer
- Holt `/cars`, sucht `status == new`, ruft `/service` auf

5) AppHost wiring (5 min)
- `AddProject` fuer API + Worker
- Worker bekommt `WithReference(carApi)`

6) Demo & Observability (10-15 min)
- `dotnet run --project AspireLiveCoding.AppHost`
- Dashboard oeffnen
- Cars posten, Worker-Logs anschauen

## Demo Requests

```
POST http://localhost:{carApiPort}/cars
{
  "make": "Volkswagen",
  "model": "Golf"
}
```

```
GET http://localhost:{carApiPort}/cars
```

Tipp: Port im Aspire-Dashboard nachsehen.
