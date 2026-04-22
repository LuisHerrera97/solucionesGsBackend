# Backend (FinancieraSoluciones)

## Requisitos
- .NET SDK 8 instalado
- PostgreSQL 14+ (recomendado 15/16)

## Abrir en Visual Studio
- Abre la solución: `Backend/FinancieraSoluciones.sln`
- Compilar: **Compilar → Compilar solución** (Ctrl+Shift+B)
- Restaurar paquetes (si lo pide): **Compilar → Restaurar paquetes NuGet**

## Swagger
- Al ejecutar el proyecto **FinancieraSoluciones.Api**, Swagger UI queda en la raíz:
  - `https://localhost:<puerto>/`
  - o directamente: `https://localhost:<puerto>/swagger`

## Base de datos (PostgreSQL)

### 1) Crear base de datos
Ejemplo usando `psql`:

```sql
CREATE DATABASE financiera_soluciones;
```

### 2) Ejecutar el script de esquema
El script está aquí:
- `Backend/Database/financiera_soluciones_schema.sql`

Ejemplo con `psql`:

```bash
psql -U postgres -h localhost -p 5432 -d financiera_soluciones -f Database/financiera_soluciones_schema.sql
```

### 3) Configurar la cadena de conexión
La API lee la cadena desde:
- `Backend/Api/appsettings.json` → `ConnectionStrings:Connection`

Formato Npgsql:

```txt
Host=localhost;Port=5432;Database=financiera_soluciones;Username=postgres;Password=postgres
```

## Paquetes NuGet (PostgreSQL)
El backend ya usa Npgsql (PostgreSQL) vía EF Core.

Paquetes principales:
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Npgsql`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Tools`

Si Visual Studio no los restaura automáticamente:
- **Compilar → Restaurar paquetes NuGet**
- o en terminal:

```bash
dotnet restore
dotnet build Backend/FinancieraSoluciones.sln
```
