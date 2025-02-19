using Microsoft.EntityFrameworkCore;

namespace Cadastro.Domain
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<PessoaFisica> PessoasFisicas  { get; set; }
        public DbSet<Telefone> Telefones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Professores> Professores { get; set; }
        public DbSet<RolesUsuario> RolesUsuario { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Materias> Materias { get; set; }

    }
}
