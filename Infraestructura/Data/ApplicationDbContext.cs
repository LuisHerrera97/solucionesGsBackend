using System;
using System.Threading;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinancieraSoluciones.Infraestructura.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Pagina> Paginas { get; set; }
        public DbSet<Boton> Botones { get; set; }
        public DbSet<PermisoModulo> PermisosModulo { get; set; }
        public DbSet<PermisoPagina> PermisosPagina { get; set; }
        public DbSet<PermisoBoton> PermisosBoton { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }
        public DbSet<ZonaCobranza> ZonasCobranza { get; set; }
        public DbSet<AuditoriaEvento> AuditoriaEventos { get; set; }
        public DbSet<Feriado> Feriados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Credito> Creditos { get; set; }
        public DbSet<Ficha> Fichas { get; set; }
        public DbSet<MovimientoCaja> MovimientosCaja { get; set; }
        public DbSet<CorteCaja> CortesCaja { get; set; }

        public override int SaveChanges()
        {
            NormalizeDateTimesToUtc();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            NormalizeDateTimesToUtc();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeDateTimesToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            NormalizeDateTimesToUtc();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.ApellidoPaterno)
                    .HasColumnName("apellido_paterno")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.ApellidoMaterno)
                    .HasColumnName("apellido_materno")
                    .HasMaxLength(100)
                    .IsRequired(false);
                    
                entity.Property(e => e.UsuarioAcceso)
                    .HasColumnName("usuario_acceso")
                    .HasMaxLength(50)
                    .IsRequired();
                    
                entity.Property(e => e.Contrasena)
                    .HasColumnName("contrasena")
                    .HasMaxLength(255)
                    .IsRequired();
                    
                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);
                    
                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();
                    
                entity.Property(e => e.UltimoAcceso)
                    .HasColumnName("ultimo_acceso");

                entity.Property(e => e.PasswordChangedAt)
                    .HasColumnName("password_changed_at");

                entity.Property(e => e.PasswordExpiresAt)
                    .HasColumnName("password_expires_at");

                entity.Property(e => e.MustChangePassword)
                    .HasColumnName("must_change_password")
                    .HasDefaultValue(false);

                entity.Property(e => e.FailedLoginCount)
                    .HasColumnName("failed_login_count")
                    .HasDefaultValue(0);

                entity.Property(e => e.LockoutUntil)
                    .HasColumnName("lockout_until");
                    
                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil")
                    .IsRequired();

                entity.Property(e => e.IdZonaCobranza)
                    .HasColumnName("id_zona_cobranza")
                    .IsRequired(false);
                    
                entity.Property(e => e.RefreshToken)
                    .HasColumnName("refresh_token")
                    .HasMaxLength(255)
                    .IsRequired(false);
                    
                entity.Property(e => e.RefreshTokenExpiryTime)
                    .HasColumnName("refresh_token_expiry_time");

                entity.HasIndex(e => e.UsuarioAcceso)
                    .IsUnique();

                entity.HasIndex(e => e.IdPerfil);

                entity.HasIndex(e => e.Activo);

                entity.HasIndex(e => e.IdZonaCobranza);

                entity.HasOne(d => d.Perfil)
                    .WithMany()
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ZonaCobranza)
                    .WithMany()
                    .HasForeignKey(d => d.IdZonaCobranza)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.ToTable("perfiles");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.Clave)
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsRequired();
                    
                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);
                    
                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();
                    
                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Clave)
                    .IsUnique();

                entity.HasIndex(e => e.Activo);

                entity.HasIndex(e => e.Orden);
            });

            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.ToTable("modulos");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.Clave)
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsRequired();
                    
                entity.Property(e => e.Icono)
                    .HasColumnName("icono")
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);
                    
                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();
                    
                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Clave)
                    .IsUnique();

                entity.HasIndex(e => e.Activo);

                entity.HasIndex(e => e.Orden);
            });

            modelBuilder.Entity<Pagina>(entity =>
            {
                entity.ToTable("paginas");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.Clave)
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsRequired();
                    
                entity.Property(e => e.Ruta)
                    .HasColumnName("ruta")
                    .HasMaxLength(255)
                    .IsRequired();
                    
                entity.Property(e => e.IdModulo)
                    .HasColumnName("id_modulo")
                    .IsRequired();
                    
                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);

                entity.Property(e => e.EnMenu)
                    .HasColumnName("en_menu")
                    .HasDefaultValue(true);
                    
                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();
                    
                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Clave)
                    .IsUnique();

                entity.HasIndex(e => new { e.IdModulo, e.Orden });

                entity.HasIndex(e => e.Activo);

                entity.HasOne(d => d.Modulo)
                    .WithMany()
                    .HasForeignKey(d => d.IdModulo)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Boton>(entity =>
            {
                entity.ToTable("botones");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();
                    
                entity.Property(e => e.Clave)
                    .HasColumnName("clave")
                    .HasMaxLength(50)
                    .IsRequired();
                    
                entity.Property(e => e.IdPagina)
                    .HasColumnName("id_pagina")
                    .IsRequired();
                    
                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);
                    
                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();
                    
                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Clave)
                    .IsUnique();

                entity.HasIndex(e => new { e.IdPagina, e.Orden });

                entity.HasIndex(e => e.Activo);

                entity.HasOne(d => d.Pagina)
                    .WithMany()
                    .HasForeignKey(d => d.IdPagina)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PermisoModulo>(entity =>
            {
                entity.ToTable("permisos_modulo");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil")
                    .IsRequired();
                    
                entity.Property(e => e.IdModulo)
                    .HasColumnName("id_modulo")
                    .IsRequired();
                    
                entity.Property(e => e.TienePermiso)
                    .HasColumnName("tiene_permiso")
                    .HasDefaultValue(false);
                    
                entity.Property(e => e.FechaAsignacion)
                    .HasColumnName("fecha_asignacion")
                    .IsRequired();

                entity.HasIndex(e => new { e.IdPerfil, e.IdModulo })
                    .IsUnique();

                entity.HasIndex(e => e.IdModulo);

                entity.HasOne(d => d.Perfil)
                    .WithMany()
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                    
                entity.HasOne(d => d.Modulo)
                    .WithMany()
                    .HasForeignKey(d => d.IdModulo)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PermisoPagina>(entity =>
            {
                entity.ToTable("permisos_pagina");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil")
                    .IsRequired();
                    
                entity.Property(e => e.IdPagina)
                    .HasColumnName("id_pagina")
                    .IsRequired();
                    
                entity.Property(e => e.TienePermiso)
                    .HasColumnName("tiene_permiso")
                    .HasDefaultValue(false);
                    
                entity.Property(e => e.FechaAsignacion)
                    .HasColumnName("fecha_asignacion")
                    .IsRequired();

                entity.HasIndex(e => new { e.IdPerfil, e.IdPagina })
                    .IsUnique();

                entity.HasIndex(e => e.IdPagina);

                entity.HasOne(d => d.Perfil)
                    .WithMany()
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                    
                entity.HasOne(d => d.Pagina)
                    .WithMany()
                    .HasForeignKey(d => d.IdPagina)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PermisoBoton>(entity =>
            {
                entity.ToTable("permisos_boton");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .HasColumnName("id");
                    
                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil")
                    .IsRequired();
                    
                entity.Property(e => e.IdBoton)
                    .HasColumnName("id_boton")
                    .IsRequired();
                    
                entity.Property(e => e.TienePermiso)
                    .HasColumnName("tiene_permiso")
                    .HasDefaultValue(false);
                    
                entity.Property(e => e.FechaAsignacion)
                    .HasColumnName("fecha_asignacion")
                    .IsRequired();

                entity.HasIndex(e => new { e.IdPerfil, e.IdBoton })
                    .IsUnique();

                entity.HasIndex(e => e.IdBoton);

                entity.HasOne(d => d.Perfil)
                    .WithMany()
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                    
                entity.HasOne(d => d.Boton)
                    .WithMany()
                    .HasForeignKey(d => d.IdBoton)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ConfiguracionSistema>(entity =>
            {
                entity.ToTable("configuracion_sistema");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.MoraDiaria)
                    .HasColumnName("mora_diaria")
                    .HasDefaultValue(20);

                entity.Property(e => e.MoraSemanal)
                    .HasColumnName("mora_semanal")
                    .HasDefaultValue(150);

                entity.Property(e => e.DiasGraciaDiaria)
                    .HasColumnName("dias_gracia_diaria")
                    .HasDefaultValue(0);

                entity.Property(e => e.DiasGraciaSemanal)
                    .HasColumnName("dias_gracia_semanal")
                    .HasDefaultValue(0);

                entity.Property(e => e.TopeMoraDiaria)
                    .HasColumnName("tope_mora_diaria")
                    .HasDefaultValue(0);

                entity.Property(e => e.TopeMoraSemanal)
                    .HasColumnName("tope_mora_semanal")
                    .HasDefaultValue(0);

                entity.Property(e => e.TasaDiaria)
                    .HasColumnName("tasa_diaria")
                    .HasDefaultValue(0.15m);

                entity.Property(e => e.TasaSemanal)
                    .HasColumnName("tasa_semanal")
                    .HasDefaultValue(0.2m);

                entity.Property(e => e.MoraMensual)
                    .HasColumnName("mora_mensual")
                    .HasDefaultValue(500);

                entity.Property(e => e.DiasGraciaMensual)
                    .HasColumnName("dias_gracia_mensual")
                    .HasDefaultValue(0);

                entity.Property(e => e.TopeMoraMensual)
                    .HasColumnName("tope_mora_mensual")
                    .HasDefaultValue(0);

                entity.Property(e => e.TasaMensual)
                    .HasColumnName("tasa_mensual")
                    .HasDefaultValue(0.25m);

                entity.Property(e => e.DomingoInhabilDefault)
                    .HasColumnName("domingo_inhabil_default")
                    .HasDefaultValue(true);

                entity.Property(e => e.AplicarFeriadosDefault)
                    .HasColumnName("aplicar_feriados_default")
                    .HasDefaultValue(false);

                entity.Property(e => e.PasswordMinLength)
                    .HasColumnName("password_min_length")
                    .HasDefaultValue(8);

                entity.Property(e => e.PasswordRequireUpper)
                    .HasColumnName("password_require_upper")
                    .HasDefaultValue(true);

                entity.Property(e => e.PasswordRequireLower)
                    .HasColumnName("password_require_lower")
                    .HasDefaultValue(true);

                entity.Property(e => e.PasswordRequireDigit)
                    .HasColumnName("password_require_digit")
                    .HasDefaultValue(true);

                entity.Property(e => e.PasswordRequireSpecial)
                    .HasColumnName("password_require_special")
                    .HasDefaultValue(false);

                entity.Property(e => e.PasswordExpireDays)
                    .HasColumnName("password_expire_days")
                    .HasDefaultValue(90);

                entity.Property(e => e.PasswordHistoryCount)
                    .HasColumnName("password_history_count")
                    .HasDefaultValue(3);

                entity.Property(e => e.LockoutMaxFailedAttempts)
                    .HasColumnName("lockout_max_failed_attempts")
                    .HasDefaultValue(5);

                entity.Property(e => e.LockoutMinutes)
                    .HasColumnName("lockout_minutes")
                    .HasDefaultValue(15);

                entity.Property(e => e.FechaActualizacion)
                    .HasColumnName("fecha_actualizacion")
                    .IsRequired();
            });

            modelBuilder.Entity<ZonaCobranza>(entity =>
            {
                entity.ToTable("zonas_cobranza");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();

                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Activo);

                entity.HasIndex(e => e.Orden);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Apellido)
                    .HasColumnName("apellido")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion")
                    .HasMaxLength(255);

                entity.Property(e => e.Negocio)
                    .HasColumnName("negocio")
                    .HasMaxLength(180);

                entity.Property(e => e.Zona)
                    .HasColumnName("zona")
                    .HasMaxLength(100);
                
                entity.Property(e => e.IdZona)
                    .HasColumnName("id_zona");

                entity.Property(e => e.Estatus)
                    .HasColumnName("estatus")
                    .HasMaxLength(30)
                    .HasDefaultValue("Activo");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo")
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();

                entity.HasIndex(e => new { e.Activo, e.Estatus });
            });

            modelBuilder.Entity<Credito>(entity =>
            {
                entity.ToTable("creditos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.ClienteId)
                    .HasColumnName("cliente_id")
                    .IsRequired();

                entity.Property(e => e.Folio)
                    .HasColumnName("folio")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.PermitirDomingo)
                    .HasColumnName("permitir_domingo")
                    .HasDefaultValue(false);

                entity.Property(e => e.AplicarFeriados)
                    .HasColumnName("aplicar_feriados")
                    .HasDefaultValue(false);

                entity.Property(e => e.Monto)
                    .HasColumnName("monto")
                    .IsRequired();

                entity.Property(e => e.InteresTotal)
                    .HasColumnName("interes_total")
                    .IsRequired();

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .IsRequired();

                entity.Property(e => e.Cuota)
                    .HasColumnName("cuota")
                    .IsRequired();

                entity.Property(e => e.TotalFichas)
                    .HasColumnName("total_fichas")
                    .IsRequired();

                entity.Property(e => e.Pagado)
                    .HasColumnName("pagado")
                    .HasDefaultValue(0);

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Estatus)
                    .HasColumnName("estatus")
                    .HasMaxLength(30)
                    .HasDefaultValue("Activo");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .IsRequired();

                entity.Property(e => e.Observacion)
                    .HasColumnName("observacion")
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.HasIndex(e => e.ClienteId);

                entity.HasIndex(e => e.Folio)
                    .IsUnique();

                entity.HasIndex(e => e.Estatus);

                entity.HasIndex(e => new { e.Estatus, e.Tipo });

                entity.HasIndex(e => e.FechaCreacion);

                entity.HasOne(d => d.Cliente)
                    .WithMany()
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Ficha>(entity =>
            {
                entity.ToTable("fichas");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.CreditoId)
                    .HasColumnName("credito_id")
                    .IsRequired();

                entity.Property(e => e.Num)
                    .HasColumnName("num")
                    .IsRequired();

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .IsRequired();

                entity.Property(e => e.FechaPago)
                    .HasColumnName("fecha_pago");

                entity.Property(e => e.Hora)
                    .HasColumnName("hora")
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.Property(e => e.Folio)
                    .HasColumnName("folio")
                    .HasMaxLength(80)
                    .IsRequired();

                entity.Property(e => e.Capital)
                    .HasColumnName("capital")
                    .IsRequired();

                entity.Property(e => e.Interes)
                    .HasColumnName("interes")
                    .IsRequired();

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .IsRequired();

                entity.Property(e => e.AbonoAcumulado)
                    .HasColumnName("abono_acumulado")
                    .HasDefaultValue(0);

                entity.Property(e => e.MoraAcumulada)
                    .HasColumnName("mora_acumulada")
                    .HasDefaultValue(0);

                entity.Property(e => e.SaldoCap)
                    .HasColumnName("saldo_cap")
                    .IsRequired();

                entity.Property(e => e.SaldoPendiente)
                    .HasColumnName("saldo_pendiente")
                    .HasDefaultValue(0);

                entity.Property(e => e.Pagada)
                    .HasColumnName("pagada")
                    .HasDefaultValue(false);

                entity.Property(e => e.Cerrada)
                    .HasColumnName("cerrada")
                    .HasDefaultValue(false);

                entity.Property(e => e.FechaCierre)
                    .HasColumnName("fecha_cierre");

                entity.HasIndex(e => new { e.CreditoId, e.Num })
                    .IsUnique();

                entity.HasIndex(e => new { e.Pagada, e.Fecha });

                entity.HasIndex(e => new { e.CreditoId, e.Pagada });

                entity.HasOne(d => d.Credito)
                    .WithMany(p => p.Fichas)
                    .HasForeignKey(d => d.CreditoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CorteCaja>(entity =>
            {
                entity.ToTable("cortes_caja");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Folio)
                    .HasColumnName("folio")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .IsRequired();

                entity.Property(e => e.Hora)
                    .HasColumnName("hora")
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.Property(e => e.TotalTeorico)
                    .HasColumnName("total_teorico")
                    .IsRequired();

                entity.Property(e => e.TotalReal)
                    .HasColumnName("total_real")
                    .IsRequired();

                entity.Property(e => e.Diferencia)
                    .HasColumnName("diferencia")
                    .IsRequired();

                entity.Property(e => e.RealizadoPorId)
                    .HasColumnName("realizado_por_id")
                    .IsRequired();



                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.Folio).IsUnique();
            });

            modelBuilder.Entity<MovimientoCaja>(entity =>
            {
                entity.ToTable("movimientos_caja");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.CorteCajaId)
                    .HasColumnName("corte_caja_id");

                entity.Property(e => e.CobradorId)
                    .HasColumnName("cobrador_id");

                entity.Property(e => e.RecibidoCaja)
                    .HasColumnName("recibido_caja")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ReversaDeId)
                    .HasColumnName("reversa_de_id");

                entity.Property(e => e.IdempotencyKey)
                    .HasColumnName("idempotency_key")
                    .HasMaxLength(120)
                    .IsRequired(false);

                entity.Property(e => e.Tipo)
                    .HasColumnName("tipo")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Concepto)
                    .HasColumnName("concepto")
                    .HasMaxLength(255);

                entity.Property(e => e.Medio)
                    .HasColumnName("medio")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .IsRequired();

                entity.Property(e => e.MontoEfectivo)
                    .HasColumnName("monto_efectivo");

                entity.Property(e => e.MontoTransferencia)
                    .HasColumnName("monto_transferencia");

                entity.Property(e => e.Abono)
                    .HasColumnName("abono");

                entity.Property(e => e.Mora)
                    .HasColumnName("mora");

                entity.Property(e => e.CreditoId)
                    .HasColumnName("credito_id");

                entity.Property(e => e.NumeroFicha)
                    .HasColumnName("numero_ficha");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .IsRequired();

                entity.Property(e => e.Hora)
                    .HasColumnName("hora")
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.Property(e => e.RegistraCaja)
                    .HasColumnName("registra_caja")
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasIndex(e => new { e.CorteCajaId, e.Fecha });

                entity.HasIndex(e => e.Fecha);

                entity.HasIndex(e => e.CreditoId);

                entity.HasIndex(e => new { e.CreditoId, e.NumeroFicha });

                entity.HasIndex(e => e.CobradorId);

                entity.HasIndex(e => e.ReversaDeId);

                entity.HasIndex(e => e.IdempotencyKey)
                    .IsUnique();

                entity.HasOne(d => d.CorteCaja)
                    .WithMany(p => p.Movimientos)
                    .HasForeignKey(d => d.CorteCajaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Credito)
                    .WithMany()
                    .HasForeignKey(d => d.CreditoId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AuditoriaEvento>(entity =>
            {
                entity.ToTable("auditoria_eventos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
                entity.Property(e => e.Accion).HasColumnName("accion").HasMaxLength(60).IsRequired();
                entity.Property(e => e.EntidadTipo).HasColumnName("entidad_tipo").HasMaxLength(60);
                entity.Property(e => e.EntidadId).HasColumnName("entidad_id");
                entity.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
                entity.Property(e => e.Detalle).HasColumnName("detalle");

                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => new { e.EntidadTipo, e.EntidadId });
            });

            modelBuilder.Entity<Feriado>(entity =>
            {
                entity.ToTable("feriados");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
                entity.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion").IsRequired();

                entity.HasIndex(e => e.Fecha).IsUnique();
                entity.HasIndex(e => e.Activo);
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("password_reset_tokens");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
                entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                entity.Property(e => e.ExpiresAt).HasColumnName("expires_at").IsRequired();
                entity.Property(e => e.UsedAt).HasColumnName("used_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

                entity.HasIndex(e => new { e.UsuarioId, e.Codigo }).IsUnique();
                entity.HasIndex(e => e.ExpiresAt);
                entity.HasIndex(e => e.UsedAt);
            });

            modelBuilder.Entity<PasswordHistory>(entity =>
            {
                entity.ToTable("password_history");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.CreatedAt);
            });

            ApplyUtcDateTimeConverters(modelBuilder);
        }

        private void NormalizeDateTimesToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                {
                    continue;
                }

                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue is DateTime dt)
                    {
                        property.CurrentValue = NormalizeToUtc(dt);
                        continue;
                    }

                    if (property.Metadata.ClrType == typeof(DateTime?) && property.CurrentValue is DateTime nullableDt)
                    {
                        property.CurrentValue = NormalizeToUtc(nullableDt);
                    }
                }
            }
        }

        private static DateTime NormalizeToUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                _ => value
            };
        }

        private static void ApplyUtcDateTimeConverters(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                value => NormalizeToUtc(value),
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                value => value.HasValue ? NormalizeToUtc(value.Value) : value,
                value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                        continue;
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}
