# Base de datos (PostgreSQL)

## Script
- [financiera_soluciones_schema.sql](file:///C:/Users/Luis%20Herrera/Documents/Luis%20jorge/Proyectos/FinancieraSoluciones/Backend/Database/financiera_soluciones_schema.sql)

## Ejecutar

1) Crear la base de datos:

```sql
CREATE DATABASE financiera_soluciones;
```

2) Ejecutar el script:

```bash
psql -U postgres -h localhost -p 5432 -d financiera_soluciones -f financiera_soluciones_schema.sql
```

Notas:
- El script crea la extensión `pgcrypto` si no existe (para `gen_random_uuid()`).
- También inserta un registro inicial en `configuracion_sistema` si la tabla está vacía.

