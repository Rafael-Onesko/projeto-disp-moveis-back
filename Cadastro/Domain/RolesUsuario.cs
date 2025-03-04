﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain
{
    [Table("RolesUsuario")]
    public class RolesUsuario
    {
        [Column("relacao_ID")]
        [Key]public int relacao_ID { get; set; }
        [Column("user_ID")]
        public string user_ID { get; set; }
        [Column("Role_ID")]
        public int Role_ID { get; set; }
    }
}
