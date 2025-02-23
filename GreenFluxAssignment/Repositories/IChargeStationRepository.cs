using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Repositories;

public interface IChargeStationRepository
{
    Task<ChargeStationDataModel> CreateChargeStationAsync(ChargeStationDataModel chargeStation);

    Task DeleteChargeStationAsync(ChargeStationDataModel chargeStation);

    Task<ChargeStationDataModel?> GetChargeStationByIdAsync(Guid id, bool includeConnectors = false);

    Task<IEnumerable<ChargeStationDataModel>> GetAllChargeStationsOfGroupAsync(Guid groupId);

    Task<ChargeStationDataModel> UpdateChargeStationAsync(ChargeStationDataModel chargeStation);
}
