# ConfReserve

Aplikacja do rezerwacji sal konferencyjnych — **ASP.NET Core 8 MVC**, EF Core 8, SQL Server, ASP.NET Core Identity.

Dwie role: `User` (przegląda sale, składa i anuluje rezerwacje) oraz `Admin` (CRUD sal, zatwierdza/odrzuca rezerwacje, widzi statystyki i przychody).

---

## Uruchomienie

Wymagania: .NET 8 SDK, SQL Server LocalDB.

```bash
git clone https://github.com/<your-username>/ConfReserve.git
cd ConfReserve/ConfReserve
dotnet ef database update
dotnet run
```

Konto admina tworzone automatycznie przy starcie:
- **Login:** `admin@confreserve.com`
- **Hasło:** `Admin123!`

---

## Stos technologiczny

| | |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Baza danych | SQL Server LocalDB |
| Autentykacja | ASP.NET Core Identity |
| Frontend | Razor Views, Bootstrap 5, jQuery |

---

## Model danych

Trzy encje: `AppUser` (rozszerzony `IdentityUser`), `ConferenceRoom`, `Reservation`.  
Relacje: `AppUser 1:N Reservation`, `ConferenceRoom 1:N Reservation` (kaskadowe usuwanie).  
Statusy rezerwacji: `Pending` → `Approved` / `Cancelled`.