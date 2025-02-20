namespace GreenFluxAssignment.Data;

public class GroupDataModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int CapacityInAmps { get; set; }

    public ICollection<ChargeStationDataModel> ChargeStations { get; set; } = [];
}
