using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditoria_eventos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    accion = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entidad_tipo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entidad_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    detalle = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_eventos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cierres_diarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cerrado_por_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evidencia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cierres_diarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    apellido = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    negocio = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    zona = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Activo"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    mora_diaria = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 20m),
                    mora_semanal = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 150m),
                    dias_gracia_diaria = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    dias_gracia_semanal = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    tope_mora_diaria = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    tope_mora_semanal = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    tasa_diaria = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0.15m),
                    tasa_semanal = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0.2m),
                    domingo_inhabil_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    aplicar_feriados_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_min_length = table.Column<int>(type: "integer", nullable: false, defaultValue: 8),
                    password_require_upper = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    password_require_lower = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    password_require_digit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    password_require_special = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_expire_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 90),
                    password_history_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    lockout_max_failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    lockout_minutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 15),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion_sistema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cortes_caja",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    total_teorico = table.Column<decimal>(type: "numeric", nullable: false),
                    total_real = table.Column<decimal>(type: "numeric", nullable: false),
                    diferencia = table.Column<decimal>(type: "numeric", nullable: false),
                    realizado_por_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evidencia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cortes_caja", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feriados",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feriados", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "liquidaciones_cobranza",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cobrador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    total_efectivo = table.Column<decimal>(type: "numeric", nullable: false),
                    total_transferencia = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    evidencia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    estatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    confirmada_por_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_confirmacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liquidaciones_cobranza", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "modulos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    icono = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modulos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "password_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perfiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "zonas_cobranza",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zonas_cobranza", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "creditos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    folio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    monto = table.Column<decimal>(type: "numeric", nullable: false),
                    interes_total = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    cuota = table.Column<decimal>(type: "numeric", nullable: false),
                    total_fichas = table.Column<int>(type: "integer", nullable: false),
                    pagado = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Activo"),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    permitir_domingo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    aplicar_feriados = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_creditos", x => x.id);
                    table.ForeignKey(
                        name: "FK_creditos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "movimientos_caja",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    corte_caja_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cobrador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    liquidacion_cobranza_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reversa_de_id = table.Column<Guid>(type: "uuid", nullable: true),
                    idempotency_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    concepto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    medio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_efectivo = table.Column<decimal>(type: "numeric", nullable: true),
                    monto_transferencia = table.Column<decimal>(type: "numeric", nullable: true),
                    abono = table.Column<decimal>(type: "numeric", nullable: true),
                    mora = table.Column<decimal>(type: "numeric", nullable: true),
                    credito_id = table.Column<Guid>(type: "uuid", nullable: true),
                    numero_ficha = table.Column<int>(type: "integer", nullable: true),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos_caja", x => x.id);
                    table.ForeignKey(
                        name: "FK_movimientos_caja_cortes_caja_corte_caja_id",
                        column: x => x.corte_caja_id,
                        principalTable: "cortes_caja",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "paginas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ruta = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    id_modulo = table.Column<Guid>(type: "uuid", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paginas", x => x.id);
                    table.ForeignKey(
                        name: "FK_paginas_modulos_id_modulo",
                        column: x => x.id_modulo,
                        principalTable: "modulos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permisos_modulo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_perfil = table.Column<Guid>(type: "uuid", nullable: false),
                    id_modulo = table.Column<Guid>(type: "uuid", nullable: false),
                    tiene_permiso = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permisos_modulo", x => x.id);
                    table.ForeignKey(
                        name: "FK_permisos_modulo_modulos_id_modulo",
                        column: x => x.id_modulo,
                        principalTable: "modulos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_permisos_modulo_perfiles_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfiles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido_paterno = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido_materno = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usuario_acceso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contrasena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    must_change_password = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    failed_login_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lockout_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_perfil = table.Column<Guid>(type: "uuid", nullable: false),
                    id_zona_cobranza = table.Column<Guid>(type: "uuid", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_perfiles_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfiles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_usuarios_zonas_cobranza_id_zona_cobranza",
                        column: x => x.id_zona_cobranza,
                        principalTable: "zonas_cobranza",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "fichas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    credito_id = table.Column<Guid>(type: "uuid", nullable: false),
                    num = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hora = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    folio = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    capital = table.Column<decimal>(type: "numeric", nullable: false),
                    interes = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    abono = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    mora = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    mora_acumulada = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    saldo_cap = table.Column<decimal>(type: "numeric", nullable: false),
                    pagada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    cerrada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fichas", x => x.id);
                    table.ForeignKey(
                        name: "FK_fichas_creditos_credito_id",
                        column: x => x.credito_id,
                        principalTable: "creditos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "botones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_pagina = table.Column<Guid>(type: "uuid", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_botones", x => x.id);
                    table.ForeignKey(
                        name: "FK_botones_paginas_id_pagina",
                        column: x => x.id_pagina,
                        principalTable: "paginas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permisos_pagina",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_perfil = table.Column<Guid>(type: "uuid", nullable: false),
                    id_pagina = table.Column<Guid>(type: "uuid", nullable: false),
                    tiene_permiso = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permisos_pagina", x => x.id);
                    table.ForeignKey(
                        name: "FK_permisos_pagina_paginas_id_pagina",
                        column: x => x.id_pagina,
                        principalTable: "paginas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_permisos_pagina_perfiles_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfiles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permisos_boton",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_perfil = table.Column<Guid>(type: "uuid", nullable: false),
                    id_boton = table.Column<Guid>(type: "uuid", nullable: false),
                    tiene_permiso = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permisos_boton", x => x.id);
                    table.ForeignKey(
                        name: "FK_permisos_boton_botones_id_boton",
                        column: x => x.id_boton,
                        principalTable: "botones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_permisos_boton_perfiles_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfiles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_eventos_entidad_tipo_entidad_id",
                table: "auditoria_eventos",
                columns: new[] { "entidad_tipo", "entidad_id" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_eventos_fecha",
                table: "auditoria_eventos",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_eventos_usuario_id",
                table: "auditoria_eventos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_botones_activo",
                table: "botones",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_botones_clave",
                table: "botones",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_botones_id_pagina_orden",
                table: "botones",
                columns: new[] { "id_pagina", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_cierres_diarios_fecha",
                table: "cierres_diarios",
                column: "fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_activo_estatus",
                table: "clientes",
                columns: new[] { "activo", "estatus" });

            migrationBuilder.CreateIndex(
                name: "IX_cortes_caja_fecha",
                table: "cortes_caja",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_cortes_caja_folio",
                table: "cortes_caja",
                column: "folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_creditos_cliente_id",
                table: "creditos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_creditos_estatus",
                table: "creditos",
                column: "estatus");

            migrationBuilder.CreateIndex(
                name: "IX_creditos_estatus_tipo",
                table: "creditos",
                columns: new[] { "estatus", "tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_creditos_fecha_creacion",
                table: "creditos",
                column: "fecha_creacion");

            migrationBuilder.CreateIndex(
                name: "IX_creditos_folio",
                table: "creditos",
                column: "folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_feriados_activo",
                table: "feriados",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_feriados_fecha",
                table: "feriados",
                column: "fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fichas_credito_id_num",
                table: "fichas",
                columns: new[] { "credito_id", "num" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fichas_credito_id_pagada",
                table: "fichas",
                columns: new[] { "credito_id", "pagada" });

            migrationBuilder.CreateIndex(
                name: "IX_fichas_pagada_fecha",
                table: "fichas",
                columns: new[] { "pagada", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_liquidaciones_cobranza_cobrador_id_fecha",
                table: "liquidaciones_cobranza",
                columns: new[] { "cobrador_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_liquidaciones_cobranza_estatus",
                table: "liquidaciones_cobranza",
                column: "estatus");

            migrationBuilder.CreateIndex(
                name: "IX_modulos_activo",
                table: "modulos",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_modulos_clave",
                table: "modulos",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modulos_orden",
                table: "modulos",
                column: "orden");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_cobrador_id",
                table: "movimientos_caja",
                column: "cobrador_id");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_corte_caja_id_fecha",
                table: "movimientos_caja",
                columns: new[] { "corte_caja_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_credito_id",
                table: "movimientos_caja",
                column: "credito_id");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_credito_id_numero_ficha",
                table: "movimientos_caja",
                columns: new[] { "credito_id", "numero_ficha" });

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_fecha",
                table: "movimientos_caja",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_idempotency_key",
                table: "movimientos_caja",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_liquidacion_cobranza_id",
                table: "movimientos_caja",
                column: "liquidacion_cobranza_id");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_caja_reversa_de_id",
                table: "movimientos_caja",
                column: "reversa_de_id");

            migrationBuilder.CreateIndex(
                name: "IX_paginas_activo",
                table: "paginas",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_paginas_clave",
                table: "paginas",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_paginas_id_modulo_orden",
                table: "paginas",
                columns: new[] { "id_modulo", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_password_history_created_at",
                table: "password_history",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_password_history_usuario_id",
                table: "password_history",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_expires_at",
                table: "password_reset_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_used_at",
                table: "password_reset_tokens",
                column: "used_at");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_usuario_id_codigo",
                table: "password_reset_tokens",
                columns: new[] { "usuario_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_perfiles_activo",
                table: "perfiles",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_perfiles_clave",
                table: "perfiles",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_perfiles_orden",
                table: "perfiles",
                column: "orden");

            migrationBuilder.CreateIndex(
                name: "IX_permisos_boton_id_boton",
                table: "permisos_boton",
                column: "id_boton");

            migrationBuilder.CreateIndex(
                name: "IX_permisos_boton_id_perfil_id_boton",
                table: "permisos_boton",
                columns: new[] { "id_perfil", "id_boton" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permisos_modulo_id_modulo",
                table: "permisos_modulo",
                column: "id_modulo");

            migrationBuilder.CreateIndex(
                name: "IX_permisos_modulo_id_perfil_id_modulo",
                table: "permisos_modulo",
                columns: new[] { "id_perfil", "id_modulo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permisos_pagina_id_pagina",
                table: "permisos_pagina",
                column: "id_pagina");

            migrationBuilder.CreateIndex(
                name: "IX_permisos_pagina_id_perfil_id_pagina",
                table: "permisos_pagina",
                columns: new[] { "id_perfil", "id_pagina" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_activo",
                table: "usuarios",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_perfil",
                table: "usuarios",
                column: "id_perfil");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_zona_cobranza",
                table: "usuarios",
                column: "id_zona_cobranza");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_usuario_acceso",
                table: "usuarios",
                column: "usuario_acceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_zonas_cobranza_activo",
                table: "zonas_cobranza",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_zonas_cobranza_orden",
                table: "zonas_cobranza",
                column: "orden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_eventos");

            migrationBuilder.DropTable(
                name: "cierres_diarios");

            migrationBuilder.DropTable(
                name: "configuracion_sistema");

            migrationBuilder.DropTable(
                name: "feriados");

            migrationBuilder.DropTable(
                name: "fichas");

            migrationBuilder.DropTable(
                name: "liquidaciones_cobranza");

            migrationBuilder.DropTable(
                name: "movimientos_caja");

            migrationBuilder.DropTable(
                name: "password_history");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "permisos_boton");

            migrationBuilder.DropTable(
                name: "permisos_modulo");

            migrationBuilder.DropTable(
                name: "permisos_pagina");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "creditos");

            migrationBuilder.DropTable(
                name: "cortes_caja");

            migrationBuilder.DropTable(
                name: "botones");

            migrationBuilder.DropTable(
                name: "perfiles");

            migrationBuilder.DropTable(
                name: "zonas_cobranza");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "paginas");

            migrationBuilder.DropTable(
                name: "modulos");
        }
    }
}
