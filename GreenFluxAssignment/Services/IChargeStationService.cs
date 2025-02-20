using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Services;

public interface IChargeStationService
{
    Task<ChargeStationDto> CreateChargeStationAsync(Guid groupId, ChargeStationDto chargeStationDto);

    Task DeleteChargeStationAsync(Guid chargeStationId);

    Task<IEnumerable<ChargeStationDto>> GetAllChargeStationsOfGroupAsync(Guid groupId);

    Task<ChargeStationDto?> GetChargeStationAsync(Guid chargeStationId);

    Task<ChargeStationDto> UpdateChargeStationAsync(Guid chargeStationId, ChargeStationDto chargeStationDto);
}
