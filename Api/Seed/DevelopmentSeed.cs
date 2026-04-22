using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancieraSoluciones.Api.Seed
{
    public static class DevelopmentSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var db = provider.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = provider.GetRequiredService<IPasswordHasherService>();
            var configuration = provider.GetRequiredService<IConfiguration>();

            var adminUser = configuration["Seed:Admin:UsuarioAcceso"] ?? "admin";
            var adminPassword = configuration["Seed:Admin:Contrasena"] ?? "Admin123!";
            var adminNombre = configuration["Seed:Admin:Nombre"] ?? "Administrador";
            var adminApellidoPaterno = configuration["Seed:Admin:ApellidoPaterno"] ?? "Sistema";
            var adminApellidoMaterno = configuration["Seed:Admin:ApellidoMaterno"] ?? string.Empty;

            var clavePerfil = configuration["Seed:Admin:PerfilClave"] ?? "ADMIN";
            var nombrePerfil = configuration["Seed:Admin:PerfilNombre"] ?? "Administrador";

            var perfil = await db.Set<Perfil>().FirstOrDefaultAsync(p => p.Clave == clavePerfil);
            if (perfil == null)
            {
                perfil = new Perfil
                {
                    Id = Guid.NewGuid(),
                    Nombre = nombrePerfil,
                    Clave = clavePerfil,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    Orden = 0
                };

                db.Set<Perfil>().Add(perfil);
                await db.SaveChangesAsync();
            }

            var usuario = await db.Set<Usuario>().FirstOrDefaultAsync(u => u.UsuarioAcceso == adminUser);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = adminNombre,
                    ApellidoPaterno = adminApellidoPaterno,
                    ApellidoMaterno = adminApellidoMaterno,
                    UsuarioAcceso = adminUser,
                    Contrasena = passwordHasher.HashPassword(adminPassword),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    UltimoAcceso = null,
                    IdPerfil = perfil.Id,
                    RefreshToken = null,
                    RefreshTokenExpiryTime = null
                };

                db.Set<Usuario>().Add(usuario);
                await db.SaveChangesAsync();
            }
        }
    }
}

