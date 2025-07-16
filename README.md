#  Birthday Booking API

API REST para la gestión de reservas de salones de cumpleaños.

---

##  Tecnologías

- **.NET 8**
- **EF Core InMemory**
- **Swagger**
- Arquitectura por capas
- AutoMapper
- Docker (opcional, no se usa RabbitMQ para este desafío)

---

##  Cómo correr el proyecto

### Opción 1: Desde Visual Studio / VS Code

1. Clonar el repo:
   ```bash
   git clone https://github.com/tu-usuario/birthday-booking.git
   cd birthday-booking
   ```

2. Ejecutar el proyecto `CotoBirthdayBooking.Api`.

3. Acceder a Swagger en:
   ```
  
   https://localhost:7285/swagger/index.html```

### Opción 2: Usando Docker

```bash
docker build -t birthdaybooking-api -f CotoBirthdayBooking.Api/Dockerfile .
docker run -p 8082:8081 birthdaybooking-api
```
 http://localhost:8082/swagger
---

## 🧪 Endpoints

### Crear una reserva

`POST /api/reserva`

```json
{
  "fecha": "2025-07-15",
  "horaInicio": "10:00:00",
  "horaFin": "12:00:00",
  "salonId": 1
}
```

✅ Reglas:

- Horario entre **09:00 y 18:00 hs**
- 30 minutos de margen entre eventos
- No se permiten solapamientos
- horaFin debe ser mayor a horaInicio

---

### Consultar reservas por fecha

`GET /api/reserva/2025-07-15`

Respuesta:

```json
[
  {
    "id": 1,
    "fecha": "2025-07-15",
    "horaInicio": "10:00:00",
    "horaFin": "12:00:00",
    "salonId": 1
  }
]
```

---

##  Manejo de concurrencia

En esta implementación **in-memory**, se utiliza un **`lock` estático** 
para garantizar que mientras una reserva se está procesando, 
las demás esperan, evitando así superposición de horarios.

---

##  Pruebas unitarias

Se encuentran en el proyecto `CotoBirthdayBooking.Tests`.

Ejecutar:

```bash
dotnet test
```

---

## Consideraciones

- No se usa RabbitMQ (se quitó para simplificar ejecución)
- Patrón Repository + Service
- Validaciones centralizadas
- Preparado para correr en contenedor o sin Docker

