/*
  Vistas de solo lectura para la base ERP del cliente.
  La API consulta estas vistas y nunca escribe en tablas del ERP.
*/

/* Ejecutar este script sobre la base ERP del cliente seleccionada en SSMS. */
GO

IF SCHEMA_ID(N'api') IS NULL
    EXEC(N'CREATE SCHEMA api AUTHORIZATION dbo;');
GO

CREATE OR ALTER VIEW api.vw_ChequesTerceros
AS
SELECT
    CAST(RTRIM(ct.cheqtrosid) AS nvarchar(15)) AS Id,
    CONVERT(nvarchar(50), ct.nrocheque) AS Numero,
    CAST(RTRIM(b.descrip) AS nvarchar(200)) AS Banco,
    CAST(ct.importe AS decimal(19, 6)) AS Importe,
    CAST(RTRIM(m.codigo) AS nvarchar(20)) AS Moneda,
    CAST(ct.fechaemi AS date) AS FechaEmision,
    CAST(ct.fechacob AS date) AS FechaCobro,
    CAST(mfIngreso.fechaas AS datetime2) AS FechaIngresoCartera,
    CAST(CASE WHEN ct.estado = 1 THEN NULL ELSE salida.fechaas END AS datetime2) AS FechaSalidaCartera,
    CAST(CASE ct.estado
        WHEN 1 THEN N'En cartera'
        WHEN 2 THEN N'Depositado'
        WHEN 3 THEN N'Descontado'
        WHEN 4 THEN N'Rechazado'
        WHEN 5 THEN N'Entregado'
        ELSE CONCAT(N'Estado ', ct.estado)
    END AS nvarchar(100)) AS Estado,
    CAST(NULLIF(RTRIM(cli.razsoc), '') AS nvarchar(200)) AS EmpresaOrigen,
    CAST(NULLIF(RTRIM(cli.nrodocum), '') AS nvarchar(20)) AS CuitEmpresaOrigen
FROM dbo.ChequesTerceros ct
INNER JOIN dbo.Banco b ON b.bancoid = ct.bancoid
INNER JOIN dbo.CuentaFondo cf ON cf.ctafondoid = ct.ctafondoid
INNER JOIN dbo.Moneda m ON m.monedaid = cf.monedaid
INNER JOIN dbo.MovimientoFondo mfIngreso ON mfIngreso.movfondid = ct.movfondid
LEFT JOIN dbo.Cliente cli ON cli.clienteid = ct.clienteid
OUTER APPLY
(
    SELECT TOP (1) mfSalida.fechaas
    FROM dbo.ChequesTercerosDependencia dep
    INNER JOIN dbo.MovimientoFondo mfSalida ON mfSalida.movfondid = dep.movfondid
    WHERE dep.cheqtrosid = ct.cheqtrosid
      AND dep.estado = ct.estado
    ORDER BY mfSalida.fechaas DESC, dep.movfondid DESC
) salida;
GO

CREATE OR ALTER VIEW api.vw_ChequesTercerosMovimientos
AS
SELECT
    CAST(RTRIM(ct.cheqtrosid) AS nvarchar(15)) AS ChequeId,
    CAST(0 AS int) AS Orden,
    CAST(N'Ingresado a cartera' AS nvarchar(100)) AS Estado,
    CAST(mf.fechaas AS datetime2) AS Fecha,
    CAST(RTRIM(cf.codcomprob) AS nvarchar(20)) AS Comprobante,
    CAST(NULLIF(RTRIM(mf.detgral), '') AS nvarchar(254)) AS Detalle
FROM dbo.ChequesTerceros ct
INNER JOIN dbo.MovimientoFondo mf ON mf.movfondid = ct.movfondid
INNER JOIN dbo.ComprobanteFondo cf ON cf.comprobid = mf.comprobid

UNION ALL

SELECT
    CAST(RTRIM(dep.cheqtrosid) AS nvarchar(15)) AS ChequeId,
    CAST(1 AS int) AS Orden,
    CAST(CASE dep.estado
        WHEN 1 THEN N'En cartera'
        WHEN 2 THEN N'Depositado'
        WHEN 3 THEN N'Descontado'
        WHEN 4 THEN N'Rechazado'
        WHEN 5 THEN N'Entregado'
        ELSE CONCAT(N'Estado ', dep.estado)
    END AS nvarchar(100)) AS Estado,
    CAST(mf.fechaas AS datetime2) AS Fecha,
    CAST(RTRIM(cf.codcomprob) AS nvarchar(20)) AS Comprobante,
    CAST(NULLIF(RTRIM(mf.detgral), '') AS nvarchar(254)) AS Detalle
FROM dbo.ChequesTercerosDependencia dep
INNER JOIN dbo.MovimientoFondo mf ON mf.movfondid = dep.movfondid
INNER JOIN dbo.ComprobanteFondo cf ON cf.comprobid = mf.comprobid;
GO

