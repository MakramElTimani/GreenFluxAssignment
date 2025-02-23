## Green Flux Assignment Version 25

### Pre-requisites

- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [EF Core tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
```
dotnet tool install --global dotnet-ef
```

### Setup

- Open a terminal and navigate to the project root directory (where the .sln file is located)
- Navigate to the GreenFluxAssignment directory which contains the api
- Run EF Database Update command to initialize the database
```
cd GreenFluxAssignment
dotnet ef database update
```

### Run Tests

```
cd GreenFluxAssignment.Tests
dotnet test
```

### Run

- Run the API
```
cd GreenFluxAssignment
dotnet run
```
- Open a browser and navigate to [http://localhost:5058/swagger/index.html](http://localhost:5058/swagger/index.html)
- NOTE: The API runs on port 5058 by default. You can change this in the launchSettings.json file


### Project Description

The API is a simple REST API that allows you to manage groups, charge stations and connectors

The database is a SQLite database that is created in the API directory and initialized using the dotnet ef database update command as described in the setup section

For a given domain model exists the following classes:
- DataModel which is the entity stored in the database
- Repository which is used to interact with the database
- Service which has the business logic and uses the repository to interact with the database
- Controller which is the entry point for the API and uses the service to interact with the database
- DTO which is the data transfer object used to transfer data between the API and the client
- Mapper which is used to map between the DataModel and the DTO