# Implementación y publicación

## Configuración obligatoria

En producción, no guardar secretos en `appsettings.json`. Configurar estas variables en el servidor:

- `ApiKey__Key`: clave larga y aleatoria para el consumidor.
- `ASPNETCORE_ENVIRONMENT=Production`.

La URL pública prevista es `https://api-test.catedralsoft.com.ar`. El certificado TLS, el DNS y el proxy IIS deben configurarse en infraestructura.

## Endpoints de negocio

Antes de implementar cada endpoint se debe acordar su ruta, filtros, contrato de
respuesta, fuente de datos y permisos. Las conexiones necesarias se agregarán
cuando esos contratos estén definidos.

## Compilación

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish src/MAPCsDashboard.Api --configuration Release --output artifacts/publish
```
