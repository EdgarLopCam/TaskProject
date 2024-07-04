# Task Management API

This RESTful API enables task management, including creating, updating, deleting, and querying tasks.
Implement good development practices, such as CQRS, DDD and design patterns such as UnitOfWork, Repository and pagination.

It is important to mention that this project did not have the need to implement complex patterns but for demonstration purposes
of skills it was decided to carry out all these implementations, in the frontend part pure javascript version ecs6 was used, given
that the requirements did not allow for any framework such as react or angular.

## Previous requirements

- **Lenguaje/Framework:** .NET Core 7.0
- **Base de Datos:** SQL Server 2016 o superior
- **Otros:** Node.js (si se necesita ejecutar un servidor local para pruebas de frontend)
- **Paquetes NuGet:**
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - MediatR
  - FluentValidation
  - Swashbuckle.AspNetCore
  - Moq
  - xUnit

## Settings

1. **Modificar el domio y puerto deacuerdo a su entorno local dentro del archivo appsettings**
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Persist Security Info=False;Initial Catalog=TaskDB;User ID=sa;Password=yourPassword;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  },
  "RetryPolicy": {
    "MaxRetryCount": 3,
    "DelayMilliseconds": 200
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}

 It will also be important to update the domain and port of the API, which is inside the config.json file. The field is this:
 "apiUrl": "https://localhost:7150/api/v1/"

2. A Retry service was also implemented in case a failure occurs, concurrency was also implemented, attempts can also be configured in appsettings.js

3. The API project has its documentation that can be viewed in your local environment domain.
4. It will be necessary to run the database script, with that we will create the db and the tables with the initial configuration.
5. Finally, you only need to run the webApi project from visual studio and it will be necessary to build the frontend
 with a local node server or any other, the port or domain of the frontend does not matter much since the api
 I made the configuration to allow any request from any domain with CORS.
 
6. The frontend project and API plus the database script are in the same folder.
7. inside the DB_Create_TaskDB_Tables_v1.0.sql script, please change the following lines with a path according to your local environment:
 DECLARE @DataFilePath NVARCHAR(260) = N'D:\TaskDB.mdf';
 DECLARE @LogFilePath NVARCHAR(260) = N'D:\TaskDB_log.ldf';
It will also be important to give write access to those folders.