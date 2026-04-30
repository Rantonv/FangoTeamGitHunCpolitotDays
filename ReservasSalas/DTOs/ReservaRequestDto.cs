using System.ComponentModel.DataAnnotations;

namespace ReservasSalas.DTOs;

/// <summary>
/// DTO para la solicitud de creación de una reserva
/// </summary>
public class ReservaRequestDto
{
    [Required(ErrorMessage = "El ID de la sala es requerido")]
    public int SalaId { get; set; }
    
    [Required(ErrorMessage = "El email del usuario es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public required string UsuarioEmail { get; set; }
    
    [Required(ErrorMessage = "El nombre del usuario es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public required string UsuarioNombre { get; set; }
    
    [Required(ErrorMessage = "La fecha y hora de inicio es requerida")]
    public DateTime FechaHoraInicio { get; set; }
    
    [Required(ErrorMessage = "La duración es requerida")]
    [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 minutos y 8 horas")]
    public int DuracionMinutos { get; set; }
    
    [Required(ErrorMessage = "El número de asistentes es requerido")]
    [Range(1, 100, ErrorMessage = "El número de asistentes debe estar entre 1 y 100")]
    public int NumeroAsistentes { get; set; }
    
    [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
    public string? Motivo { get; set; }
}
