#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE USER admin WITH PASSWORD 'admin' SUPERUSER;
    
    CREATE DATABASE "patterns-account-db" OWNER admin;
    CREATE DATABASE "patterns-core-db" OWNER admin;
    CREATE DATABASE "patterns-loan-db" OWNER admin;
    CREATE DATABASE "monitoring" OWNER admin;
    
    GRANT ALL PRIVILEGES ON DATABASE "patterns-account-db" TO admin;
    GRANT ALL PRIVILEGES ON DATABASE "patterns-core-db" TO admin;
    GRANT ALL PRIVILEGES ON DATABASE "patterns-loan-db" TO admin;
    GRANT ALL PRIVILEGES ON DATABASE "monitoring" TO admin;
EOSQL