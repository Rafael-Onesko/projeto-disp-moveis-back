using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Colaborador")]
    public class Colaborador
    {
        [Column("Id")]
        [Key] public int Id { get; set; }
        [Column("PessoaFisica")]
        public int PessoaFisica { get; set; }
        [Column("Tipo")]
        public short Tipo { get; set; }
        [Column("Matricula")]
        public int Matricula { get; set; }
        [Column("DataAdmissao")]
        public DateTime? DataAdmissao { get; set; }
        [Column("ValorContribuicao")]
        public decimal? ValorContribuicao { get; set; }
    }
}
