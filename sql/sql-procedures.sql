use AulaDB

/* --- Procedures Genéricas --- */

go
create or alter procedure spDelete
( 
  @id int,
  @tabela varchar(max) 
)
as 
begin
   declare @sql varchar(max); 
   set @sql = ' delete ' + @tabela +  
       ' where id = ' + cast(@id as varchar(max)) 
   exec(@sql) 
end
 
go
create or alter procedure spConsulta 
( 
  @id int, 
  @tabela varchar(max) 
) 
as 
begin 
   declare @sql varchar(max); 
   set @sql = 'select * from  ' + @tabela +  
        ' where id = ' + cast(@id as varchar(max)) 
   exec(@sql) 
end 
GO 
create or alter procedure spListagem
( 
   @tabela varchar(max), 
   @ordem varchar(max)) 
as 
begin 
   exec('select * from ' + @tabela +  
        ' order by ' + @ordem) 
end


-- os IDs estão como identity(1,1) (inicia em 1, passso 1)
-- logo, a seguinte procedure provavelmente não será necessária
GO
create or alter procedure spProximoId
(@tabela  varchar(max)) 
as 
begin 
     exec('select isnull(max(id) +1, 1) as MAIOR from '  
          +@tabela) 
end


/* --- Procedures Específicas por Tabela --- */


/*
create table tbUsuarios(
	id int not null identity(1,1) primary key,
	nomeUsuario varchar(100) not null,
	senhaUsuario varchar(100) not null,
	tipoUsuario varchar(100) not null,
	fotoPerfil varbinary(max)
)
*/

go
CREATE OR ALTER PROCEDURE spInsert_tbUsuarios(
	@ID int,
	@NOME_USUARIO VARCHAR(100) ,
	@SENHA_USUARIO VARCHAR(100) ,
	@TIPO_USUARIO VARCHAR(100) ,
	@FOTO_PERFIL VARBINARY(MAX)
)
AS 
BEGIN

	INSERT INTO tbUsuarios (nomeUsuario, senhaUsuario, tipoUsuario, fotoPerfil) VALUES
	(@NOME_USUARIO,@SENHA_USUARIO,@TIPO_USUARIO,@FOTO_PERFIL)

END
	
GO

--EXEC spInsert_USUARIO 'DANIEL','dani1234','admin',00100101010100010100101

go
CREATE OR ALTER PROCEDURE spUpdate_tbUsuarios(
	@ID int,
	@NOME_USUARIO VARCHAR(100) ,
	@SENHA_USUARIO VARCHAR(100) ,
	@TIPO_USUARIO VARCHAR(100) ,
	@FOTO_PERFIL VARBINARY(MAX)
)
AS
BEGIN
		
	UPDATE A
	SET A.nomeUsuario = @NOME_USUARIO,
		A.senhaUsuario = @SENHA_USUARIO,
		A.FOTOPERFIL = @FOTO_PERFIL
	FROM tbUsuarios A
	WHERE A.ID = @ID

END

go
CREATE OR ALTER PROCEDURE spConsultaPorNome(
   @NOME_USUARIO varchar(max)
)
as
BEGIN
   select * from tbUsuarios
   where nomeUsuario = @NOME_USUARIO
END

--EXEC spConsultaPorNome 'tbUsuarios','admin'
select * from tbUsuarios
where nomeUsuario = 'admin'
/*
create table tbDispositivos (
	id int not null identity(1,1) primary key,
	descricao varchar(100),
	idUsuario int not null foreign key references tbUsuarios(id)
    -- referencia o usuario dono do dispositivo
)
*/
go
CREATE OR ALTER PROCEDURE spInsert_tbDISPOSITIVOS(
	@ID_DISPOSITIVO INT,
	@DESCRICAO VARCHAR(100),
	@ID_USUARIO INT
)
AS
BEGIN

	INSERT INTO tbDispositivos (DESCRICAO,idUsuario) VALUES
	(@DESCRICAO, @ID_USUARIO)

END
GO

CREATE OR ALTER PROCEDURE spUpdate_tbDISPOSITIVOS(
	@ID_DISPOSITIVO INT,
	@DESCRICAO VARCHAR(100),
	@ID_USUARIO INT
)
AS
BEGIN
	
	UPDATE A
	SET A.DESCRICAO = @DESCRICAO
	FROM tbDispositivos A
	WHERE A.ID = @ID_DISPOSITIVO

END

-- EXEC spUpdate_DISPOSITIVO 2,'Atualizado por SP'

/*
create table tbRegistros (
	id int not null identity(1,1) primary key,
	idDispositivo int not null foreign key references tbDispositivos(id),
    -- referencia o dispositivo que gerou o registro
	valorUmidade int,
	valorLuminosidade int,
	valorTemperatura decimal(5,2),
	dataHora datetime default getdate() not null -- valor automatico (omitir durante INSERT e UPDATE)
)
*/

go
CREATE OR ALTER PROCEDURE spInsert_tbREGISTROS(
	@idDispositivo INT,
	@valorUmidade INT,
	@valorLuminosidade INT,
	@valorTemperatura DECIMAL(5,2)
)
AS
BEGIN
	
	INSERT INTO tbRegistros(idDispositivo,valorUmidade,valorLuminosidade,valorTemperatura)
	VALUES (@idDispositivo,@valorUmidade,@valorLuminosidade,@valorTemperatura)


END
GO

/*
SELECT * FROM tbusuarios
SELECT * FROM tbDispositivos
SELECT * FROM tbRegistros
*/

-- View para dispositivos
/*
SELECT d.id, d.descricao, d.idUsuario, u.nomeUsuario
FROM tbDispositivos d
INNER JOIN tbUsuarios u ON d.idUsuario = u.id
*/
