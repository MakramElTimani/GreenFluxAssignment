using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreenFluxAssignment.Controllers;

[Route("api/chargestations/{chargeStationId}/connectors")]
[ApiController]
[Produces("application/json")]
public class ConnectorsController : ControllerBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorsController(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    /// <summary>
    /// Get all connectors of a charge station
    /// </summary>
    /// <param name="chargeStationId">Guid of the charge station</param>
    /// <response code="200">Returns an array of connectors of the charge station</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid chargeStationId)
    {
        return Ok(await _connectorService.GetAllConnectorsOfChargeStationAsync(chargeStationId));
    }

    /// <summary>
    /// Get a specific connector of a charge station
    /// </summary>
    /// <param name="chargeStationId">Guid of the charge station</param>
    /// <param name="id">integer id of the connector</param>
    /// <response code="200">Returns the requested connector of the charge station</response>
    /// <response code="404">Charge station or connector not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid chargeStationId, int id)
    {
        var connector = await _connectorService.GetConnectorAsync(chargeStationId, id);
        if (connector is null)
        {
            return NotFound();
        }
        return Ok(connector);
    }

    /// <summary>
    /// Create a connector for a charge station
    /// </summary>
    /// <param name="chargeStationId">Guid of the charge station to add the connector to</param>
    /// <param name="connectorDto">Supply the max current in amps</param>
    /// <response code="201">Returns the created connector </response>
    /// <response code="404">Charge station not found</response>
    /// <response code="400">Group capacity reached the limit</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid chargeStationId, [FromBody] CreateConnectorDto connectorDto)
    {
        var createdConnector = await _connectorService.CreateConnector(chargeStationId, connectorDto);
        return CreatedAtAction(nameof(Get), new { chargeStationId, id = createdConnector.Id }, createdConnector);
    }

    /// <summary>
    /// Update a specific connector of a charge station
    /// </summary>
    /// <param name="chargeStationId">Guid of the charge station the connector belongs to</param>
    /// <param name="id">integer id of the connector</param>
    /// <param name="connectorDto">Supplt the new max current in amps</param>
    /// <response code="204">If the connector has been successfully updated </response>
    /// <response code="404">Charge station OR Connector not found</response>
    /// <response code="400">Group capacity reached the limit</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(Guid chargeStationId, int id, [FromBody] UpdateConnectorDto connectorDto)
    {
        _ = await _connectorService.UpdateConnectorAsync(chargeStationId, id, connectorDto);
        return NoContent();
    }

    /// <summary>
    /// Delete a specific connector of a charge station
    /// </summary>
    /// <param name="chargeStationId">Guid of the charge station the connector belongs to</param>
    /// <param name="id">integer id of the connector</param>
    /// <response code="204">If the connector has been successfully deleted </response>
    /// <response code="400">If trying to delete the last connector of a charge station </response>
    /// <response code="404">Charge station OR Connector not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid chargeStationId, int id)
    {
        await _connectorService.DeleteConnectorAsync(chargeStationId, id);
        return NoContent();
    }
}
