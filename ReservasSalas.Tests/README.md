# Tests de Playwright para API de Reservas

Este proyecto contiene tests automatizados con Playwright para validar la API REST de reservas de salas.

## 📋 Pre-requisitos

- Node.js instalado
- La API debe estar ejecutándose en `http://localhost:5000`

## 🚀 Configuración Inicial

1. Instalar las dependencias de Node:
```powershell
cd ReservasSalas.Tests
npm install
```

2. Instalar los navegadores de Playwright (solo la primera vez):
```powershell
npx playwright install
```

## ▶️ Ejecutar la API

Desde la carpeta raíz del proyecto, ejecutar:

```powershell
cd ReservasSalas
dotnet run --launch-profile http
```

La API se ejecutará en `http://localhost:5000`

## 🧪 Ejecutar los Tests

### Todos los tests de la API
```powershell
cd ReservasSalas.Tests
npm run test:api
```

### Todos los tests del proyecto
```powershell
npm test
```

### Tests en modo visual (headed)
```powershell
npm run test:headed
```

### Tests en modo debug
```powershell
npm run test:debug
```

### Ver el reporte de tests
```powershell
npm run report
```

## 📝 Tests Implementados

### 1. ✅ Crear una reserva correctamente (201 Created)
Valida que se puede crear una reserva válida y que retorna 201 con los datos correctos.

### 2. ❌ Intentar crear una reserva solapada (409 Conflict)
Crea dos reservas en la misma sala con horarios que se solapan y valida que la segunda es rechazada con 409.

### 3. ✅ Cancelar una reserva con más de 60 minutos de antelación (204 No Content)
Crea una reserva con suficiente tiempo de anticipación y valida que se puede cancelar correctamente.

### 4. ❌ Intentar cancelar una reserva con menos de 60 minutos (409 Conflict)
Crea una reserva próxima (30 minutos) y valida que no se puede cancelar, retornando 409.

### Tests adicionales:

- **Validar exceso de capacidad**: Intenta reservar una sala con más asistentes que su capacidad máxima (409).
- **Listar salas disponibles**: Obtiene todas las salas activas (200).
- **Obtener sala específica**: Obtiene los datos de una sala por ID (200).
- **Validar datos inválidos**: Intenta crear reserva sin campos requeridos (400).

## 📊 Estructura de Tests

```
ReservasSalas.Tests/
├── tests/
│   ├── reservas-api.spec.ts    ← Tests de la API
│   └── example.spec.ts          ← Tests de ejemplo de Playwright
├── playwright.config.ts          ← Configuración de Playwright
└── package.json                  ← Dependencias y scripts
```

## 🎯 Códigos de Estado HTTP Validados

- **200 OK**: Consultas exitosas
- **201 Created**: Reserva creada
- **204 No Content**: Cancelación exitosa
- **400 Bad Request**: Validación de datos fallida
- **404 Not Found**: Recurso no encontrado
- **409 Conflict**: Solapamiento o capacidad excedida

## 🔍 Ver Resultados

Después de ejecutar los tests, se generará un reporte HTML. Para verlo:

```powershell
npm run report
```

Este comando abrirá el navegador con un reporte detallado de la ejecución de todos los tests.
