$composeFile = "docker-compose.ollama.yml"
$modelName   = "lfm2.5-thinking"

docker compose -f $composeFile down --remove-orphans
docker container prune -f

$images = @("currency-api-image", "ai-agent-api-image", "frontend-image")

foreach ($imageName in $images) {
    $imageId = docker images -q $imageName
    if ($imageId) {
        try {
            docker image rm $imageName -f
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to remove image '$imageName'."
            }
            Write-Host "Image '$imageName' removed successfully."
        } catch {
            throw "Error removing image '$imageName': $_"
        }
    } else {
        Write-Host "Image '$imageName' does not exist. Skipping removal."
    }
}

docker compose -f $composeFile up -d

Write-Host "Waiting for Ollama container to become healthy..."

$maxWaitSeconds = 120
$waited = 0
$intervalSeconds = 5

while ($waited -lt $maxWaitSeconds) {
    $health = docker inspect --format "{{.State.Health.Status}}" ollama 2>$null
    if ($health -eq "healthy") {
        Write-Host "Ollama is healthy."
        break
    }
    Write-Host "  Ollama status: $health - retrying in ${intervalSeconds}s..."
    Start-Sleep -Seconds $intervalSeconds
    $waited += $intervalSeconds
}

if ($waited -ge $maxWaitSeconds) {
    Write-Warning "Ollama did not become healthy within $maxWaitSeconds seconds. Skipping model check."
    exit 1
}

$existingModels = docker exec ollama ollama list 2>&1
if ($existingModels -match [regex]::Escape($modelName)) {
    Write-Host "Model '$modelName' is already present. No pull needed."
} else {
    Write-Host "Model '$modelName' not found - pulling now (this only happens once per volume)..."
    docker exec ollama ollama pull $modelName
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Failed to pull model '$modelName'."
        exit 1
    }
    Write-Host "Model '$modelName' pulled successfully."
}
