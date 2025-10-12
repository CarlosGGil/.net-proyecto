<#
.SYNOPSIS
  Script de arranque para "Espectáculos" (docker compose + checks).
#>

param(
    [switch]$Down,
    [switch]$FollowLogs,
    [switch]$Seed,
    [int]$OpenDelayMs = 1200
)

$ErrorActionPreference = "Stop"
Write-Host "ℹ️  Espectáculos - Script de arranque" -ForegroundColor Cyan

# Cargar .env (opcional)
$envPath = Join-Path $PSScriptRoot "..\.env"
$hasEnvFile = Test-Path $envPath
if ($hasEnvFile) {
    Write-Host "ℹ️  Cargando variables desde .env" -ForegroundColor Cyan
    Get-Content $envPath | ForEach-Object {
        if ($_ -match "^\s*#") { return }
        if ([string]::IsNullOrWhiteSpace($_)) { return }
        $kv = $_ -split "=",2
        if ($kv.Length -eq 2) {
            [System.Environment]::SetEnvironmentVariable($kv[0].Trim(), $kv[1].Trim())
        }
    }
}

# Defaults locales
if (-not $env:DB_PORT) { $env:DB_PORT = "5432" }
if (-not $env:WEB_PORT) { $env:WEB_PORT = "8080" }
if (-not $env:POSTGRES_USER) { $env:POSTGRES_USER = "postgres" }
if (-not $env:POSTGRES_PASSWORD) { $env:POSTGRES_PASSWORD = "postgres" }
if (-not $env:POSTGRES_DB) { $env:POSTGRES_DB = "espectaculosdb" }

function Get-ComposeVariant {
    $hasDocker = Get-Command docker -ErrorAction SilentlyContinue
    $hasLegacy = Get-Command docker-compose -ErrorAction SilentlyContinue
    if ($hasDocker) { try { & docker compose version | Out-Null; if ($LASTEXITCODE -eq 0) { return "v2" } } catch { } }
    if ($hasLegacy) { return "legacy" }
    throw "⛔️  No se encontró ni 'docker compose' (v2) ni 'docker-compose' (legacy)."
}
$composeVariant = Get-ComposeVariant
# fijar en scope de script para que Invoke-Compose lo vea
$script:composeVariant = $composeVariant
Write-Host "ℹ️  Docker Compose: $composeVariant" -ForegroundColor Cyan

$ComposeBaseArgs = @()
if ($hasEnvFile) { $ComposeBaseArgs += @("--env-file", $envPath) }

function Invoke-Compose {
    param([Parameter(ValueFromRemainingArguments=$true)][string[]]$Args)
    if ($script:composeVariant -eq "legacy") {
        & docker-compose @ComposeBaseArgs @Args
    } else {
        & docker compose @ComposeBaseArgs @Args
    }
}

if ($Down) {
    Write-Host "▶️  Bajando servicios..." -ForegroundColor Cyan
    Invoke-Compose down
    Write-Host "✅  Listo." -ForegroundColor Green
    exit 0
}

# Semántica:
# - con -Seed → down -v, AUTO_SEED=true, SEED_RESET=true
# - sin -Seed → no tocar DB, AUTO_SEED=false, SEED_RESET=false
if ($Seed) {
    Write-Host "🧹  Reseed solicitado → docker compose down -v..." -ForegroundColor Yellow
    Invoke-Compose down -v
    $env:AUTO_SEED = "true"
    $env:SEED_RESET = "true"
} else {
    $env:AUTO_SEED = "false"
    $env:SEED_RESET = "false"
}

# Migrar siempre en runtime
$env:AUTO_MIGRATE = "true"

Write-Host ("ℹ️  Flags: AUTO_MIGRATE={0}  AUTO_SEED={1}  SEED_RESET={2}" -f $env:AUTO_MIGRATE, $env:AUTO_SEED, $env:SEED_RESET) -ForegroundColor Cyan

# Levantar DB
Write-Host "▶️  Levantando Postgres..." -ForegroundColor Cyan
Invoke-Compose up --detach db

# Esperar DB healthy
Write-Host "⏳  Esperando DB healthy..." -ForegroundColor Cyan
$retries = 60
$container = "espectaculos_db"
while ($retries -gt 0) {
    try {
        $status = (& docker inspect -f '{{.State.Health.Status}}' $container) 2>$null
        if ($status -eq "healthy") { break }
    } catch { }
    Start-Sleep -Seconds 2
    $retries--
}
if ($retries -le 0) { throw "⛔️  La base de datos no alcanzó estado healthy a tiempo." }
Write-Host "✅  DB healthy." -ForegroundColor Green

# Web
Write-Host "▶️  Construyendo y levantando Web..." -ForegroundColor Cyan
if ($Seed) { Invoke-Compose build --no-cache web }
Invoke-Compose up --detach --build web

# Health Web
$webBase = "http://localhost:$($env:WEB_PORT)"
$webUrl  = "$webBase/health"
Write-Host "⏳  Esperando /health..." -ForegroundColor Cyan
$ok = $false
for ($i=0; $i -lt 40; $i++) {
    try {
        $resp = Invoke-WebRequest -Uri $webUrl -UseBasicParsing -TimeoutSec 3
        if ($resp.StatusCode -eq 200) { $ok = $true; break }
    } catch { Start-Sleep -Seconds 2 }
}
if ($ok) {
    try {
        $resp2 = Invoke-WebRequest -Uri "$webBase/healthz" -UseBasicParsing -TimeoutSec 3
        if ($resp2.StatusCode -eq 200) {
            Write-Host "✅  Web saludable y lista (health + healthz)." -ForegroundColor Green
        } else {
            Write-Host "⚠️  /health OK pero /healthz no respondió 200." -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠️  /health OK pero /healthz no accesible." -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️  No se pudo confirmar el health de la Web." -ForegroundColor Yellow
}

# Abrir Swagger
Start-Sleep -Milliseconds $OpenDelayMs
$root = "$webBase/swagger"
Write-Host "🌐  $root" -ForegroundColor Cyan
try {
    if ($IsWindows) { Start-Process $root }
    elseif ($IsLinux) { xdg-open $root }
    elseif ($IsMacOS) { open $root }
    else { Start-Process $root }
} catch { }

if ($FollowLogs) {
    Write-Host "▶️  Logs web (Ctrl+C para salir)..." -ForegroundColor Cyan
    Invoke-Compose logs -f web
}
