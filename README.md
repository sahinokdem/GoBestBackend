# 🧭 GoBest — Multi-Modal Travel Reservation Backend

> 🇹🇷 Türkçe için: [README.tr.md](README.tr.md)

GoBest is the backend of a travel platform that brings **flights, buses, and trains** together in a single interface. When a user searches from one city to another and there is no direct service, the system **automatically builds multi-leg transfer routes** and returns the most suitable options.

> In short: it turns the classic "single mode, direct trips only" approach into a multi-modal system that composes transfer routes, caches results, and is designed for scale.

---

## 🚀 Why This Project?

Most reservation systems handle a single transport mode and only direct trips. GoBest aims to:

- Combine flight / bus / train services into **one unified list**
- Build a **transfer route on the fly** when no direct trip exists (or a cheaper one is possible)
- Use **smart caching** instead of recomputing the same search repeatedly
- Keep the active dataset light through a **scalable database design**

So the project is an **end-to-end system design** covering the data model, external service integration, the route-search algorithm, and the booking flow together.

---

## 🧠 Quick Explanation (Non-Technical)

When a user searches "Istanbul → Berlin", the system:

1. Collects all services for that day (flight, bus, train)
2. Returns a direct trip if one exists; otherwise builds a **transfer route** (e.g. Istanbul → Frankfurt → Berlin)
3. Evaluates these routes by price / duration / number of transfers
4. If the same search was made before, returns it **instantly from cache** instead of recomputing

**Result:** faster responses, no redundant computation, and a structure that won't slow down as it grows.

---

## 🏗️ Architecture Flow

```
[Route API (Python/FastAPI)]  ──►  service data (flight / bus / train)
            │  (backend periodically fetches and stores it)
            ▼
┌─────────────────────────────────────────────┐
│              .NET Core Backend               │
│                                              │
│   Search   ─►  constrained BFS over a city   │
│                graph  ─►  itinerary list     │
│                                              │
│   Rerank   ─►  results sent to GoBestModel,  │
│                reordered by relevance        │
│                                              │
│   Cache    ─►  generated itineraries stored  │
│                in DB, served on repeat       │
│                                              │
│   Booking  ─►  a selected itinerary becomes  │
│                a reservation                 │
└─────────────────────────────────────────────┘
            │                         │
            ▼                         ▼
     [PostgreSQL]            [GoBestModel — ML service]
  itinerary + leg model      LightGBM reranking
   (designed for scale)      (separate repo)
```

