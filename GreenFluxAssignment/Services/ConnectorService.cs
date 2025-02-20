using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Repositories;
using System.Net;

namespace GreenFluxAssignment.Services;

public class ConnectorService : IConnectorService
{
    private readonly IConnectorRepository _connectorRepository;
    private readonly ILogger<ConnectorService> _logger;
    private readonly IMapper _mapper;
    private readonly IChargeStationService _chargeStationService;
    private readonly IGroupService _groupService;

    public ConnectorService(IConnectorRepository connectorRepository, 
        ILogger<ConnectorService> logger, 
        IMapper mapper, 
        IChargeStationService chargeStationService,
        IGroupService groupService)
    {
        _connectorRepository = connectorRepository;
        _logger = logger;
        _mapper = mapper;
        _chargeStationService = chargeStationService;
        _groupService = groupService;
    }

    public async Task<ConnectorDto> CreateConnector(Guid chargeStationId, ConnectorDto connector)
    {
        var chargeStation = await ValidateCapacity(chargeStationId, connector);

        // check if charge station has less than 5 connectors
        if (chargeStation.Connectors.Count >= 5)
        {
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Charge Station Full", "Cannot create more than 5 connectors in a charge station");
        }

        // map ConnectorDto to ConnectorDataModel
        var connectorDataModel = new ConnectorDataModel
        {
            Id = chargeStation.Connectors.Count + 1,
            ChargeStationId = chargeStationId,
            MaxCurrentInAmps = connector.MaxCurrentInAmps,
        };

        // call repository to create connector
        ConnectorDataModel createdConnector = await _connectorRepository.CreateConnectorAsync(connectorDataModel);

        return _mapper.Map<ConnectorDto>(createdConnector);
    }

    public async Task DeleteConnectorAsync(Guid chargeStationId, int id)
    {
        int count = await _connectorRepository.CountChargeStationConnectors(chargeStationId);
        
        if(count == 1)
        {
           throw new InvalidOperationException("Cannot delete the last connector of a charge station");
        }

        bool isDeleted = await _connectorRepository.DeleteConnectorAsync(chargeStationId, id);

        if(!isDeleted)
        {
            throw new InvalidOperationException("Unable to delete! Connector not found");
        }
    }

    public async Task<IEnumerable<ConnectorDto>> GetAllConnectorsOfChargeStationAsync(Guid chargeStationId)
    {
        var dataModels = await _connectorRepository.GetAllConnectorsOfChargeStation(chargeStationId);
        return _mapper.Map<List<ConnectorDto>>(dataModels);
    }

    public async Task<ConnectorDto?> GetConnectorAsync(Guid chargeStationId, int id)
    {
        var dataModel = await _connectorRepository.GetConnectorByIdAsync(chargeStationId, id);
        if (dataModel is null)
        {
            return null;
        }
        return _mapper.Map<ConnectorDto>(dataModel);
    }

    public async Task<ConnectorDto> UpdateConnectorAsync(Guid chargeStationId, int id, ConnectorDto connector)
    {
        await ValidateCapacity(chargeStationId, connector);

        // map ConnectorDto to ConnectorDataModel
        var connectorDataModel = new ConnectorDataModel
        {
            Id = id,
            ChargeStationId = chargeStationId,
            MaxCurrentInAmps = connector.MaxCurrentInAmps,
        };

        // call repository to update connector
        ConnectorDataModel updatedConnector = await _connectorRepository.UpdateConnectorAsync(connectorDataModel);

        return _mapper.Map<ConnectorDto>(updatedConnector);
    }

    private async Task<ChargeStationDto> ValidateCapacity(Guid chargeStationId, ConnectorDto connector)
    {
        // TODO: Try to merge this into one query for better performance

        // get charge station
        var chargeStation = await _chargeStationService.GetChargeStationAsync(chargeStationId);
        if(chargeStation is null)
        {
           throw new ProblemException(HttpStatusCode.NotFound, "Charge Station Not Found", "Charge Station not found");
        }

        // get group with total capacity
        (_, int TotalCurrent, int MaxAllowedCurrent) = await _groupService.GetGroupCurrentLimitsAsync(chargeStation.GroupId);

        // check if the total current of the group is less than the max allowed current
        if (TotalCurrent + connector.MaxCurrentInAmps > MaxAllowedCurrent)
        {
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Group Capacity Exceeded", "Cannot add connector to charge station. Group capacity exceeded");
        }

        return chargeStation;
    }

}
