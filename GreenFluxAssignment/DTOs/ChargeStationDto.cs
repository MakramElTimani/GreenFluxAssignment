using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class ChargeStationDto
{
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [MinLength(1)]
    [MaxLength(5)]
    public List<ConnectorDto> Connectors { get; set; } = [];

    public Guid GroupId { get; set; }
}
