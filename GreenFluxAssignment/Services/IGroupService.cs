using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Services;

public interface IGroupService
{
    Task<GroupDto> CreateGroupAsync(GroupDto groupDto);

    Task<GroupDto?> GetGroupAsync(Guid groupId);

    Task<GroupDto?> UpdateGroupAsync(Guid groupId, GroupDto groupDto);

    Task DeleteGroupAsync(Guid groupId);

    Task<IEnumerable<GroupDto>> GetAllGroupsAsync();

    Task<bool> Exists(Guid groupId);

    Task<(bool Exists, int TotalCurrent, int MaxAllowedCurrent)> GetGroupCurrentLimitsAsync(Guid groupId);
}
