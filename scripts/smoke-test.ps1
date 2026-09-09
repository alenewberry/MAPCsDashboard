param(
    [string]$BaseUrl = "http://127.0.0.1:5188"
)

$tests = @(
    @{ Name = "Estado público"; Method = "GET"; Path = "/api/v1/estado"; Expected = 200 },
    @{ Name = "Cheques sin clave"; Method = "GET"; Path = "/api/v1/cheques"; Expected = 401 },
    @{ Name = "Escritura bloqueada"; Method = "POST"; Path = "/api/v1/estado"; Expected = 405 },
    @{ Name = "OpenAPI"; Method = "GET"; Path = "/swagger/v1/swagger.json"; Expected = 200 },
    @{ Name = "Documentación"; Method = "GET"; Path = "/documentacion/index.html"; Expected = 200 }
)

$failed = $false
foreach ($test in $tests) {
    try {
        $response = Invoke-WebRequest -Uri "$BaseUrl$($test.Path)" -Method $test.Method -UseBasicParsing
        $status = [int]$response.StatusCode
    }
    catch {
        $status = [int]$_.Exception.Response.StatusCode
    }

    $ok = $status -eq $test.Expected
    $failed = $failed -or !$ok
    [pscustomobject]@{
        Prueba = $test.Name
        Esperado = $test.Expected
        Obtenido = $status
        Resultado = if ($ok) { "OK" } else { "ERROR" }
    }
}

if ($failed) { exit 1 }
