using ReservasSalas.DTOs;
using ReservasSalas.Models;

namespace ReservasSalas.Services;

/// <summary>
/// Servicio de gestión de reservas con almacenamiento en memoria
/// </summary>
public class ReservasService : IReservasService
{
    private readonly List<Sala> _salas;
    private readonly List<Reserva> _reservas;
    private int _nextReservaId = 1;

    public ReservasService()
    {
        // Inicializar salas de ejemplo
        _salas = new List<Sala>
        {
            new Sala
            {
                Id = 1,
                Nombre = "Sala Ejecutiva",
                Ubicacion = "Piso 5 - Ala Norte",
                CapacidadMaxima = 10,
                Descripcion = "Sala equipada para reuniones ejecutivas",
                Equipamiento = new List<string> { "Proyector", "Pizarra", "Videoconferencia", "Café" },
                Activa = true
            },
            new Sala
            {
                Id = 2,
                Nombre = "Sala Creativa",
                Ubicacion = "Piso 3 - Ala Sur",
                CapacidadMaxima = 6,
                Descripcion = "Espacio informal para brainstorming",
                Equipamiento = new List<string> { "Pizarra", "TV", "Sillones" },
                Activa = true
            },
            new Sala
            {
                Id = 3,
                Nombre = "Auditorio",
                Ubicacion = "Planta Baja",
                CapacidadMaxima = 50,
                Descripcion = "Sala grande para presentaciones y eventos",
                Equipamiento = new List<string> { "Proyector", "Sistema de audio", "Videoconferencia", "Micrófono" },
                Activa = true
            },
            new Sala
            {
                Id = 4,
                Nombre = "Sala de Juntas",
                Ubicacion = "Piso 6 - Dirección",
                CapacidadMaxima = 12,
                Descripcion = "Sala formal para reuniones de alto nivel",
                Equipamiento = new List<string> { "Proyector", "Videoconferencia", "Mesa de juntas", "Teléfono" },
                Activa = true
            }
        };

        _reservas = new List<Reserva>();
    }

    public Task<(bool Success, ReservaResponseDto? Reserva, string? Error)> CrearReservaAsync(ReservaRequestDto request)
    {
        // Validar que la fecha de inicio no sea en el pasado
        if (request.FechaHoraInicio < DateTime.UtcNow)
        {
            return Task.FromResult<(bool, ReservaResponseDto?, string?)>((false, null, "La fecha de inicio no puede ser en el pasado"));
        }

        // Buscar la sala
        var sala = _salas.FirstOrDefault(s => s.Id == request.SalaId);
        if (sala == null)
        {
            return Task.FromResult<(bool, ReservaResponseDto?, string?)>((false, null, "La sala especificada no existe"));
        }

        if (!sala.Activa)
        {
            return Task.FromResult<(bool, ReservaResponseDto?, string?)>((false, null, "La sala no está activa"));
        }

        // VALIDACIÓN 1: Verificar capacidad máxima
        if (request.NumeroAsistentes > sala.CapacidadMaxima)
        {
            return Task.FromResult<(bool, ReservaResponseDto?, string?)>(
                (false, null, $"El número de asistentes ({request.NumeroAsistentes}) excede la capacidad máxima de la sala ({sala.CapacidadMaxima})"));
        }

        // Calcular fecha de fin
        var fechaHoraFin = request.FechaHoraInicio.AddMinutes(request.DuracionMinutos);

        // VALIDACIÓN 2: Verificar solapamiento con otras reservas de la misma sala
        var reservasExistentes = _reservas
            .Where(r => r.SalaId == request.SalaId && r.Estado == EstadoReserva.Confirmada)
            .ToList();

        foreach (var reservaExistente in reservasExistentes)
        {
            if (reservaExistente.SeSolapaCon(request.FechaHoraInicio, fechaHoraFin))
            {
                return Task.FromResult<(bool, ReservaResponseDto?, string?)>(
                    (false, null, 
                     $"La sala no está disponible en ese horario. Existe un solapamiento con una reserva de {reservaExistente.FechaHoraInicio:dd/MM/yyyy HH:mm} a {reservaExistente.FechaHoraFin:dd/MM/yyyy HH:mm}"));
            }
        }

        // Crear la nueva reserva
        var nuevaReserva = new Reserva
        {
            Id = _nextReservaId++,
            SalaId = request.SalaId,
            UsuarioEmail = request.UsuarioEmail,
            UsuarioNombre = request.UsuarioNombre,
            FechaHoraInicio = request.FechaHoraInicio,
            DuracionMinutos = request.DuracionMinutos,
            NumeroAsistentes = request.NumeroAsistentes,
            Motivo = request.Motivo,
            Estado = EstadoReserva.Confirmada,
            FechaCreacion = DateTime.UtcNow,
            Sala = sala
        };

        _reservas.Add(nuevaReserva);

        var response = ReservaResponseDto.FromReserva(nuevaReserva);
        return Task.FromResult<(bool, ReservaResponseDto?, string?)>((true, response, null));
    }

