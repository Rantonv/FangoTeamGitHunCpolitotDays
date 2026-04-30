using ReservasSalas.Models;

namespace ReservasSalas.DTOs;

/// <summary>
/// DTO para la respuesta con la información de una reserva
/// </summary>
public class ReservaResponseDto
{
    public int Id { get; set; }
    
    public int SalaId { get; set; }
    
    public string SalaNombre { get; set; } = string.Empty;
    
    public string UsuarioEmail { get; set; } = string.Empty;
    
    public string UsuarioNombre { get; set; } = string.Empty;
    
    public DateTime FechaHoraInicio { get; set; }
    
    public DateTime FechaHoraFin { get; set; }
    
    public int DuracionMinutos { get; set; }
    
    public int NumeroAsistentes { get; set; }
    
    public string? Motivo { get; set; }
    
    public string Estado { get; set; } = string.Empty;
    
    public DateTime FechaCreacion { get; set; }
    
    public bool PuedeCancelarse { get; set; }
    
    /// <summary>
    /// Mapea una entidad Reserva a un DTO de respuesta
    /// </summary>
    public static ReservaResponseDto FromReserva(Reserva reserva)
    {
        return new ReservaResponseDto
        {
            Id = reserva.Id,
            SalaId = reserva.SalaId,
            SalaNombre = reserva.Sala?.Nombre ?? string.Empty,
            UsuarioEmail = reserva.UsuarioEmail,
            UsuarioNombre = reserva.UsuarioNombre,
            FechaHoraInicio = reserva.FechaHoraInicio,
            FechaHoraFin = reserva.FechaHoraFin,
            DuracionMinutos = reserva.DuracionMinutos,
            NumeroAsistentes = reserva.NumeroAsistentes,
            Motivo = reserva.Motivo,
            Estado = reserva.Estado.ToString(),
            FechaCreacion = reserva.FechaCreacion,
            PuedeCancelarse = reserva.PuedeCancelarse()
        };
    }
}
