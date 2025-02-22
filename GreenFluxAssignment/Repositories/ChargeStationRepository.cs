using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GreenFluxAssignment.Repositories;

public class ChargeStationRepository : IChargeStationRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<ChargeStationRepository> _logger;

    public ChargeStationRepository(ApplicationDbContext applicationDbContext, ILogger<ChargeStationRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<ChargeStationDataModel> CreateChargeStationAsync(ChargeStationDataModel chargeStation)
    {
        if (chargeStation.Connectors is null || !chargeStation.Connectors.Any())
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Connectors Required", "Charge station must have at least one connector.");
        }

        var connectorsList = chargeStation.Connectors.ToList(); 

        if (connectorsList.Count > 5)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Too Many Connectors","Charge station cannot have more than 5 connectors.");
        }

        // Assign unique IDs within the charge station context
        for (int i = 0; i < connectorsList.Count; i++)
        {
            connectorsList[i].Id = i + 1;
            connectorsList[i].ChargeStationId = chargeStation.Id;
        }

        chargeStation.Connectors = connectorsList; // Ensure the updated list is set
        await _applicationDbContext.ChargeStations.AddAsync(chargeStation);
        if(await _applicationDbContext.SaveChangesAsync() == 0)
        {
            _logger.LogError("Unexpected error occurred while saving creating a ChargeStation. Nothing was updated");
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to insert", "Failed to save changes to the database");
        }
        return chargeStation;
    }

    public async Task<bool> DeleteChargeStationAsync(Guid id)
    {
        var chargeStation = await GetChargeStationByIdAsync(id);

        if (chargeStation is null)
        {
            return false; // Return false if the entity wasn't found
        }

        _applicationDbContext.ChargeStations.Remove(chargeStation);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ChargeStationDataModel>> GetAllChargeStationsOfGroupAsync(Guid groupId)
    {
        return await _applicationDbContext.ChargeStations
            .Include(c => c.Connectors)
            .Where(c => c.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<ChargeStationDataModel?> GetChargeStationByIdAsync(Guid id)
    {
        return await _applicationDbContext.ChargeStations
            .Include(c => c.Connectors)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ChargeStationDataModel> UpdateChargeStationAsync(ChargeStationDataModel chargeStation)
    {
        _applicationDbContext.ChargeStations.Update(chargeStation);
        if (await _applicationDbContext.SaveChangesAsync() == 0)
        {
            _logger.LogError("An unexpected error occurred while updating a ChargeStation. Nothing was updated");
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to update", "Failed to save changes to the database");
        }
        return chargeStation;
    }
}
