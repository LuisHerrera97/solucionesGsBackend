using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditoCondonarInteresBotonPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Misma página que detalle de crédito (reestructura o, en su defecto, abonar); permisos copiados desde esos botones
            migrationBuilder.Sql(@"
INSERT INTO botones (id, nombre, clave, id_pagina, activo, fecha_creacion, orden)
SELECT gen_random_uuid(), 'Condonar interés', 'CREDITO_CONDONAR_INTERES',
  COALESCE(
    (SELECT id_pagina FROM botones WHERE clave = 'CREDITO_REESTRUCTURAR' LIMIT 1),
    (SELECT id_pagina FROM botones WHERE clave = 'CREDITO_ABONAR_FICHA' LIMIT 1)
  ),
  true, NOW() AT TIME ZONE 'UTC', 14
WHERE NOT EXISTS (SELECT 1 FROM botones WHERE clave = 'CREDITO_CONDONAR_INTERES')
AND COALESCE(
  (SELECT id_pagina FROM botones WHERE clave = 'CREDITO_REESTRUCTURAR' LIMIT 1),
  (SELECT id_pagina FROM botones WHERE clave = 'CREDITO_ABONAR_FICHA' LIMIT 1)
) IS NOT NULL;

INSERT INTO permisos_boton (id, id_perfil, id_boton, tiene_permiso, fecha_asignacion)
SELECT gen_random_uuid(), pb.id_perfil, np.id, pb.tiene_permiso, NOW() AT TIME ZONE 'UTC'
FROM permisos_boton pb
INNER JOIN botones old ON old.id = pb.id_boton AND old.clave = 'CREDITO_REESTRUCTURAR'
INNER JOIN botones np ON np.clave = 'CREDITO_CONDONAR_INTERES'
WHERE NOT EXISTS (
  SELECT 1 FROM permisos_boton x
  INNER JOIN botones bx ON bx.id = x.id_boton
  WHERE x.id_perfil = pb.id_perfil AND bx.clave = 'CREDITO_CONDONAR_INTERES'
);

INSERT INTO permisos_boton (id, id_perfil, id_boton, tiene_permiso, fecha_asignacion)
SELECT gen_random_uuid(), pb.id_perfil, np.id, pb.tiene_permiso, NOW() AT TIME ZONE 'UTC'
FROM permisos_boton pb
INNER JOIN botones old ON old.id = pb.id_boton AND old.clave = 'CREDITO_ABONAR_FICHA'
INNER JOIN botones np ON np.clave = 'CREDITO_CONDONAR_INTERES'
WHERE NOT EXISTS (
  SELECT 1 FROM permisos_boton x
  INNER JOIN botones bx ON bx.id = x.id_boton
  WHERE x.id_perfil = pb.id_perfil AND bx.clave = 'CREDITO_CONDONAR_INTERES'
);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM permisos_boton WHERE id_boton IN (SELECT id FROM botones WHERE clave = 'CREDITO_CONDONAR_INTERES');
DELETE FROM botones WHERE clave = 'CREDITO_CONDONAR_INTERES';
");
        }
    }
}
