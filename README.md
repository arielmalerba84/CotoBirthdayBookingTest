# CotoBirthdayBooking - Tests Unitarios para ReservaService

Este repositorio contiene los tests unitarios para el servicio de reservas de la API de gestión de salones de cumpleaños (`ReservaService`).

---

## Descripción

Los tests cubren los principales casos de validación de la lógica de negocio para la creación de reservas:

- Validación de solapamiento de reservas.
- Prevención de reservas simultáneas para un mismo salón y horario.
- Otras validaciones claves pueden ser agregadas (como horarios válidos, salón válido, etc.).

Los tests están escritos con:

- **xUnit** como framework de pruebas.
- **Moq** para mockear las dependencias (repositorios).
- **AutoMapper** para el mapeo de entidades y DTOs.

---

## Estructura del repositorio

- `ReservaServiceTests.cs`: Tests unitarios para la lógica del servicio de reservas.
- (Opcional) Otros archivos de configuración para el entorno de pruebas.

---

## Cómo ejecutar los tests

1. Clonar el repositorio

```bash
git clone https://github.com/arielmalerba84/CotoBirthdayBookingTest.git

