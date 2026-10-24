#!/usr/bin/env bash
set -euo pipefail

DB_PASSWORD="${1:-}"
if [ -z "$DB_PASSWORD" ]; then
  read -r -s -p "Veritabani parolasi (gencpusula_app icin): " DB_PASSWORD; echo
fi
[ -z "$DB_PASSWORD" ] && { echo "Parola bos olamaz."; exit 1; }
case "$DB_PASSWORD" in *";"*|*"'"*) echo "Parolada ; veya ' kullanmayin."; exit 1;; esac

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

command -v psql >/dev/null 2>&1 || {
  echo "psql bulunamadi. scripts/setup-db.sql dosyasini elle calistirin."
  exit 1
}

echo "==> 1/4  .NET araclari geri yukleniyor"
(cd "$ROOT/api" && dotnet tool restore)

echo "==> 2/4  Veritabani ve kullanici"
if psql -w -U postgres -v db_password="$DB_PASSWORD" -f "$ROOT/scripts/setup-db.sql"; then
  echo "   (yerel psql ile tamamlandi)"
elif sudo -u postgres psql -v db_password="$DB_PASSWORD" < "$ROOT/scripts/setup-db.sql"; then
  echo "   (sudo -u postgres ile tamamlandi)"
else
  echo "Veritabani kurulumu basarisiz. SQL'i elle calistirin:"
  echo "  sudo -u postgres psql -v db_password='...' < scripts/setup-db.sql"
  exit 1
fi

echo "==> 3/4  Gizli ayarlar"
cd "$ROOT/api/GencPusula.Web"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Database=gencpusula;Username=gencpusula_app;Password=$DB_PASSWORD" >/dev/null
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 48)" >/dev/null

echo "==> 4/4  Sema"
dotnet ef database update

echo; echo "Tamam. Calistir: cd $ROOT/api/GencPusula.Web && dotnet run"