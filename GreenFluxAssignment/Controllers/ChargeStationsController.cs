using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GreenFluxAssignment.Controllers;

[Route("api/groups/{groupId}/chargestations")]
[ApiController]
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
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get(Guid groupId)
    {
        return Ok(await _chargeStationService.GetAllChargeStationsOfGroupAsync(groupId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid groupId, Guid id)
    {
        var chargeStation = await _chargeStationService.GetChargeStationAsync(id);
        if (chargeStation is null)
        {
            return NotFound();
        }
        return Ok(chargeStation);
    }

    /// <summary>
    /// Create a charge station for a Group
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post(Guid groupId, [FromBody] CreateChargeStationDto request)
    {
        var createdChargeStation = await _chargeStationService.CreateChargeStationAsync(groupId, request);
        return CreatedAtAction(nameof(Get), new { groupId, id = createdChargeStation.Id }, createdChargeStation);
    }

    /// <summary>
    /// Update a charge station of a Group
    /// </summary>
    [HttpPut("{id}")]
    public void Put(Guid groupId, Guid id, [FromBody] ChargeStationDto request)
    {
    }

    /// <summary>
    /// Delete a charge station of a Group
    /// </summary>
    [HttpDelete("{chargeStationId}")]
    public async Task<IActionResult> Delete(Guid groupId, Guid chargeStationId)
    {
        await _chargeStationService.DeleteChargeStationAsync(chargeStationId);
        return NoContent();
    }
}
