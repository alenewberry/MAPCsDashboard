# MAPCsDashboard

API REST .NET 8 para las integraciones de MAP, basada en la solución de
Industrias Sthal de Catedral Software.

## Incluye

- Endpoints exclusivamente GET, con rechazo global de escrituras.
- Autenticación por `X-API-Key` con comparación segura.
- SQL parametrizado y conexión independiente por solicitud.
- Paginación y filtros para cheques de terceros.
- Swagger en `/documentacion` y guía para usuarios no técnicos.
- Contrato mediante la vista SQL `api.vw_ChequesTerceros`.

Los endpoints de cheques se conservan inicialmente para mantener equivalencia
funcional con la solución de referencia. Se reemplazarán o ampliarán cuando se
defina el contrato específico de MAP.

## Inicio local

1. Configurar `ConnectionStrings:Erp` y `ApiKey:Key` mediante secretos de usuario
   o las variables `ConnectionStrings__Erp` y `ApiKey__Key`. No guardar valores
   reales en `appsettings.json`.
2. Completar la vista de `database/01-contract-view.sql` con el esquema real del ERP.
3. Ejecutar `dotnet run --project src/MAPCsDashboard.Api`.
4. Abrir `https://localhost:<puerto>/documentacion`.

Con la API en ejecución, las pruebas básicas pueden correrse con
`./scripts/smoke-test.ps1 -BaseUrl http://127.0.0.1:<puerto>`.

Consultar [la guía de usuario](docs/GUIA-USUARIO.md) y [la guía de implementación](docs/IMPLEMENTACION.md).
