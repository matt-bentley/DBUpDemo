CREATE TABLE [dbo].[Clients](
	[client_pid] [int] IDENTITY(1,1) NOT NULL,
	[client_name] [varchar](100) NOT NULL,
	CONSTRAINT PK_Clients PRIMARY KEY (client_pid)
) ON [PRIMARY]

CREATE TABLE Entities (
	entity_pid INT NOT NULL IDENTITY(1,1),
	client_fid INT NOT NULL,
	[entity_name] VARCHAR(100) NOT NULL,
	CONSTRAINT PK_Entities PRIMARY KEY (entity_pid)
)

DECLARE @client_pid INT
insert into Clients values ('Mickey Mouse Group')
SET @client_pid = SCOPE_IDENTITY()
insert into Entities values (@client_pid, 'Mickey Ltd'),(@client_pid, 'Minnie Ltd'),(@client_pid, 'Donald D')

insert into Clients values ('Shrek Plc')
SET @client_pid = SCOPE_IDENTITY()
insert into Entities values (@client_pid, 'Shrek Ltd'),(@client_pid, 'Donkey Trading')
