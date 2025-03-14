﻿using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Services;

public interface IConnectorService
{
    Task<ConnectorDto> CreateConnector(Guid chargeStationId, CreateConnectorDto connector);

    Task DeleteConnectorAsync(Guid chargeStationId, int id);

    Task<ConnectorDto?> GetConnectorAsync(Guid chargeStationId, int id);

    Task<IEnumerable<ConnectorDto>> GetAllConnectorsOfChargeStationAsync(Guid chargeStationId);

    Task<ConnectorDto> UpdateConnectorAsync(Guid chargeStationId, int id, UpdateConnectorDto connector);
}
