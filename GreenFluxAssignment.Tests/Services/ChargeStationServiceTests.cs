using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Profiles;
using GreenFluxAssignment.Repositories;
using GreenFluxAssignment.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFluxAssignment.Tests.Services;

public class ChargeStationServiceTests
{
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly IMapper _mapper;

    public ChargeStationServiceTests()
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _groupServiceMock = new Mock<IGroupService>();
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<GroupProfile>();
            cfg.AddProfile<ChargeStationProfile>();
            cfg.AddProfile<ConnectorProfile>();
        });
        _mapper = config.CreateMapper();
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync returns a ChargeStationDto when ChargeStationRepository returns a valid ChargeStationDataModel
    /// </summary>
    [Fact]
    public async Task CreateChargeStationAsync_WhenCalled_ShouldReturnChargeStationDto()
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1",
            Connectors = new List<CreateOrUpdateConnectorDto>
            {
                new CreateOrUpdateConnectorDto
                {
                    Id = 1,
                    MaxCurrentInAmps = 10
                },
                new CreateOrUpdateConnectorDto
                {
                    Id = 2,
                    MaxCurrentInAmps = 20
                }
            }
        };

        var chargeStationDataModel = new ChargeStationDataModel
        {
            Id = Guid.NewGuid(),
            Name = chargeStationDto.Name,
            GroupId = Guid.NewGuid(),
            Connectors = new List<ConnectorDataModel>
            {
                new ConnectorDataModel
                {
                    Id = 1,
                    ChargeStationId = Guid.NewGuid(),
                    MaxCurrentInAmps = 10
                },
                new ConnectorDataModel
                {
                    Id = 2,
                    ChargeStationId = Guid.NewGuid(),
                    MaxCurrentInAmps = 20
                }
            }
        };

        _chargeStationRepositoryMock.Setup(x => x.CreateChargeStationAsync(It.IsAny<ChargeStationDataModel>())).ReturnsAsync(chargeStationDataModel);

        // mock group service to return a valid group
        _groupServiceMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 30, 100));

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        var result = await chargeStationService.CreateChargeStationAsync(chargeStationDataModel.GroupId, chargeStationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(chargeStationDataModel.Id, result.Id);
        Assert.Equal(chargeStationDataModel.Name, result.Name);
        Assert.Equal(chargeStationDataModel.GroupId, result.GroupId);
        Assert.Equal(chargeStationDataModel.Connectors.Count, result.Connectors.Count);
        Assert.Equal(chargeStationDataModel.Connectors.First().Id, result.Connectors.First().Id);
        Assert.Equal(chargeStationDataModel.Connectors.First().ChargeStationId, result.Connectors.First().ChargeStationId);
        Assert.Equal(chargeStationDataModel.Connectors.First().MaxCurrentInAmps, result.Connectors.First().MaxCurrentInAmps);
        Assert.Equal(chargeStationDataModel.Connectors.Last().Id, result.Connectors.Last().Id);
        Assert.Equal(chargeStationDataModel.Connectors.Last().ChargeStationId, result.Connectors.Last().ChargeStationId);
        Assert.Equal(chargeStationDataModel.Connectors.Last().MaxCurrentInAmps, result.Connectors.Last().MaxCurrentInAmps);
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync throws an exception when the number of connectors is less than 1 and greater than 5
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateChargeStationAsync_WhenInvalidNumberOfConnectors_ShouldThrowProblemException(int numberOfConnectors)
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1"
        };
        for (int i = 0; i < numberOfConnectors; i++)
        {
            chargeStationDto.Connectors.Add(new CreateOrUpdateConnectorDto
            {
                MaxCurrentInAmps = 10
            });
        }

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync throws an exception when group does not exist
    /// </summary>
    [Fact]
    public async Task CreateChargeStationAsync_WhenGroupDoesNotExist_ShouldThrowProblemException()
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1",
            Connectors = new List<CreateOrUpdateConnectorDto>
            {
                new CreateOrUpdateConnectorDto
                {
                    MaxCurrentInAmps = 10
                },
                new CreateOrUpdateConnectorDto
                {
                    MaxCurrentInAmps = 20
                }
            }
        };

        // mock group service to return a group that does not exist
        _groupServiceMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((false, 0, 0));

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync throws an exception when the total current exceeds the max allowed current
    /// </summary>
    [Fact]
    public async Task CreateChargeStationAsync_WhenTotalCurrentExceedsMaxAllowedCurrent_ShouldThrowProblemException()
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1",
            Connectors = new List<CreateOrUpdateConnectorDto>
            {
                new CreateOrUpdateConnectorDto
                {
                    MaxCurrentInAmps = 10
                },
                new CreateOrUpdateConnectorDto
                {
                    MaxCurrentInAmps = 20
                }
            }
        };

        // mock group service to return a group with total current that exceeds the max allowed current
        _groupServiceMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 100, 100));

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync throws an exception when a connector has an invalid Id
    /// </summary>
    [Fact]
    public async Task CreateChargeStationAsync_WhenConnectorHasInvalidId_ShouldThrowProblemException()
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1",
            Connectors = new List<CreateOrUpdateConnectorDto>
            {
                new CreateOrUpdateConnectorDto
                {
                    Id = 6,
                    MaxCurrentInAmps = 10
                },
                new CreateOrUpdateConnectorDto
                {
                    Id = 1,
                    MaxCurrentInAmps = 20
                }
            }
        };

        // mock group service to return a valid group
        _groupServiceMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 30, 100));

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }

    /// <summary>
    /// Test to see if CreateChargeStationAsync throws an exception when a connectors have duplicate id
    /// </summary>
    [Fact]
    public async Task CreateChargeStationAsync_WhenConnectorsHaveDuplicateId_ShouldThrowProblemException()
    {
        // Arrange
        var chargeStationDto = new CreateChargeStationDto
        {
            Name = "Charge Station 1",
            Connectors = new List<CreateOrUpdateConnectorDto>
            {
                new CreateOrUpdateConnectorDto
                {
                    Id = 1,
                    MaxCurrentInAmps = 10
                },
                new CreateOrUpdateConnectorDto
                {
                    Id = 1,
                    MaxCurrentInAmps = 20
                }
            }
        };

        // mock group service to return a valid group
        _groupServiceMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 30, 100));

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.CreateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }

    /// <summary>
    /// Test to see if UpdateChargeStationAsync returns a ChargeStationDto when ChargeStationRepository returns a valid ChargeStationDataModel
    /// </summary>
    [Fact]
    public async Task UpdateChargeStationAsync_WhenCalled_ShouldReturnChargeStationDto()
    {
        // Arrange
        var chargeStationDto = new UpdateChargeStationDto
        {
            Name = "Charge Station 1"
        };

        var chargeStationDataModel = new ChargeStationDataModel
        {
            Id = Guid.NewGuid(),
            Name = "Old name",
            GroupId = Guid.NewGuid(),
            Connectors = new List<ConnectorDataModel>
            {
                new ConnectorDataModel
                {
                    Id = 1,
                    ChargeStationId = Guid.NewGuid(),
                    MaxCurrentInAmps = 10
                },
                new ConnectorDataModel
                {
                    Id = 2,
                    ChargeStationId = Guid.NewGuid(),
                    MaxCurrentInAmps = 20
                }
            }
        };

        _chargeStationRepositoryMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(chargeStationDataModel);
        _chargeStationRepositoryMock.Setup(x => x.UpdateChargeStationAsync(It.IsAny<ChargeStationDataModel>())).ReturnsAsync(chargeStationDataModel);

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        var result = await chargeStationService.UpdateChargeStationAsync(chargeStationDataModel.Id, chargeStationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(chargeStationDataModel.Id, result.Id);
        Assert.Equal(chargeStationDataModel.Name, result.Name);
        Assert.Equal(chargeStationDataModel.GroupId, result.GroupId);
        Assert.Equal(chargeStationDataModel.Connectors.Count, result.Connectors.Count);
        Assert.Equal(chargeStationDataModel.Connectors.First().Id, result.Connectors.First().Id);
        Assert.Equal(chargeStationDataModel.Connectors.First().ChargeStationId, result.Connectors.First().ChargeStationId);
        Assert.Equal(chargeStationDataModel.Connectors.First().MaxCurrentInAmps, result.Connectors.First().MaxCurrentInAmps);
        Assert.Equal(chargeStationDataModel.Connectors.Last().Id, result.Connectors.Last().Id);
        Assert.Equal(chargeStationDataModel.Connectors.Last().ChargeStationId, result.Connectors.Last().ChargeStationId);
        Assert.Equal(chargeStationDataModel.Connectors.Last().MaxCurrentInAmps, result.Connectors.Last().MaxCurrentInAmps);
    }

    /// <summary>
    /// Test to see if UpdateChargeSationAsync returns a ProblemException when Charge Station does not exist
    /// </summary>
    [Fact]
    public async Task UpdateChargeStationAsync_WhenChargeStationDoesNotExist_ShouldThrowProblemException()
    {
        // Arrange
        var chargeStationDto = new UpdateChargeStationDto
        {
            Name = "Charge Station 1"
        };

        _chargeStationRepositoryMock.Setup(x => x.GetChargeStationByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ChargeStationDataModel?)null);

        var chargeStationService = new ChargeStationService(_chargeStationRepositoryMock.Object, _groupServiceMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => chargeStationService.UpdateChargeStationAsync(Guid.NewGuid(), chargeStationDto));
    }
}
