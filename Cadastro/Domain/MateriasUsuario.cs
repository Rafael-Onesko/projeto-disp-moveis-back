using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("MateriasUsuario")]
    public class MateriasUsuario
    {
        [Column("relacao_ID")]
        [Key] public int relacao_ID { get; set; }
        [Column("user_ID")]
        public string user_ID { get; set; }
        [Column("Materia_ID")]
        public string Materia_ID { get; set; }
    }
}
