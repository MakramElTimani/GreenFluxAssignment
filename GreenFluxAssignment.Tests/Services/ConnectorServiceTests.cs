using AutoMapper;
using Castle.Core.Logging;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Profiles;
using GreenFluxAssignment.Repositories;
using GreenFluxAssignment.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFluxAssignment.Tests.Services;

public class ConnectorServiceTests
{
    private readonly Mock<IConnectorRepository> _connectorRepositoryMock;
    private readonly Mock<IChargeStationService> _chargeStationServiceMock;
    private readonly Mock<IGroupService> _groupService;
    private readonly IMapper _mapper;


    public ConnectorServiceTests()
    {
        _connectorRepositoryMock = new Mock<IConnectorRepository>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GroupProfile>();
            cfg.AddProfile<ChargeStationProfile>();
            cfg.AddProfile<ConnectorProfile>();
        });
        _mapper = config.CreateMapper();
        _chargeStationServiceMock = new Mock<IChargeStationService>();
        _groupService = new Mock<IGroupService>();
    }

    /// <summary>
    /// Test to see if CreateConnector returns a ConnectorDto when ConnectorRepository returns a valid ConnectorDataModel
    /// </summary>
    [Fact]
    public async Task CreateConnector_WhenCalled_ShouldReturnConnectorDto()
    {
        // Arrange
        var connectorDto = new CreateConnectorDto
        {
            Id = 1,
            MaxCurrentInAmps = 10
        };

        var connectorDataModel = new ConnectorDataModel
        {
            Id = 1,
            ChargeStationId = Guid.NewGuid(),
            MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
        };

        _chargeStationServiceMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new ChargeStationDto { Id = connectorDataModel.ChargeStationId });

        _groupService.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 20, 100));

        _connectorRepositoryMock.Setup(x => x.CreateConnectorAsync(It.IsAny<ConnectorDataModel>())).ReturnsAsync(connectorDataModel);

        var connectorService = new ConnectorService(_connectorRepositoryMock.Object, Mock.Of<ILogger<ConnectorService>>(), _mapper, _chargeStationServiceMock.Object, _groupService.Object);

        // Act
        var result = await connectorService.CreateConnector(connectorDataModel.ChargeStationId, connectorDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(connectorDataModel.Id, result.Id);
        Assert.Equal(connectorDataModel.ChargeStationId, result.ChargeStationId);
        Assert.Equal(connectorDataModel.MaxCurrentInAmps, result.MaxCurrentInAmps);
    }

    /// <summary>
    /// Test to see if CreateConnector throws ProblemException when ChargeStation has 5 connectors already
    /// </summary>
    [Fact]
    public async Task CreateConnector_WhenChargeStationHas5Connectors_ShouldThrowProblemException()
    {
        // Arrange
        var connectorDto = new CreateConnectorDto
        {
            MaxCurrentInAmps = 10
        };

        var connectorDataModel = new ConnectorDataModel
        {
            Id = 1,
            ChargeStationId = Guid.NewGuid(),
            MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
        };

        var chargeStation = new ChargeStationDto
        {
            Id = connectorDataModel.ChargeStationId,
            Connectors = new List<ConnectorDto>
            {
                new ConnectorDto { Id = 1, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 2, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 3, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 4, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 5, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 }
            }
        };

        _chargeStationServiceMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(chargeStation);

        var connectorService = new ConnectorService(_connectorRepositoryMock.Object, Mock.Of<ILogger<ConnectorService>>(), _mapper, _chargeStationServiceMock.Object, _groupService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ProblemException>(() => connectorService.CreateConnector(connectorDataModel.ChargeStationId, connectorDto));
    }

    /// <summary>
    /// Test to see if CreateConnector throws ProblemException when Group current limit is exceeded
    /// </summary>
    [Fact]
    public async Task CreateConnector_WhenGroupCurrentLimitExceeded_ShouldThrowProblemException()
    {
        // Arrange
        var connectorDto = new CreateConnectorDto
        {
            MaxCurrentInAmps = 10
        };

        var connectorDataModel = new ConnectorDataModel
        {
            Id = 1,
            ChargeStationId = Guid.NewGuid(),
            MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
        };

        var chargeStation = new ChargeStationDto
        {
            Id = connectorDataModel.ChargeStationId,
            Connectors = new List<ConnectorDto>
            {
                new ConnectorDto { Id = 1, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 2, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 3, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 4, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 }
            }
        };

        _chargeStationServiceMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(chargeStation);

        _groupService.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 41, 50));

        var connectorService = new ConnectorService(_connectorRepositoryMock.Object, Mock.Of<ILogger<ConnectorService>>(), _mapper, _chargeStationServiceMock.Object, _groupService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ProblemException>(() => connectorService.CreateConnector(connectorDataModel.ChargeStationId, connectorDto));
    }

    /// <summary>
    /// Test to see if UpdateConnector returns a ConnectorDto when ConnectorRepository returns a valid ConnectorDataModel
    /// </summary>
    [Fact]
    public async Task UpdateConnector_WhenCalled_ShouldReturnConnectorDto()
    {
        // Arrange
        var connectorDto = new UpdateConnectorDto
        {
            MaxCurrentInAmps = 10
        };

        var connectorDataModel = new ConnectorDataModel
        {
            Id = 1,
            ChargeStationId = Guid.NewGuid(),
            MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
        };

        _chargeStationServiceMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new ChargeStationDto
            {
                Id = connectorDataModel.ChargeStationId,
                Connectors = [
                    new() 
                    {
                        Id = 1,
                        ChargeStationId = connectorDataModel.ChargeStationId,
                        MaxCurrentInAmps = 5
                    }
                ]
        });

        _groupService.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 20, 100));

        _connectorRepositoryMock.Setup(x => x.GetConnectorByIdAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(connectorDataModel);
        _connectorRepositoryMock.Setup(x => x.UpdateConnectorAsync(It.IsAny<ConnectorDataModel>())).ReturnsAsync(connectorDataModel);

        var connectorService = new ConnectorService(_connectorRepositoryMock.Object, Mock.Of<ILogger<ConnectorService>>(), _mapper, _chargeStationServiceMock.Object, _groupService.Object);

        // Act
        var result = await connectorService.UpdateConnectorAsync(connectorDataModel.ChargeStationId, connectorDataModel.Id, connectorDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(connectorDataModel.Id, result.Id);
        Assert.Equal(connectorDataModel.ChargeStationId, result.ChargeStationId);
        Assert.Equal(connectorDataModel.MaxCurrentInAmps, result.MaxCurrentInAmps);
    }

    /// <summary>
    /// Test to see if UpdateConnect throws ProblemException when Group current limit is exceeded
    /// </summary>
    [Fact]
    public async Task UpdateConnector_WhenGroupCurrentLimitExceeded_ShouldThrowProblemException()
    {
        // Arrange
        var connectorDto = new UpdateConnectorDto
        {
            MaxCurrentInAmps = 10
        };

        var connectorDataModel = new ConnectorDataModel
        {
            Id = 1,
            ChargeStationId = Guid.NewGuid(),
            MaxCurrentInAmps = connectorDto.MaxCurrentInAmps
        };

        var chargeStation = new ChargeStationDto
        {
            Id = connectorDataModel.ChargeStationId,
            Connectors = new List<ConnectorDto>
            {
                new ConnectorDto { Id = 1, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 2, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 3, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 },
                new ConnectorDto { Id = 4, ChargeStationId = connectorDataModel.ChargeStationId, MaxCurrentInAmps = 10 }
            }
        };

        _chargeStationServiceMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(chargeStation);

        _groupService.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 41, 50));

        var connectorService = new ConnectorService(_connectorRepositoryMock.Object, Mock.Of<ILogger<ConnectorService>>(), _mapper, _chargeStationServiceMock.Object, _groupService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ProblemException>(() => connectorService.UpdateConnectorAsync(connectorDataModel.ChargeStationId, connectorDataModel.Id, connectorDto));
    }
}
