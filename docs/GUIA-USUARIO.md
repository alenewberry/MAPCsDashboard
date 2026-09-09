# Guía simple de uso

La API será el punto de acceso de solo lectura para las integraciones de MAP. Los
endpoints de negocio todavía están pendientes de definición.

## Dirección de prueba

- Documentación interactiva: `https://api-test.catedralsoft.com.ar/documentacion`
- Estado del servicio: `https://api-test.catedralsoft.com.ar/api/v1/estado`

## Estado actual

Por el momento está disponible únicamente:

`GET /api/v1/estado`

Este endpoint es público y confirma que la aplicación está en ejecución. Los
endpoints futuros utilizarán una clave privada en el encabezado `X-API-Key`.

## Significado de las respuestas

- `200`: la consulta fue correcta.
- `401`: falta la clave o no es válida.
- `404`: el recurso solicitado no existe.
- `405`: se intentó modificar información; la API acepta solo GET.
- `500`: ocurrió un problema interno. Informar a soporte la fecha y hora de la consulta.
