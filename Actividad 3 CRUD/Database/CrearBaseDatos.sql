/* Paso 6: ejecutar este archivo en SQL Server Management Studio conectado a LocalDB. */
IF DB_ID(N'Actividad3CRUD') IS NULL
    CREATE DATABASE Actividad3CRUD;
GO
USE Actividad3CRUD;
GO

IF OBJECT_ID(N'dbo.Compras', N'U') IS NOT NULL DROP TABLE dbo.Compras;
IF OBJECT_ID(N'dbo.Productos', N'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID(N'dbo.Proveedores', N'U') IS NOT NULL DROP TABLE dbo.Proveedores;
GO

CREATE TABLE dbo.Clientes (
    ClienteId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(80) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono VARCHAR(20) NOT NULL
);

CREATE TABLE dbo.Proveedores (
    ProveedorId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nif VARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(180) NOT NULL
);

CREATE TABLE dbo.Productos (
    ProductoId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
    ProveedorId INT NOT NULL,
    CONSTRAINT FK_Productos_Proveedores FOREIGN KEY (ProveedorId)
        REFERENCES dbo.Proveedores(ProveedorId)
);

CREATE TABLE dbo.Compras (
    CompraId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ClienteId INT NOT NULL,
    ProductoId INT NOT NULL,
    CONSTRAINT UQ_Compras_Cliente_Producto UNIQUE (ClienteId, ProductoId),
    CONSTRAINT FK_Compras_Clientes FOREIGN KEY (ClienteId)
        REFERENCES dbo.Clientes(ClienteId),
    CONSTRAINT FK_Compras_Productos FOREIGN KEY (ProductoId)
        REFERENCES dbo.Productos(ProductoId)
);
GO
