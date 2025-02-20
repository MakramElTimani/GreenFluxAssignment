﻿using GreenFluxAssignment.Data;

namespace GreenFluxAssignment.Repositories;

public interface IGroupRepository
{
    Task<GroupDataModel?> CreateGroupAsync(GroupDataModel group);

    Task<GroupDataModel?> GetGroupByIdAsync(Guid id);

    Task<IEnumerable<GroupDataModel>> GetAllGroups();

    Task<GroupDataModel> UpdateGroupAsync(GroupDataModel group);

    Task<bool> DeleteGroupAsync(Guid id);

    Task<(bool Exists, int TotalCurrent, int MaxAllowedCurrent)> GetGroupCurrentLimitsAsync(Guid groupId);

    Task<bool> Exists(Guid groupId);
}
