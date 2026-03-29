# Revert all Ollama environment variable changes and restart Ollama (requires admin).
[System.Environment]::SetEnvironmentVariable("OLLAMA_ORIGINS", $null, "Machine")
[System.Environment]::SetEnvironmentVariable("OLLAMA_HOST", $null, "Machine")

Write-Host "Ollama environment variables cleared."

$ollamaProcess = Get-Process -Name "ollama" -ErrorAction SilentlyContinue
if ($ollamaProcess) {
    Write-Host "Restarting Ollama..."
    Stop-Process -Name "ollama" -Force
    Start-Sleep -Seconds 2
    Start-Process "ollama" -ArgumentList "serve" -WindowStyle Hidden
    Write-Host "Ollama restarted with default settings."
} else {
    Write-Host "Ollama is not running. Start it manually if needed."
}
