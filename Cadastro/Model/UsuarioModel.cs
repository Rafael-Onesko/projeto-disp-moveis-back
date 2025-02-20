using Cadastro.Domain;

namespace Cadastro.Model
{
    public class UsuarioModel
    {
        public List<RolesModel> RolesUsuario { get; set; }
        public List<MateriasModel> MateriasUsuario { get; set; }
        public UsuarioModel()
        {
            RolesUsuario = new List<RolesModel>();
            MateriasUsuario = new List<MateriasModel>();
        }
        public string Email { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Senha { get; set; }
        public bool Admin { get; set; }
    }
    //public class RolesInscritoModel
    //{
    //    public string RoleNome { get; set; }
    //}
    //public class RolesInscritoModel
    //{
    //    public string RoleNome { get; set; }
    //}

}
