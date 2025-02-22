using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Repositories;

public interface IChargeStationRepository
{
    Task<ChargeStationDataModel> CreateChargeStationAsync(ChargeStationDataModel chargeStation);

    Task<bool> DeleteChargeStationAsync(Guid id);

    Task<ChargeStationDataModel?> GetChargeStationByIdAsync(Guid id);

    Task<IEnumerable<ChargeStationDataModel>> GetAllChargeStationsOfGroupAsync(Guid groupId);

    Task<ChargeStationDataModel> UpdateChargeStationAsync(ChargeStationDataModel chargeStation);
}
