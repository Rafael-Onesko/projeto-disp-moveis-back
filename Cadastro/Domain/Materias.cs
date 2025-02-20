using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Materias")]
    public class Materias
    {
        [Column("Materia_ID")]
        [Key] public string Materia_ID { get; set; }
        [Column("Materia_Nome")]
        public string Materia_Nome { get; set; }
        [Column("Materia_Bloco")]
        public string Materia_Bloco { get; set; }
        [Column("Professor_ID")]
        public string Professor_ID { get; set; }
        [Column("Professor_Nome")]
        public string Professor_Nome { get; set; }
    }
}
