using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditoFichaBotonesPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CREDITO_PAGAR_FICHA: misma página que abonar; permisos copiados desde CREDITO_ABONAR_FICHA
            migrationBuilder.Sql(@"
INSERT INTO botones (id, nombre, clave, id_pagina, activo, fecha_creacion, orden)
SELECT gen_random_uuid(), 'Pagar ficha', 'CREDITO_PAGAR_FICHA', id_pagina, true, NOW() AT TIME ZONE 'UTC', 12
FROM botones WHERE clave = 'CREDITO_ABONAR_FICHA' LIMIT 1
AND NOT EXISTS (SELECT 1 FROM botones WHERE clave = 'CREDITO_PAGAR_FICHA');

INSERT INTO permisos_boton (id, id_perfil, id_boton, tiene_permiso, fecha_asignacion)
SELECT gen_random_uuid(), pb.id_perfil, np.id, pb.tiene_permiso, NOW() AT TIME ZONE 'UTC'
FROM permisos_boton pb
INNER JOIN botones old ON old.id = pb.id_boton AND old.clave = 'CREDITO_ABONAR_FICHA'
INNER JOIN botones np ON np.clave = 'CREDITO_PAGAR_FICHA'
WHERE NOT EXISTS (
  SELECT 1 FROM permisos_boton x
  INNER JOIN botones bx ON bx.id = x.id_boton
  WHERE x.id_perfil = pb.id_perfil AND bx.clave = 'CREDITO_PAGAR_FICHA'
);

INSERT INTO botones (id, nombre, clave, id_pagina, activo, fecha_creacion, orden)
SELECT gen_random_uuid(), 'Penalizar ficha', 'CREDITO_PENALIZAR_FICHA', id_pagina, true, NOW() AT TIME ZONE 'UTC', 13
FROM botones WHERE clave = 'CREDITO_ABONAR_FICHA' LIMIT 1
AND NOT EXISTS (SELECT 1 FROM botones WHERE clave = 'CREDITO_PENALIZAR_FICHA');

INSERT INTO permisos_boton (id, id_perfil, id_boton, tiene_permiso, fecha_asignacion)
SELECT gen_random_uuid(), pb.id_perfil, np.id, pb.tiene_permiso, NOW() AT TIME ZONE 'UTC'
FROM permisos_boton pb
INNER JOIN botones old ON old.id = pb.id_boton AND old.clave = 'CREDITO_ABONAR_FICHA'
INNER JOIN botones np ON np.clave = 'CREDITO_PENALIZAR_FICHA'
WHERE NOT EXISTS (
  SELECT 1 FROM permisos_boton x
  INNER JOIN botones bx ON bx.id = x.id_boton
  WHERE x.id_perfil = pb.id_perfil AND bx.clave = 'CREDITO_PENALIZAR_FICHA'
);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM permisos_boton WHERE id_boton IN (SELECT id FROM botones WHERE clave IN ('CREDITO_PAGAR_FICHA', 'CREDITO_PENALIZAR_FICHA'));
DELETE FROM botones WHERE clave IN ('CREDITO_PAGAR_FICHA', 'CREDITO_PENALIZAR_FICHA');
");
        }
    }
}
