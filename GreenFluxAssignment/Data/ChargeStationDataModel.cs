namespace GreenFluxAssignment.Data;

public class ChargeStationDataModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Guid GroupId { get; set; }

    public GroupDataModel? Group { get; set; }

    public ICollection<ConnectorDataModel> Connectors { get; set; } = [];
}
