# MAPCsDashboard

API REST .NET 8 para las integraciones de MAP, basada en la solución de
Industrias Sthal de Catedral Software.

## Incluye

- Endpoints exclusivamente GET, con rechazo global de escrituras.
- Autenticación por `X-API-Key` con comparación segura.
- Swagger en `/documentacion` y guía para usuarios no técnicos.
- Estado del servicio en `/api/v1/estado`.

Los endpoints de negocio se incorporarán cuando se defina el contrato específico
de MAP.

## Inicio local

1. Configurar `ApiKey:Key` mediante `appsettings.Local.json` en desarrollo o la
   variable `ApiKey__Key`. No guardar valores reales en `appsettings.json`.
2. Ejecutar `dotnet run --project src/MAPCsDashboard.Api`.
3. Abrir `https://localhost:<puerto>/documentacion`.

Con la API en ejecución, las pruebas básicas pueden correrse con
`./scripts/smoke-test.ps1 -BaseUrl http://127.0.0.1:<puerto>`.

Consultar [la guía de usuario](docs/GUIA-USUARIO.md) y [la guía de implementación](docs/IMPLEMENTACION.md).
