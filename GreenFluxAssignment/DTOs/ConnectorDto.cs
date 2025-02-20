using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class ConnectorDto
{
    [Required]
    [Range(1, 5)]
    public int Id { get; set; }

    public Guid ChargeStationId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCurrentInAmps { get; set; }
}
