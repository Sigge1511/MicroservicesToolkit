Bygg upp ett toolkit-projekt som jag sedan kan tweaka och ladda upp på github. Det ska vara kommenterat på engelska, speciellt i de mer komplexa delarna med apierna, men kommentarer ska vara:

Korta och koncisa, inte ha några emojis eller liknande symboler, inte vara numrerade.

Jag kan själv bygga en lokal sql-databas och koppla med rätt connectionstring \- men jag vill att du preppar allt för db-kopplingen med db context och db set osv.

Jag vill även att du lägger upp ett eget projekt i denna solution där tester genomförs med främst xunit, och då vill jag ha både integrationstester och enhetstester.

Jag vill även att du bygger ett projekt med ett webbgränssnitt så man kan se/testa allt mer ux-vänligt än bara med console. Bygg då enkelt med razor pages och code behind och sedan enkel men best practice med hur detta projekt ska prata och interagera med de andra projekten i denna solution.  
Designen/css:en ska vara monokrom med svart/vitt/gråskalor, kännas modern men enkel & ux-vänlig, gärna ha lite inspo från designen från “alternative”-estitik. Den ska även vara responsive så den ser vettig ut på både bredden/desktop och på höjden/på tex mobiler. Använd moderna, smarta och nya css-lösningar.

Jag vill alltså ha en komplett solution som uppfyller alla krav som jag sedan kan ladda ner, öppna, tweaka och sedan ladda upp på git :)

Lyckas du bra så kommer du få dricks, misslyckas du så kommer jag att bli besviken.

Vill du att jag förtydligar något innan du bygger allt så är det fritt fram att ställa följdfrågor först så att du känner dig trygg med att du har all info och alla instruktioner som du behöver 

Skapa ett enkelt system med microservices i C\# och .NET Web API. Du ska bygga 3 microservices som pratar med varandra via HTTP. Microservices som ska byggas ProductService → hanterar produkter OrderService → skapar ordrar LoggingService → samlar loggar från alla tjänster Alla tjänster ska kommunicera via HTTP API. Systemarkitektur OrderService \---\> ProductService | | v v LoggingService Kommunikation OrderService anropar ProductService för att kontrollera produkt ProductService skickar loggar till LoggingService OrderService skickar loggar till LoggingService LoggingService fungerar som ett centralt loggsystem. 

Uppgift 1 \- Skapa solution och projekt Skapa en .NET solution med 3 Web API-projekt: ProductService OrderService LoggingService Varje service ska kunna köras separat på olika portar. 

Uppgift 2 — Product Microservice Krav Skapa ett Web API som hanterar produkter. Produktmodell Varje produkt ska ha: Id Name Price API endpoints Skapa följande: Hämta alla produkter (GET) Hämta produkt via id (GET) Använd in-memory lista (ingen databas behövs). Loggning Varje gång någon anropar Product API: Skicka logg till LoggingService. Exempel: Service: ProductService Message: "Get all products called" 

Uppgift 3 — Order Microservice Krav Skapa ett API för ordrar. Ordermodell Varje order ska ha: Id ProductId Quantity API endpoints Skapa: Skapa order (POST) Hämta alla ordrar (GET) Använd in-memory lista. Viktig logik När en order skapas: Anropa ProductService för att kontrollera att produkten finns Om produkten inte finns → returnera fel Om produkten finns → skapa order Loggning Skicka loggar till LoggingService: Exempel: "Create order called" "Product found" "Order created successfully" "Product not found" Uppgift 4 — Logging Microservice Skapa central loggtjänst. Loggmodell Varje logg ska innehålla: Service name Level (INFO, ERROR) Message Time API endpoints POST logg GET alla loggar Spara loggar i minnet (lista).  
Alla andra services ska skicka loggar hit.   
Uppgift 5 — Testning Starta alla 3 services samtidigt. Testa: Hämta produkter Skapa order Skapa order med fel produkt-id Öppna LoggingService och kontrollera loggar Du ska se loggar från: ProductService OrderService 

Bonusuppgifter som jag vill att du bygger eller hjälper mig med att bygga:

Bonus 1 — Docker Skapa Dockerfile för varje service och kör med docker-compose.   
Bonus 2 — Databas Spara loggar i SQL Server istället för minne.   
Bonus 3 — API Gateway Skapa en gateway som anropar alla services.   
Bonus 4 — Swagger Testa alla API med Swagger.