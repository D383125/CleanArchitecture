#!/usr/bin/env pwsh
# Save this as "pre-commit" with no extension

Write-Host "Running pre-commit hook..."
dotnet build /p:RunAnalyzersDuringBuild=true /p:AnalysisMode=Security .\CleanArch.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build or Security Anaysis failed! Commit aborted."
    exit 1
}

dotnet test .\src\WebApp\
if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed! Commit aborted."
    exit 1
}
Write-Host "Tests passed. Proceeding with commit."
