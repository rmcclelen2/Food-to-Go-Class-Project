using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace FA21.P05.Tests.Web.Helpers
{
    public sealed class SqlServerTestDatabaseProvider : IDisposable
    {
        private const string DbPrefix = "CMPS383-FA21-";
        private static string databaseName;
        private static string backupPath;

        public static void AssemblyInit()
        {
            if (databaseName != null)
            {
                return;
            }

            databaseName = $"{DbPrefix}{Guid.NewGuid():N}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var sqlConnection = new SqlConnection("server=(localdb)\\mssqllocaldb");

                sqlConnection.Open();
                var tempPath = Environment.GetEnvironmentVariable("SqlServerTestDatabaseMdfPath") ?? Path.GetTempPath();
                var mdfPath = $"{tempPath}{databaseName}";
                var sql = $@"
        CREATE DATABASE
            [{databaseName}]
        ON PRIMARY (
           NAME=Test_data,
           FILENAME = '{mdfPath}.mdf'
        )
        LOG ON (
            NAME=Test_log,
            FILENAME = '{mdfPath}.ldf'
        )";

                var command = new SqlCommand(sql, sqlConnection);
                command.ExecuteNonQuery();
            }
            else
            {
                var master = GetConnection("master");
                using var sqlConnection = new SqlConnection(master);

                sqlConnection.Open();

                var command = new SqlCommand($"CREATE DATABASE [{databaseName}]", sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        public static void ApplicationCleanup()
        {
            DeleteDatabase();
            databaseName = null;
        }

        public string GetConnectionString()
        {
            if (databaseName == null)
            {
                throw new Exception("SQL not configured");
            }
            return GetConnection(databaseName);
        }

        public void RestoreBackup()
        {
            if (databaseName == null || backupPath == null)
            {
                return;
            }
            var master = GetConnection("master");
            using var sqlConnection = new SqlConnection(master);

            sqlConnection.Open();
            var command = new SqlCommand($@"
ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE

RESTORE DATABASE [{databaseName}]
FROM DISK = N'{backupPath}'
WITH REPLACE

ALTER DATABASE [{databaseName}] SET MULTI_USER
", sqlConnection);
            command.ExecuteNonQuery();
        }

        public void CreateBackup()
        {
            if (databaseName == null || backupPath != null)
            {
                return;
            }

            var master = GetConnection("master");
            using var sqlConnection = new SqlConnection(master);
            backupPath = Path.Combine(Environment.GetEnvironmentVariable("SqlServerTestDatabaseMdfPath") ?? Path.GetTempPath(), Guid.NewGuid() + ".bak");

            sqlConnection.Open();
            var command = new SqlCommand($@"
BACKUP DATABASE [{databaseName}]
TO DISK = N'{backupPath}'
", sqlConnection);
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (databaseName == null)
            {
                return;
            }

            var master = GetConnection("master");
            using var sqlConnection = new SqlConnection(master);

            sqlConnection.Open();
            //see: https://stackoverflow.com/a/14997851/1590723
            var command = new SqlCommand($@"
USE [{databaseName}]

DECLARE @name VARCHAR(128)
DECLARE @SQL VARCHAR(254)
DECLARE @constraint VARCHAR(254)

/* Drop all non-system stored procs */
SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id WHERE [type] = 'P' AND category = 0 ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')

WHILE @name is not null
BEGIN
    SELECT @SQL = 'DROP PROCEDURE ' + @name
    EXEC (@SQL)
    PRINT 'Dropped Procedure: ' + @name
    SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id WHERE [type] = 'P' AND category = 0 AND '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' > @name ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')
END

/* Drop all views */
SELECT @name = (SELECT TOP 1  '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id  WHERE [type] = 'V' AND category = 0 ORDER BY  '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP VIEW ' + @name
    EXEC (@SQL)
    PRINT 'Dropped View: ' + @name
    SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id WHERE [type] = 'V' AND category = 0 AND  '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' > @name ORDER BY  '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')
END

/* Drop all functions */
SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id  WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' )

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP FUNCTION ' + @name
    EXEC (@SQL)
    PRINT 'Dropped Function: ' + @name
    SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']'  FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id  WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 AND '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' > @name ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' )
END

/* Drop all Foreign Key constraints */
SELECT @name = (SELECT TOP 1 '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' ORDER BY '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']')

WHILE @name is not null
BEGIN
    SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' = @name ORDER BY CONSTRAINT_NAME)
    WHILE @constraint IS NOT NULL
    BEGIN
        SELECT @SQL = 'ALTER TABLE ' + @name +' DROP CONSTRAINT [' + RTRIM(@constraint) +']'
        EXEC (@SQL)
        PRINT 'Dropped FK Constraint: ' + @constraint + ' on ' + @name
        SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND CONSTRAINT_NAME > @constraint AND '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' = @name ORDER BY CONSTRAINT_NAME)
    END
	SELECT @name = (SELECT TOP 1 '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'FOREIGN KEY' AND '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' > @name ORDER BY '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']')
	PRINT 'Heading to ' + @name
END

/* Drop all Primary Key constraints */
SELECT @name = (SELECT TOP 1 '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' ORDER BY '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']')

WHILE @name IS NOT NULL
BEGIN
    SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = @name ORDER BY CONSTRAINT_NAME)
    WHILE @constraint is not null
    BEGIN
        SELECT @SQL = 'ALTER TABLE '+@name+' DROP CONSTRAINT [' + RTRIM(@constraint)+']'
        EXEC (@SQL)
        PRINT 'Dropped PK Constraint: ' + @constraint + ' on ' + @name
        SELECT @constraint = (SELECT TOP 1 CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' AND CONSTRAINT_NAME <> @constraint AND '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' = @name ORDER BY CONSTRAINT_NAME)
    END
	SELECT @name = (SELECT TOP 1 '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE constraint_catalog=DB_NAME() AND CONSTRAINT_TYPE = 'PRIMARY KEY' AND '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']' > @name ORDER BY '[' + RTRIM(CONSTRAINT_SCHEMA) + '].[' + RTRIM(TABLE_NAME) + ']')
	PRINT 'Heading to ' + @name
END

/* Drop all tables */
SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id WHERE sysobjects.[type] = 'U' AND sysobjects.category = 0 ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')

WHILE @name IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP TABLE ' + @name
    EXEC (@SQL)
    PRINT 'Dropped Table: ' + @name
    SELECT @name = (SELECT TOP 1 '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' FROM sysobjects INNER JOIN sys.schemas ON sysobjects.uid = sys.schemas.schema_id WHERE sysobjects.[type] = 'U' AND sysobjects.category = 0 AND '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']' > @name ORDER BY '['+RTRIM(sys.schemas.[name]) + '].[' + RTRIM(sysobjects.[name]) + ']')
END
", sqlConnection);
            command.ExecuteNonQuery();
        }

        private static string GetConnection(string name)
        {
            string connection;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                connection = $"Server=localhost,1433;Database={name};User Id=sa;Password=Password123!;";
            }
            else
            {
                connection = $"Server=(localdb)\\mssqllocaldb;Database={name};Trusted_Connection=True";
            }

            return connection;
        }

        private static void DeleteDatabase()
        {
            if (backupPath != null)
            {
                File.Delete(backupPath);
                backupPath = null;
            }
            var master = GetConnection("master");
            var cleanProcessPath = Environment.GetEnvironmentVariable("SqlServerTestDatabaseCleanTool");
            if (string.IsNullOrWhiteSpace(cleanProcessPath))
            {
                using var sqlConnection = new SqlConnection(master);

                sqlConnection.Open();
                var command = new SqlCommand(@$"
IF EXISTS(select * from sys.databases where name='{databaseName}')
BEGIN
    ALTER DATABASE  [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
END

DROP DATABASE IF EXISTS [{databaseName}];
", sqlConnection);
                command.ExecuteNonQuery();
            }
            else
            {
                Process.Start(cleanProcessPath, $"\"{master}\" \"{databaseName}\"");
            }
        }
    }
}