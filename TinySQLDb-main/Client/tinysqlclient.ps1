param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port
)

$ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse("127.0.0.1"), 11000)

function Send-Message {
    param (
        [Parameter(Mandatory=$true)]
        [pscustomobject]$message,
        [Parameter(Mandatory=$true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $writer = New-Object System.IO.StreamWriter($stream)
    try {
        $writer.WriteLine($message)
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}

function Receive-Message {
    param (
        [System.Net.Sockets.Socket]$client
    )
    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $reader = New-Object System.IO.StreamReader($stream)
    try {
        return $null -ne $reader.ReadLine ? $reader.ReadLine() : ""
    }
    finally {
        $reader.Close()
        $stream.Close()
    }
}
function Send-SQLCommand {
    param (
        [string]$command
    )
    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
    $client.Connect($ipEndPoint)
    $requestObject = [PSCustomObject]@{
        RequestType = 0;
        RequestBody = $command
    }
    Write-Host -ForegroundColor Green "Sending command: $command"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    $response = Receive-Message -client $client

    Write-Host -ForegroundColor Green "Response received: $response"
    
    $responseObject = ConvertFrom-Json -InputObject $response
    Write-Output $responseObject
    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()
}
# Función para leer un archivo SQL y ejecutar múltiples sentencias
function Execute-SQLScript {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile
    )

    # Leer todo el contenido del archivo SQL
    $queries = Get-Content -Path $QueryFile -Raw
    
    # Separar las sentencias por punto y coma
    $queriesArray = $queries -split ";"
    
    # Ejecutar cada sentencia una por una
    foreach ($query in $queriesArray) {
        if ($query.Trim() -ne "") {
            # Enviar cada sentencia al servidor
            Send-SQLCommand -command $query.Trim()
        }
    }
}

# Modificar la función Send-SQLCommand para agregar medición de tiempo, formateo de salida y manejo de errores
function Send-SQLCommand {
    param (
        [string]$command
    )

    try {
        $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
        $client.Connect($ipEndPoint)
        $requestObject = [PSCustomObject]@{
            RequestType = 0;
            RequestBody = $command
        }
        Write-Host -ForegroundColor Green "Sending command: $command"

        $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
        
        # Medir el tiempo de ejecución
        $executionTime = Measure-Command {
            Send-Message -client $client -message $jsonMessage
            $response = Receive-Message -client $client
        }

        Write-Host -ForegroundColor Green "Response received: $response"

        $responseObject = ConvertFrom-Json -InputObject $response

        # Formatear la salida como tabla si es aplicable
        if ($responseObject -is [System.Collections.IEnumerable]) {
            $responseObject | Format-Table -AutoSize
        } else {
            Write-Output $responseObject
        }

        Write-Host "Execution time: $($executionTime.TotalSeconds) seconds"
        
        $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
        $client.Close()
    }
    catch {
        Write-Host -ForegroundColor Red "Error occurred: $_"
    }
}

# This is an example, should not be called here
Send-SQLCommand -command "CREATE TABLE ESTUDIANTE"
Send-SQlCommand -command "SELECT * FROM ESTUDIANTE"