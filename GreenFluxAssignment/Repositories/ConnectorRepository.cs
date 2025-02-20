using GreenFluxAssignment.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<int> CountChargeStationConnectors(Guid chargeStationId)
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
            throw new Exception("Failed to save changes to the database"); //TODO: customize exception
        }
        return connector;
    }

    public async Task<bool> DeleteConnectorAsync(Guid chargeStationId, int id)
    {
        var connector = await GetConnectorByIdAsync(chargeStationId, id);
        if (connector is null)
        {
            return false;
        }
        _applicationDbContext.Connectors.Remove(connector);
        await _applicationDbContext.SaveChangesAsync();
        return true;
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
            throw new Exception("Failed to save changes to the database"); //TODO: customize exception
        }
        return connector;
    }
}
