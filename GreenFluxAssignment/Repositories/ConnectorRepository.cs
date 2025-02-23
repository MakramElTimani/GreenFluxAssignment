using GreenFluxAssignment.Data;
using GreenFluxAssignment.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GreenFluxAssignment.Repositories;

public class ConnectorRepository : IConnectorRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<ConnectorRepository> _logger;

    public ConnectorRepository(ApplicationDbContext applicationDbContext, ILogger<ConnectorRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<int> CountChargeStationConnectorsAsync(Guid chargeStationId)
    {
        return await _applicationDbContext.Connectors
            .CountAsync(c => c.ChargeStationId == chargeStationId);
    }

    public async Task<ConnectorDataModel> CreateConnectorAsync(ConnectorDataModel connector)
    {
        await _applicationDbContext.Connectors.AddAsync(connector);
        if (await _applicationDbContext.SaveChangesAsync() == 0)
        {
            _logger.LogError("Unexpected error occurred while saving creating a Connector. Nothing was updated");
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to insert", "Failed to save changes to the database");
        }
        return connector;
    }

    public async Task DeleteConnectorAsync(ConnectorDataModel connector)
    {
        _applicationDbContext.Connectors.Remove(connector);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ConnectorDataModel>> GetAllConnectorsOfChargeStation(Guid chargeStationId)
    {
        return await _applicationDbContext.Connectors
            .Where(c => c.ChargeStationId == chargeStationId)
            .ToListAsync();
    }

    public async Task<ConnectorDataModel?> GetConnectorByIdAsync(Guid chargeStationId, int id)
    {
        return await _applicationDbContext.Connectors
            .FindAsync(id, chargeStationId);
    }

    public async Task<ConnectorDataModel> UpdateConnectorAsync(ConnectorDataModel connector)
    {
        _applicationDbContext.Connectors.Update(connector);
        if (await _applicationDbContext.SaveChangesAsync() == 0)
        {
            _logger.LogError("Unexpected error occurred while saving updating a Connector. Nothing was updated");
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Unable to update", "Failed to save changes to the database"); 
        }
        return connector;
    }
}
