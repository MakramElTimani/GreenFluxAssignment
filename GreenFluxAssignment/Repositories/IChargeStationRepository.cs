using GreenFluxAssignment.Data;
using GreenFluxAssignment.DTOs;

namespace GreenFluxAssignment.Repositories;

public interface IChargeStationRepository
{
    Task<ChargeStationDataModel> CreateChargeStationAsync(ChargeStationDataModel chargeStation);

    Task<bool> DeleteChargeStationAsync(Guid id);

    Task<ChargeStationDataModel?> GetChargeStationByIdAsync(Guid id);

    Task<IEnumerable<ChargeStationDataModel>> GetAllChargeStationsOfGroup(Guid groupId);

    Task<ChargeStationDataModel> UpdateChargeStationAsync(ChargeStationDataModel chargeStation);
}
