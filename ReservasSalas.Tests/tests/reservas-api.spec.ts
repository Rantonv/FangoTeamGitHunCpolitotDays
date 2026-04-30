import { test, expect } from '@playwright/test';

const API_BASE_URL = 'http://localhost:5000/api';

test.describe('API de Reservas de Salas', () => {
  
  test('1. Crear una reserva correctamente (201 Created)', async ({ request }) => {
    // Crear una reserva con fecha futura (mañana a las 10:00)
    const fechaReserva = new Date();
    fechaReserva.setDate(fechaReserva.getDate() + 1);
    fechaReserva.setHours(10, 0, 0, 0);

    const reservaRequest = {
      salaId: 1,
      usuarioEmail: "test@empresa.com",
      usuarioNombre: "Usuario Test",
      fechaHoraInicio: fechaReserva.toISOString(),
      duracionMinutos: 60,
      numeroAsistentes: 8,
      motivo: "Test de creación de reserva"
    };

    const response = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaRequest
    });

    // Verificar que la respuesta sea 201 Created
    expect(response.status()).toBe(201);

    const reservaCreada = await response.json();
    
    // Verificar que la respuesta contiene los datos correctos
    expect(reservaCreada).toHaveProperty('id');
    expect(reservaCreada.salaId).toBe(reservaRequest.salaId);
    expect(reservaCreada.usuarioEmail).toBe(reservaRequest.usuarioEmail);
    expect(reservaCreada.numeroAsistentes).toBe(reservaRequest.numeroAsistentes);
    expect(reservaCreada.estado).toBe('Confirmada');
    expect(reservaCreada.puedeCancelarse).toBe(true);

    console.log('✅ Reserva creada exitosamente:', reservaCreada);
  });

  test('2. Intentar crear una reserva solapada (409 Conflict)', async ({ request }) => {
    // Primero crear una reserva base
    const fechaBase = new Date();
    fechaBase.setDate(fechaBase.getDate() + 2);
    fechaBase.setHours(14, 0, 0, 0);

    const primeraReserva = {
      salaId: 2,
      usuarioEmail: "usuario1@empresa.com",
      usuarioNombre: "Primer Usuario",
      fechaHoraInicio: fechaBase.toISOString(),
      duracionMinutos: 90,
      numeroAsistentes: 4,
      motivo: "Primera reserva"
    };

    const response1 = await request.post(`${API_BASE_URL}/reservas`, {
      data: primeraReserva
    });

    expect(response1.status()).toBe(201);
    console.log('✅ Primera reserva creada para probar solapamiento');

    // Intentar crear una reserva que se solapa (30 minutos después, pero la primera dura 90)
    const fechaSolapada = new Date(fechaBase);
    fechaSolapada.setMinutes(fechaSolapada.getMinutes() + 30);

    const reservaSolapada = {
      salaId: 2, // Misma sala
      usuarioEmail: "usuario2@empresa.com",
      usuarioNombre: "Segundo Usuario",
      fechaHoraInicio: fechaSolapada.toISOString(),
      duracionMinutos: 60,
      numeroAsistentes: 3,
      motivo: "Intento de reserva solapada"
    };

    const response2 = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaSolapada
    });

    // Verificar que retorna 409 Conflict
    expect(response2.status()).toBe(409);

    const errorResponse = await response2.json();
    
    // Verificar que el error contiene información sobre el solapamiento
    expect(errorResponse).toHaveProperty('detail');
    expect(errorResponse.detail).toContain('solapamiento');
    expect(errorResponse.status).toBe(409);
    expect(errorResponse.title).toContain('Conflicto');

    console.log('✅ Solapamiento detectado correctamente:', errorResponse.detail);
  });

  test('3. Cancelar una reserva con más de 60 minutos de antelación (204 No Content)', async ({ request }) => {
    // Crear una reserva para luego cancelarla
    const fechaReserva = new Date();
    fechaReserva.setDate(fechaReserva.getDate() + 3);
    fechaReserva.setHours(16, 0, 0, 0);

    const reservaRequest = {
      salaId: 3,
      usuarioEmail: "cancelacion@empresa.com",
      usuarioNombre: "Usuario Cancelación",
      fechaHoraInicio: fechaReserva.toISOString(),
      duracionMinutos: 45,
      numeroAsistentes: 20,
      motivo: "Reserva para cancelar"
    };

    // Crear la reserva
    const createResponse = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaRequest
    });

    expect(createResponse.status()).toBe(201);
    const reservaCreada = await createResponse.json();
    const reservaId = reservaCreada.id;

    console.log(`✅ Reserva creada con ID ${reservaId} para cancelación`);

    // Cancelar la reserva
    const deleteResponse = await request.delete(`${API_BASE_URL}/reservas/${reservaId}`);

    // Verificar que retorna 204 No Content (cancelación exitosa)
    expect(deleteResponse.status()).toBe(204);

    console.log('✅ Reserva cancelada exitosamente (más de 60 minutos de antelación)');

    // Verificar que la reserva ahora aparece como cancelada
    const getResponse = await request.get(`${API_BASE_URL}/reservas/${reservaId}`);
    const reservaCancelada = await getResponse.json();
    
    expect(reservaCancelada.estado).toBe('Cancelada');
    expect(reservaCancelada.puedeCancelarse).toBe(false);
  });

  test('4. Intentar cancelar una reserva con menos de 60 minutos (409 Conflict)', async ({ request }) => {
    // Crear una reserva muy próxima (30 minutos en el futuro)
    const fechaCercana = new Date();
    fechaCercana.setMinutes(fechaCercana.getMinutes() + 30);

    const reservaRequest = {
      salaId: 4,
      usuarioEmail: "urgente@empresa.com",
      usuarioNombre: "Usuario Urgente",
      fechaHoraInicio: fechaCercana.toISOString(),
      duracionMinutos: 30,
      numeroAsistentes: 10,
      motivo: "Reserva urgente - menos de 1 hora"
    };

    // Crear la reserva
    const createResponse = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaRequest
    });

    expect(createResponse.status()).toBe(201);
    const reservaCreada = await createResponse.json();
    const reservaId = reservaCreada.id;

    console.log(`✅ Reserva creada con ID ${reservaId} (30 minutos en el futuro)`);

    // Intentar cancelar la reserva (debe fallar porque faltan menos de 60 minutos)
    const deleteResponse = await request.delete(`${API_BASE_URL}/reservas/${reservaId}`);

    // Verificar que retorna 409 Conflict (no se puede cancelar)
    expect(deleteResponse.status()).toBe(409);

    const errorResponse = await deleteResponse.json();
    
    // Verificar que el error menciona la restricción de tiempo
    expect(errorResponse).toHaveProperty('detail');
    expect(errorResponse.detail).toContain('1 hora de anticipación');
    expect(errorResponse.status).toBe(409);

    console.log('✅ Cancelación rechazada correctamente:', errorResponse.detail);
  });

  test('Validar exceso de capacidad en sala (409 Conflict)', async ({ request }) => {
    // Intentar crear una reserva que exceda la capacidad de la sala
    const fechaReserva = new Date();
    fechaReserva.setDate(fechaReserva.getDate() + 1);
    fechaReserva.setHours(11, 0, 0, 0);

    const reservaRequest = {
      salaId: 2, // Sala Creativa tiene capacidad máxima de 6
      usuarioEmail: "exceso@empresa.com",
      usuarioNombre: "Usuario Exceso",
      fechaHoraInicio: fechaReserva.toISOString(),
      duracionMinutos: 60,
      numeroAsistentes: 10, // Más de 6 (capacidad máxima)
      motivo: "Test de capacidad excedida"
    };

    const response = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaRequest
    });

    // Verificar que retorna 409 Conflict
    expect(response.status()).toBe(409);

    const errorResponse = await response.json();
    
    // Verificar que el error menciona la capacidad
    expect(errorResponse.detail).toContain('capacidad máxima');
    expect(errorResponse.status).toBe(409);

    console.log('✅ Exceso de capacidad detectado correctamente:', errorResponse.detail);
  });

  test('Listar todas las salas disponibles', async ({ request }) => {
    const response = await request.get(`${API_BASE_URL}/salas`);

    expect(response.status()).toBe(200);

    const salas = await response.json();
    
    // Verificar que retorna un array
    expect(Array.isArray(salas)).toBe(true);
    expect(salas.length).toBeGreaterThan(0);

    // Verificar estructura de una sala
    const sala = salas[0];
    expect(sala).toHaveProperty('id');
    expect(sala).toHaveProperty('nombre');
    expect(sala).toHaveProperty('capacidadMaxima');
    expect(sala).toHaveProperty('equipamiento');

    console.log(`✅ Se encontraron ${salas.length} salas disponibles`);
  });

  test('Obtener una sala específica', async ({ request }) => {
    const salaId = 1;
    const response = await request.get(`${API_BASE_URL}/salas/${salaId}`);

    expect(response.status()).toBe(200);

    const sala = await response.json();
    
    expect(sala.id).toBe(salaId);
    expect(sala).toHaveProperty('nombre');
    expect(sala).toHaveProperty('ubicacion');
    expect(sala).toHaveProperty('capacidadMaxima');

    console.log('✅ Sala obtenida:', sala.nombre);
  });

  test('Validar datos de entrada inválidos (400 Bad Request)', async ({ request }) => {
    // Intentar crear reserva sin campos requeridos
    const reservaInvalida = {
      salaId: 1,
      // Faltan campos requeridos
      duracionMinutos: 60,
      numeroAsistentes: 5
    };

    const response = await request.post(`${API_BASE_URL}/reservas`, {
      data: reservaInvalida
    });

    // Verificar que retorna 400 Bad Request por validación
    expect(response.status()).toBe(400);

    const errorResponse = await response.json();
    expect(errorResponse).toHaveProperty('errors');

    console.log('✅ Validación de datos inválidos funcionando correctamente');
  });
});
