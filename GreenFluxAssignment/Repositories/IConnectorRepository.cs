using GreenFluxAssignment.Data;

namespace GreenFluxAssignment.Repositories;

public interface IConnectorRepository
{
    Task<ConnectorDataModel> CreateConnectorAsync(ConnectorDataModel connector);

    Task<bool> DeleteConnectorAsync(Guid chargeStationId, int id);

    Task<ConnectorDataModel?> GetConnectorByIdAsync(Guid chargeStationId, int id);

    Task<IEnumerable<ConnectorDataModel>> GetAllConnectorsOfChargeStation(Guid chargeStationId);

    Task<int> CountChargeStationConnectorsAsync(Guid chargeStationId);

    Task<ConnectorDataModel> UpdateConnectorAsync(ConnectorDataModel connector);
}
