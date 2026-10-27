@echo off
echo ==============================================================
echo           Tic Tac Toe Assessment Application Launcher
echo ==============================================================
echo.
echo Launching .NET 8 Backend API on http://localhost:5000 ...
start "TicTacToe Backend (.NET 8)" cmd /k "cd /d %~dp0backend\TicTacToe.Api && dotnet run --launch-profile http"

echo Waiting for backend to initialize...
timeout /t 4 /nobreak >nul

echo Launching React Frontend on http://localhost:3000 ...
start "TicTacToe Frontend (React Vite)" cmd /k "cd /d %~dp0frontend && npm run dev"

echo.
echo ==============================================================
echo Application running!
echo Frontend: http://localhost:3000
echo Backend API / Swagger: http://localhost:5000/swagger
echo ==============================================================
echo.
pause
