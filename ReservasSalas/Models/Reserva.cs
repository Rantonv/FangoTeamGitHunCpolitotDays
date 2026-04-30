namespace ReservasSalas.Models;

/// <summary>
/// Representa una reserva de sala de reunión
/// </summary>
public class Reserva
{
    public int Id { get; set; }
    
    public int SalaId { get; set; }
    
    public required string UsuarioEmail { get; set; }
    
    public required string UsuarioNombre { get; set; }
    
    /// <summary>
    /// Fecha y hora de inicio de la reserva
    /// </summary>
    public DateTime FechaHoraInicio { get; set; }
    
    /// <summary>
    /// Duración de la reserva en minutos
    /// </summary>
    public int DuracionMinutos { get; set; }
    
    /// <summary>
    /// Fecha y hora de fin calculada (FechaHoraInicio + DuracionMinutos)
    /// </summary>
    public DateTime FechaHoraFin => FechaHoraInicio.AddMinutes(DuracionMinutos);
    
    /// <summary>
    /// Número de asistentes esperados
    /// </summary>
    public int NumeroAsistentes { get; set; }
    
    public string? Motivo { get; set; }
    
    /// <summary>
    /// Estado de la reserva
    /// </summary>
    public EstadoReserva Estado { get; set; } = EstadoReserva.Confirmada;
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public DateTime? FechaCancelacion { get; set; }
    
    /// <summary>
    /// Navegación a la sala reservada
    /// </summary>
    public Sala? Sala { get; set; }
    
    /// <summary>
    /// Verifica si la reserva puede ser cancelada (al menos 1 hora antes del inicio)
    /// </summary>
    public bool PuedeCancelarse()
    {
        return Estado == EstadoReserva.Confirmada && 
               FechaHoraInicio.Subtract(DateTime.UtcNow).TotalHours >= 1;
    }
    
    /// <summary>
    /// Verifica si esta reserva se solapa con otra en el mismo periodo de tiempo
    /// </summary>
    public bool SeSolapaCon(DateTime inicioOtra, DateTime finOtra)
    {
        return FechaHoraInicio < finOtra && FechaHoraFin > inicioOtra;
    }
}

/// <summary>
/// Estados posibles de una reserva
/// </summary>
public enum EstadoReserva
{
    Confirmada,
    Cancelada,
    Completada
}
