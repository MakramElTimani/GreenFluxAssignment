using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

/// <summary>
/// DTO to create a connector
/// </summary>
public class CreateConnectorDto
{
    /// <summary>
    /// Integer Id of the connector. Values must be between 1 and 5 and cannot be duplicated in the same charge station
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Id { get; set; }

    /// <summary>
    /// Maximum current in amps that the connector can handle
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCurrentInAmps { get; set; }
}

/// <summary>
/// DTO to update a connector
/// </summary>
public class UpdateConnectorDto 
{
    /// <summary>
    /// New maximum current in amps that the connector can handle
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCurrentInAmps { get; set; }
}   

/// <summary>
/// DTO for a connector. Used for GET requests
/// </summary>
public class ConnectorDto : CreateConnectorDto
{
    /// <summary>
    /// Id of the charge station where the connector is located
    /// </summary>
    public Guid ChargeStationId { get; set; }
}
