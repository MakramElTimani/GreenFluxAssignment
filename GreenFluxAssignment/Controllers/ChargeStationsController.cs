using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GreenFluxAssignment.Controllers;

[Route("api/groups/{groupId}/chargestations")]
[ApiController]
[Produces("application/json")]
public class ChargeStationsController : ControllerBase
{
    private readonly IChargeStationService _chargeStationService;

    public ChargeStationsController(IChargeStationService chargeStationService)
    {
        _chargeStationService = chargeStationService;
    }

    /// <summary>
    /// Get all charge stations of a Group
    /// </summary>
    /// <param name="groupId">Guid of the group</param>
    /// <response code="200">Returns an array of charge stations of the group</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid groupId)
    {
        return Ok(await _chargeStationService.GetAllChargeStationsOfGroupAsync(groupId));
    }

    /// <summary>
    /// Get a specific charge station of a Group
    /// </summary>
    /// <param name="groupId">Guid of the group</param>
    /// <param name="id">Guid of the charge station</param>
    /// <response code="200">Returns returns the charge station object</response>
    /// <response code="404">Charge station not found OR the group is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid groupId, Guid id)
    {
        var chargeStation = await _chargeStationService.GetChargeStationAsync(groupId, id);
        if (chargeStation is null)
        {
            return NotFound();
        }
        return Ok(chargeStation);
    }

    /// <summary>
    /// Create a charge station for a Group
    /// </summary>
    /// <param name="groupId">Guid of the group to add the station to</param>
    /// <param name="request">Supply the name of the charge station along with 1 to 5 connectors</param>
    /// <response code="201">Returns the created charge station</response>
    /// <response code="400">If the name is empty, the connectors aren't valid, or the max group capacity has been reached</response>
    /// <response code="404">If the group is not found</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(Guid groupId, [FromBody] CreateChargeStationDto request)
    {
        var createdChargeStation = await _chargeStationService.CreateChargeStationAsync(groupId, request);
        return CreatedAtAction(nameof(Get), new { groupId, id = createdChargeStation.Id }, createdChargeStation);
    }

    /// <summary>
    /// Update a charge station of a Group
    /// </summary>
    /// <param name="groupId">Guid of the group where the charge station belongs to</param>
    /// <param name="id">Guid of the charge station to update</param>
    /// <param name="request">Supply the name of the charge station to update</param>
    /// <response code="204">If the item is successfully updated</response>
    /// <response code="404">If the charge station is not found</response>
    /// <response code="400">If the name is empty OR the charge station doesn't belong to the group</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid groupId, Guid id, [FromBody] UpdateChargeStationDto request)
    {
        await _chargeStationService.UpdateChargeStationAsync(groupId, id, request);
        return NoContent();
    }

    /// <summary>
    /// Delete a charge station of a Group
    /// </summary>
    /// <param name="groupId">Guid of the group where the charge station belongs to</param>
    /// <param name="chargeStationId">Guid of the charge station to delete</param>
    /// <response code="204">If the item is successfully deleted</response>
    /// <response code="404">If the charge station is not found</response>
    /// <response code="400">If the charge station doesn't belong to the group</response>
    [HttpDelete("{chargeStationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid groupId, Guid chargeStationId)
    {
        await _chargeStationService.DeleteChargeStationAsync(groupId, chargeStationId);
        return NoContent();
    }
}
