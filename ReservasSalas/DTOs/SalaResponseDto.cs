using ReservasSalas.Models;

namespace ReservasSalas.DTOs;

/// <summary>
/// DTO para la respuesta con la información de una sala
/// </summary>
public class SalaResponseDto
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
    
    public string Ubicacion { get; set; } = string.Empty;
    
    public int CapacidadMaxima { get; set; }
    
    public string? Descripcion { get; set; }
    
    public List<string> Equipamiento { get; set; } = new();
    
    public bool Activa { get; set; }
    
    /// <summary>
    /// Mapea una entidad Sala a un DTO de respuesta
    /// </summary>
    public static SalaResponseDto FromSala(Sala sala)
    {
        return new SalaResponseDto
        {
            Id = sala.Id,
            Nombre = sala.Nombre,
            Ubicacion = sala.Ubicacion,
            CapacidadMaxima = sala.CapacidadMaxima,
            Descripcion = sala.Descripcion,
            Equipamiento = sala.Equipamiento,
            Activa = sala.Activa
        };
    }
}
