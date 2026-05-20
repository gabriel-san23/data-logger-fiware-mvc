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