# Mikroservice Toolkit - Kom Igång

## Snabbstart med Docker (Rekommenderat)

Detta är det enklaste sättet att köra hela systemet:

```bash
# 1. Navigera till projektet
cd MicroservicesToolkit

# 2. Starta allt med Docker Compose
docker-compose up --build

# 3. Öppna webbgränssnittet
http://localhost:5004
```

Klart! Alla services körs nu:
- Web UI: http://localhost:5004
- Product Service: http://localhost:5001/swagger
- Order Service: http://localhost:5002/swagger
- Logging Service: http://localhost:5003/swagger
- API Gateway: http://localhost:5000

## Lokal utveckling utan Docker

### Steg 1: Sätt upp SQL Server

Du behöver en SQL Server-instans. Enklast är att använda Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password123" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Eller använd din lokala SQL Server installation.

### Steg 2: Uppdatera connection string

Öppna `src/LoggingService/appsettings.json` och ändra connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MicroservicesLogging;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
  }
}
```

### Steg 3: Starta services i rätt ordning

**Terminal 1 - LoggingService (måste startas först):**
```bash
cd src/LoggingService
dotnet run
```
Vänta tills den säger "Application started"

**Terminal 2 - ProductService:**
```bash
cd src/ProductService
dotnet run
```

**Terminal 3 - OrderService:**
```bash
cd src/OrderService
dotnet run
```

**Terminal 4 - API Gateway (valfritt):**
```bash
cd src/ApiGateway
dotnet run
```

**Terminal 5 - Web UI:**
```bash
cd src/Web
dotnet run
```

### Steg 4: Testa systemet

1. Öppna http://localhost:5004 i din webbläsare
2. Klicka på "Products" för att se produkter
3. Gå till "Orders" och skapa en order
4. Kolla "Logs" för att se all aktivitet

## Kör tester

Kör alla tester:
```bash
dotnet test
```

Kör specifika test-projekt:
```bash
dotnet test tests/ProductService.Tests
dotnet test tests/OrderService.Tests
dotnet test tests/LoggingService.Tests
```

## Vanliga problem och lösningar

### "Database connection failed"
**Problem:** LoggingService kan inte ansluta till databasen

**Lösning:**
1. Kolla att SQL Server körs
2. Verifiera connection string i `src/LoggingService/appsettings.json`
3. Testa anslutningen manuellt med SQL Server Management Studio eller Azure Data Studio

### "Port already in use"
**Problem:** Porten är redan upptagen

**Lösning:**
Ändra port i `launchSettings.json` eller kör med custom port:
```bash
dotnet run --urls "http://localhost:5005"
```

### Services kan inte prata med varandra
**Problem:** OrderService hittar inte ProductService

**Lösning:**
1. Kolla att alla services körs
2. Verifiera service URLs i `appsettings.json`
3. Kontrollera att rätt portar används

### Swagger öppnas inte
**Problem:** Swagger UI visas inte

**Lösning:**
Navigera manuellt till: http://localhost:PORT/swagger

## Struktur

```
MicroservicesToolkit/
├── src/                          # Källkod
│   ├── ProductService/          # Produkter (Port 5001)
│   ├── OrderService/            # Ordrar (Port 5002)
│   ├── LoggingService/          # Loggar (Port 5003)
│   ├── ApiGateway/              # Gateway (Port 5000)
│   └── Web/                     # Webbgränssnitt (Port 5004)
├── tests/                        # Tester
│   ├── ProductService.Tests/
│   ├── OrderService.Tests/
│   └── LoggingService.Tests/
└── docker-compose.yml            # Docker konfiguration
```

## Nästa steg

1. **Utforska API:erna** - Öppna Swagger för varje service
2. **Kolla designen** - Webbgränssnittet har en cool monokrom design
3. **Skapa ordrar** - Testa error handling genom att använda ogiltiga product IDs
4. **Följ loggarna** - Se hur alla services kommunicerar

## Tweak och anpassa

Detta projekt är byggt för att vara enkelt att tweaka:

- **Lägg till nya produkter** - Ändra listan i `ProductService/Controllers/ProductsController.cs`
- **Ändra designen** - Redigera `Web/wwwroot/css/site.css`
- **Lägg till endpoints** - Skapa nya controllers i respektive service
- **Utöka databasen** - Lägg till migrations i LoggingService

## Support

Om något inte fungerar:
1. Kolla README.md för mer detaljerad info
2. Verifiera att alla prerequisites är installerade
3. Testa att starta services en och en för att hitta problemet

Ha kul med ditt mikroservice toolkit! 🚀
