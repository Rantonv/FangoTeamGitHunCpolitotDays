using Microsoft.AspNetCore.Mvc;
using ReservasSalas.DTOs;
using ReservasSalas.Services;

namespace ReservasSalas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReservasController : ControllerBase
{
    private readonly IReservasService _reservasService;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(IReservasService reservasService, ILogger<ReservasController> logger)
    {
        _reservasService = reservasService;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva reserva de sala
    /// </summary>
    /// <param name="request">Datos de la reserva</param>
    /// <returns>Reserva creada</returns>
    /// <response code="201">Reserva creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="409">Conflicto: solapamiento con otra reserva o capacidad excedida</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CrearReserva([FromBody] ReservaRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (success, reserva, error) = await _reservasService.CrearReservaAsync(request);

        if (!success)
        {
            _logger.LogWarning("Error al crear reserva: {Error}", error);

            // Determinar el tipo de error para el código de estado correcto
            if (error!.Contains("solapamiento", StringComparison.OrdinalIgnoreCase) ||
                error.Contains("no está disponible", StringComparison.OrdinalIgnoreCase) ||
                error.Contains("excede la capacidad", StringComparison.OrdinalIgnoreCase))
            {
                // 409 Conflict para solapamientos y problemas de capacidad
                return Conflict(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Title = "Conflicto al crear la reserva",
                    Status = StatusCodes.Status409Conflict,
                    Detail = error,
                    Instance = HttpContext.Request.Path
                });
            }

            // 400 Bad Request para otros errores de validación
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Error de validación",
                Status = StatusCodes.Status400BadRequest,
                Detail = error,
                Instance = HttpContext.Request.Path
            });
        }

        _logger.LogInformation(
            "Reserva creada: ID={ReservaId}, Sala={SalaId}, Usuario={Usuario}, Fecha={Fecha}",
            reserva!.Id, reserva.SalaId, reserva.UsuarioEmail, reserva.FechaHoraInicio);

        return CreatedAtAction(
            nameof(ObtenerReserva),
            new { id = reserva.Id },
            reserva);
    }

    /// <summary>
    /// Cancela una reserva existente
    /// </summary>
    /// <param name="id">ID de la reserva a cancelar</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="204">Reserva cancelada exitosamente</response>
    /// <response code="404">Reserva no encontrada</response>
    /// <response code="409">No se puede cancelar: falta menos de 1 hora para el inicio</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        var (success, error) = await _reservasService.CancelarReservaAsync(id);

        if (!success)
        {
            _logger.LogWarning("Error al cancelar reserva {ReservaId}: {Error}", id, error);

            // Si la reserva no existe, devolver 404
            if (error!.Contains("no existe", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Reserva no encontrada",
                    Status = StatusCodes.Status404NotFound,
                    Detail = error,
                    Instance = HttpContext.Request.Path
                });
            }

            // Para errores de tiempo de anticipación o estado, devolver 409 Conflict
            return Conflict(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "No se puede cancelar la reserva",
                Status = StatusCodes.Status409Conflict,
                Detail = error,
                Instance = HttpContext.Request.Path
            });
        }

        _logger.LogInformation("Reserva cancelada exitosamente: ID={ReservaId}", id);

        return NoContent();
    }

    /// <summary>
    /// Obtiene todas las reservas
    /// </summary>
    /// <returns>Lista de reservas</returns>
    /// <response code="200">Lista de reservas</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReservaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerReservas()
    {
        var reservas = await _reservasService.ObtenerReservasAsync();
        return Ok(reservas);
    }

    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    /// <param name="id">ID de la reserva</param>
    /// <returns>Datos de la reserva</returns>
    /// <response code="200">Reserva encontrada</response>
    /// <response code="404">Reserva no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReservaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerReserva(int id)
    {
        var reserva = await _reservasService.ObtenerReservaPorIdAsync(id);

        if (reserva == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Reserva no encontrada",
                Status = StatusCodes.Status404NotFound,
                Detail = $"No existe una reserva con ID {id}",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(reserva);
    }
}
