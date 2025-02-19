using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("RolesUsuario")]
    public class RolesUsuario
    {
        [Column("user_ID")]
        [Key] public string user_ID { get; set; }
        [Column("Role_ID")]
        public int Role_ID { get; set; }
    }
}
