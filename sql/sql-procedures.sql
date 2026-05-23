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
	
	IF EXISTS ( SELECT 1 FROM tbUsuarios WHERE nomeUsuario = @NOME_USUARIO)
	BEGIN
		PRINT 'Inserção não realizada, usuario já cadastrado na base'
		RETURN 1
	END
	ELSE
		INSERT INTO tbUsuarios (nomeUsuario, senhaUsuario, tipoUsuario, fotoPerfil) VALUES
		(@NOME_USUARIO,@SENHA_USUARIO,@TIPO_USUARIO,@FOTO_PERFIL)
	END
	
GO

--EXEC spInsert_USUARIO 'DANIEL','dani1234','admin',00100101010100010100101

go
CREATE OR ALTER PROCEDURE spUpdate_USUARIO_NOME(
	@ID_USUARIO INT, 
	@NOVO_NOME VARCHAR(100)
)
AS
BEGIN
		
	UPDATE A
	SET A.nomeUsuario = @NOVO_NOME
	FROM tbUsuarios A
	WHERE A.ID = @ID_USUARIO

END
GO

--EXEC spUpdate_USUARIO_NOME 4,'DANIEL NOVO'

go
CREATE OR ALTER PROCEDURE spUpdate_USUARIO_SENHA(
	@ID_USUARIO INT, 
	@NOVA_SENHA VARCHAR(100)
)
AS
BEGIN
		
	UPDATE A
	SET A.senhaUsuario = @NOVA_SENHA
	FROM tbUsuarios A
	WHERE A.ID = @ID_USUARIO


END
GO

--EXEC spUpdate_USUARIO_SENHA 4,'Senha Nova'

go
CREATE OR ALTER PROCEDURE spUpdate_USUARIO_FOTO(
	@ID_USUARIO INT, 
	@NOVA_FOTO VARBINARY(MAX)
)
AS
BEGIN
		
	UPDATE A
	SET A.FOTOPERFIL = @NOVA_FOTO
	FROM tbUsuarios A
	WHERE A.ID = @ID_USUARIO


END
GO

--exec spUpdate_USUARIO_FOTO 4,01010011111001010100101001



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
	@DESCRICAO VARCHAR(100)
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
	@ID_DISPOSITIVO INT,
	@VALOR_UMIDADE INT,
	@VALOR_LUMINOSIDADE INT,
	@VALOR_TEMPERATURA DECIMAL(5,2)
)
AS
BEGIN
	
	INSERT INTO tbRegistros(idDispositivo,valorUmidade,valorLuminosidade,valorTemperatura)
	VALUES (@ID_DISPOSITIVO,@VALOR_UMIDADE,@VALOR_LUMINOSIDADE,@VALOR_TEMPERATURA)


END
GO

/*
SELECT * FROM tbusuarios
SELECT * FROM tbDispositivos
SELECT * FROM tbRegistros
*/
