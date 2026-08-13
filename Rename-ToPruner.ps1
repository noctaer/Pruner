<#
    Rename-ToPruner.ps1
    Renomeia todo o projeto de Pruner/Pruner para Pruner.
    Executa substituicoes de texto em arquivos fonte e renomeia arquivos/pastas.
    Requer Git inicializado — faca commit antes de rodar.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root = $PSScriptRoot

# Pastas ignoradas — artefatos de build, nao codigo fonte
$excludedDirs = @("bin", "obj", "Installer", ".git", "Output")

# Extensoes de texto a processar
$textExtensions = @(
    ".cs", ".xaml", ".csproj", ".slnx", ".sln", ".iss",
    ".json", ".md", ".txt", ".ps1", ".xml", ".config"
)

# Mapa de substituicoes — ordem importa: mais especifico primeiro
$replacements = [ordered]@{
    "Pruner.UI"    = "Pruner.UI"
    "Pruner.Core"  = "Pruner.Core"
    "Pruner.IO"    = "Pruner.IO"
    "Pruner.CLI"   = "Pruner.CLI"
    "Pruner.Tests" = "Pruner.Tests"
    "Pruner"       = "Pruner"
    "PrunerUI"     = "Pruner"
    "Pruner"      = "Pruner"
    "Pruner"      = "Pruner"
    "Pruner"     = "Pruner"
}

function ShouldExclude($path) {
    foreach ($dir in $excludedDirs) {
        if ($path -match [regex]::Escape([IO.Path]::DirectorySeparatorChar + $dir + [IO.Path]::DirectorySeparatorChar) -or
            $path -match [regex]::Escape([IO.Path]::DirectorySeparatorChar + $dir + "$")) {
            return $true
        }
    }
    return $false
}

function ApplyReplacements($text) {
    foreach ($key in $replacements.Keys) {
        $text = $text.Replace($key, $replacements[$key])
    }
    return $text
}

Write-Host "`n=== Pruner Rename Script ===" -ForegroundColor Cyan
Write-Host "Root: $root`n"

# --- FASE 1: substituicao de conteudo nos arquivos ---
Write-Host "[1/3] Substituindo conteudo dos arquivos..." -ForegroundColor Yellow

$files = Get-ChildItem -Path $root -Recurse -File | Where-Object {
    -not (ShouldExclude $_.FullName) -and
    ($textExtensions -contains $_.Extension.ToLower())
}

$changedFiles = 0
foreach ($file in $files) {
    $original = [IO.File]::ReadAllText($file.FullName, [Text.Encoding]::UTF8)
    $updated  = ApplyReplacements $original
    if ($original -ne $updated) {
        [IO.File]::WriteAllText($file.FullName, $updated, [Text.Encoding]::UTF8)
        Write-Host "  [txt] $($file.FullName.Replace($root, ''))" -ForegroundColor Gray
        $changedFiles++
    }
}
Write-Host "  $changedFiles arquivo(s) modificado(s)`n"

# --- FASE 2: renomear arquivos ---
Write-Host "[2/3] Renomeando arquivos..." -ForegroundColor Yellow

$filesToRename = Get-ChildItem -Path $root -Recurse -File | Where-Object {
    -not (ShouldExclude $_.FullName) -and
    ($_.Name -match "Pruner|Pruner|PrunerUI")
}

$renamedFiles = 0
foreach ($file in $filesToRename) {
    $newName = ApplyReplacements $file.Name
    if ($newName -ne $file.Name) {
        $newPath = Join-Path $file.DirectoryName $newName
        Rename-Item -Path $file.FullName -NewName $newName
        Write-Host "  [file] $($file.Name) -> $newName" -ForegroundColor Gray
        $renamedFiles++
    }
}
Write-Host "  $renamedFiles arquivo(s) renomeado(s)`n"

# --- FASE 3: renomear pastas (de dentro para fora) ---
Write-Host "[3/3] Renomeando pastas..." -ForegroundColor Yellow

$dirsToRename = Get-ChildItem -Path $root -Recurse -Directory |
    Where-Object { -not (ShouldExclude $_.FullName) -and ($_.Name -match "Pruner|Pruner") } |
    Sort-Object { $_.FullName.Length } -Descending

$renamedDirs = 0
foreach ($dir in $dirsToRename) {
    $newName = ApplyReplacements $dir.Name
    if ($newName -ne $dir.Name) {
        $newPath = Join-Path $dir.Parent.FullName $newName
        Rename-Item -Path $dir.FullName -NewName $newName
        Write-Host "  [dir]  $($dir.Name) -> $newName" -ForegroundColor Gray
        $renamedDirs++
    }
}
Write-Host "  $renamedDirs pasta(s) renomeada(s)`n"

Write-Host "=== Concluido ===" -ForegroundColor Green
Write-Host "Execute: dotnet build -c Release para verificar`n"