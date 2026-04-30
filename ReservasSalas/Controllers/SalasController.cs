using Microsoft.AspNetCore.Mvc;
using ReservasSalas.DTOs;
using ReservasSalas.Services;

namespace ReservasSalas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalasController : ControllerBase
{
    private readonly IReservasService _reservasService;

    public SalasController(IReservasService reservasService)
    {
        _reservasService = reservasService;
    }

    /// <summary>
    /// Obtiene todas las salas disponibles
    /// </summary>
    /// <returns>Lista de salas</returns>
    /// <response code="200">Lista de salas</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SalaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerSalas()
    {
        var salas = await _reservasService.ObtenerSalasAsync();
        return Ok(salas);
    }

    /// <summary>
    /// Obtiene una sala por su ID
    /// </summary>
    /// <param name="id">ID de la sala</param>
    /// <returns>Datos de la sala</returns>
    /// <response code="200">Sala encontrada</response>
    /// <response code="404">Sala no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SalaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerSala(int id)
    {
        var sala = await _reservasService.ObtenerSalaPorIdAsync(id);

        if (sala == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Sala no encontrada",
                Status = StatusCodes.Status404NotFound,
                Detail = $"No existe una sala con ID {id}",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(sala);
    }
}
