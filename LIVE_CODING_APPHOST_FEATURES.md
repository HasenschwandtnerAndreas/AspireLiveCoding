# Aspire AppHost Showcase (ca. 60-75 Minuten)

Ziel: Ein AppHost mit vielen Features (Postgres, Redis, RabbitMQ, Seq, Keycloak), damit man im Live Coding viel zeigen kann.

## Inhalte im AppHost

1) Postgres
- Resource: `postgres` + Database `cardb`
- Persistente Daten via Volume `postgres-data`
- Nutzen: Connection String wird automatisch in abh. Services injiziert

2) Redis
- Resource: `redis`
- Nutzen: Cache / verteilte Daten, Connection String per Service Discovery

3) RabbitMQ
- Resource: `rabbitmq`
- Nutzen: Messaging, Demo mit Queue/Exchange

4) Seq
- Resource: `seq`
- Nutzen: zentrales Log/Tracing Ziel fuer Demo der Observability

5) Keycloak
- Resource: `keycloak` (Container)
- Default Admin: `admin` / `admin`
- Nutzen: Identity Provider fuer Login/Token Demos

6) App Services
- `carapi` + `carworker` als einfache Demo-Services
- `carapi` bekommt Referenzen auf Postgres/Redis/RabbitMQ/Seq (Connection Strings kommen automatisch)

## Live Coding Ablauf (Vorschlag)

1) AppHost bauen (10 min)
- Resources anlegen (Postgres, Redis, RabbitMQ, Seq, Keycloak)
- `carapi` + `carworker` referenzieren

2) Dashboard zeigen (10 min)
- Services und Ressourcen sichtbar
- Ports/Endpoints anschauen
- Logs pro Service anzeigen

3) Postgres zeigen (10 min)
- `cardb` in der Resource-Liste
- Connection String in `carapi` (Environment in Dashboard)

4) Redis / RabbitMQ zeigen (10 min)
- Resource-Details, Ports, Logs
- Erklaeren, wie man das in Services einbindet

5) Seq zeigen (5 min)
- Logs aus `carworker` und `carapi` im Seq-UI

6) Keycloak zeigen (10 min)
- Admin-Login, Realm anlegen, Users zeigen
- Erklaeren, wie Token in API genutzt werden koennen

## Starten

```
dotnet run --project AspireLiveCoding.AppHost
```

- Dashboard-URL in der Konsole oeffnen
- Ports fuer `carapi`, `seq`, `keycloak` nachsehen

## Demo Requests (Car API)

```
POST http://localhost:{carApiPort}/cars
{
  "make": "Audi",
  "model": "A3"
}
```

```
GET http://localhost:{carApiPort}/cars
```
