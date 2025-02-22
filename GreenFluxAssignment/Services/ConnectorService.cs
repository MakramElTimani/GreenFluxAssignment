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

    public async Task<ConnectorDto> CreateConnector(Guid chargeStationId, CreateOrUpdateConnectorDto connector)
    {
        var chargeStation = await ValidateCapacity(chargeStationId, connector);

        // check if charge station has less than 5 connectors
        if (chargeStation.Connectors.Count >= 5)
        {
            throw new ProblemException(HttpStatusCode.UnprocessableEntity, "Charge Station Full", "Cannot create more than 5 connectors in a charge station");
        }

        // check if the connector already exists
        if (chargeStation.Connectors.Any(c => c.Id == connector.Id))
        {
            throw new ProblemException(HttpStatusCode.Conflict, "Connector Already Exists", "Connector with the same Id already exists");
        }

        // map ConnectorDto to ConnectorDataModel
        var connectorDataModel = new ConnectorDataModel
        {
            Id = connector.Id,
            ChargeStationId = chargeStationId,
            MaxCurrentInAmps = connector.MaxCurrentInAmps,
        };
        // call repository to create connector
        ConnectorDataModel createdConnector = await _connectorRepository.CreateConnectorAsync(connectorDataModel);

        return _mapper.Map<ConnectorDto>(createdConnector);
    }

    public async Task DeleteConnectorAsync(Guid chargeStationId, int id)
    {
        // check if the connector is the last connector of the charge station
        int count = await _connectorRepository.CountChargeStationConnectorsAsync(chargeStationId);
        if (count == 1)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Not Allowed", "Cannot delete the last connector of a charge station");
        }

        if (!await _connectorRepository.DeleteConnectorAsync(chargeStationId, id))
        {
            throw new ProblemException(HttpStatusCode.NotFound, "Not Found", "Unable to delete! Connector not found");
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

    public async Task<ConnectorDto> UpdateConnectorAsync(Guid chargeStationId, int id, CreateOrUpdateConnectorDto connector)
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

    /// <summary>
    /// This method is used to validate the capacity of the group 
    /// 1. Validates the Id of the connector
    /// 2. Check if the charge station exists
    /// 3. Get the group's total current and max allowed current
    /// 4. Check if adding the connector's max current in amps to the group's total current exceeds the max allowed current
    /// </summary>
    /// <exception cref="ProblemException">If charge station is not found</exception>
    /// <exception cref="ProblemException">If capacity is full for the group</exception>
    private async Task<ChargeStationDto> ValidateCapacity(Guid chargeStationId, CreateOrUpdateConnectorDto connector)
    {
        if (connector.Id < 1 || connector.Id > 5)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Invalid Connector Id", "Connector Id should be between 1 and 5");
        }

        // get charge station
        var chargeStation = await _chargeStationService.GetChargeStationAsync(chargeStationId);
        if (chargeStation is null)
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
