-- =====================================
-- CREACIÓN DE BASE DE DATOS
-- =====================================
CREATE DATABASE CineDB;
GO

USE CineDB;
GO

-- =====================================
-- TABLA: Roles
-- =====================================
CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL,
    UsuarioCreacion VARCHAR(100),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioModificacion VARCHAR(100),
    FechaModificacion DATETIME NULL
);
GO

INSERT INTO Roles (NombreRol, UsuarioCreacion)
VALUES ('ADMIN', 'system'), ('INVITADO', 'system');
GO

-- =====================================
-- TABLA: Usuarios
-- =====================================
CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Contrasenia VARCHAR(255) NOT NULL,
    Activo BIT DEFAULT 1,
    IdRol INT NOT NULL,
    UsuarioCreacion VARCHAR(100) NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioModificacion VARCHAR(100),
    FechaModificacion DATETIME NULL,
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);
GO

-- =====================================
-- TABLA: Peliculas
-- =====================================
CREATE TABLE Peliculas (
    IdPelicula INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(150) NOT NULL,
    Descripcion VARCHAR(500),
    Director VARCHAR(100),
    Anio INT,
    Genero VARCHAR(100),
    UsuarioCreacion VARCHAR(100),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioModificacion VARCHAR(100),
    FechaModificacion DATETIME NULL
);
GO

-- =====================================
-- STORED PROCEDURES PARA USUARIOS
-- =====================================

USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_InsertUsuario]
    @FullName         VARCHAR(100),
    @Correo           VARCHAR(100),
    @Contrasenia      VARCHAR(255),
    @IdRol            INT,
    @UsuarioCreacion  VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Usuarios (FullName, Correo, Contrasenia, IdRol, UsuarioCreacion)
    OUTPUT 
        inserted.IdUsuario,
        inserted.FullName,
        inserted.Correo,
        inserted.Contrasenia,
        inserted.IdRol,
        inserted.UsuarioCreacion
    VALUES (@FullName, @Correo, @Contrasenia, @IdRol, @UsuarioCreacion);
END;

-- Listar usuarios invitados segun sea creado por su administrador
USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_ListUsuarios]
    @UsuarioCreacion VARCHAR(100)  
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.IdUsuario,
        u.FullName,
        u.Correo,
        u.Activo,
        r.NombreRol,
        u.FechaCreacion,
        u.UsuarioCreacion
    FROM dbo.Usuarios u
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE (u.IdRol = 2 OR UPPER(r.NombreRol) = 'INVITADO')  
      AND u.UsuarioCreacion = @UsuarioCreacion              
    ORDER BY u.FechaCreacion DESC;
END


-- Actualizar usuario invitado desde la cuenta logueada del admin

USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[sp_UpdateUsuarioInvitado]
    @IdUsuario    INT,
    @UsuarioAdmin VARCHAR(100),
    @FullName     VARCHAR(100) = NULL,
    @Correo       VARCHAR(100) = NULL,
    @Activo       BIT = NULL
AS
BEGIN


    IF NOT EXISTS (
        SELECT 1 FROM dbo.Usuarios
        WHERE IdUsuario = @IdUsuario AND IdRol = 2 AND UsuarioCreacion = @UsuarioAdmin
    )
    BEGIN
        RAISERROR('Usuario no permitido o no es INVITADO del admin.', 16, 1);
        RETURN;
    END

    IF @Correo IS NOT NULL AND EXISTS(
        SELECT 1 FROM dbo.Usuarios WHERE Correo = @Correo AND IdUsuario <> @IdUsuario
    )
    BEGIN
        RAISERROR('El correo ya existe.', 16, 1);
        RETURN;
    END

    UPDATE u
       SET FullName            = COALESCE(@FullName, u.FullName),
           Correo              = COALESCE(@Correo,   u.Correo),
           Activo              = CASE WHEN @Activo IS NOT NULL THEN @Activo ELSE u.Activo END,
           UsuarioModificacion = @UsuarioAdmin,
           FechaModificacion   = GETDATE()
    FROM dbo.Usuarios u
    WHERE u.IdUsuario = @IdUsuario
      AND u.IdRol = 2
      AND u.UsuarioCreacion = @UsuarioAdmin;
END

CREATE PROCEDURE sp_UpdateUsuario
    @IdUsuario INT,
    @FullName VARCHAR(100),
    @Correo VARCHAR(100),
    @Contrasenia VARCHAR(255),
    @Activo BIT,
    @IdRol INT,
    @UsuarioModificacion VARCHAR(100)
AS
BEGIN
    UPDATE Usuarios
    SET FullName = @FullName,
        Correo = @Correo,
        Contrasenia = @Contrasenia,
        Activo = @Activo,
        IdRol = @IdRol,
        UsuarioModificacion = @UsuarioModificacion,
        FechaModificacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- Eliminar usuario invitado creado por su administrador
USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[sp_DeleteUsuarioInvitado]
    @IdUsuario     INT,
    @UsuarioAdmin  VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE u
    FROM dbo.Usuarios u
    WHERE u.IdUsuario = @IdUsuario
      AND u.IdRol = 2
      AND u.UsuarioCreacion = @UsuarioAdmin;

    SELECT @@ROWCOUNT AS RowsAffected;
END


-- =====================================
-- STORED PROCEDURES PARA PELICULAS
-- =====================================

-- Insertar película
USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Insertar película
CREATE PROCEDURE [dbo].[sp_InsertPelicula]
    @Titulo VARCHAR(150),
    @Descripcion VARCHAR(500),
    @Director VARCHAR(100),
    @Anio INT,
    @Genero VARCHAR(100),
    @UsuarioCreacion VARCHAR(100)
AS
BEGIN
    INSERT INTO Peliculas (Titulo, Descripcion, Director, Anio, Genero, UsuarioCreacion)
    VALUES (@Titulo, @Descripcion, @Director, @Anio, @Genero, @UsuarioCreacion);
END;


-- Listar películas
USE [CineDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[sp_ListPeliculas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPelicula,
        Titulo,
        Descripcion,
        Director,
        Anio,
        Genero,
        FechaCreacion
    FROM dbo.Peliculas
    ORDER BY IdPelicula DESC;
END

GO

-- Eliminar película
USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Eliminar película
CREATE  PROCEDURE [dbo].[sp_DeletePelicula]
    @IdPelicula INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Peliculas
    WHERE IdPelicula = @IdPelicula;

    SELECT @@ROWCOUNT AS RowsAffected;
END

-- Token de los usuarios para las APIS y se pueda loguear
USE [CineDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_LoginUsuario]
    @Correo VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        u.IdUsuario,
        u.FullName,
        u.Correo,
        u.Contrasenia,
        u.IdRol,
        r.NombreRol,
        u.UsuarioCreacion
    FROM dbo.Usuarios u
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE u.Correo = @Correo
      AND u.Activo = 1;
END

GO
