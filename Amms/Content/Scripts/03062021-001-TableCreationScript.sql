-------------------------Employee Table-----------------------------
CREATE TABLE Employee
(
	EmployeeID	INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	FirstName	VARCHAR(50),
	LastName	VARCHAR(50),
	Address	VARCHAR(150),
	MOb		INT,
	Designation	VARCHAR(50)
)

INSERT INTO [dbo].[Employee](FirstName,LastName,Address,MOb,Designation)
     VALUES
           ( N'Vindhi', N'Ruki',null,null,null)
GO

INSERT INTO [dbo].[Employee](FirstName,LastName,Address,MOb,Designation)
     VALUES
           ( N'Danushka', N'Dharmaratne',null,null,null)
GO



------------------------User Table-------------------------------

CREATE TABLE [Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UserName]	VARCHAR(50),
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[EmailID] [varchar](254) NOT NULL,
	[DateOfBirth] [datetime] NULL,
	[Password] [nvarchar](max) NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[ActivationCode] [uniqueidentifier] NOT NULL,
	[UserGroup]	VARCHAR(50),
	[EmployeeID] INT

	FOREIGN KEY (EmployeeID) REFERENCES Employee (EmployeeID)
)


INSERT [dbo].[Users] ([UserName], [FirstName], [LastName], [EmailID], [DateOfBirth], [Password], [IsEmailVerified], [ActivationCode], [UserGroup], [EmployeeID]) 
VALUES (N'vindhiz@gmail.com', N'Vindhi', N'Ruki', N'vindhiz@gmail.com', CAST(N'2004-06-16T00:00:00.000' AS DateTime), N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 1, N'1e97c79c-623f-48ae-ada5-49fcac28571f',null, 1)
GO
INSERT [dbo].[Users] ( [UserName], [FirstName], [LastName], [EmailID], [DateOfBirth], [Password], [IsEmailVerified], [ActivationCode], [UserGroup], [EmployeeID]) 
VALUES (N'danushkad@hoolala.com', N'danushka', N'Dharmaratne', N'danushkad@hoolala.com', CAST(N'2018-03-10T00:00:00.000' AS DateTime), N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 1, N'7b5329d7-9e08-4046-90dc-0c9d05a4905a', null, 2)
GO

-------------------------Customer Table--------------------------
CREATE TABLE Customer
(
	CustomerID	INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	CustomerFirstName	VARCHAR(50),
	CustomerLastName	VARCHAR(50),
	CustomerAddress	VARCHAR(150),
	CustomerMOb		INT,
	CustomerEmail	VARCHAR(50)
)

-------------------------Service Table--------------------------
CREATE TABLE Service
(
	ServiceID		INT IDENTITY(1,1) PRIMARY KEY,
	ServiceName		VARCHAR(50),
	ServiceDescription		VARCHAR(50),
	ServicePrice	DECIMAL(5,2),
	ServiceTime		TIME
)

------------------------Unit Table------------------------------
CREATE TABLE Unit
(
	UnitID		INT IDENTITY(1,1) PRIMARY KEY,
	UnitName	VARCHAR(50),
	UnitDescription		VARCHAR(50),
)

------------------------Category Table--------------------------
CREATE TABLE Category
(
	CategoryID		INT IDENTITY(1,1) PRIMARY KEY,
	CategoryName		VARCHAR(50),
	CategoryDescription		VARCHAR(50),
)

--------------------------Item Table-----------------------------
CREATE TABLE Item
(
	ItemID		INT IDENTITY(1,1) PRIMARY KEY,
	ItemName	VARCHAR(50),
	ItemDescription		VARCHAR(50),
	ItemPrice 	DECIMAL(5,2),
	UnitID		INT,
	CategoryID 		INT,
	
	FOREIGN KEY (UnitID) REFERENCES Unit (UnitID),
	FOREIGN KEY (CategoryID) REFERENCES Category (CategoryID)
)

--------------------------Stock Table-----------------------------
CREATE TABLE Stock
(
	StockID			INT IDENTITY(1,1) PRIMARY KEY,
	AvailQty		INT,
	MinOrderQty		INT,
	DamageQty		INT,
	PurchasePrice	DECIMAL(5,2),
	SellingPrice 	DECIMAL(5,2),
	ItemID			INT,
	
	FOREIGN KEY (ItemID) REFERENCES Item (ItemID)
)

----------------------------Vehicle Table-----------------------------
CREATE TABLE Vehicle
(
	VehicleID		INT IDENTITY(1,1) PRIMARY KEY,
	VehicleType		VARCHAR(50),
	VehicleModel	VARCHAR(50),
	Year			INT,
	CustomerID			INT,

	FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID)
)

-------------------------------Appointment Table---------------------------
CREATE TABLE Appointment
(
	AppointmentID			INT IDENTITY(1,1) PRIMARY KEY,
	AppointmentDate			VARCHAR(50),
	AppointmentTime			VARCHAR(50),
	AppointmentDescription 	VARCHAR(50),
	AppointmentStatus 		VARCHAR(10),
	CustomerID			INT,

	FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID)
)

---------------------------------Invoice Table----------------------------
CREATE TABLE Invoice
(
	InvoiceID	INT IDENTITY(1,1) PRIMARY KEY,
	Total		DECIMAL(5,2),
	Discount	DECIMAL(5,2),
	InvDate 	VARCHAR(50),
	InvTime 	VARCHAR(10),
	PayType		INT,
	CustomerID		INT,
	VehicleID	INT,

	FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID),
	FOREIGN KEY (VehicleID) REFERENCES Vehicle (VehicleID)
)

-----------------------------InvoiceDetails Table---------------------------
CREATE TABLE InvoiceDetails
(
	DetailID		INT IDENTITY(1,1) PRIMARY KEY,
	Description		VARCHAR(100),
	SerValue		CHAR(10),
	InvoiceID		INT,
	ServiceID			INT,
	ItemID			INT,
	EmployeeID			INT,

	FOREIGN KEY (InvoiceID) REFERENCES Invoice (InvoiceID),
	FOREIGN KEY (ServiceID) REFERENCES Service (ServiceID),
	FOREIGN KEY (EmployeeID) REFERENCES Employee (EmployeeID),
	FOREIGN KEY (ItemID) REFERENCES Item (ItemID)
)

------------------------------ContactUs Table-------------------------------
CREATE TABLE ContactUs
(
	Id		INT IDENTITY(1,1) PRIMARY KEY,
	Name	VARCHAR(100),
	Email	VARCHAR(50),
	Message VARCHAR(500)
)

-------------------------DROP TABLES------------------------------------
DROP TABLE InvoiceDetails
DROP TABLE Invoice
DROP TABLE Appointment
DROP TABLE Vehicle
DROP TABLE Stock
DROP TABLE Item
DROP TABLE Service
DROP TABLE Category
DROP TABLE Unit
DROP TABLE Customer
DROP TABLE User
DROP TABLE Employee
DROP TABLE ContactUs