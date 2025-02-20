-- Create databases
CREATE DATABASE userservicedb;
CREATE DATABASE identitydb;

-- Connect to userservicedb
\c userservicedb

-- Create extension if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create users table
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Username" VARCHAR(50) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "FirstName" VARCHAR(50) NOT NULL,
    "LastName" VARCHAR(50) NOT NULL,
    "DateOfBirth" DATE NOT NULL,
    "PhoneNumber" VARCHAR(20),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create unique indexes
CREATE UNIQUE INDEX idx_users_username ON "Users"("Username");
CREATE UNIQUE INDEX idx_users_email ON "Users"("Email");