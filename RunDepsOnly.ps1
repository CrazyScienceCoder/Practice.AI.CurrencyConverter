$composeFile = "docker-compose.deps.yml"

docker compose -f $composeFile down

docker compose -f $composeFile up -d

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to start dependency containers."
    exit 1
}

Write-Host ""
Write-Host "Dependency containers are up. Native connection strings:"
Write-Host "  Redis          -> localhost:6379"
Write-Host "  Keycloak       -> http://localhost:8080  (admin / admin)"
Write-Host "  OpenSearch     -> http://localhost:9200"
Write-Host "  OTLP Collector -> http://localhost:4317"
Write-Host "  Jaeger UI      -> http://localhost:16686"
Write-Host "  Grafana        -> http://localhost:3001"
Write-Host "  Prometheus     -> http://localhost:9090"
Write-Host ""
Write-Host "Run your projects natively with these overrides (or via launchSettings):"
Write-Host "  Redis__ConnectionString=localhost:6379"
Write-Host "  Keycloak__Authority=http://localhost:8080/realms/currency-converter"
Write-Host "  OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317"
Write-Host "  OPENSEARCH_NODE_URIS=http://localhost:9200"