    public Task<(bool Success, string? Error)> CancelarReservaAsync(int reservaId)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == reservaId);
        
        if (reserva == null)
        {
            return Task.FromResult<(bool, string?)>((false, "La reserva no existe"));
        }

        if (reserva.Estado == EstadoReserva.Cancelada)
        {
            return Task.FromResult<(bool, string?)>((false, "La reserva ya está cancelada"));
        }

        if (reserva.Estado == EstadoReserva.Completada)
        {
            return Task.FromResult<(bool, string?)>((false, "No se puede cancelar una reserva completada"));
        }

        // VALIDACIÓN: Verificar que falten más de 60 minutos (1 hora) para el inicio
        var tiempoRestante = reserva.FechaHoraInicio.Subtract(DateTime.UtcNow);
        if (tiempoRestante.TotalMinutes < 60)
        {
            return Task.FromResult<(bool, string?)>((false, 
                $"No se puede cancelar la reserva. Debe hacerlo con al menos 1 hora de anticipación. Tiempo restante: {tiempoRestante.TotalMinutes:F0} minutos"));
        }

        // Cancelar la reserva
        reserva.Estado = EstadoReserva.Cancelada;
        reserva.FechaCancelacion = DateTime.UtcNow;

        return Task.FromResult<(bool, string?)>((true, null));
    }

    public Task<IEnumerable<ReservaResponseDto>> ObtenerReservasAsync()
    {
        var reservas = _reservas
            .Select(r => 
            {
                r.Sala ??= _salas.FirstOrDefault(s => s.Id == r.SalaId);
                return ReservaResponseDto.FromReserva(r);
            })
            .OrderByDescending(r => r.FechaHoraInicio)
            .ToList();

        return Task.FromResult<IEnumerable<ReservaResponseDto>>(reservas);
    }

    public Task<ReservaResponseDto?> ObtenerReservaPorIdAsync(int id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id);
        if (reserva == null)
        {
            return Task.FromResult<ReservaResponseDto?>(null);
        }

        reserva.Sala ??= _salas.FirstOrDefault(s => s.Id == reserva.SalaId);
        return Task.FromResult<ReservaResponseDto?>(ReservaResponseDto.FromReserva(reserva));
    }

    public Task<IEnumerable<SalaResponseDto>> ObtenerSalasAsync()
    {
        var salas = _salas
            .Where(s => s.Activa)
            .Select(SalaResponseDto.FromSala)
            .OrderBy(s => s.Nombre)
            .ToList();

        return Task.FromResult<IEnumerable<SalaResponseDto>>(salas);
    }

    public Task<SalaResponseDto?> ObtenerSalaPorIdAsync(int id)
    {
        var sala = _salas.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(sala != null ? SalaResponseDto.FromSala(sala) : null);
    }
}
