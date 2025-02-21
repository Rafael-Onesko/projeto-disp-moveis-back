using Microsoft.EntityFrameworkCore;

namespace Cadastro.Domain
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Professores> Professores { get; set; }
        public DbSet<RolesUsuario> RolesUsuario { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Materias> Materias { get; set; }
        public DbSet<MateriasUsuario> MateriasUsuario { get; set; }

    }
}
