using AutoMapper;
using Castle.Core.Logging;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Profiles;
using GreenFluxAssignment.Repositories;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFluxAssignment.Tests.IntegrationTests;

public class DomainIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GroupService _groupService;
    private readonly ChargeStationService _chargeStationService;
    private readonly ConnectorService _connectorService;
    private readonly IMapper _mapper;

    public DomainIntegrationTests()
    {
        // Setup In-Memory DB Context
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        _dbContext = new ApplicationDbContext(options);

        //configure automapper
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<GroupProfile>();
            cfg.AddProfile<ChargeStationProfile>();
            cfg.AddProfile<ConnectorProfile>();
        });
        _mapper = config.CreateMapper();

        // Initialize services
        _groupService = new GroupService(
            new GroupRepository(_dbContext, Mock.Of<ILogger<GroupRepository>>()), 
            _mapper);

        _chargeStationService = new ChargeStationService(
            new ChargeStationRepository(_dbContext, Mock.Of<ILogger<ChargeStationRepository>>()), 
            _groupService,
            _mapper);

        _connectorService = new ConnectorService(
            new ConnectorRepository(_dbContext, Mock.Of<ILogger<ConnectorRepository>>()), 
            Mock.Of<ILogger<ConnectorService>>(),
            _mapper, 
            _chargeStationService, 
            _groupService);
    }

    [Fact]
    public async Task DomainFlow_ShouldHandleFullLifecycleCorrectly()
    {
        // 1. Create Group
        var createGroupDto = new CreateOrUpdateGroupDto 
        { 
            Name = "Test Group", 
            CapacityInAmps = 100 
        };
        GroupDto createdGroup = await _groupService.CreateGroupAsync(createGroupDto);
        Assert.NotNull(createdGroup);
        Assert.Equal(createGroupDto.Name, createdGroup.Name);
        Assert.Equal(createGroupDto.CapacityInAmps, createdGroup.CapacityInAmps);

        // 2. Create Charge Station with 5 Connectors
        var createChargeStationDto = new CreateChargeStationDto
        {
            Name = "Test Station",
            Connectors = [
                new () { Id = 1, MaxCurrentInAmps = 10 },    
                new () { Id = 2, MaxCurrentInAmps = 10 },    
                new () { Id = 3, MaxCurrentInAmps = 10 },    
                new () { Id = 4, MaxCurrentInAmps = 10 },
                new () { Id = 5, MaxCurrentInAmps = 10 }
            ]
        };
        var createdChargeStation = await _chargeStationService.CreateChargeStationAsync(createdGroup.Id, createChargeStationDto);
        Assert.NotNull(createdChargeStation);
        Assert.Equal(5, createdChargeStation.Connectors.Count);
        Assert.Equal(createChargeStationDto.Name, createdChargeStation.Name);
        for(int i = 0; i < createdChargeStation.Connectors.Count; i++)
        {
            Assert.Equal(i + 1, createdChargeStation.Connectors[i].Id); // verify ID assignment
            Assert.Equal(10, createdChargeStation.Connectors[i].MaxCurrentInAmps); 
        }

        // 3. Try to Add 1 More Connectors (Should throw exception)
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.CreateConnector(createdChargeStation.Id, new CreateOrUpdateConnectorDto { MaxCurrentInAmps = 10 })
        );

        // 4. Create Another Charge Station with 1 Connector
        var createdChargeStation2 = await _chargeStationService.CreateChargeStationAsync(createdGroup.Id, new CreateChargeStationDto
        {
            Name = "Station 2",
            Connectors = [
                new() { Id = 1, MaxCurrentInAmps = 20 },    
            ]
        });
        Assert.NotNull(createdChargeStation2);
        Assert.NotEqual(createdChargeStation.Id, createdChargeStation2.Id); // Ensure different IDs

        // 5. Try to Add Connector Exceeding Capacity (Should Throw Exception)
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.CreateConnector(createdChargeStation2.Id, new CreateOrUpdateConnectorDto { Id = 1, MaxCurrentInAmps = 50 })
        );

        // 6. Delete a connector from first charge station
        var connectorToDelete = createdChargeStation.Connectors.First();
        await _connectorService.DeleteConnectorAsync(createdChargeStation.Id, connectorToDelete.Id);
        // validate connector deletion
        Assert.Null(await _dbContext.Connectors.FirstOrDefaultAsync(m => m.Id == connectorToDelete.Id && m.ChargeStationId == connectorToDelete.ChargeStationId));

        // 7. try to delete connector from station 2 (Should throw exception)
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.DeleteConnectorAsync(createdChargeStation2.Id, 1)
        );

        // 8. Delete First Charge Station and Validate Cascade Delete
        await _chargeStationService.DeleteChargeStationAsync(createdChargeStation.Id);
        Assert.Null(await _dbContext.ChargeStations.FindAsync(createdChargeStation.Id));
        Assert.Empty(await _dbContext.Connectors.Where(c => c.ChargeStationId == createdChargeStation.Id).ToListAsync());

        // 9. Delete Group and Validate Cascade Delete createdChargeStation2 and its connector
        await _groupService.DeleteGroupAsync(createdGroup.Id);
        Assert.Null(await _dbContext.Groups.FindAsync(createdGroup.Id));
        Assert.Empty(_dbContext.ChargeStations.Where(cs => cs.GroupId == createdGroup.Id));
        Assert.Empty(_dbContext.Connectors.Where(c => c.ChargeStationId == createdChargeStation2.Id));
    }

    [Fact]
    public async Task DomainFlow_ShouldNotAllowOrphanedObjects()
    {
        await Assert.ThrowsAsync<ProblemException>(async () =>
        {
            // Create a Charge Station without a Group
            await _chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), new CreateChargeStationDto
            {
                Name = "Orphaned Station",
                Connectors = [
                    new() { MaxCurrentInAmps = 10 },
                ]
            });
        });

        await Assert.ThrowsAsync<ProblemException>(async () =>
        {
            // Create a Connector without a Charge Station
            await _connectorService.CreateConnector(Guid.NewGuid(), new CreateOrUpdateConnectorDto { MaxCurrentInAmps = 10 });
        });
    }

    [Fact]
    public async Task DomainFlow_DeletingConnectorFromList_ShouldAllowToAddAgain()
    {
        // 1. Create Group
        var createGroupDto = new CreateOrUpdateGroupDto
        {
            Name = "Test Group",
            CapacityInAmps = 100
        };
        GroupDto createdGroup = await _groupService.CreateGroupAsync(createGroupDto);
        Assert.NotNull(createdGroup);


        // 2. Create Charge Station with 5 Connectors
        var createChargeStationDto = new CreateChargeStationDto
        {
            Name = "Test Station",
            Connectors = [
                new () { Id = 1, MaxCurrentInAmps = 10 },
                new () { Id = 2, MaxCurrentInAmps = 10 },
                new () { Id = 3, MaxCurrentInAmps = 10 },
                new () { Id = 4, MaxCurrentInAmps = 10 },
                new () { Id = 5, MaxCurrentInAmps = 10 }
            ]
        };
        var createdChargeStation = await _chargeStationService.CreateChargeStationAsync(createdGroup.Id, createChargeStationDto);
        Assert.NotNull(createdChargeStation);
        Assert.Equal(5, createdChargeStation.Connectors.Count);

        // 3. Delete a connector from first charge station
        var connectorToDelete = createdChargeStation.Connectors[2]; // Id 3
        await _connectorService.DeleteConnectorAsync(createdChargeStation.Id, connectorToDelete.Id);
        // validate connector deletion
        Assert.Null(await _dbContext.Connectors.FirstOrDefaultAsync(m => m.Id == connectorToDelete.Id && m.ChargeStationId == connectorToDelete.ChargeStationId));

        // 4. Add a new connector with different Id than deleted (Should throw exception)
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.CreateConnector(createdChargeStation.Id, new CreateOrUpdateConnectorDto { Id = 5, MaxCurrentInAmps = 10 })
        );

        // 5. Add a new connector with invalid id (Should throw exception)
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.CreateConnector(createdChargeStation.Id, new CreateOrUpdateConnectorDto { Id = 6, MaxCurrentInAmps = 10 })
        ); 
        await Assert.ThrowsAsync<ProblemException>(async () =>
            await _connectorService.CreateConnector(createdChargeStation.Id, new CreateOrUpdateConnectorDto { Id = -1, MaxCurrentInAmps = 10 })
        );

        // 6. Add a new connector with valid id
        var newConnector = await _connectorService.CreateConnector(createdChargeStation.Id, new CreateOrUpdateConnectorDto { Id = 3, MaxCurrentInAmps = 10 });
        Assert.NotNull(newConnector);
        Assert.Equal(3, newConnector.Id);
        Assert.Equal(10, newConnector.MaxCurrentInAmps);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
