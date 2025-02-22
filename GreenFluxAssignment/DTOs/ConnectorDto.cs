using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class CreateOrUpdateConnectorDto
{
    [Required]
    [Range(1, 5)]
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCurrentInAmps { get; set; }
}

public class ConnectorDto : CreateOrUpdateConnectorDto
{
    public Guid ChargeStationId { get; set; }
}
