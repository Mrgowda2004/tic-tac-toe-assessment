@echo off
echo Starting .NET 8 Backend API on http://localhost:5000 ...
cd backend\TicTacToe.Api
dotnet run --launch-profile http
pause
