using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreenFluxAssignment.Controllers;

[Route("api/groups")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    // GET: api/groups
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _groupService.GetAllGroupsAsync());
    }

    // GET api/groups/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var group = await _groupService.GetGroupAsync(id);
        if (group is null)
        {
            return NotFound();
        }
        return Ok(group);
    }

    // POST api/groups
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateOrUpdateGroupDto request)
    {
        var createdGroup = await _groupService.CreateGroupAsync(request);
        return CreatedAtAction(nameof(Get), new { id = createdGroup.Id }, createdGroup);
    }

    // PUT api/groups/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] CreateOrUpdateGroupDto updateRequest)
    {
        await _groupService.UpdateGroupAsync(id, updateRequest);
        return NoContent();
    }

    // DELETE api/groups/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _groupService.DeleteGroupAsync(id);
        return NoContent();
    }
}
