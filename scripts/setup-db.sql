-- GencPusula veritabani + uygulama kullanicisi (ilk kurulum, tekrar calistirilabilir)
-- Kullanim: psql -U postgres -v db_password='...' -f scripts/setup-db.sql

SELECT 'CREATE DATABASE gencpusula'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'gencpusula')\gexec

SELECT 'CREATE ROLE gencpusula_app LOGIN PASSWORD ' || quote_literal(:'db_password')
WHERE NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'gencpusula_app')\gexec

ALTER ROLE gencpusula_app WITH PASSWORD :'db_password';
GRANT ALL PRIVILEGES ON DATABASE gencpusula TO gencpusula_app;

\connect gencpusula
GRANT ALL ON SCHEMA public TO gencpusula_app;