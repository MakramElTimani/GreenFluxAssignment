using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Services;

public interface IChargeStationService
{
    Task<ChargeStationDto> CreateChargeStationAsync(Guid groupId, CreateChargeStationDto chargeStationDto);

    Task DeleteChargeStationAsync(Guid groupId, Guid chargeStationId);

    Task<IEnumerable<ChargeStationDto>> GetAllChargeStationsOfGroupAsync(Guid groupId);

    Task<ChargeStationDto?> GetChargeStationAsync(Guid groupId, Guid chargeStationId);

    Task<ChargeStationDto?> GetChargeStationByIdAsync(Guid chargeStationId);

    Task<ChargeStationDto> UpdateChargeStationAsync(Guid groupId, Guid chargeStationId, UpdateChargeStationDto chargeStationDto);
}
