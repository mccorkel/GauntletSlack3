#!/bin/bash

cd C:\Users\maxmc\source\repos\mccorkel\GauntletSlack3

echo "Removing existing migrations..."
dotnet ef migrations remove -p GauntletSlack3.Api

echo "Dropping database..."
dotnet ef database drop -p GauntletSlack3.Api --force

echo "Creating new migration..."
dotnet ef migrations add InitialCreate -p GauntletSlack3.Api

echo "Updating database..."
dotnet ef database update -p GauntletSlack3.Api

echo "Database reset complete!"