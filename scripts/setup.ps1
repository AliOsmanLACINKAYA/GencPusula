param([string]$DbPassword)

$ErrorActionPreference = "Stop"
if (-not $DbPassword) {
  $sec = Read-Host -AsSecureString "Veritabani parolasi (gencpusula_app icin)"
  $DbPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
      [Runtime.InteropServices.Marshal]::SecureStringToBSTR($sec))
}
if ($DbPassword.Contains(';') -or $DbPassword.Contains("'") -or $DbPassword.Contains('$')) {
  throw "Parolada noktali virgul, tek tirnak veya dolar isareti kullanmayin."
}

$Root = Split-Path -Parent $PSScriptRoot

Write-Host "==> 1/4  .NET araclari geri yukleniyor"
Push-Location "$Root\api"
dotnet tool restore
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "dotnet tool restore basarisiz." }
Pop-Location

Write-Host "==> 2/4  Veritabani ve kullanici"
$sqlFile = "$Root\scripts\setup-db.sql"
$psql = Get-Command psql -ErrorAction SilentlyContinue
if (-not $psql) {
  $psql = Get-ChildItem "C:\Program Files\PostgreSQL\*\bin\psql.exe" -ErrorAction SilentlyContinue |
          Select-Object -Last 1
}
if (-not $psql) {
  Write-Host "psql bulunamadi: scripts\setup-db.sql dosyasini pgAdmin ile calistirip tekrar dene." -ForegroundColor Yellow
  exit 1
}
$exe = if ($psql -is [System.Management.Automation.ApplicationInfo]) { $psql.Source } else { $psql.FullName }
& $exe -U postgres -v db_password="$DbPassword" -f $sqlFile
if ($LASTEXITCODE -ne 0) {
  if (Get-Command sudo -ErrorAction SilentlyContinue) {
    Write-Host "   yerel psql basarisiz; sudo -u postgres ile deneniyor..."
    Get-Content $sqlFile | & sudo -u postgres psql -v db_password="$DbPassword"
  }
  if ($LASTEXITCODE -ne 0) {
    throw "Veritabani kurulumu basarisiz. Parolayi kontrol edin; gerekirse scripts\setup-db.sql dosyasini pgAdmin ile calistirin."
  }
}

Write-Host "==> 3/4  Gizli ayarlar"
Push-Location "$Root\api\GencPusula.Web"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=gencpusula;Username=gencpusula_app;Password=$DbPassword" | Out-Null
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "user-secrets yazilamadi." }
dotnet user-secrets set "Jwt:Key" (-join ((1..96) | ForEach-Object { '{0:x}' -f (Get-Random -Maximum 16) })) | Out-Null
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "user-secrets yazilamadi." }

Write-Host "==> 4/4  Sema"
dotnet ef database update
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "dotnet ef database update basarisiz." }
Pop-Location

Write-Host "`nTamam. Calistir: cd $Root\api\GencPusula.Web; dotnet run" -ForegroundColor Green