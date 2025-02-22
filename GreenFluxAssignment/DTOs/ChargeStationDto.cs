using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class CreateChargeStationDto
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [MinLength(1)]
    [MaxLength(5)]
    public virtual List<CreateOrUpdateConnectorDto> Connectors { get; set; } = [];
}

public class UpdateChargeStationDto 
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;
}

public class ChargeStationDto
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [MinLength(1)]
    [MaxLength(5)]
    public List<ConnectorDto> Connectors { get; set; } = [];
}
