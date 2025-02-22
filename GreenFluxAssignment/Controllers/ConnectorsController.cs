using GreenFluxAssignment.DTOs;
using GreenFluxAssignment.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreenFluxAssignment.Controllers;

[Route("api/chargestations/{chargeStationId}/connectors")]
[ApiController]
public class ConnectorsController : ControllerBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorsController(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid chargeStationId)
    {
        return Ok(await _connectorService.GetAllConnectorsOfChargeStationAsync(chargeStationId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid chargeStationId, int id)
    {
        var connector = await _connectorService.GetConnectorAsync(chargeStationId, id);
        if (connector is null)
        {
            return NotFound();
        }
        return Ok(connector);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid chargeStationId, [FromBody] CreateOrUpdateConnectorDto connectorDto)
    {
        var createdConnector = await _connectorService.CreateConnector(chargeStationId, connectorDto);
        return CreatedAtAction(nameof(Get), new { chargeStationId, id = createdConnector.Id }, createdConnector);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid chargeStationId, int id, [FromBody] CreateOrUpdateConnectorDto connectorDto)
    {
        _ = await _connectorService.UpdateConnectorAsync(chargeStationId, id, connectorDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid chargeStationId, int id)
    {
        await _connectorService.DeleteConnectorAsync(chargeStationId, id);
        return NoContent();
    }
}
