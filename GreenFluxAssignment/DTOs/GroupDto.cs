using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int CapacityInAmps { get; set; }
}
