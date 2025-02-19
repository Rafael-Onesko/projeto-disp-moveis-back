using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Usuario")]
    public class Usuario
    {
        [Column("Email")]
        [Key] public string Email { get; set; }
        [Column("PrimeiroNome")]
        public string PrimeiroNome { get; set; }
        [Column("UltimoNome")]
        public string UltimoNome { get; set; }
        [Column("Senha")]
        public string Senha { get; set; }
        [Column("Admin")]
        public bool Admin { get; set; }

    }
}
