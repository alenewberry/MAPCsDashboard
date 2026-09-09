# Guía simple de uso

La API permite **consultar** cheques del ERP. No permite crear, modificar ni borrar información.

## Dirección de prueba

- Documentación interactiva: `https://api-test.catedralsoft.com.ar/documentacion`
- Estado del servicio: `https://api-test.catedralsoft.com.ar/api/v1/estado`

## Antes de comenzar

Catedral Software entrega una clave privada. La clave debe enviarse en cada consulta dentro del encabezado `X-API-Key`. No debe compartirse por correo ni incluirse en capturas de pantalla.

## Consultar cheques

Dirección base:

`GET https://api-test.catedralsoft.com.ar/api/v1/cheques`

Cada resultado informa emisión, cobro, ingreso a cartera, salida de cartera, estado actual y empresa de origen.

Filtros disponibles:

| Filtro | Para qué sirve | Ejemplo |
|---|---|---|
| `estado` | En cartera, entregado, depositado, rechazado, etc. | `estado=En cartera` |
| `empresa` | Razón social o CUIT de quien entregó el cheque | `empresa=30-12345678-9` |
| `fechaCobroDesde` | Fecha de cobro inicial | `fechaCobroDesde=2026-07-01` |
| `fechaCobroHasta` | Fecha de cobro final | `fechaCobroHasta=2026-07-31` |
| `pagina` | Número de página | `pagina=1` |
| `tamanio` | Resultados por página, máximo 200 | `tamanio=50` |

Ejemplo completo:

`GET /api/v1/cheques?estado=En%20cartera&empresa=30-12345678-9&fechaCobroDesde=2026-07-01&fechaCobroHasta=2026-07-31`

Para conocer la historia completa de un cheque:

`GET /api/v1/cheques/{id}/movimientos`

## Significado de las respuestas

- `200`: la consulta fue correcta.
- `400`: algún filtro tiene un formato incorrecto.
- `401`: falta la clave o no es válida.
- `404`: el cheque solicitado no existe.
- `405`: se intentó modificar información; la API acepta solo GET.
- `500`: ocurrió un problema interno. Informar a soporte la fecha y hora de la consulta.
