BEGIN;

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS perfiles (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  clave varchar(50) NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  orden integer NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_perfiles_clave ON perfiles (clave);

CREATE TABLE IF NOT EXISTS usuarios (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  apellido_paterno varchar(100) NOT NULL,
  apellido_materno varchar(100) NULL,
  usuario_acceso varchar(50) NOT NULL,
  contrasena varchar(255) NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  ultimo_acceso timestamp without time zone NULL,
  id_perfil uuid NOT NULL,
  refresh_token varchar(255) NULL,
  refresh_token_expiry_time timestamp without time zone NULL,
  CONSTRAINT fk_usuarios_perfiles
    FOREIGN KEY (id_perfil) REFERENCES perfiles (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_usuarios_usuario_acceso ON usuarios (usuario_acceso);
CREATE INDEX IF NOT EXISTS ix_usuarios_id_perfil ON usuarios (id_perfil);

CREATE TABLE IF NOT EXISTS modulos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  clave varchar(50) NOT NULL,
  icono varchar(100) NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  orden integer NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_modulos_clave ON modulos (clave);

CREATE TABLE IF NOT EXISTS paginas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  clave varchar(50) NOT NULL,
  ruta varchar(255) NOT NULL,
  id_modulo uuid NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  orden integer NOT NULL DEFAULT 0,
  CONSTRAINT fk_paginas_modulos
    FOREIGN KEY (id_modulo) REFERENCES modulos (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_paginas_clave ON paginas (clave);
CREATE INDEX IF NOT EXISTS ix_paginas_id_modulo ON paginas (id_modulo);

CREATE TABLE IF NOT EXISTS botones (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  clave varchar(50) NOT NULL,
  id_pagina uuid NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  orden integer NOT NULL DEFAULT 0,
  CONSTRAINT fk_botones_paginas
    FOREIGN KEY (id_pagina) REFERENCES paginas (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_botones_clave ON botones (clave);
CREATE INDEX IF NOT EXISTS ix_botones_id_pagina ON botones (id_pagina);

CREATE TABLE IF NOT EXISTS permisos_modulo (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  id_perfil uuid NOT NULL,
  id_modulo uuid NOT NULL,
  tiene_permiso boolean NOT NULL DEFAULT false,
  fecha_asignacion timestamp without time zone NOT NULL,
  CONSTRAINT fk_permisos_modulo_perfiles
    FOREIGN KEY (id_perfil) REFERENCES perfiles (id) ON DELETE RESTRICT,
  CONSTRAINT fk_permisos_modulo_modulos
    FOREIGN KEY (id_modulo) REFERENCES modulos (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_permisos_modulo_perfil_modulo ON permisos_modulo (id_perfil, id_modulo);
CREATE INDEX IF NOT EXISTS ix_permisos_modulo_id_perfil ON permisos_modulo (id_perfil);
CREATE INDEX IF NOT EXISTS ix_permisos_modulo_id_modulo ON permisos_modulo (id_modulo);

CREATE TABLE IF NOT EXISTS permisos_pagina (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  id_perfil uuid NOT NULL,
  id_pagina uuid NOT NULL,
  tiene_permiso boolean NOT NULL DEFAULT false,
  fecha_asignacion timestamp without time zone NOT NULL,
  CONSTRAINT fk_permisos_pagina_perfiles
    FOREIGN KEY (id_perfil) REFERENCES perfiles (id) ON DELETE RESTRICT,
  CONSTRAINT fk_permisos_pagina_paginas
    FOREIGN KEY (id_pagina) REFERENCES paginas (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_permisos_pagina_perfil_pagina ON permisos_pagina (id_perfil, id_pagina);
CREATE INDEX IF NOT EXISTS ix_permisos_pagina_id_perfil ON permisos_pagina (id_perfil);
CREATE INDEX IF NOT EXISTS ix_permisos_pagina_id_pagina ON permisos_pagina (id_pagina);

CREATE TABLE IF NOT EXISTS permisos_boton (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  id_perfil uuid NOT NULL,
  id_boton uuid NOT NULL,
  tiene_permiso boolean NOT NULL DEFAULT false,
  fecha_asignacion timestamp without time zone NOT NULL,
  CONSTRAINT fk_permisos_boton_perfiles
    FOREIGN KEY (id_perfil) REFERENCES perfiles (id) ON DELETE RESTRICT,
  CONSTRAINT fk_permisos_boton_botones
    FOREIGN KEY (id_boton) REFERENCES botones (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_permisos_boton_perfil_boton ON permisos_boton (id_perfil, id_boton);
CREATE INDEX IF NOT EXISTS ix_permisos_boton_id_perfil ON permisos_boton (id_perfil);
CREATE INDEX IF NOT EXISTS ix_permisos_boton_id_boton ON permisos_boton (id_boton);

CREATE TABLE IF NOT EXISTS configuracion_sistema (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  mora_diaria numeric(18,2) NOT NULL DEFAULT 20,
  mora_semanal numeric(18,2) NOT NULL DEFAULT 150,
  tasa_diaria numeric(10,4) NOT NULL DEFAULT 0.15,
  tasa_semanal numeric(10,4) NOT NULL DEFAULT 0.20,
  fecha_actualizacion timestamp without time zone NOT NULL
);

INSERT INTO configuracion_sistema (id, mora_diaria, mora_semanal, tasa_diaria, tasa_semanal, fecha_actualizacion)
SELECT gen_random_uuid(), 20, 150, 0.15, 0.20, NOW()
WHERE NOT EXISTS (SELECT 1 FROM configuracion_sistema);

CREATE TABLE IF NOT EXISTS zonas_cobranza (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(100) NOT NULL,
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL,
  orden integer NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS clientes (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  nombre varchar(120) NOT NULL,
  apellido varchar(120) NOT NULL,
  direccion varchar(255) NULL,
  negocio varchar(180) NULL,
  zona varchar(100) NULL,
  estatus varchar(30) NOT NULL DEFAULT 'Activo',
  activo boolean NOT NULL DEFAULT true,
  fecha_creacion timestamp without time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS creditos (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  cliente_id uuid NOT NULL,
  monto numeric(18,2) NOT NULL,
  interes_total numeric(18,2) NOT NULL,
  total numeric(18,2) NOT NULL,
  cuota numeric(18,2) NOT NULL,
  total_fichas integer NOT NULL,
  pagado numeric(18,2) NOT NULL DEFAULT 0,
  tipo varchar(20) NOT NULL,
  estatus varchar(30) NOT NULL DEFAULT 'Activo',
  fecha_creacion timestamp without time zone NOT NULL,
  CONSTRAINT fk_creditos_clientes
    FOREIGN KEY (cliente_id) REFERENCES clientes (id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS ix_creditos_cliente_id ON creditos (cliente_id);

CREATE TABLE IF NOT EXISTS fichas (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  credito_id uuid NOT NULL,
  num integer NOT NULL,
  fecha timestamp without time zone NOT NULL,
  fecha_pago timestamp without time zone NULL,
  hora varchar(10) NULL,
  folio varchar(80) NOT NULL,
  capital numeric(18,2) NOT NULL,
  interes numeric(18,2) NOT NULL,
  total numeric(18,2) NOT NULL,
  abono numeric(18,2) NOT NULL DEFAULT 0,
  mora numeric(18,2) NOT NULL DEFAULT 0,
  saldo_cap numeric(18,2) NOT NULL,
  pagada boolean NOT NULL DEFAULT false,
  cerrada boolean NOT NULL DEFAULT false,
  fecha_cierre timestamp without time zone NULL,
  CONSTRAINT fk_fichas_creditos
    FOREIGN KEY (credito_id) REFERENCES creditos (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_fichas_credito_num ON fichas (credito_id, num);
CREATE INDEX IF NOT EXISTS ix_fichas_credito_id ON fichas (credito_id);
CREATE INDEX IF NOT EXISTS ix_fichas_fecha ON fichas (fecha);

CREATE TABLE IF NOT EXISTS cortes_caja (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  fecha timestamp without time zone NOT NULL,
  hora varchar(10) NULL,
  total_teorico numeric(18,2) NOT NULL,
  total_real numeric(18,2) NOT NULL,
  diferencia numeric(18,2) NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_cortes_caja_fecha ON cortes_caja (fecha);

CREATE TABLE IF NOT EXISTS movimientos_caja (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  corte_caja_id uuid NULL,
  tipo varchar(30) NOT NULL,
  concepto varchar(255) NULL,
  medio varchar(30) NOT NULL,
  total numeric(18,2) NOT NULL,
  monto_efectivo numeric(18,2) NULL,
  monto_transferencia numeric(18,2) NULL,
  abono numeric(18,2) NULL,
  mora numeric(18,2) NULL,
  credito_id uuid NULL,
  numero_ficha integer NULL,
  fecha timestamp without time zone NOT NULL,
  hora varchar(10) NULL,
  CONSTRAINT fk_movimientos_caja_cortes
    FOREIGN KEY (corte_caja_id) REFERENCES cortes_caja (id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_movimientos_caja_fecha ON movimientos_caja (fecha);
CREATE INDEX IF NOT EXISTS ix_movimientos_caja_corte_caja_id ON movimientos_caja (corte_caja_id);
CREATE INDEX IF NOT EXISTS ix_movimientos_caja_credito_id ON movimientos_caja (credito_id);

COMMIT;

