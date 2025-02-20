using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Repositories;

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

    public async Task<GroupDto> CreateGroupAsync(GroupDto groupDto)
    {
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
        await _groupRepository.DeleteGroupAsync(groupId);
    }

    public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
    {
        var groupDataModels = await _groupRepository.GetAllGroups();
        return _mapper.Map<List<GroupDto>>(groupDataModels);
    }

    public async Task<GroupDto?> GetGroupAsync(Guid groupId)
    {
        var dataModel = await _groupRepository.GetGroupByIdAsync(groupId);
        return _mapper.Map<GroupDto>(dataModel);
    }

    public async Task<bool> Exists(Guid groupId)
    {
        return await _groupRepository.Exists(groupId);
    }

    public async Task<GroupDto?> UpdateGroupAsync(Guid groupId, GroupDto groupDto)
    {
        // Get group by Id
        GroupDataModel? groupDataModel = await _groupRepository.GetGroupByIdAsync(groupId);   
        if (groupDataModel is null)
        {
            return null;
        }
        //TODO: Validate the group capacity

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
