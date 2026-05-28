using Microsoft.AspNetCore.Http;
using System;

namespace DataLogger.Models
{
    public class UsuarioViewModel : PadraoViewModel
    {
        public string NomeUsuario { get; set; }
        public string SenhaUsuario{ get; set; }
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Imagem recebida do form pelo controller
        /// </summary>
        public IFormFile FotoPerfil { get; set; }
        /// <summary>
        /// Imagem em bytes pronta para ser salva
        /// </summary>
        public byte[] FotoPerfilEmByte { get; set; }
        /// <summary>
        /// Imagem usada para ser enviada ao form no formato para ser exibida
        /// </summary>
        public string FotoPerfilEmBase64
        {
            get
            {
                if (FotoPerfilEmByte != null)
                    return Convert.ToBase64String(FotoPerfilEmByte);
                else
                    return string.Empty;
            }
        }
    }
}

/*
create table tbUsuarios(
	id int not null identity(1,1) primary key,
	nomeUsuario varchar(100) not null,
	senhaUsuario varchar(100) not null,
	tipoUsuario varchar(100) not null,
	fotoPerfil varbinary(max)
) 
 */