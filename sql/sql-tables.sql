-- cria database
CREATE DATABASE AulaDB;
 
GO
use AulaDB
 
/* ------------- Tabelas ------------- */
 
go
create table tbUsuarios(
	id int not null identity(1,1) primary key,
	usuario varchar(100) not null,
	senha varchar(100) not null,
	tipoUsuario varchar(100) not null,
	fotoPerfil varbinary(max)
)
 
go
create table tbDispositivos (
	id int not null identity(1,1) primary key,
	descricao varchar(100),
	idUsuario int not null foreign key references tbUsuarios(id)
    -- referencia o usuario dono do dispositivo
)
go
create table tbRegistros (
	id int not null identity(1,1) primary key,
	idDispositivo int not null foreign key references tbDispositivos(id),
    -- referencia o dispositivo que gerou o registro
	umidade int,
	luminosidade int,
	temperatura decimal(5,2),
	dataHora datetime default getdate() not null -- valor automatico (omitir durante INSERT e UPDATE)
)
