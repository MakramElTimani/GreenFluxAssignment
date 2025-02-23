using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

/// <summary>
/// DTO for creating a charge station
/// </summary>
public class CreateChargeStationDto
{
    /// <summary>
    /// Name of the charge station
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A list of 1-5 connectors
    /// </summary>
    [MinLength(1)]
    [MaxLength(5)]
    public virtual List<CreateConnectorDto> Connectors { get; set; } = [];
}

/// <summary>
/// DTO for updating a charge station
/// </summary>
public class UpdateChargeStationDto 
{
    /// <summary>
    /// New name of the charge station
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Charge station DTO. Used for GET requests
/// </summary>
public class ChargeStationDto
{
    /// <summary>
    /// Id of the charge station
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Group Id where the charge station belongs
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Name of the charge station
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of 1-5 connectors
    /// </summary>
    [MinLength(1)]
    [MaxLength(5)]
    public List<ConnectorDto> Connectors { get; set; } = [];
}
