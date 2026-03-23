-- ============================================
-- Script de inicialización de la base de datos StayHub
-- ============================================

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StayHubDb')
BEGIN
    CREATE DATABASE StayHubDb;
END
GO

USE StayHubDb;
GO

-- ============================================
-- Tabla: Hoteles
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hoteles' AND xtype='U')
BEGIN
    CREATE TABLE Hoteles (
        HotelId INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Ciudad NVARCHAR(50) NOT NULL,
        Direccion NVARCHAR(200) NOT NULL,
        Estado INT NOT NULL DEFAULT 1, -- 0: Inactivo, 1: Activo
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_Hoteles_Ciudad ON Hoteles(Ciudad);
    CREATE INDEX IX_Hoteles_Estado ON Hoteles(Estado);
END
GO

-- ============================================
-- Tabla: Habitaciones
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Habitaciones' AND xtype='U')
BEGIN
    CREATE TABLE Habitaciones (
        HabitacionId INT IDENTITY(1,1) PRIMARY KEY,
        HotelId INT NOT NULL,
        NumeroHabitacion NVARCHAR(10) NOT NULL,
        TipoHabitacion NVARCHAR(50) NOT NULL,
        Capacidad INT NOT NULL,
        TarifaNoche DECIMAL(18,2) NOT NULL,
        Estado INT NOT NULL DEFAULT 1, -- 0: Inactivo, 1: Activo
        CONSTRAINT FK_Habitaciones_Hotel FOREIGN KEY (HotelId) REFERENCES Hoteles(HotelId)
    );

    CREATE INDEX IX_Habitaciones_HotelId ON Habitaciones(HotelId);
    CREATE INDEX IX_Habitaciones_Estado ON Habitaciones(Estado);
    CREATE UNIQUE INDEX IX_Habitaciones_HotelId_Numero ON Habitaciones(HotelId, NumeroHabitacion);
END
GO

-- ============================================
-- Tabla: Reservas
-- ============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Reservas' AND xtype='U')
BEGIN
    CREATE TABLE Reservas (
        ReservaId INT IDENTITY(1,1) PRIMARY KEY,
        HotelId INT NOT NULL,
        HabitacionId INT NOT NULL,
        HuespedNombre NVARCHAR(100) NOT NULL,
        HuespedDocumento NVARCHAR(20) NOT NULL,
        FechaEntrada DATE NOT NULL,
        FechaSalida DATE NOT NULL,
        CantidadHuespedes INT NOT NULL,
        ValorNoche DECIMAL(18,2) NOT NULL,
        TotalReserva DECIMAL(18,2) NOT NULL,
        EstadoReserva INT NOT NULL DEFAULT 1, -- 0: Cancelada, 1: Activa, 2: Completada
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Reservas_Hotel FOREIGN KEY (HotelId) REFERENCES Hoteles(HotelId),
        CONSTRAINT FK_Reservas_Habitacion FOREIGN KEY (HabitacionId) REFERENCES Habitaciones(HabitacionId),
        CONSTRAINT CK_Reservas_Fechas CHECK (FechaSalida > FechaEntrada)
    );

    CREATE INDEX IX_Reservas_HotelId ON Reservas(HotelId);
    CREATE INDEX IX_Reservas_HabitacionId ON Reservas(HabitacionId);
    CREATE INDEX IX_Reservas_Estado ON Reservas(EstadoReserva);
    CREATE INDEX IX_Reservas_Fechas ON Reservas(HabitacionId, FechaEntrada, FechaSalida);
END
GO

-- ============================================
-- Datos de prueba
-- ============================================

-- Insertar hoteles de ejemplo
IF NOT EXISTS (SELECT 1 FROM Hoteles)
BEGIN
    INSERT INTO Hoteles (Nombre, Ciudad, Direccion, Estado) VALUES
    ('Hotel Plaza Central', 'Bogotá', 'Carrera 7 #32-16', 1),
    ('Hotel Caribe Resort', 'Cartagena', 'Av. San Martín #8-44', 1),
    ('Hotel Montaña Azul', 'Medellín', 'Calle 10 #43-12', 1);
END
GO

-- Insertar habitaciones de ejemplo
IF NOT EXISTS (SELECT 1 FROM Habitaciones)
BEGIN
    -- Hotel Plaza Central (HotelId = 1)
    INSERT INTO Habitaciones (HotelId, NumeroHabitacion, TipoHabitacion, Capacidad, TarifaNoche, Estado) VALUES
    (1, '101', 'Sencilla', 1, 150000, 1),
    (1, '102', 'Sencilla', 1, 150000, 1),
    (1, '201', 'Doble', 2, 250000, 1),
    (1, '202', 'Doble', 2, 250000, 1),
    (1, '301', 'Suite', 4, 450000, 1);

    -- Hotel Caribe Resort (HotelId = 2)
    INSERT INTO Habitaciones (HotelId, NumeroHabitacion, TipoHabitacion, Capacidad, TarifaNoche, Estado) VALUES
    (2, 'A-101', 'Estándar', 2, 280000, 1),
    (2, 'A-102', 'Estándar', 2, 280000, 1),
    (2, 'B-201', 'Deluxe', 3, 380000, 1),
    (2, 'B-202', 'Deluxe', 3, 380000, 1),
    (2, 'P-001', 'Presidencial', 6, 850000, 1);

    -- Hotel Montaña Azul (HotelId = 3)
    INSERT INTO Habitaciones (HotelId, NumeroHabitacion, TipoHabitacion, Capacidad, TarifaNoche, Estado) VALUES
    (3, '1A', 'Económica', 2, 120000, 1),
    (3, '1B', 'Económica', 2, 120000, 1),
    (3, '2A', 'Familiar', 4, 220000, 1),
    (3, '2B', 'Familiar', 4, 220000, 1);
END
GO

PRINT 'Base de datos StayHubDb inicializada correctamente.';
GO
