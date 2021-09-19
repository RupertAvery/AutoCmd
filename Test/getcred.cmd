@echo off

echo Enter your Username and Password

set /p user=Username:
set /p pass=Password:

echo.
echo The quick brown fox jumps over the lazy dogs

pause

echo Username = %user%
echo Password = %pass%

echo.

echo Exiting...

exit 0