- **API Layer:** ASP.NET Core (.NET 8)
- **Auth:** JWT + role-based access (Customer / Maintainer / Admin)
- **Database:** PostgreSQL + Entity Framework Core (database-first)
- **External Data:** Route API (Python / FastAPI)
- **ML Reranking:** separate service → [GoBestModel](https://github.com/sahinokdem/GoBestModel)

---

## 👥 Roles

- **Customer** — search, filter, view route details, book tickets, view past bookings
- **Company Maintainer** — manage their own company's services, schedules, and seat info
- **Admin** — create maintainer accounts, complete missing city / station / company data coming from the external API

---

## ⚡ Hard Parts & Engineering Solutions

### 1) Transfer Route Generation (Endless / Nonsensical Routes)

**Problem:** A direct service between two cities doesn't always exist, so transfers must be composed — but a naive approach produces endless or nonsensical routes (10-leg trips, or connections that depart before the previous leg arrives).

**Solution:** A **BFS over a city graph, pruned with real-world constraints**:

- **Max 2 transfers** (3 legs) — prevents endless chains
- **Minimum transfer buffer** — enough time between a leg's arrival and the next leg's departure (realistic connections)
- **Same-day horizon** — the whole route must fit a sensible time window
- Expands every **seat-type combination** across legs (Eco–Eco, Eco–Bus, …) as independent options

### 2) Separating Search Routes from Bookings

**Problem:** The same composed route plays two very different roles. As a *search result* it's disposable — if nobody buys it, it's just clutter once its date passes. As a *purchase* it must be kept for the user's history. Treating both the same way either bloats the database with dead search data or risks losing real bookings.

**Solution:** The model splits this into two structures with different lifecycles, keyed off a `sold` flag on each `service`:

- **Itinerary + ItineraryLegs** → a generated/cached search result. The `Itinerary` holds the summary (total price, duration, transfer count); each `ItineraryLeg` holds one segment (order, service, seat type, price). Cached and re-served for repeated searches.
- **Booking + BookingLegs** → a purchase. The `Booking` references the itinerary it was made from, and each `BookingLeg` records the purchased segment (service, seat type).

The lifecycle rule is what ties it together:

- A service that is **never sold** (`sold = false`), along with any itineraries/legs generated from it, is **deleted once its date passes** — dead search data doesn't accumulate.
- A service that **is sold** (`sold = true`), along with its itinerary, legs, and booking, is **never deleted** while live. Because a booking always implies a sold service, the itinerary behind it is never a deletion target — so the user's history stays intact without needing any cascade trickery.

This is why `BookingLeg` stores its own `service` and `seat_type` references: a booking is self-describing and doesn't depend on the search-side records surviving.

**Why price lives on the itinerary, not the booking leg:** per-segment price sits on `ItineraryLeg`, and the booking keeps the agreed total on `Booking.TotalPrice`. This avoids duplicating pricing and keeps the booking leg focused on *what* was purchased (service + seat type).

### 3) Redundant Computation (Performance)

**Problem:** Hundreds of users search the same popular route (e.g. Istanbul–Ankara). Recomposing it every time is wasteful.

**Solution:** A **cache-aside** strategy. Generated itineraries are persisted to the DB; when the same route + date is searched again, it's served directly from the database without recomposition. Target: **search ≤ 3 seconds**.

### 4) Dynamic Pricing & Seat Availability

**Problem:** In a multi-leg route, each service has different seat types and price multipliers, and there must be enough free seats for the requested passenger count.

**Solution:** Pricing is computed dynamically (base price × passengers, with seat-type multipliers). Before results are returned, **seat availability is validated** against the requested passenger count, so invalid options are never shown.

### 5) Smarter Ranking Than Price Alone

**Problem:** Sorting purely by lowest price ignores what users actually prefer (duration, transfers, company, past behavior).

**Solution:** Search results are passed to a separate ML service, [GoBestModel](https://github.com/sahinokdem/GoBestModel), which **reranks them by predicted relevance** using a LightGBM model trained on interaction features (clicks / bookings).

---

## 📈 Designed for Scale

The system was designed from the database layer up with growth in mind. The `sold` flag on each service drives a two-track data lifecycle that keeps the active database small while preserving everything a user actually bought.

![Database schema (ER diagram)](docs/er-diagram.png)

The schema is built for this; the cleanup and partitioning jobs themselves are the next implementation phase (designed, not yet implemented):

- **Database-first design:** Schema, relationships, and constraints were modeled before most of the application logic, with indexing and query shape considered early.
- **Cleanup of unsold data (planned):** Unsold services past their date — and the itineraries/legs generated from them — are designed to be purged automatically, so dead search data never piles up in the hot tables.
- **Partitioning for sold data (planned):** Sold services and everything tied to them (itinerary, legs, booking) are designed to be moved out of the hot path via partitioning, then dropped entirely after a long retention window (e.g. 1–3 years).

The goal throughout is performance and disk efficiency: keep the live dataset to what's actually active, archive what was purchased, and eventually retire the very old.

---

## 🧩 Module Structure

```
Auth/        – authentication & JWT
Users/       – user accounts & roles
Companies/   – transport companies
Stations/    – cities & stations
Routes/      – external service data & fetching
Itinaries/   – route search (BFS) & itinerary generation
Seats/       – seat types & inventory
Bookings/    – reservations
```

---

## 🛠️ Local Setup

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Configure the database connection and Route API URL in `appsettings.Development.json`.
