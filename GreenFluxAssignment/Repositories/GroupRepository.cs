﻿using GreenFluxAssignment.Data;
using GreenFluxAssignment.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GreenFluxAssignment.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<GroupRepository> _logger;

    public GroupRepository(ApplicationDbContext applicationDbContext, ILogger<GroupRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<GroupDataModel?> CreateGroupAsync(GroupDataModel group)
    {
        var entryEntity = await _applicationDbContext.Groups.AddAsync(group);
        
        int response = await _applicationDbContext.SaveChangesAsync();
        if(response == 0)
        {
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to insert", "Failed to save changes to the database");
        }

        return entryEntity.Entity;
    }

    public async Task DeleteGroupAsync(GroupDataModel group)
    {
        _applicationDbContext.Groups.Remove(group);
        int response = await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<GroupDataModel?> GetGroupByIdAsync(Guid id)
    {
        return await _applicationDbContext.Groups
            .Include(m => m.ChargeStations)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<GroupDataModel>> GetAllGroupsAsync()
    {
        return await _applicationDbContext.Groups.ToListAsync();
    }

    public async Task<(bool Exists, int TotalCurrent, int MaxAllowedCurrent)> GetGroupCurrentLimitsAsync(Guid groupId)
    {
        return await _applicationDbContext.Groups
            .Where(g => g.Id == groupId)
            .Select(g => new
            {
                Exists = true,
                TotalCurrent = g.ChargeStations.SelectMany(cs => cs.Connectors).Sum(c => c.MaxCurrentInAmps),
                MaxAllowedCurrent = g.CapacityInAmps
            })
            .FirstOrDefaultAsync() switch
            {
                null => (false, 0, 0),
                var result => (result.Exists, result.TotalCurrent, result.MaxAllowedCurrent)
            };
    }

    public async Task<GroupDataModel> UpdateGroupAsync(GroupDataModel group)
    {
        _applicationDbContext.Groups.Update(group);
        int response = await _applicationDbContext.SaveChangesAsync();
        if(response == 0)
        {
            _logger.LogError("Unexpected error occurred while updating a Group. Nothing was updated");
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to update", "Failed to save changes to the database");
        }
        return group;
    }
}
