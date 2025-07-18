##  WaterTank-Forecast
This project is a **prototype/conceptual system** designed to simulate and forecast resource consumption over time. The core component is the **Main Web API**, which performs **linear forecasting** to estimate when a resource (e.g., water) will be depleted based on historical usage data

## Tech

-   [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) - Cross-platform. Open source.  
A framework for building web apps and services with .NET and C#
-   .NET 9
-   [Scalar](https://github.com/scalar/scalar/tree/main/integrations/aspnetcore) - A modern, open-source alternative to Swagger UI for generating interactive, user-friendly documentation from OpenAPI specs
- [MQTTnet](https://github.com/dotnet/MQTTnet) - High performance .NET library for MQTT based communication. It provides a MQTT client and a MQTT server (broker)
-   [Dapper](https://github.com/DapperLib/Dapper) - micro ORM for .net
-   [TimescaleDB](https://github.com/timescale/timescaledb) - A time-series database for high-performance real-time analytics packaged as a Postgres extension
-   [Mosquitto](https://github.com/eclipse-mosquitto/mosquitto) - An open source MQTT broker
-   [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) - .NET UI client for building Desktop, Embedded, Mobile and WebAssembly apps with C# and XAML
 - [xUnit](https://github.com/xunit/xunit) -  free, open source, community-focused unit testing tool for .NET
- [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) - A library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions
-   [Docker](https://www.docker.com/) - u know


[Third Party Licenses](#third-party-licenses)

## Architecture
<img width="2059" height="1246" alt="prov2" src="https://github.com/user-attachments/assets/5079bcbd-e307-46ce-b5e8-27ddc6f03b60" />    

 1. **Console Tool** - used to insert items directly into the database, supporting both automatic and manual modes for fine-grained control
 2. **AvaloniaUi tool**  - A desktop application(windows) used to populate the database by simulating a real IoT devices(be aware of Docker port starvation). It sends data to the MQTT broker , which is then picked up by the DataIngest API and stored in the database. Only automatic mode is available, with limited edit options. Edge cases insertions are not supported.
 3. **Database** -  Timescaledb ,  responsible for storing all data. The initial idea was to have two databases (one for writes and one read-only), but it was simplified to a single database for reduced complexity and scripting. Ideally, two databases with r&w should be used
 4.  **DataIngest Microservice** – This service connects the MQTT broker to the database. It is designed to scale automatically thanks to  MQTT v5 shared subscription groups
 5. **ASP.NET Core Web api**  – Responsible for retrieving data from the database and providing resource forecasting. The linear forecast formula is implemented on the SQL side due to its simplicity, meaning no intermediate forecast data is processed by the API

### Forecast Formula
	Linear forecast :  
		lastDay +
		(currentResourceLevel /
		(totalAverageUsagePerDay/ totalDaysAppliedTo))

```
totalAverageUsagePerDay  = (Tankday1Level − Tankday2Level) + (TankDay2Level - TankDay3Level) ...
until the specified range is reached, excluding refills and accounting for missing days
```

## Build
> [!IMPORTANT]
> Require docker engine & compose
> 
Startup:
 - `docker compose -p watertank_project up`
 - go to http://localhost:9944/
 - remember to fill the database with the forecastData , check [**tools**](#tools)

Cleanup:

 - Ctrl+c the main terminal
 - `docker compose -p watertank_project down --volumes --rmi all`

## Tools
Database is empty by default , use one of these tools(the executable not included in docker automatic build up ).

 1. [Download & Install .net SDK](https://dotnet.microsoft.com/en-us/download) or (use IDE & skip)
 2. Navigate to the folder of the tool you intend to use
 3. `dotnet run -c Release`

 - WaterTankMockTool_MQTT :
   
 ![Animazione](https://github.com/user-attachments/assets/16f89973-3681-4e56-b680-1f8a4a9364dc)

 - DirectInserttoDB

 template data for manual insertion into data.txt file
 
## Tests

> [!IMPORTANT]
> Require docker engine

Tests->Watertank.api.integration.test: 
 
Custom insertions and formula checks without building the full solution or docker compose. Requires docker to mock the database



## Third Party Licenses

check third party folder "THIRD_PARTY/licenses.txt"
