using Xunit;
using Moq;
using AutoMapper;
using CotoBirthDayBooking.Application.Service;
using CotoBirthDayBooking.Domain.Interfaces;
using CotoBirthDayBooking.Domain.Models;
using CotoBirthDayBooking.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CotoBirthdayBooking.Tests.Services
{
    public class ReservaServiceTests
    {
        private readonly Mock<IReservaRepository> _mockRepo;  // Mock del repositorio para controlar respuestas simuladas
        private readonly IMapper _mapper;                       // Mapper para convertir entidades a DTOs
        private readonly ReservaService _service;               // Servicio bajo prueba

        public ReservaServiceTests()
        {
            // Crear mock para el repositorio
            _mockRepo = new Mock<IReservaRepository>();

            // Configurar AutoMapper para mapear Reserva a ReservaResponse
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Reserva, ReservaResponse>();
            });
            _mapper = config.CreateMapper();

            // Instanciar el servicio con el mock y el mapper
            _service = new ReservaService(_mockRepo.Object, _mapper);
        }

        // Test: Debe lanzar excepción si la reserva se solapa con otra existente
        [Fact]
        public async Task CrearReservaAsync_DeberiaLanzarExcepcion_SiHaySolapamiento()
        {
            var reserva = new Reserva
            {
                Fecha = DateTime.Today,
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                SalonId = 1
            };

            // Simular que ya existe reserva solapada (retorna true)
            _mockRepo.Setup(r => r.ExisteReservaSolapadaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), reserva.SalonId))
                     .ReturnsAsync(true);

            // Simular la transacción (no hace nada real)
            _mockRepo.Setup(r => r.BeginTransactionAsync(It.IsAny<System.Data.IsolationLevel>()))
                     .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());

            // Ejecutar y esperar que lance excepción por solapamiento
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CrearReservaAsync(reserva));
            Assert.Equal("La reserva se solapa con otra existente o no respeta los 30 minutos de margen.", ex.Message);
        }

        // Test: No debería permitir dos reservas simultáneas usando el lock para sincronización
        [Fact]
        public async Task CrearReservaAsync_NoDeberiaPermitirDosReservasSimultaneas_UsandoLock()
        {
            var reserva1 = new Reserva
            {
                Fecha = DateTime.Today,
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                SalonId = 1
            };

            var reserva2 = new Reserva
            {
                Fecha = DateTime.Today,
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                SalonId = 1
            };

            // Mockear transacción vacía
            _mockRepo.Setup(r => r.BeginTransactionAsync(It.IsAny<System.Data.IsolationLevel>()))
                     .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());

            // Primer chequeo de solapamiento: falso (permite reserva)
            // Segundo chequeo: verdadero (no permite reserva)
            _mockRepo.SetupSequence(r => r.ExisteReservaSolapadaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), reserva1.SalonId))
                     .ReturnsAsync(false)
                     .ReturnsAsync(true);

            // Simular que GenerarReservaAsync devuelve la reserva que recibe
            _mockRepo.Setup(r => r.GenerarReservaAsync(It.IsAny<Reserva>()))
                     .ReturnsAsync((Reserva r) => r);

            // Ejecutar ambas reservas casi simultáneamente en tareas paralelas
            var task1 = Task.Run(() => _service.CrearReservaAsync(reserva1));
            var task2 = Task.Run(() => _service.CrearReservaAsync(reserva2));

            // Esperar que la primera tarea termine sin error
            var result1 = await task1;
            Assert.NotNull(result1);

            // Esperar que la segunda tarea lance excepción por solapamiento
            var ex = await Assert.ThrowsAsync<Exception>(() => task2);
            Assert.Equal("La reserva se solapa con otra existente o no respeta los 30 minutos de margen.", ex.Message);
        }

        
    }
}
