using ReservasSalas.DTOs;
using ReservasSalas.Models;

namespace ReservasSalas.Services;

/// <summary>
/// Interfaz para el servicio de gestión de reservas
/// </summary>
public interface IReservasService
{
    /// <summary>
    /// Crea una nueva reserva validando solapamiento y capacidad
    /// </summary>
    Task<(bool Success, ReservaResponseDto? Reserva, string? Error)> CrearReservaAsync(ReservaRequestDto request);
    
    /// <summary>
    /// Cancela una reserva existente validando el tiempo mínimo de 1 hora
    /// </summary>
    Task<(bool Success, string? Error)> CancelarReservaAsync(int reservaId);
    
    /// <summary>
    /// Obtiene todas las reservas
    /// </summary>
    Task<IEnumerable<ReservaResponseDto>> ObtenerReservasAsync();
    
    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    Task<ReservaResponseDto?> ObtenerReservaPorIdAsync(int id);
    
    /// <summary>
    /// Obtiene todas las salas disponibles
    /// </summary>
    Task<IEnumerable<SalaResponseDto>> ObtenerSalasAsync();
    
    /// <summary>
    /// Obtiene una sala por su ID
    /// </summary>
    Task<SalaResponseDto?> ObtenerSalaPorIdAsync(int id);
}
