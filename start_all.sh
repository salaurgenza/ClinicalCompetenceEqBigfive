#!/bin/bash
# Avvia backend .NET e frontend Angular nello stesso terminale

# --- Backend .NET ---
cd "/Users/stefanopepe/Downloads/ClinicalCompetence_IIS_Angular_DotNet copia/ClinicalCompetence.Api"
ASPNETCORE_ENVIRONMENT=Development dotnet run &
BACKEND_PID=$!

# --- Frontend Angular ---
cd "/Users/stefanopepe/Downloads/ClinicalCompetence_IIS_Angular_DotNet copia/ClinicalCompetence.Web"
npm start &
FRONTEND_PID=$!

echo "🚀 Backend PID=$BACKEND_PID e Frontend PID=$FRONTEND_PID avviati"
echo "Apri http://localhost:4200/ nel browser"
echo "Premi Ctrl+C per chiudere entrambi i processi"

# Attende che entrambi i processi siano terminati
wait $BACKEND_PID
wait $FRONTEND_PID