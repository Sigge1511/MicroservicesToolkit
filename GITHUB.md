# GitHub Upload Guide

## Ladda upp till GitHub

### Steg 1: Skapa ett nytt repository på GitHub

1. Gå till https://github.com/new
2. Namnge ditt repo (t.ex. "microservices-toolkit")
3. Lägg INTE till README, .gitignore eller license (vi har redan dessa)
4. Klicka "Create repository"

### Steg 2: Initiera git i projektet

Öppna en terminal i projektets root-mapp och kör:

```bash
cd MicroservicesToolkit
git init
git add .
git commit -m "Initial commit: Complete microservices toolkit"
```

### Steg 3: Pusha till GitHub

Ersätt `YOUR_USERNAME` och `YOUR_REPO` med dina värden:

```bash
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git
git branch -M main
git push -u origin main
```

### Alternativ: Använd SSH

Om du använder SSH keys:

```bash
git remote add origin git@github.com:YOUR_USERNAME/YOUR_REPO.git
git branch -M main
git push -u origin main
```

## Uppdatera senare

När du gör ändringar:

```bash
git add .
git commit -m "Beskrivning av ändringar"
git push
```

## Tips för GitHub

### Lägg till en bra beskrivning

Gå till ditt repo på GitHub och lägg till:
- **Description**: "Microservices toolkit built with .NET 10, featuring Product, Order, and Logging services with API Gateway and modern web UI"
- **Topics**: dotnet, microservices, csharp, api-gateway, docker, entity-framework, xunit, razor-pages

### Skapa en bra README preview

Ditt README.md kommer automatiskt visas på GitHub's framsida.

### GitHub Actions (Optional)

Skapa `.github/workflows/dotnet.yml` för CI/CD:

```yaml
name: .NET

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

Detta kommer automatiskt bygga och testa ditt projekt vid varje push!

## Klart!

Nu är ditt projekt på GitHub och redo att delas med världen! 🎉
