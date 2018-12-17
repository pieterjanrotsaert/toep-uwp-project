# Project: Mobile Apps II (Windows) 

## Groepsleden: Pieter-Jan Rotsaert -- Jeffrey Waegneer

------------------
### Instructies

> NOOT: Deze applicatie gebruikt standaard een SQL databank op het volgend adres: 18.188.130.207.
         Zorg ervoor dat er internet verbinding is OF vervang dit adres in 'appsettings.json' van de
         backend met een lokale (localhost) databank. Het schema zal bij het eerste gebruik automatisch
         aangemaakt worden.
         
#### Om dit project te starten vanuit Visual Studio:

##### Optie 1: Lokale uitvoering

1. Voer eerste PrettigLokaalBackend uit zonder te debuggen. (Bv. door Ctrl+F5 te drukken.)
Zorg dat deze in de achtergrond loopt.

BELANGRIJK: Verander het profiel naar 'PrettigLokaalBackend' (dus NIET 'IIS Express'),
            ga dan naar project properties van PrettigLokaalBackend en zet 'Launch Browser' af, 
            en verander de 'App URL' naar 'https://localhost:5001'.

2. Voer vervolgens de App (PrettigLokaal) uit.

##### Optie 2: Uitvoeren gebruik makend van externe server

Zet simpelweg de configuratie op 'Release' bij de app en voer deze uit m.b.v. Ctrl + F5.

U hoeft de backend niet te builden of te runnen, de release build van de app is voorgeconfigureerd om met 
een externe server te verbinden waar de backend reeds online staat.

#### Beschikbare Test Accounts

Er kan indien gewenst een eigen account aangemaakt worden in de app zelf. 
Indien het toch wenselijk is om met een ander account in te loggen zijn deze accounts reeds beschikbaar:

> NOOT: Deze accounts zijn alleen beschikbaar als u de standaard database (18.188.130.207) gebruikt.
        Als u zelf een nieuwe DB aanmaakt zal die natuurlijk aanvankelijk geen accounts bevatten.
        

1. Verkoper: <br/>
>Login naam: 'verkoper' <br/>
>Login wachtwoord: 'verkoper'

2. Gewone gebruiker: <br/> 
>Login naam: 'klant' <br/>
>Login wachtwoord: 'klant'
