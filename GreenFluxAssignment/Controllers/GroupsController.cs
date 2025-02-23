using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GreenFluxAssignment.Controllers;

[Route("api/groups")]
[ApiController]
[Produces("application/json")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    /// <summary>
    /// Get All Groups
    /// </summary>
    /// <response code="200">Returns an array of all groups in the database</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        return Ok(await _groupService.GetAllGroupsAsync());
    }

    /// <summary>
    /// Get group by id
    /// </summary>
    /// <param name="id">Guid with the Id of the group</param>
    /// <response code="200">Returns the specified group object</response>
    /// <response code="404">If the group is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var group = await _groupService.GetGroupAsync(id);
        if (group is null)
        {
            return NotFound();
        }
        return Ok(group);
    }

    /// <summary>
    /// Create a group
    /// </summary>
    /// <param name="request">Supply the name and capacity in amps</param>
    /// <response code="201">Returns the created group</response>
    /// <response code="400">If the name or capacity supplied are invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateOrUpdateGroupDto request)
    {
        var createdGroup = await _groupService.CreateGroupAsync(request);
        return CreatedAtAction(nameof(Get), new { id = createdGroup.Id }, createdGroup);
    }

    /// <summary>
    /// Update the group object
    /// </summary>
    /// <param name="id">Id of the group to update</param>
    /// <param name="updateRequest">The update request containing name and capacity in amps</param>
    /// <response code="204">If the group has been successfully update</response>
    /// <response code="404">Group not found</response>
    /// <response code="400">Name or capacity invalid OR the new capacity value would be exceeded and is invalid</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid id, [FromBody] CreateOrUpdateGroupDto updateRequest)
    {
        await _groupService.UpdateGroupAsync(id, updateRequest);
        return NoContent();
    }

    /// <summary>
    /// Delete the group by Id
    /// </summary>
    /// <param name="id">Id of the group to delete</param>
    /// <response code="204">The group has been successfully deleted</response>
    /// <response code="404">Group not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _groupService.DeleteGroupAsync(id);
        return NoContent();
    }
}
