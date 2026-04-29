/*
  Elimina la página "Caja" (/caja) del menú y permisos asociados.

  --- PostgreSQL (por defecto en este archivo) ---
  Ejecutar en la base de datos de la aplicación.
  Orden: permisos_boton → botones → permisos_pagina → paginas
*/

BEGIN;

DELETE FROM permisos_boton AS pb
USING botones AS b
WHERE b.id = pb.id_boton
  AND b.id_pagina IN (SELECT id FROM paginas WHERE ruta = '/caja');

DELETE FROM botones
WHERE id_pagina IN (SELECT id FROM paginas WHERE ruta = '/caja');

DELETE FROM permisos_pagina
WHERE id_pagina IN (SELECT id FROM paginas WHERE ruta = '/caja');

DELETE FROM paginas
WHERE ruta = '/caja';

COMMIT;

-- Mensaje opcional (ejecutar aparte si quieres comprobar antes):
-- SELECT id, nombre, ruta FROM paginas WHERE ruta = '/caja';


/*
  =============================================================================
  SQL Server (referencia; no ejecutar junto con el bloque de arriba)
  =============================================================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

DECLARE @IdsPagina TABLE (id UNIQUEIDENTIFIER);
INSERT INTO @IdsPagina (id)
SELECT id FROM dbo.paginas WHERE ruta = N'/caja';

IF NOT EXISTS (SELECT 1 FROM @IdsPagina)
BEGIN
  PRINT N'No se encontró página con ruta /caja; no se aplicaron cambios.';
  COMMIT TRANSACTION;
  RETURN;
END;

DELETE pb
FROM dbo.permisos_boton AS pb
INNER JOIN dbo.botones AS b ON b.id = pb.id_boton
WHERE b.id_pagina IN (SELECT id FROM @IdsPagina);

DELETE FROM dbo.botones
WHERE id_pagina IN (SELECT id FROM @IdsPagina);

DELETE FROM dbo.permisos_pagina
WHERE id_pagina IN (SELECT id FROM @IdsPagina);

DELETE FROM dbo.paginas
WHERE id IN (SELECT id FROM @IdsPagina);

COMMIT TRANSACTION;
PRINT N'Página /caja y botones/permisos vinculados eliminados.';
*/
