using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Professores")]
    public class Professores
    {
        [Column("Professor_ID")]
        [Key] public string Professor_ID { get; set; }
        [Column("Professor_Nome")]
        public string Professor_Nome { get; set; }
    }
}
