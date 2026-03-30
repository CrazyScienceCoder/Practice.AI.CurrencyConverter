$ports = @(
    @{ Port = 9200;  Service = "OpenSearch" },
    @{ Port = 9600;  Service = "OpenSearch transport" },
    @{ Port = 5601;  Service = "OpenSearch Dashboards" },
    @{ Port = 16686; Service = "Jaeger UI" },
    @{ Port = 9090;  Service = "Prometheus" },
    @{ Port = 3001;  Service = "Grafana" },
    @{ Port = 8889;  Service = "OTEL Collector metrics" },
    @{ Port = 4317;  Service = "OTEL Collector gRPC" },
    @{ Port = 6379;  Service = "Redis" },
    @{ Port = 8900;  Service = "Keycloak" },
    @{ Port = 11434; Service = "Ollama" },
    @{ Port = 9081;  Service = "Currency API" },
    @{ Port = 9082;  Service = "AI Agent API" },
    @{ Port = 3000;  Service = "Frontend" }
)

foreach ($entry in $ports) {
    $port = $entry.Port
    $service = $entry.Service

    $existing = netsh int ipv4 show excludedportrange protocol=tcp |
                Select-String "^\s*$port\s"

    if ($existing) {
        Write-Host "[$port] Already reserved - $service" -ForegroundColor Yellow
    } else {
        $result = netsh int ipv4 add excludedportrange protocol=tcp startport=$port numberofports=1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$port] Reserved - $service" -ForegroundColor Green
        } else {
            Write-Warning "[$port] Failed to reserve - $service`n$result"
        }
    }
}
