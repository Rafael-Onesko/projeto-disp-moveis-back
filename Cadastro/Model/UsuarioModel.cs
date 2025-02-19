using Cadastro.Domain;

namespace Cadastro.Model
{
    public class UsuarioModel
    {
        List<RolesUsuario> RolesUsuario { get; set; }
        List<Materias> MateriasUsuario { get; set; }
        //public UsuarioModel()
        //{
        //    RolesUsuario = new List<RolesUsuario>();
        //    MateriasUsuario = new List<Materias>(); 
        //}
        public string Email { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Senha { get; set; }
        public bool Admin { get; set; }
    }
}
