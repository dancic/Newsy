cd Newsy.Persistence
call migrate.bat

cd ..
call dotnet build

cd Newsy.API
call dotnet run