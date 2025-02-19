using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Telefone")]
    public class Telefone
    {
        [Column("Id")]
        [Key] public int Id { get; set; }
        [Column("PessoaFisica")]
        public int PessoaFisica { get; set; }
        [Column("Numero")]
        public string Numero { get; set; }
        [Column("Tipo")]
        public short Tipo { get; set; }
    }
}
