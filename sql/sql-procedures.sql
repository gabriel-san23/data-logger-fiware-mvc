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

/*
create table tbUsuarios(
	id int not null identity(1,1) primary key,
	usuario varchar(100) not null,
	senha varchar(100) not null,
	tipoUsuario varchar(100) not null,
	fotoPerfil varbinary(max)
)
*/

CREATE OR ALTER PROCEDURE SP_INSERE_USUARIO(
	@USUARIO VARCHAR(100) ,
	@SENHA VARCHAR(100) ,
	@TIPO_USUARIO VARCHAR(100) ,
	@FOTO_PERFIL VARBINARY(MAX)
)
AS 
BEGIN
	
	IF EXISTS ( SELECT 1 FROM tbUsuarios WHERE usuario = @USUARIO)
	BEGIN
		PRINT 'Inserção não realizada, usuario já cadastrado na base'
		RETURN 1
	END
	ELSE
		INSERT INTO tbUsuarios (usuario, senha ,tipoUsuario,fotoPerfil) VALUES
		(@USUARIO,@SENHA,@TIPO_USUARIO,@FOTO_PERFIL)
	END
	
GO

EXEC SP_INSERE_USUARIO 'DANIEL','dani1234','admin',00100101010100010100101

CREATE OR ALTER PROCEDURE SP_UPDATE_USUARIO_NOME(
	@ID_USUARIO INT, 
	@NOVO_USUARIO VARCHAR(100)
)
AS
BEGIN
		
	UPDATE A
	SET A.USUARIO = @NOVO_USUARIO
	FROM tbUsuarios A
	WHERE A.ID = @ID_USUARIO


END
GO

EXEC SP_UPDATE_USUARIO_NOME 4,'DANIEL NOVO'

CREATE OR ALTER PROCEDURE SP_UPDATE_USUARIO_SENHA(
	@ID_USUARIO INT, 
	@NOVA_SENHA VARCHAR(100)
)
AS
BEGIN
		
	UPDATE A
	SET A.SENHA = @NOVA_SENHA
	FROM tbUsuarios A
	WHERE A.ID = @ID_USUARIO


END
GO

EXEC SP_UPDATE_USUARIO_SENHA 4,'Senha Nova'

CREATE OR ALTER PROCEDURE SP_UPDATE_USUARIO_FOTO(
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

exec SP_UPDATE_USUARIO_FOTO 4,01010011111001010100101001



/*
create table tbDispositivos (
	id int not null identity(1,1) primary key,
	descricao varchar(100),
	idUsuario int not null foreign key references tbUsuarios(id)
    -- referencia o usuario dono do dispositivo
)
*/

CREATE OR ALTER PROCEDURE SP_INSERE_DISPOSITIVO(
	@DESCRICAO VARCHAR(100),
	@ID_USUARIO INT
)
AS
BEGIN

	INSERT INTO tbDispositivos (DESCRICAO,idUsuario) VALUES
	(@DESCRICAO, @ID_USUARIO)

END
GO

CREATE OR ALTER PROCEDURE SP_UPDATE_DISPOSITIVO(
	@ID_USUARIO INT,
	@DESCRICAO VARCHAR(100)
)
AS
BEGIN
	
	UPDATE A
	SET A.DESCRICAO = @DESCRICAO
	FROM tbDispositivos A
	WHERE A.ID = @ID_USUARIO

END

EXEC SP_UPDATE_DISPOSITIVO 2,'Atualizado por SP'

/*
create table tbRegistros (
	id int not null identity(1,1) primary key,
	idDispositivo int not null foreign key references tbDispositivos(id),
    -- referencia o dispositivo que gerou o registro
	umidade int,
	luminosidade int,
	temperatura decimal(5,2),
	dataHora datetime default getdate() not null -- valor automatico (omitir durante INSERT e UPDATE)
)
*/


CREATE OR ALTER PROCEDURE SP_INSERE_REGISTRO(
	@ID_DISPOSITIVO INT,
	@UMIDADE INT,
	@LUMINOSIDADE INT,
	@TEMPERATURA DECIMAL(5,2)
)
AS
BEGIN
	
	INSERT INTO tbRegistros(idDispositivo,umidade,luminosidade,temperatura,dataHora)
	VALUES (@ID_DISPOSITIVO,@UMIDADE,@LUMINOSIDADE,@TEMPERATURA,GETDATE())


END
GO



SELECT * FROM tbusuarios
SELECT * FROM tbDispositivos
SELECT * FROM tbRegistros

