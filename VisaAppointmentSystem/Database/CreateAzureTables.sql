-- Script para crear las tablas en Azure SQL Database

-- Tabla para almacenar los horarios disponibles para citas
CREATE TABLE AvailableSlots (
    SlotID INT PRIMARY KEY IDENTITY(1,1),
    SlotDateTime DATETIME NOT NULL,
    IsBooked BIT NOT NULL DEFAULT 0,
    Location NVARCHAR(100) NOT NULL
);
GO

-- Tabla para almacenar la información de los usuarios (solicitantes)
-- En un escenario real con Azure AD, el UserID podría ser el Object ID de Azure AD.
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(20) NULL
);
GO

-- Tabla para almacenar las citas agendadas por los usuarios
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    SlotID INT NOT NULL,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Scheduled', -- Ej. Scheduled, Cancelled
    CONSTRAINT FK_Appointments_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Appointments_Slots FOREIGN KEY (SlotID) REFERENCES AvailableSlots(SlotID)
);
GO

-- Insertar algunos horarios de ejemplo para pruebas
-- (En una aplicación real, esto se gestionaría desde un panel de administración)
INSERT INTO AvailableSlots (SlotDateTime, Location) VALUES
('2025-12-01 09:00:00', 'Ciudad de México'),
('2025-12-01 09:30:00', 'Ciudad de México'),
('2025-12-01 10:00:00', 'Ciudad de México'),
('2025-12-02 09:00:00', 'Monterrey'),
('2025-12-02 09:30:00', 'Monterrey');
GO

PRINT 'Tablas creadas y datos de ejemplo insertados correctamente.';
