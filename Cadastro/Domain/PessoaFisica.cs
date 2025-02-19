using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("PessoaFisica")]
    public class PessoaFisica
    {
        [Column("Id")]
        [Key] public int Id { get; set; }
        [Column("CPF")]
        public decimal CPF { get; set; }
        [Column("Nome")]
        public string Nome { get; set; }
        [Column("DataNascimento")]
        public DateTime DataNascimento { get; set; }
        [Column("RG")]
        public string RG { get; set; }
    }
}
