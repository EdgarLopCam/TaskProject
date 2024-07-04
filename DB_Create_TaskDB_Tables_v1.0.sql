/*
    Creation Date: 21-Jun-2024
    Autor: Edgar Lopez
    Version: 1.1
    Purpose:
    This script creates the "TaskDB" database and configures its data and log files.
    Prerequisites:
    - SQL Server 2016 or higher
    - sysadmin or dbcreator permissions
    Dependencies:
    - None
*/
BEGIN TRY
    DECLARE @IsSysAdmin BIT;
    DECLARE @IsDbCreator BIT;
    DECLARE @DatabaseName NVARCHAR(128) = N'TaskDB';
    DECLARE @DataFilePath NVARCHAR(260) = N'D:\TaskDB.mdf';
    DECLARE @LogFilePath NVARCHAR(260) = N'D:\TaskDB_log.ldf';

    SELECT @IsSysAdmin = IS_SRVROLEMEMBER('sysadmin');
    SELECT @IsDbCreator = IS_SRVROLEMEMBER('dbcreator');
    -- Check if the user has permissions to create the database.
    IF @IsSysAdmin = 1 OR @IsDbCreator = 1
    BEGIN
        -- Check if the specified paths exist.
        IF NOT EXISTS (SELECT * FROM sys.master_files WHERE physical_name = @DataFilePath)
        BEGIN
            -- Validate if the database exists.
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
            BEGIN
                DECLARE @Sql NVARCHAR(MAX);
                SET @Sql = N'
                CREATE DATABASE [' + @DatabaseName + N']
                ON 
                PRIMARY (
                    NAME = N''' + @DatabaseName + N''', 
                    FILENAME = N''' + @DataFilePath + N''', 
                    SIZE = 10MB, 
                    MAXSIZE = 500MB, -- UNLIMITED can also be used according to the context.
                    FILEGROWTH = 10MB
                )
                LOG ON (
                    NAME = N''' + @DatabaseName + N'_log'', 
                    FILENAME = N''' + @LogFilePath + N''', 
                    SIZE = 10MB, 
                    MAXSIZE = 500MB, -- UNLIMITED can also be used according to the context.
                    FILEGROWTH = 10MB
                )
                COLLATE Latin1_General_CI_AS; -- Collation to support case insensitivity.

                ALTER DATABASE [' + @DatabaseName + N'] 
                SET RECOVERY SIMPLE; -- To reduce the size of the transaction log.

                -- Configuration of ANSI NULLS and QUOTED IDENTIFIER.
                ALTER DATABASE [' + @DatabaseName + N'] SET ANSI_NULLS ON;
                ALTER DATABASE [' + @DatabaseName + N'] SET QUOTED_IDENTIFIER ON;
                ';
                
                EXEC sp_executesql @Sql;

                PRINT 'Database "' + @DatabaseName + '" created successfully.';
            END
            ELSE
            BEGIN
                PRINT 'The database "' + @DatabaseName + '" already exists.';
            END;
        END
        ELSE
        BEGIN
            PRINT 'The specified path for the files already exists.';
        END;
    END
    ELSE
    BEGIN
        PRINT 'The user does not have the necessary permissions to create a database.';
        RETURN;
    END;
END TRY
BEGIN CATCH
    PRINT 'An error occurred: ' + ERROR_MESSAGE();
    RETURN;
END CATCH;
GO

/*
    Creation Date: 21-Jun-2024
    Autor: Edgar Lopez
    Version: 1.0
    Purpose:
    This script creates the tables and configures its fields, Normalization was also implemented in the tables.
    Prerequisites:
    - create data base TaskDB
    Dependencies:
    - None
*/
DECLARE @DatabaseName NVARCHAR(128) = 'TaskDB';

BEGIN TRY
    -- Validate if the database exists.
    IF EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
    BEGIN
        -- Use the created or existing database.
        USE [TaskDB];

        -- Check if the TaskPriority table exists.
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaskPriority')
        BEGIN
            -- Create task priority table.
            CREATE TABLE [dbo].[TaskPriority] (
                [PriorityId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [PriorityName] NVARCHAR(50) NOT NULL UNIQUE
            );

            -- Inserting default values into the table.
            INSERT INTO [dbo].[TaskPriority] ([PriorityName]) VALUES ('Low'), ('Medium'), ('High');

            PRINT 'Table "TaskPriority" created successfully.';
        END
        ELSE
        BEGIN
            PRINT 'The table "TaskPriority" already exists.';
        END;

        -- Check if the TaskStatus table exists.
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaskStatus')
        BEGIN
            -- Create task status table.
            CREATE TABLE [dbo].[TaskStatus] (
                [StatusId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [StatusName] NVARCHAR(50) NOT NULL UNIQUE
            );

            -- Inserting default values into the table.
            INSERT INTO [dbo].[TaskStatus] ([StatusName]) VALUES ('Not Started'), ('In Progress'), ('Completed');

            PRINT 'Table "TaskStatus" created successfully.';
        END
        ELSE
        BEGIN
            PRINT 'The table "TaskStatus" already exists.';
        END;

        -- Check if the Tasks table exists.
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tasks')
        BEGIN
            -- Create tasks table.
            CREATE TABLE [dbo].[Tasks] (
                [TaskId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [Title] NVARCHAR(255) NOT NULL,
                [Description] NVARCHAR(MAX) NULL,
                [PriorityId] INT NOT NULL,
                [DueDate] DATETIME NULL,
                [StatusId] INT NOT NULL,
				[RowVersion] ROWVERSION NOT NULL, -- manage concurrency
                CONSTRAINT [UQ_Title] UNIQUE ([Title]),
                CONSTRAINT [FK_TaskPriority] FOREIGN KEY ([PriorityId]) REFERENCES [dbo].[TaskPriority]([PriorityId]),
                CONSTRAINT [FK_TaskStatus] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[TaskStatus]([StatusId])
            );

            -- Improve the performance of queries that filter or sort according to field
            CREATE INDEX [IX_DueDate] ON [dbo].[Tasks] ([DueDate]);
            CREATE INDEX [IX_PriorityId] ON [dbo].[Tasks] ([PriorityId]);
            CREATE INDEX [IX_StatusId] ON [dbo].[Tasks] ([StatusId]);
            CREATE INDEX [IX_Title_PriorityId] ON [dbo].[Tasks] ([Title], [PriorityId]);

            PRINT 'Table "Tasks" created successfully.';
        END
        ELSE
        BEGIN
            PRINT 'The table "Tasks" already exists.';
        END;

        -- Ensure that the SQL Server query optimizer has accurate and up-to-date information about the distribution of data in tables, 
        -- which is crucial for generating optimal execution plans and improving query performance.
        UPDATE STATISTICS [dbo].[Tasks];
        UPDATE STATISTICS [dbo].[TaskPriority];
        UPDATE STATISTICS [dbo].[TaskStatus];

        PRINT 'Statistics updated successfully.';
    END
    ELSE
    BEGIN
        PRINT 'The database "TaskDB" does not exist.';
    END;
END TRY
BEGIN CATCH
    PRINT 'An error occurred: ' + ERROR_MESSAGE();
END CATCH;
GO
