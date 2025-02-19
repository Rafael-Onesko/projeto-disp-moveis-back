using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("Roles")]
    public class Roles
    {
        [Column("Role_ID")]
        [Key] public int Role_ID { get; set; }
        [Column("RoleNome")]
        public string RoleNome { get; set; }

    }
}
