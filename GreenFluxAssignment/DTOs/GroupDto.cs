using System.ComponentModel.DataAnnotations;

namespace GreenFluxAssignment.DTOs;

public class CreateOrUpdateGroupDto
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int CapacityInAmps { get; set; }
}

public class GroupDto : CreateOrUpdateGroupDto
{
    public Guid Id { get; set; }
}
