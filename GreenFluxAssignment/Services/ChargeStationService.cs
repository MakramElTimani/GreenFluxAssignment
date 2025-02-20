using AutoMapper;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Repositories;
using System.Net;
using System.Text.RegularExpressions;

namespace GreenFluxAssignment.Services;

public class ChargeStationService : IChargeStationService
{
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IGroupService _groupService;
    private readonly IMapper _mapper;

    public ChargeStationService(IChargeStationRepository charStationRepository, IGroupService groupService, IMapper mapper)
    {
        _chargeStationRepository = charStationRepository;
        _groupService = groupService;
        _mapper = mapper;
    }

    public async Task<ChargeStationDto> CreateChargeStationAsync(Guid groupId, ChargeStationDto chargeStationDto)
    {
        if (chargeStationDto.Connectors.Count is < 1 or > 5)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Invalid number of connectors", "Charge station must have between 1 and 5 connectors.");
        }

        // check sum of max current in amps for group and if group exists
        var groupCheck = await _groupService.GetGroupCurrentLimitsAsync(groupId);
        if(!groupCheck.Exists)
        {
            throw new ProblemException(HttpStatusCode.NotFound, "Not Found", "Group does not exist.");
        }
        if(groupCheck.TotalCurrent + chargeStationDto.Connectors.Sum(c => c.MaxCurrentInAmps) > groupCheck.MaxAllowedCurrent)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Exceeded Amps Capacity", $"Cannot add charge station. Total current in amps for group will exceed {groupCheck.MaxAllowedCurrent}.");
        }

        // map ChargeStationDto to ChargeStationDataModel
        var chargeStationDataModel = _mapper.Map<ChargeStationDataModel>(chargeStationDto);
        chargeStationDataModel.GroupId = groupId;

        // call repository to create charge station
        ChargeStationDataModel createdChargeStation = await _chargeStationRepository.CreateChargeStationAsync(chargeStationDataModel);

        return _mapper.Map<ChargeStationDto>(createdChargeStation);
    }

    public async Task DeleteChargeStationAsync(Guid chargeStationId)
    {
        await _chargeStationRepository.DeleteChargeStationAsync(chargeStationId);
    }

    public async Task<IEnumerable<ChargeStationDto>> GetAllChargeStationsOfGroupAsync(Guid groupId)
    {
        var dataModels = await _chargeStationRepository.GetAllChargeStationsOfGroup(groupId);
        return _mapper.Map<IEnumerable<ChargeStationDto>>(dataModels);
    }

    public async Task<ChargeStationDto?> GetChargeStationAsync(Guid chargeStationId)
    {
        ChargeStationDataModel? dataModel = await _chargeStationRepository.GetChargeStationByIdAsync(chargeStationId);
        if (dataModel is null)
        {
            return null;
        }
        return _mapper.Map<ChargeStationDto>(dataModel);
    }


    public async Task<ChargeStationDto> UpdateChargeStationAsync(Guid chargeStationId, ChargeStationDto chargeStationDto)
    {
        if (chargeStationDto.Connectors.Count is < 1 or > 5)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Invalid number of connectors", "Charge station must have between 1 and 5 connectors.");
        }

        // get charge station
        var chargeStation = await GetChargeStationAsync(chargeStationId);
        if (chargeStation is null)
        {
            throw new ProblemException(HttpStatusCode.NotFound, "Charge Station Not Found", "Charge Station not found");
        }

        // check sum of max current in amps for group and if group exists
        var groupCheck = await _groupService.GetGroupCurrentLimitsAsync(chargeStation.GroupId);
        if (!groupCheck.Exists)
        {
            throw new ProblemException(HttpStatusCode.NotFound, "Not Found", "Group does not exist.");
        }
        if (groupCheck.TotalCurrent + chargeStationDto.Connectors.Sum(c => c.MaxCurrentInAmps) > groupCheck.MaxAllowedCurrent)
        {
            throw new ProblemException(HttpStatusCode.BadRequest, "Exceeded Amps Capacity", $"Cannot add charge station. Total current in amps for group will exceed {groupCheck.MaxAllowedCurrent}.");
        }

        // map ChargeStationDto to ChargeStationDataModel
        var chargeStationDataModel = _mapper.Map<ChargeStationDataModel>(chargeStationDto);
        chargeStationDataModel.GroupId = chargeStation.GroupId;

        // call repository to create charge station
        ChargeStationDataModel createdChargeStation = await _chargeStationRepository.UpdateChargeStationAsync(chargeStationDataModel);

        return _mapper.Map<ChargeStationDto>(createdChargeStation);
    }
}
