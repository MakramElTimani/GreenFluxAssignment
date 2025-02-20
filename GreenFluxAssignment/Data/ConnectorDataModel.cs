namespace GreenFluxAssignment.Data;

public class ConnectorDataModel
{
    public int Id { get; set; }

    public int MaxCurrentInAmps { get; set; }

    public Guid ChargeStationId { get; set; }

    public ChargeStationDataModel? ChargeStation { get; set; }
}
