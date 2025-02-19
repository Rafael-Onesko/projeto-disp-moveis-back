using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Usuario")]
    public class Usuario
    {
        [Column("Login")]
        [Key] public string Login { get; set; }
        [Column("Nome")]
        public string Nome { get; set; }
        [Column("Senha")]
        public string Senha { get; set; }
        [Column("Administrador")]
        public bool Administrador { get; set; }
    }
}
