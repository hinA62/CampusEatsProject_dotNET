-- 🗄️ CampusEats Database Initialization Script
-- This script runs automatically when PostgreSQL container starts

\echo '🚀 Starting CampusEats database initialization...'

-- Create database if it doesn't exist (handled by POSTGRES_DB env var)
\echo '✅ Database CampusEatsDB created/verified'

-- Connect to the database
\c CampusEatsDB

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
\echo '✅ UUID extension enabled'

-- Create schema for future organization (optional)
-- CREATE SCHEMA IF NOT EXISTS campuseats;
-- \echo '✅ Schema created'

-- Note: Tables will be created automatically by EF Core migrations
\echo '⏳ Waiting for EF Core migrations to create tables...'

\echo '🎉 Database initialization complete!'
\echo '📊 Database: CampusEatsDB'
\echo '🔐 User: postgres'
\echo '🌐 Port: 5432'
