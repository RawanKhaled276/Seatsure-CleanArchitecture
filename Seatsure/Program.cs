using Microsoft.EntityFrameworkCore;
using Seatsure.Application.Interfaces;
using Seatsure.Application.Security;
using Seatsure.Application.Services;
using Seatsure.Application.Services.Implementations;
using Seatsure.Application.Services.Interfaces;
using Seatsure.Infrastructure;
using Seatsure.Infrastructure.Data;
using Seatsure.Infrastructure.Repositories;

namespace Seatsure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // JWT configuration
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("Jwt"));

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

            // Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            // Security
            builder.Services.AddScoped<IJwtService, JwtService>();

            // Controllers & Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}