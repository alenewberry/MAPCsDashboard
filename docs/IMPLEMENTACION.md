# Implementación y publicación

## Configuración obligatoria

En producción, no guardar secretos en `appsettings.json`. Configurar estas variables en el servidor:

- `ApiKey__Key`: clave larga y aleatoria para el consumidor.
- `ConnectionStrings__Erp`: conexión SQL Server de un usuario con permisos exclusivamente de lectura sobre `api.vw_ChequesTerceros`.
- `ASPNETCORE_ENVIRONMENT=Production`.

La URL pública prevista es `https://api-test.catedralsoft.com.ar`. El certificado TLS, el DNS y el proxy IIS deben configurarse en infraestructura.

## Integración con la base ERP del cliente

1. Ejecutar `database/01-contract-view.sql` en la base del cliente.
2. Validar que los códigos de estado coincidan con esa versión del ERP.
3. Conceder al usuario SQL de la API únicamente `SELECT` sobre las dos vistas del esquema `api`.
4. Probar cada estado conocido con casos reales antes de publicar.

## Compilación

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish src/MAPCsDashboard.Api --configuration Release --output artifacts/publish
```
