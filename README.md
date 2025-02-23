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

The API is a simple REST API that allows you to manage charge stations and charge points. The API has the following endpoints: