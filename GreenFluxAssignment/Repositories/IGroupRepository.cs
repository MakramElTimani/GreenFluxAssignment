using GreenFluxAssignment.Data;

namespace GreenFluxAssignment.Repositories;

public interface IGroupRepository
{
    Task<GroupDataModel?> CreateGroupAsync(GroupDataModel group);

    Task<GroupDataModel?> GetGroupByIdAsync(Guid id);

    Task<IEnumerable<GroupDataModel>> GetAllGroupsAsync();

    Task<GroupDataModel> UpdateGroupAsync(GroupDataModel group);

    Task DeleteGroupAsync(GroupDataModel group);

    Task<(bool Exists, int TotalCurrent, int MaxAllowedCurrent)> GetGroupCurrentLimitsAsync(Guid groupId);
}
