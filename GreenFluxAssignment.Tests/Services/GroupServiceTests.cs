using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Profiles;
using GreenFluxAssignment.Repositories;
using GreenFluxAssignment.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFluxAssignment.Tests.Services;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly IMapper _mapper;

    public GroupServiceTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<GroupProfile>();
            cfg.AddProfile<ChargeStationProfile>();
            cfg.AddProfile<ConnectorProfile>();
        });
        _mapper = config.CreateMapper();
    }

    /// <summary>
    /// Test to see if CreateGroupAsync returns a GroupDto when GroupRepository returns a valid GroupDataModel
    /// </summary>
    [Fact]
    public async Task CreateGroupAsync_WhenCalled_ShouldReturnGroupDto()
    {
        // Arrange
        var groupDto = new CreateOrUpdateGroupDto
        {
            Name = "Group 1",
            CapacityInAmps = 100
        };

        var groupDataModel = new GroupDataModel
        {
            Id = Guid.NewGuid(),
            Name = groupDto.Name,
            CapacityInAmps = groupDto.CapacityInAmps
        };

        _groupRepositoryMock.Setup(x => x.CreateGroupAsync(It.IsAny<GroupDataModel>())).ReturnsAsync(groupDataModel);

        var groupService = new GroupService(_groupRepositoryMock.Object, _mapper);

        // Act
        var result = await groupService.CreateGroupAsync(groupDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(groupDataModel.Id, result.Id);
        Assert.Equal(groupDataModel.Name, result.Name);
        Assert.Equal(groupDataModel.CapacityInAmps, result.CapacityInAmps);
    }

    /// <summary>
    /// Test to see if UpdateGroupAsync returns a GroupDto when GroupRepository returns a valid GroupDataModel
    /// </summary>
    [Fact]
    public async Task UpdateGroupAsync_WhenCalled_ShouldReturnGroupDto()
    {
        // Arrange
        var groupDto = new CreateOrUpdateGroupDto
        {
            Name = "updated name",
            CapacityInAmps = 500
        };

        var groupDataModel = new GroupDataModel
        {
            Id = Guid.NewGuid(),
            Name = "old name",
            CapacityInAmps = 100
        };

        var updatedGroupDataModel = new GroupDataModel
        {
            Id = groupDataModel.Id,
            Name = groupDto.Name,
            CapacityInAmps = groupDto.CapacityInAmps
        };

        _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(It.IsAny<Guid>())).ReturnsAsync(groupDataModel);

        _groupRepositoryMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, groupDataModel.CapacityInAmps, groupDataModel.CapacityInAmps));

        _groupRepositoryMock.Setup(x => x.UpdateGroupAsync(It.IsAny<GroupDataModel>())).ReturnsAsync(updatedGroupDataModel);

        var groupService = new GroupService(_groupRepositoryMock.Object, _mapper);

        // Act
        var result = await groupService.UpdateGroupAsync(groupDataModel.Id, groupDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(groupDataModel.Id, result.Id);
        Assert.Equal(groupDataModel.Name, result.Name);
        Assert.Equal(groupDataModel.CapacityInAmps, result.CapacityInAmps);
    }

    /// <summary>
    /// Test to see if UpdateGroupAsync will throw exception when Group Capacity is exceeded
    /// </summary>
    [Fact]
    public async Task UpdateGroupAsync_WhenGroupCapacityExceeded_ShouldThrowProblemException()
    {
        // Arrange
        var groupDto = new CreateOrUpdateGroupDto
        {
            Name = "updated name",
            CapacityInAmps = 400
        };

        var groupDataModel = new GroupDataModel
        {
            Id = Guid.NewGuid(),
            Name = "old name",
            CapacityInAmps = 500
        };

        _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(It.IsAny<Guid>())).ReturnsAsync(groupDataModel);

        // total current 450 and max allowed current 100
        _groupRepositoryMock.Setup(x => x.GetGroupCurrentLimitsAsync(It.IsAny<Guid>())).ReturnsAsync((true, 450, 500));

        var groupService = new GroupService(_groupRepositoryMock.Object, _mapper);

        // Act
        // Assert
        await Assert.ThrowsAsync<ProblemException>(() => groupService.UpdateGroupAsync(groupDataModel.Id, groupDto));
    }
}
