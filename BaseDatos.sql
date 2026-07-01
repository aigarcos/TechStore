CREATE DATABASE TechStore;
GO
USE TechStore;
GO

-- Usuarios del sistema
CREATE TABLE Usuarios (
    Id               INT PRIMARY KEY IDENTITY,
    NombreUsuario    NVARCHAR(50)  NOT NULL UNIQUE,
    ContrasenaHash   NVARCHAR(256) NOT NULL,
    Rol              NVARCHAR(20)  NOT NULL CHECK (Rol IN ('Cajero','Administrador')),
    IntentosFailidos INT           DEFAULT 0,
    Activo           BIT           DEFAULT 1
);

-- Clientes de la tienda
CREATE TABLE Clientes (
    Id        INT PRIMARY KEY IDENTITY,
    Nombre    NVARCHAR(100) NOT NULL,
    Documento NVARCHAR(20)  NOT NULL UNIQUE,
    Telefono  NVARCHAR(20),
    Correo    NVARCHAR(100)
);

-- Catálogo de productos
CREATE TABLE Productos (
    Id        INT PRIMARY KEY IDENTITY,
    Codigo    NVARCHAR(30)   NOT NULL UNIQUE,
    Nombre    NVARCHAR(100)  NOT NULL,
    Categoria NVARCHAR(50),
    Precio    DECIMAL(10,2)  NOT NULL,
    Stock     INT            NOT NULL DEFAULT 0
);

-- Cabecera de ventas
CREATE TABLE Ventas (
    Id        INT PRIMARY KEY IDENTITY,
    Fecha     DATETIME      DEFAULT GETDATE(),
    ClienteId INT           REFERENCES Clientes(Id),
    UsuarioId INT           REFERENCES Usuarios(Id),
    Total     DECIMAL(10,2) NOT NULL
);

-- Líneas de detalle por venta
CREATE TABLE DetalleVenta (
    Id             INT PRIMARY KEY IDENTITY,
    VentaId        INT           REFERENCES Ventas(Id),
    ProductoId     INT           REFERENCES Productos(Id),
    Cantidad       INT           NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal       DECIMAL(10,2) NOT NULL
);

-- Facturas generadas
CREATE TABLE Facturas (
    Id            INT PRIMARY KEY IDENTITY,
    VentaId       INT           REFERENCES Ventas(Id),
    NumeroFactura NVARCHAR(20)  NOT NULL UNIQUE,
    FechaEmision  DATETIME      DEFAULT GETDATE(),
    MontoTotal    DECIMAL(10,2) NOT NULL
);

-- CMDB: infraestructura tecnológica de la tienda
-- No almacena productos vendidos. Solo activos tecnológicos propios de la empresa.
CREATE TABLE ItemsConfiguracion (
    Id                      INT PRIMARY KEY IDENTITY,
    Nombre                  NVARCHAR(100) NOT NULL,
    Tipo                    NVARCHAR(50)  NOT NULL
                              CHECK (Tipo IN ('Hardware','Software','Red')),
    Estado                  NVARCHAR(30)  NOT NULL
                              CHECK (Estado IN ('Activo','EnMantenimiento','Baja')),
    NumeroSerie             NVARCHAR(100),
    Descripcion             NVARCHAR(500),
    FechaAlta               DATETIME DEFAULT GETDATE(),
    InicioMantenimiento     DATETIME NULL,
    FinMantenimiento        DATETIME NULL,
    MotivoMantenimiento     NVARCHAR(300) NULL,
    FechaBaja               DATETIME NULL,
    MotivoBaja              NVARCHAR(300) NULL
);

-- Relaciones entre ítems de configuración
CREATE TABLE RelacionesCI (
    Id           INT PRIMARY KEY IDENTITY,
    CIOrigenId   INT          REFERENCES ItemsConfiguracion(Id),
    CIDestinoId  INT          REFERENCES ItemsConfiguracion(Id),
    TipoRelacion NVARCHAR(50) NOT NULL
                   CHECK (TipoRelacion IN ('aloja','ejecuta','conecta','depende_de')),
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Datos iniciales: usuarios
INSERT INTO Usuarios (NombreUsuario, ContrasenaHash, Rol)
VALUES
('admin',   '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',   'Administrador'), -- admin123 SHA256
('cajero1', 'eb71c89eb25c56784d16efccb5c4ad2036c0d8fb483be75abec74b47ebbd249a', 'Cajero'); -- cajero123 SHA256

-- Datos iniciales: productos
INSERT INTO Productos (Codigo, Nombre, Categoria, Precio, Stock) VALUES
('LAP001', 'Laptop Lenovo IdeaPad 5',    'Laptops',      1200.00, 8),
('LAP002', 'Laptop HP Pavilion 15',      'Laptops',       950.00, 5),
('MOU001', 'Mouse Logitech MX Master 3', 'Periféricos',    85.00, 20),
('KEY001', 'Teclado Mecánico Redragon',  'Periféricos',    65.00, 0),
('MON001', 'Monitor LG 27" 4K',          'Monitores',     650.00, 2),
('AUR001', 'Auriculares Sony WH-1000XM5','Audio',         380.00, 3),
('USB001', 'Hub USB-C 7 puertos',        'Accesorios',     35.00, 15);

-- Datos iniciales: CMDB (infraestructura de la tienda)
INSERT INTO ItemsConfiguracion (Nombre, Tipo, Estado, NumeroSerie, Descripcion) VALUES
('Servidor BD principal',  'Hardware', 'Activo',        'DELL-R740-001', 'Dell PowerEdge R740 - SQL Server host'),
('SQL Server 2019',        'Software', 'Activo',        NULL,            'Motor de base de datos principal'),
('PC Caja 01',             'Hardware', 'Activo',        'PC-CAJA-001',   'Equipo de escritorio del cajero 1'),
('PC Caja 02',             'Hardware', 'Activo',        'PC-CAJA-002',   'Equipo de escritorio del cajero 2'),
('TechStore App v1.0',     'Software', 'Activo',        NULL,            'Sistema de gestión TechStore'),
('Switch Cisco SG350',     'Red',      'EnMantenimiento', 'CISCO-SG350-01','Switch principal de la red local'),
('Router principal',       'Red',      'Activo',        'RT-MAIN-001',   'Router de acceso a internet'),
('Impresora facturas',     'Hardware', 'Activo',        'EPSON-TM-001',  'Epson TM-T20III para tickets');

-- Relaciones CMDB iniciales
-- Servidor aloja SQL Server
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (1, 2, 'aloja');
-- PC Caja 01 ejecuta TechStore App
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (3, 5, 'ejecuta');
-- PC Caja 02 ejecuta TechStore App
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (4, 5, 'ejecuta');
-- Switch conecta a PC Caja 01, PC Caja 02 y Servidor
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (6, 3, 'conecta');
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (6, 4, 'conecta');
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (6, 1, 'conecta');
-- TechStore App depende de SQL Server
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (5, 2, 'depende_de');
-- Impresora conecta a PC Caja 01
INSERT INTO RelacionesCI (CIOrigenId, CIDestinoId, TipoRelacion) VALUES (8, 3, 'conecta');
