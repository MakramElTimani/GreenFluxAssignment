using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Repositories;
using System.Net;

namespace GreenFluxAssignment.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<GroupDto> CreateGroupAsync(CreateOrUpdateGroupDto groupDto)
    {
        if (groupDto.CapacityInAmps <= 0)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Invalid Capacity", "Group capacity must be greater than 0.");
        }

        //map GroupDto to GroupDataModel
        var groupDataModel = _mapper.Map<GroupDataModel>(groupDto);

        //call repository to create group
        var createdGroup = await _groupRepository.CreateGroupAsync(groupDataModel);

        // map GroupDataModel to GroupDto
        var createdGroupDto = _mapper.Map<GroupDto>(createdGroup);

        return createdGroupDto;
    }

    public async Task DeleteGroupAsync(Guid groupId)
    {
        if (!await _groupRepository.DeleteGroupAsync(groupId))
        {
            throw new ProblemException(System.Net.HttpStatusCode.NotFound, "Group Not Found", "Could not delete Group because it was not found");
        }
    }

    public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
    {
        var groupDataModels = await _groupRepository.GetAllGroupsAsync();
        return _mapper.Map<List<GroupDto>>(groupDataModels);
    }

    public async Task<GroupDto?> GetGroupAsync(Guid groupId)
    {
        var dataModel = await _groupRepository.GetGroupByIdAsync(groupId);
        return _mapper.Map<GroupDto>(dataModel);
    }

    public async Task<GroupDto?> UpdateGroupAsync(Guid groupId, CreateOrUpdateGroupDto groupDto)
    {
        if (groupDto.CapacityInAmps <= 0)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Invalid Capacity", "Group capacity must be greater than 0.");
        }

        // Get group by Id
        GroupDataModel? groupDataModel = await _groupRepository.GetGroupByIdAsync(groupId);
        if (groupDataModel is null)
        {
            return null;
        }
        //Validate the group capacity
        (_, int TotalCurrent, int MaxAllowedCurrent) = await GetGroupCurrentLimitsAsync(groupId);
        if (TotalCurrent > groupDto.CapacityInAmps)
        {
            throw new ProblemException(System.Net.HttpStatusCode.BadRequest, "Invalid Capacity", "Group capacity cannot be less than the total current of the charge stations in the group.");
        }

        // Update group properties
        groupDataModel.Name = groupDto.Name;
        groupDataModel.CapacityInAmps = groupDto.CapacityInAmps;

        // call repository to update group
        GroupDataModel updatedGroup = await _groupRepository.UpdateGroupAsync(groupDataModel);

        // map GroupDataModel to GroupDto
        return _mapper.Map<GroupDto>(updatedGroup);
    }

    public async Task<(bool Exists, int TotalCurrent, int MaxAllowedCurrent)> GetGroupCurrentLimitsAsync(Guid groupId)
    {
        return await _groupRepository.GetGroupCurrentLimitsAsync(groupId);
    }
}
