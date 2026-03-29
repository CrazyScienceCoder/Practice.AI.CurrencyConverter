docker compose down --remove-orphans
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

docker compose up -d
