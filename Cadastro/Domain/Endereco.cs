using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Endereco")]
    public class Endereco
    {
        [Column("Id")]
        [Key] public int Id { get; set; }
        [Column("PessoaFisica")]
        public int PessoaFisica { get; set; }
        [Column("Logradouro")]
        public string Logradouro { get; set; }
        [Column("Numero")]
        public string Numero { get; set; }
        [Column("Bairro")]
        public string Bairro { get; set; }
        [Column("Cidade")]
        public string Cidade { get; set; }
        [Column("CEP")]
        public decimal CEP { get; set; }
        [Column("Tipo")]
        public short Tipo { get; set; }
    }
}
