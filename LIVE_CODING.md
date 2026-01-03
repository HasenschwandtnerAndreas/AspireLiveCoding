# Aspire Live Coding (ca. 60 Minuten)

Ziel: Ein kleines Aspire-Beispiel mit Web-API + Worker, die per Service Discovery kommunizieren.
Die API nimmt Bestellungen an, der Worker verarbeitet sie periodisch.

## Ablauf (Vorschlag)

1) Intro (5 min)
- Kurzer Rundflug: `AspireLiveCoding.AppHost` und `AspireLiveCoding.ServiceDefaults`
- Was zeigt Aspire? Orchestrierung, Service Discovery, Telemetrie, Health

2) Services anlegen (10 min)
- Neues Web-API-Projekt: `AspireLiveCoding.ApiService`
- Neues Worker-Projekt: `AspireLiveCoding.Worker`
- Beide referenzieren `AspireLiveCoding.ServiceDefaults`

3) API implementieren (15 min)
- Minimal API mit Endpunkten:
  - `POST /orders` (Bestellung erstellen)
  - `GET /orders` (alle Bestellungen)
  - `POST /orders/next` (naechste pending Bestellung claimen)
  - `POST /orders/{id}/complete` (Bestellung abschliessen)
- In-Memory Store mit Queue + Dictionary

4) Worker implementieren (10 min)
- BackgroundService mit PeriodicTimer
- Holt `/orders/next`, simuliert Verarbeitung, ruft `/complete` auf

5) AppHost wiring (5 min)
- `AddProject` fuer API und Worker
- Worker bekommt `WithReference(api)`

6) Demo & Observability (10-15 min)
- `dotnet run --project AspireLiveCoding.AppHost`
- Dashboard oeffnen, Services sehen
- Orders posten, Logs anschauen

## Demo Requests

```
POST http://localhost:{apiPort}/orders
{
  "item": "Espresso"
}
```

```
GET http://localhost:{apiPort}/orders
```

Tipp: Port im Aspire-Dashboard nachsehen.

## Hinweise fuer die Live-Session
- Erst alles zeigen, dann tippen.
- Kleine Zwischenziele nennen ("jetzt die API", "jetzt der Worker").
- Immer wieder Dashboard oeffnen, um den Mehrwert von Aspire zu zeigen.
