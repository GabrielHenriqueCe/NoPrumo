@echo off
REM ---------------------------------------------------------------
REM  NoPrumo - sobe o ambiente de desenvolvimento local.
REM
REM  Abre duas janelas, uma para a API e outra para o front, e chama
REM  o navegador na tela de login. Ctrl+C em cada janela para parar.
REM
REM  Pre-requisitos: MySQL no ar e as chaves Jwt:* em User Secrets.
REM ---------------------------------------------------------------

cd /d "%~dp0"

echo Subindo a API (porta 5262)...
start "NoPrumo API" cmd /k dotnet run --project back\01-Presentation --launch-profile http

if not exist "front\node_modules" (
    echo Primeira execucao: instalando as dependencias do front...
    call npm install --prefix front
)

echo Subindo o front (porta 5173)...
start "NoPrumo front" cmd /k "pushd front && npm run dev"

REM O Vite leva alguns segundos para atender; abrir antes daria erro de conexao.
timeout /t 8 /nobreak >nul
start http://localhost:5173

echo.
echo Pronto. O sistema roda nas duas janelas que abriram.
