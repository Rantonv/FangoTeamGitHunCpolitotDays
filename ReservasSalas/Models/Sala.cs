namespace ReservasSalas.Models;

/// <summary>
/// Representa una sala de reunión disponible para reservas
/// </summary>
public class Sala
{
    public int Id { get; set; }
    
    public required string Nombre { get; set; }
    
    public required string Ubicacion { get; set; }
    
    /// <summary>
    /// Capacidad máxima de personas que pueden ocupar la sala
    /// </summary>
    public int CapacidadMaxima { get; set; }
    
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Equipamiento disponible (proyector, pizarra, videoconferencia, etc.)
    /// </summary>
    public List<string> Equipamiento { get; set; } = new();
    
    public bool Activa { get; set; } = true;
    
    /// <summary>
    /// Navegación a las reservas de esta sala
    /// </summary>
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
