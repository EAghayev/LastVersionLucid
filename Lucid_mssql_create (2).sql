CREATE TABLE [Employees] (
	Id int NOT NULL,
	Name nvarchar(50) NOT NULL,
	Surname nvarchar(50) NOT NULL,
	Salary money NOT NULL,
	Phone nvarchar(25) NOT NULL,
	JoinDate date NOT NULL,
	RoleId int NOT NULL,
	Gender bit NOT NULL DEFAULT '0',
	Email nvarchar(100) NOT NULL,
	Facebook nvarchar(150),
	Twitter nvarchar(150),
	Linkedin nvarchar(150),
	EmployeeNo int NOT NULL UNIQUE,
	HRA money,
	Bonus money,
	Conveyance money,
	OtherAllowances money,
	TDS money,
	ESI money,
	ProvidentFund money,
	BankLoan money,
	therDeductions money,
  CONSTRAINT [PK_EMPLOYEES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Roles] (
	Id int NOT NULL,
	DepartmentId int NOT NULL,
	Name nvarchar(100) NOT NULL,
	[Desc] nvarchar(500) NOT NULL,
  CONSTRAINT [PK_ROLES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Departments] (
	Id int NOT NULL,
	Name nvarchar(100) NOT NULL,
	Head int NOT NULL,
  CONSTRAINT [PK_DEPARTMENTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Holidays] (
	Id int NOT NULL,
	Name nvarchar(50) NOT NULL,
	Date date NOT NULL,
  CONSTRAINT [PK_HOLIDAYS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Events] (
	Id int NOT NULL,
	StandartEventsId int NOT NULL,
	[Desc] ntext,
	Date date NOT NULL,
	Lacation nvarchar(150),
	StartTime time,
	FinishTime time,
  CONSTRAINT [PK_EVENTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [StandartEvents] (
	Id int NOT NULL,
	Title nvarchar(150) NOT NULL,
  CONSTRAINT [PK_STANDARTEVENTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [LeaveRequests] (
	Id int NOT NULL,
	EmployeeId int NOT NULL,
	Reason nvarchar(500) NOT NULL,
	LeaveTypeId int NOT NULL,
	StratDate date NOT NULL,
	FinishDate date NOT NULL,
  CONSTRAINT [PK_LEAVEREQUESTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [LeaveTypes] (
	Id int NOT NULL,
	Name nvarchar(150) NOT NULL,
  CONSTRAINT [PK_LEAVETYPES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Expenses] (
	Id int NOT NULL,
	Item nvarchar(150) NOT NULL,
	Count int NOT NULL,
	OrderBy int NOT NULL,
	From nvarchar(150) NOT NULL,
	Date date NOT NULL,
	Status bit NOT NULL DEFAULT '0',
	Price money NOT NULL,
	PaymentsTypeId int NOT NULL,
  CONSTRAINT [PK_EXPENSES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [PaymentTypes] (
	Id int NOT NULL,
	Name nvarchar(150) NOT NULL,
	Profile nvarchar(150) NOT NULL,
  CONSTRAINT [PK_PAYMENTTYPES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Invoices] (
	Id int NOT NULL,
	Number int NOT NULL,
	ClientId int NOT NULL,
	Date date NOT NULL,
	PaymentTypeId int NOT NULL,
	Status bit NOT NULL DEFAULT '0',
	Amount money NOT NULL,
  CONSTRAINT [PK_INVOICES] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Clients] (
	Id int NOT NULL,
	Name nvarchar(100) NOT NULL,
  CONSTRAINT [PK_CLIENTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Payments] (
	Id int NOT NULL,
	ClientId int NOT NULL,
	Date date NOT NULL,
	ProjectId int NOT NULL,
	PaymentTypeId int NOT NULL,
	Amount money NOT NULL,
  CONSTRAINT [PK_PAYMENTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Projects] (
	Id int NOT NULL,
	Name nvarchar(100) NOT NULL,
	[Desc] ntext,
  CONSTRAINT [PK_PROJECTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [ClientsProjects] (
	Id int NOT NULL,
	ClientId int NOT NULL,
	ProjectId int NOT NULL,
  CONSTRAINT [PK_CLIENTSPROJECTS] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
CREATE TABLE [Attendance] (
	Id int NOT NULL,
	EmployeeId int NOT NULL,
	Date date NOT NULL,
	Atd bit NOT NULL DEFAULT '1',
  CONSTRAINT [PK_ATTENDANCE] PRIMARY KEY CLUSTERED
  (
  [Id] ASC
  ) WITH (IGNORE_DUP_KEY = OFF)

)
GO
ALTER TABLE [Employees] WITH CHECK ADD CONSTRAINT [Employees_fk0] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Employees] CHECK CONSTRAINT [Employees_fk0]
GO

ALTER TABLE [Roles] WITH CHECK ADD CONSTRAINT [Roles_fk0] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Roles] CHECK CONSTRAINT [Roles_fk0]
GO

ALTER TABLE [Departments] WITH CHECK ADD CONSTRAINT [Departments_fk0] FOREIGN KEY ([Head]) REFERENCES [Employees]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Departments] CHECK CONSTRAINT [Departments_fk0]
GO


ALTER TABLE [Events] WITH CHECK ADD CONSTRAINT [Events_fk0] FOREIGN KEY ([StandartEventsId]) REFERENCES [StandartEvents]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Events] CHECK CONSTRAINT [Events_fk0]
GO


ALTER TABLE [LeaveRequests] WITH CHECK ADD CONSTRAINT [LeaveRequests_fk0] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [LeaveRequests] CHECK CONSTRAINT [LeaveRequests_fk0]
GO
ALTER TABLE [LeaveRequests] WITH CHECK ADD CONSTRAINT [LeaveRequests_fk1] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [LeaveRequests] CHECK CONSTRAINT [LeaveRequests_fk1]
GO


ALTER TABLE [Expenses] WITH CHECK ADD CONSTRAINT [Expenses_fk0] FOREIGN KEY ([OrderBy]) REFERENCES [Employees]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Expenses] CHECK CONSTRAINT [Expenses_fk0]
GO
ALTER TABLE [Expenses] WITH CHECK ADD CONSTRAINT [Expenses_fk1] FOREIGN KEY ([PaymentsTypeId]) REFERENCES [PaymentTypes]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Expenses] CHECK CONSTRAINT [Expenses_fk1]
GO


ALTER TABLE [Invoices] WITH CHECK ADD CONSTRAINT [Invoices_fk0] FOREIGN KEY ([ClientId]) REFERENCES [Clients]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Invoices] CHECK CONSTRAINT [Invoices_fk0]
GO
ALTER TABLE [Invoices] WITH CHECK ADD CONSTRAINT [Invoices_fk1] FOREIGN KEY ([PaymentTypeId]) REFERENCES [PaymentTypes]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Invoices] CHECK CONSTRAINT [Invoices_fk1]
GO


ALTER TABLE [Payments] WITH CHECK ADD CONSTRAINT [Payments_fk0] FOREIGN KEY ([ClientId]) REFERENCES [Clients]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Payments] CHECK CONSTRAINT [Payments_fk0]
GO
ALTER TABLE [Payments] WITH CHECK ADD CONSTRAINT [Payments_fk1] FOREIGN KEY ([PaymentTypeId]) REFERENCES [PaymentTypes]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Payments] CHECK CONSTRAINT [Payments_fk1]
GO


ALTER TABLE [ClientsProjects] WITH CHECK ADD CONSTRAINT [ClientsProjects_fk0] FOREIGN KEY ([ClientId]) REFERENCES [Clients]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ClientsProjects] CHECK CONSTRAINT [ClientsProjects_fk0]
GO
ALTER TABLE [ClientsProjects] WITH CHECK ADD CONSTRAINT [ClientsProjects_fk1] FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [ClientsProjects] CHECK CONSTRAINT [ClientsProjects_fk1]
GO

ALTER TABLE [Attendance] WITH CHECK ADD CONSTRAINT [Attendance_fk0] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees]([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [Attendance] CHECK CONSTRAINT [Attendance_fk0]
GO

