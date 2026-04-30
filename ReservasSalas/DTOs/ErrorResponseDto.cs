namespace ReservasSalas.DTOs;

/// <summary>
/// DTO para respuestas de error estandarizadas
/// </summary>
public class ErrorResponseDto
{
    public string Mensaje { get; set; } = string.Empty;
    
    public string? Detalle { get; set; }
    
    public Dictionary<string, string[]>? Errores { get; set; }
    
    public ErrorResponseDto(string mensaje, string? detalle = null)
    {
        Mensaje = mensaje;
        Detalle = detalle;
    }
}
