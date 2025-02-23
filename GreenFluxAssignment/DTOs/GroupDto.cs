using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

/// <summary>
/// DTO to create AND update a group
/// </summary>
public class CreateOrUpdateGroupDto
{
    /// <summary>
    /// Name of the group
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maximum capacity in amps that the group can handle
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int CapacityInAmps { get; set; }
}

/// <summary>
/// DTO for a group. Used for GET requests
/// </summary>
public class GroupDto : CreateOrUpdateGroupDto
{
    /// <summary>
    /// Id of the group
    /// </summary>
    public Guid Id { get; set; }
}
